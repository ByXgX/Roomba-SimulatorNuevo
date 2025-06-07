using UnityEngine;

public class PilaController : MonoBehaviour
{
    public float cantidadRecargaBateria = 20f;
    private SoundController soundController;

    void Start()
    {
        soundController = FindFirstObjectByType<SoundController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            RoombaScript detectedRoomba = other.GetComponent<RoombaScript>();

            if (detectedRoomba != null)
            {
                detectedRoomba.RecargarBateria(cantidadRecargaBateria); 

                if (soundController != null)
                {
                    soundController.SeleccionAudio(6, 0.8f, false);
                }
                
                Destroy(this.gameObject); 
            }
           
        }
    }

}
