using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class MainMenuController : MonoBehaviour
{
    public GameObject videoPlayerPanel;
    public VideoPlayer videoPlayer;
    public float videoDuration = 10f;
    
    public void GoToStart()
    {
        SceneManager.LoadScene("OutDoorsScene");
    }
    public void GoStart()
    {
        if (videoPlayerPanel != null)
        {
            videoPlayerPanel.SetActive(true);
        }
        else
        {
            return;
        }
        if (videoPlayer != null)
        {
            videoPlayer.Play();
            StartCoroutine(WaitToScene());
        }
        else
        {
            SceneManager.LoadScene("OutDoorsScene"); 
        }
    }
    private IEnumerator WaitToScene()
    {
        yield return new WaitForSeconds(10f);

        Debug.Log("Tiempo terminado. Cambiando de escena a OutDoorsScene.");

        // Opcional: Ocultar el panel del video y detener el VideoPlayer (limpieza visual)
       
        if (videoPlayer != null)
        {
            videoPlayer.Stop();
        }
        SceneManager.LoadScene("OutDoorsScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
