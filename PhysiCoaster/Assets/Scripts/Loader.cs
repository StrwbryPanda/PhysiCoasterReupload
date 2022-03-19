using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour
{
    string levelID;
    // Start is called before the first frame update
    void Start()
    {
        levelID = "Level";
    }

    public void OnPressed()
    {
        SceneManager.LoadScene(levelID);
    }




}
