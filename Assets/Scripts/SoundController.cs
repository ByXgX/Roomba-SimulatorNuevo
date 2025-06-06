using UnityEngine;

public class SoundController : MonoBehaviour
{
    [SerializeField] private AudioClip[] audios;
    private AudioSource controlAudio;

    private void Awake()
    {
        controlAudio = GetComponent<AudioSource>();
    }

    public void SeleccionAudio(int indice, float volumen, bool loop)
    {
        if (loop)
        {
            controlAudio.clip = audios[indice];
            controlAudio.loop = true;
            controlAudio.volume = volumen;
            controlAudio.Play();
        }
        else
        {
            controlAudio.loop = false;
            controlAudio.PlayOneShot(audios[indice], volumen);
        }
    }

    public void DetenerAudio()
    {
        controlAudio.Stop();
    }
}
