using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishedScript : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            SceneManager.LoadScene("VictoryScreen");
        }else if (collision.gameObject.CompareTag("Machine"))
        {
            SceneManager.LoadScene("DefeatScreen");

        }
    }
}
