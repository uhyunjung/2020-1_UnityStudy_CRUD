using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public GameManager gameManager;
    public AudioClip audioJump;
    public AudioClip audioAttack;
    public AudioClip audioDamaged;
    public AudioClip audioItem;
    public AudioClip audioDie;
    public AudioClip audioFinish;

    public float maxSpeed;
    public float jumpPower;
    int jumpCount=0;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    CapsuleCollider2D capsulecollider;
    Animator anim;
    AudioSource audioSource;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        capsulecollider = GetComponent<CapsuleCollider2D>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()  // 단발적 키 입력
    {
        // Jump
        if ((Input.GetButtonDown("Jump")) && (jumpCount < 2))
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("isJump", true);
            PlaySound("JUMP");
            jumpCount++;
        }

        // Stop Speed
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(0.5f * rigid.velocity.normalized.x, rigid.velocity.y);
        }

        // Direction Sprite
        if (Input.GetButton("Horizontal"))
        {
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }

        //  Animation
        if (Mathf.Abs(rigid.velocity.x) < 0.6)
        {
            anim.SetBool("isWalk", false);
        }
        else
        {
            anim.SetBool("isWalk", true);
        }

        // Landing Platform
        if (rigid.velocity.y < 0)  // 내려갈 때만 Ray 생김
        {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));  //에디터 상에서만 Ray를 그려주는 함수
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform")); // Ray에 닿은 오브젝트
            if (rayHit.collider != null)
            {
                if (rayHit.distance < 0.5f)
                {
                    anim.SetBool("isJump", false);
                    jumpCount = 0;
                }
            }
        }
    }

    void FixedUpdate()
    {
        // Move Speed
        // Move By Key Control
        float h = Input.GetAxisRaw("Horizontal");

        rigid.AddForce(new Vector2(2*h, 0), ForceMode2D.Impulse);  //1초에 50번, 키보드 누르면 상당히 빠름
        
        // Max Speed
        if(rigid.velocity.x > maxSpeed)  // Right Max Speed
        {
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);  // y좌표에 0 사용하지 말 것
        }

        else if(rigid.velocity.x < (-1)*maxSpeed)  // Left Max Speed
        {
            rigid.velocity = new Vector2(maxSpeed*(-1), rigid.velocity.y);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            //Attack : 속도 아래로, 몬스터 위에 위치
            if((rigid.velocity.y<0) && (transform.position.y > collision.transform.position.y+0.5))
            {
                OnAttack(collision.transform);
            }
            else //Damaged
                OnDamaged(collision.transform.position);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Item")
        {
            //Point
            bool isBronze = collision.gameObject.name.Contains("Bronze");
            bool isSilver = collision.gameObject.name.Contains("Silver");
            bool isGold = collision.gameObject.name.Contains("Gold");

            if (isBronze)
                gameManager.stagePoint += 50;
            else if (isSilver)
                gameManager.stagePoint += 100;
            else if (isGold)
                gameManager.stagePoint += 300;

            //Deactive ITem
            collision.gameObject.SetActive(false);
            //Sount
            PlaySound("ITEM");
        }

        else if (collision.gameObject.tag == "Finish")
        {
            //Next Stage
            gameManager.NextStage();
            //Sound
            PlaySound("FINISH");
        }
    }

    void OnAttack(Transform enemy)
    {
        //Point
        gameManager.stagePoint += 100;

        //Reaction Force
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        //Enemy Die
        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
        enemyMove.OnDamaged();
        //Sound
        PlaySound("ATTACK");
    }

    public void OnDamaged(Vector2 targetPos) // 무적 시간
    {
        // Health Down
        gameManager.HealthDown();
        // Change Layer(Immortal Active)
        gameObject.layer = 11;  // 레이어 숫자

        spriteRenderer.color = new Color(1, 1, 1, 0.4f);  // 0.4f 투명도

        // Reaction Force
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc, 1) * 5, ForceMode2D.Impulse);

        // Animation
        anim.SetTrigger("doDamaged");

        //Sound
        PlaySound("DAMAGED");

        Invoke("OffDamaged", 3);
    }

    void OffDamaged()
    {
        gameObject.gameObject.layer = 10;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    public void OnDie()
    {
        //Sprite Alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        //Sprite Flip Y
        spriteRenderer.flipY = true;
        //Collider Disable
        capsulecollider.enabled = false;
        //Die Effect Jump
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        //Sound
        PlaySound("DIE");
        //Destroy
        Invoke("DeActive", 5);
    }

    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
    }

    void PlaySound(string action)
    {
        switch (action)
        {
            case "JUMP":
                audioSource.clip = audioJump;
                break;
            case "ATTACK":
                audioSource.clip = audioAttack;
                break;
            case "DAMAGED":
                audioSource.clip = audioDamaged;
                break;
            case "ITEM":
                audioSource.clip = audioItem;
                break;
            case "DIE":
                audioSource.clip = audioDie;
                break;
            case "FINISH":
                audioSource.clip = audioFinish;
                break;
        }

        audioSource.Play();
    }
}
