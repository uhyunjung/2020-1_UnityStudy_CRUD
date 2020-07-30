using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeBar : MonoBehaviour
{
    public PlayerMove player;
    public Text TimeTxt;
    public Image Mask;
    public float currentTime;
    public float startingTime;

    // Start is called before the first frame update
    void Start()
    {
        currentTime = startingTime;
    }

    // Update is called once per frame
    void Update()
    {
        currentTime = currentTime - Time.deltaTime;  // 한프레임 시간
        Debug.Log(currentTime);
        TimeTxt.text = currentTime.ToString("0");

        if (currentTime <=0)  // 완전 0 불가능
        {
            player.OnDamaged(player.transform.position);
            currentTime = 10;

        }
        GetCurrentFill();
    }

    void GetCurrentFill()
    {
        float fillAmount = currentTime / startingTime ;
        Mask.fillAmount = fillAmount;
    }
}
