using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    
    public void GoToStart()
    {
        SceneManager.LoadScene("OutDoorsScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
