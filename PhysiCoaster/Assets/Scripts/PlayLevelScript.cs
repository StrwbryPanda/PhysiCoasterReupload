using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayLevelScript : MonoBehaviour
{
    GameObject graph;
    bool playing;
    GameObject cart;
    // Start is called before the first frame update
    void Start()
    {
        graph = GameObject.Find("Line Graph Container");
        cart = GameObject.Find("Cart Anchor");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayLevel()
    {
        if (!playing)
        {
            playing = true;
            graph.GetComponent<LineGraphScript>().UpdateValues();
            graph.GetComponent<LineGraphScript>().StartRecording();
            cart.GetComponent<CartMovementScript>().SwitchCurrentMode(1);
        }
        
    }

    public void StopPlaying()
    {
        playing = false;
    }
}
