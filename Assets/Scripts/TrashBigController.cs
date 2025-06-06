using UnityEngine;

public class TrashBigController : MonoBehaviour
{
    private SoundController soundController;
    private void Start()
    {

        soundController = FindFirstObjectByType<SoundController>();

    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RoombaScript roomba = other.GetComponent<RoombaScript>();
            roomba.money += 50;
            roomba.points += 100;
            soundController.SeleccionAudio(7, 0.8f, false); 
            Destroy(gameObject);
        }
    }
}
