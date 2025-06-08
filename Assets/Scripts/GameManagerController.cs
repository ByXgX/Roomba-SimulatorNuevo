using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;


public class GameManagerController : MonoBehaviour
{
   public static GameManagerController Instance;

    public GameObject pauseMenuPanel;
    public GameObject settingsMenuPanel;
    public GameObject controlesPanel;


    private bool isPaused = false;

    private Stack<GameObject> activeUIPanels = new Stack<GameObject>();


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }

        if (settingsMenuPanel != null)
        {
            settingsMenuPanel.SetActive(false);
        }

        if (controlesPanel != null)
        {
            controlesPanel.SetActive(false);
        }
      
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P)) // Puedes usar 'P' o 'Escape'
        {
            if (activeUIPanels.Count > 0)
            {
                CloseTopPanel();
            }
            else if (!isPaused)
            {
                OpenPauseMenu();
            }
        }
    }
    public void OpenPauseMenu()
    {
        if (pauseMenuPanel != null)
        {
            PushPanel(pauseMenuPanel);
            Time.timeScale = 0f; 
            isPaused = true;
        }
    }

    public void ResumeGame()
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
            Time.timeScale = 1f;
            isPaused = false;
            Debug.Log("Juego Reanudado.");
        }
    }
    
    public void OpenSettingsMenu()
    {
        if (settingsMenuPanel != null)
        {
            PushPanel(settingsMenuPanel); 
            Debug.Log("Menú de Ajustes abierto.");
        }
    }

    public void openControlesPanel()
    {
        if (controlesPanel != null)
        {
            PushPanel(controlesPanel); 
        }
    }

     private void PushPanel(GameObject panel)
    {
        if (activeUIPanels.Count > 0)
        {
            activeUIPanels.Peek().SetActive(false);
        }
        panel.SetActive(true);
        activeUIPanels.Push(panel);
    }

    private void CloseTopPanel()
    {
        if (activeUIPanels.Count > 0)
        {
            GameObject topPanel = activeUIPanels.Pop();
            topPanel.SetActive(false);
            Debug.Log(topPanel.name + " cerrado.");

            if (activeUIPanels.Count > 0)
            {
                activeUIPanels.Peek().SetActive(true);
            }
            else 
            {
                ResumeGame();
            }
        }
    }

    public void GoToMainMenu()
    {
        ResumeGame();
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
