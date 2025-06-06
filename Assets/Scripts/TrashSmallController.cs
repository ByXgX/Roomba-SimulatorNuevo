using UnityEngine;

public class TrashSmallController : MonoBehaviour
{
    private SoundController soundController;
    private void Start() {

        soundController = FindFirstObjectByType<SoundController>();

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RoombaScript roomba = other.GetComponent<RoombaScript>();
            roomba.money += 10;
            roomba.points += 50;
            soundController.SeleccionAudio(7, 0.8f, false); 
            Destroy(gameObject);
        }
    }
}
