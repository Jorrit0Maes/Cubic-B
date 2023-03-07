using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameClicked : MonoBehaviour
{
    public void clicky()
    {
        SceneManager.LoadScene("Game");
    }
}
