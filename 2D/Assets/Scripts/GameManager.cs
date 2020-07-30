using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int health;
    public PlayerMove player;
    public GameObject[] Stages;

    public Image[] UIHealth;
    public Text UIPoint;
    public Text UIStage;
    public GameObject UIRestartBtn;
    public GameObject UIStartBtn;
    public ProgressBar progress;
    public Text ProgressTxt;
    public TimeBar timebar;
    
    void Start()
    {
        Text startbtnText = UIStartBtn.GetComponentInChildren<Text>();
        startbtnText.text = "Start!";
        UIStartBtn.SetActive(true);
        Time.timeScale = 0;
        progress.current = 0;
        ProgressTxt.text = (progress.current).ToString() + "%";
    }

    void Update()
   {
        UIPoint.text = (totalPoint + stagePoint).ToString()+" ";
        UIStage.text = "STAGE" + (stageIndex + 1).ToString();

    }
    public void NextStage()
    {

        progress.current = (int)(((float)(stageIndex+1) / Stages.Length) * 100);
        ProgressTxt.text = (progress.current).ToString() + "%";

        Stages[stageIndex].SetActive(false);
        stageIndex++;
        
        //Change Stage
        if (stageIndex < Stages.Length)
        {
            
            Stages[stageIndex].SetActive(true);
            PlayerReposition();
        }
        else  //Game Clear
        {
            progress.current = 100;
            ProgressTxt.text = (progress.current).ToString() + "%";

            stageIndex--;

            //Player Control Lock
            Time.timeScale = 0;

            //Restart Button UI
            Text btnText = UIRestartBtn.GetComponentInChildren<Text>();
            btnText.text = "Clear!";
            ViewBtn();

            //버튼 텍스트는 자식 오브젝트
        }
        
        //Calculate Point
        totalPoint += stagePoint;
        stagePoint = 0;
    }

    public void HealthDown()
    {
        if (health > 1)
        {
            health--;
            UIHealth[health].color = new Color(1, 0, 0, 0.4f);
        }

        else
        {
            //All Health UI Off
            UIHealth[0].color = new Color(1, 0, 0, 0.4f);
            //Player Die Effect
            player.OnDie();

            //Retry Button UI
            Text btnText = UIRestartBtn.GetComponentInChildren<Text>();
            btnText.text = "Restart?";
            ViewBtn();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (health > 1)
            {
                PlayerReposition();
            }

            // Health Down
            HealthDown();
        }
    }

    void PlayerReposition()
    {
        player.transform.position = new Vector3(0, 2, -5);
        player.VelocityZero();
    }

    void ViewBtn()
    {
        UIRestartBtn.SetActive(true);
    }

    public void Restart()
    {
        Time.timeScale = 1;
        
        SceneManager.LoadScene(0);  // OnClick()
    }

    public void Startbtn()
    {
        Time.timeScale = 1;
        UIStartBtn.SetActive(false);
    }
}
