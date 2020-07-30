using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public float maxSpeed;
    public float jumpPower;
    int jumpCount = 0;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    void Update()  // 단발적 키 입력
    {
        // Jump
        if ((Input.GetButtonDown("Jump")) && (jumpCount < 2))
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("isJump", true);
            jumpCount++;
        }

        // Stop Speed
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(0.5f * rigid.velocity.normalized.x, rigid.velocity.y);
        }

        // Direction Sprite
        if (Input.GetButtonDown("Horizontal"))
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
        Debug.Log(h);
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);  //1초에 50번, 키보드 누르면 상당히 빠름

        // Max Speed
        if (rigid.velocity.x > maxSpeed)  // Right Max Speed
        {
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);  // y좌표에 0 사용하지 말 것
        }

        else if (rigid.velocity.x < (-1) * maxSpeed)  // Left Max Speed
        {
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);
        }
    }
}
