using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Failed : MonoBehaviour
{
    GameObject starting;
    GameObject graph;
    GameObject playButton;

    private void Start()
    {
        starting = GameObject.FindGameObjectWithTag("Start");
        graph = GameObject.FindGameObjectWithTag("Graph");
        playButton = GameObject.Find("Play Level Button");
    }
    private void OnCollisionEnter(Collision collision)
    {
        // Debug.Log("collided");
        // Debug.Log(collision.gameObject.tag);
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<CartMovementScript>().ShowCrashFailurePrompt();
            collision.gameObject.GetComponent<CartMovementScript>().ResetCart();
            graph.GetComponent<LineGraphScript>().StopRecordingAndClearData(true);
            collision.gameObject.GetComponent<CartMovementScript>().SwitchCurrentMode(0);
            
            playButton.GetComponent<PlayLevelScript>().StopPlaying();
            //collision.gameObject.GetComponent<Rigidbody>().Sleep();
            //Debug.Log("Current mode reset");
        }
    }
}
