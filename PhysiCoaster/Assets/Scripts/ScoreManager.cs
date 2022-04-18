using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    GameObject player;
    int gems;
    int corctAnswr;
    //public Text GemScore;
   // public Text QuestionScore;
    //public Camera main;
    bool complete;
    int[] stars = { 0, 0, 0, 0, 0 };
    bool trigger1;
    bool trigger2;
    public GameObject check1;
    public GameObject check2;
    int bounding;
    int prevmode;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        trigger1 = true;
        trigger2 = true;
        complete = false;
        //bounding = (int)main.gameObject.GetComponent<CameraControllerScript>().xBoundsRight;
        bounding = (int)GameObject.FindGameObjectWithTag("End").transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        CheckTrigger();
        GrabScore();
        if (complete == true)
        {
            UpdateScore();
        }
    }
    void GrabScore()
    {
        gems = player.gameObject.GetComponent<GemCollect>().gems;
        for (int i = 0; i < (gems + corctAnswr); i++)
        {
            stars[i] = 1;
        }
    }
    void UpdateScore()
    {
      //  GemScore.text = gems + " out of 3 Collected";
        Debug.Log(gems + " out of 3 Collected");
      //  QuestionScore.text = corctAnswr + " out of 2 Correct";
        Debug.Log(corctAnswr + " out of 2 Correct");
    }
    public void IncreaseScore()
    {
            corctAnswr++;
    }
    void CheckTrigger()
    {
        int stage = (int)player.gameObject.transform.position.x / (bounding/3);
        if((trigger1 == true)&&(stage == 1))
        {
            trigger1 = false;
            prevmode = (int)player.gameObject.GetComponent<CartMovementScript>().currentMode;
            check1.SetActive(true);
            player.gameObject.GetComponent<CartMovementScript>().SwitchCurrentMode(0);
            Debug.Log("Question 1 Triggered");
        }
        if ((trigger2 == true) && (stage == 2) && (trigger1 == false))
        {
            trigger2 = false;
            prevmode = (int)player.gameObject.GetComponent<CartMovementScript>().currentMode;
            check2.SetActive(true);
            player.gameObject.GetComponent<CartMovementScript>().SwitchCurrentMode(0);
            Debug.Log("Question 2 Triggered");
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            complete = true;
        }
    }
    public void ReturnRun()
    {
        player.gameObject.GetComponent<CartMovementScript>().SwitchCurrentMode(prevmode);
    }
}
