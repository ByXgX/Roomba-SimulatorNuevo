using UnityEngine;

public class PowerUpController : MonoBehaviour
{
    public float duration = 15f;   
    public float value = 2f;

    private SoundController soundController;
    void Start()
    {
        soundController = FindFirstObjectByType<SoundController>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RoombaScript roomba = null;
            roomba = other.GetComponent<RoombaScript>();
            roomba.ActivatePuntosPowerUp(duration, value);
            soundController.SeleccionAudio(13, 0.8f, false);
            Destroy(this.gameObject);
           
        }
    }
}
