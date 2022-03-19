using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Failed : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            other.gameObject.transform.position = new Vector3(0.5f, 8.0f, 0.0f);

    }
}
