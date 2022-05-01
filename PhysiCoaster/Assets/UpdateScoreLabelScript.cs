using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpdateScoreLabelScript : MonoBehaviour
{
    GameObject end;
    int gemsCollected;
    int questionsCorrect;
    public GameObject gemsText;
    public GameObject questionsText;
    // Start is called before the first frame update
    void Start()
    {
        end = GameObject.FindGameObjectWithTag("End");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateLabels()
    {
        end = GameObject.FindGameObjectWithTag("End");
        gemsCollected = end.GetComponent<ScoreManager>().gems;
        questionsCorrect = end.GetComponent<ScoreManager>().corctAnswr;
        gemsText.GetComponent<TextMeshProUGUI>().text = gemsCollected.ToString()+" / 3";
        questionsText.GetComponent<TextMeshProUGUI>().text = questionsCorrect.ToString() + " / 2";
    }
}
