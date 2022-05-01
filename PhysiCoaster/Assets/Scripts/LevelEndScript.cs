using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEndScript : MonoBehaviour
{
    GameObject lineGraphHolder;
    GameObject lineGraph;
    GameObject barGraph;
    GameObject levelEndDisplay;
    // Start is called before the first frame update
    void Start()
    {
        lineGraphHolder = GameObject.Find("Line Graph Container");
        lineGraph = GameObject.FindGameObjectWithTag("Graph");
        //lineGraph.SetActive(false);
        barGraph = GameObject.Find("Energy Bar Chart Panel");
        levelEndDisplay = GameObject.Find("Final Screen");
        levelEndDisplay.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            lineGraphHolder.GetComponent<LineGraphScript>().StopRecordingAndClearData(false);
            collision.gameObject.GetComponent<CartMovementScript>().SwitchCurrentMode(0);
            levelEndDisplay.SetActive(true);
            levelEndDisplay.GetComponent<UpdateScoreLabelScript>().UpdateLabels();
            barGraph.SetActive(false);
        }
    }
}
