using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEndScript : MonoBehaviour
{
    GameObject graph;
    public GameObject levelEndDisplay;
    GameObject playButton;
    // Start is called before the first frame update
    void Start()
    {
        graph = GameObject.FindGameObjectWithTag("Graph");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            graph.GetComponent<LineGraphScript>().StopRecordingAndClearData(false);
            collision.gameObject.GetComponent<CartMovementScript>().SwitchCurrentMode(0);
            levelEndDisplay.SetActive(true);
            playButton = GameObject.Find("Play Level Button");
            playButton.SetActive(false);
        }
    }
}
