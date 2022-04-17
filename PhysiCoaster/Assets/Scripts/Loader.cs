using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour
{
    int LevelID = 0;
    public void OnPressed(int LevelID)
    {
        SceneManager.LoadScene(LevelID);
    }
}
