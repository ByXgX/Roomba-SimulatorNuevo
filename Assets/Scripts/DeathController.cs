using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathController : MonoBehaviour
{
    public static DeathController Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // Solo si quieres que persista entre cargas de escena (cuidado con esto para reinicios)
        }
    }

    public void ReloadCurrentScene()
    {
        Time.timeScale = 1f;
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        Debug.Log($"Reiniciando escena: {SceneManager.GetActiveScene().name} (Index: {currentSceneIndex})");
        SceneManager.LoadScene(currentSceneIndex);
        
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
     public void QuitGame()
    {
        Application.Quit();
    }
}
