using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public GameObject titleScreen;
    public GameObject gameOverScreen;

    public bool isGameActive;

    public void StartGame()
    {
        isGameActive = true;
        titleScreen.SetActive(false);
    }

    public void GameOver()
    {
        isGameActive = false;
        gameOverScreen.SetActive(true);
    }

    public void RestartGame()
    {
        // restarts game by reloading the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
