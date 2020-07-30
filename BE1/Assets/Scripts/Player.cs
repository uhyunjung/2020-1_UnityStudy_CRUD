using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.SceneManagement;   // 장면 관리 기본 클래스

public class Player : MonoBehaviour
{
    public float jumpPower;
    public int itemCount;
    public Game_Manager manager;

    int isJump = 0;
    int jumpCount = 2;
    Rigidbody rigid;
    AudioSource audio;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        audio = GetComponent<AudioSource>();
    }


    void Update()
    {
        if ((Input.GetButtonDown("Jump")) && (jumpCount != 0))
        {
            jumpCount--;
            rigid.AddForce(new Vector3(0, jumpPower, 0), ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        rigid.AddForce(new Vector3(h, 0, v), ForceMode.Impulse);
    }

    void OnCollisionEnter(Collision collision)  // 바닥에 닿아야만 다시 점프 가능
    {
        if (collision.gameObject.tag == "Floor")
            jumpCount = 2;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            itemCount++;
            audio.Play();
            other.gameObject.SetActive(false); // 오브젝트 활성화 함수
            manager.GetItem(itemCount);
        }
        else if (other.tag == "Finish")
        {
            // GameObject.Find : Find 계열 함수는 부하 초래, CPU 사용됨 -> 피하는 거 권장
            if(itemCount == manager.TotalItemCount)  // Game Clear;
            {
                SceneManager.LoadScene(manager.stage+1);  // 새로운 레벨로 이동
            }
            else   // Restart;
            {
                SceneManager.LoadScene(manager.stage);
            }
        }
    }
}
