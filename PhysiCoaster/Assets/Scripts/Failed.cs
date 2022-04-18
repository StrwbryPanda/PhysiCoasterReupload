using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Failed : MonoBehaviour
{
    GameObject starting;
    GameObject graph;
    private void Start()
    {
        starting = GameObject.FindGameObjectWithTag("Start");
        graph = GameObject.FindGameObjectWithTag("Graph");
    }
    private void OnCollisionEnter(Collision collision)
    {
        // Debug.Log("collided");
        // Debug.Log(collision.gameObject.tag);
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.transform.position = starting.transform.position + new Vector3(0.0f, 0.365f, 0.0f);
            graph.GetComponent<LineGraphScript>().StopRecordingAndClearData(false);
            collision.gameObject.GetComponent<CartMovementScript>().SwitchCurrentMode(0);
            //collision.gameObject.GetComponent<Rigidbody>().Sleep();
            //Debug.Log("Current mode reset");
        }
    }
}
