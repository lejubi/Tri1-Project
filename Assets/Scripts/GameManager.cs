using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool deathScreen = false;
    public bool homeScreen = false;
    public bool platformerScreen = false;
    public bool winScreen = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SceneManager.LoadScene("Start");
    }

    private void Update()
    {
        if (deathScreen)
        {
            SceneManager.LoadScene("Death");
            deathScreen = false;
        }
        else if (homeScreen)
        {
            SceneManager.LoadScene("Start");
            homeScreen = false;
        }
        else if (platformerScreen)
        {
            SceneManager.LoadScene("Platformer");
            platformerScreen = false;
        }
        else if (winScreen)
        {
            SceneManager.LoadScene("Win");
            winScreen = false;
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Start");
    }
}