using UnityEngine;

public class BgMusicController : MonoBehaviour
{
     public static BgMusicController instance;

    private AudioSource audioSource; // Referencia al AudioSource en este GameObject

    void Awake()
    {
        if (instance == null)
        {
            instance = this; 
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>(); 
            if (audioSource != null && !audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else 
        {
            Destroy(gameObject);
        }
    }
}
