using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour
{
    public int LevelID;
    public void OnPressed(int LevelID)
    {
        SceneManager.LoadScene(LevelID);
    }
}
