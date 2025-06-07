using UnityEngine;

public class TrashSmallController : MonoBehaviour
{
    public int pointsToAdd = 50;
    public int moneyToAdd = 10;

    private SoundController soundController;

    private void Start()
    {
        soundController = FindFirstObjectByType<SoundController>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RoombaScript roomba = null;
            roomba = other.GetComponent<RoombaScript>();
            roomba.AddPointsAndMoney(pointsToAdd, moneyToAdd);
            soundController.SeleccionAudio(7, 0.8f, false); 
            Destroy(gameObject);
        }
    }
}
