using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemCollect : MonoBehaviour
{
    int gems = 0;
    // Start is called before the first frame update
    void Awake()
    {
        Debug.Log("Gems collected: " + gems);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Gem")
        {
            gems++;
            Destroy(other.gameObject);
            Debug.Log("Gems collected: " + gems);
        }
    }
}
