using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Failed : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("collided");
        Debug.Log(collision.gameObject.tag);
        if (collision.gameObject.tag == "Player")
            collision.gameObject.transform.position = new Vector3(0.5f, 8.0f, 0.0f);

    }
}
