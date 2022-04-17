using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    GameObject player;
    int gems;
    int corctAnswr;
    int completed;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        /*if(player != null)
        {
            Debug.Log("player found");
        }
        */
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
