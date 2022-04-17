using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Failed : MonoBehaviour
{
    public GameObject graph;
    public Vector3 startPosition;

    private void Start()
    {
        if (GameObject.FindGameObjectsWithTag("Player")[0]!=null)
        {
            startPosition = GameObject.FindGameObjectsWithTag("Player")[0].transform.position;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("collided");
        Debug.Log(collision.gameObject.tag);
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.transform.position = startPosition;
            collision.gameObject.GetComponent<CartMovementScript>().SwitchCurrentMode(0);
            graph.GetComponent<LineGraphScript>().StopRecordingAndClearData(false);
            
        }
            


    }
}
