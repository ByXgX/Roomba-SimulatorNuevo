using UnityEngine;

public class PilaController : MonoBehaviour
{
   public float cantidadRecargaBateria = 20f;

    public RoombaScript roomba;
    private SoundController soundController;

    void Start()
    {
        soundController = FindFirstObjectByType<SoundController>();
    }
    private void OnTriggerEnter(Collider other)
    {
        roomba = other.GetComponent<RoombaScript>();
        roomba.RecargarBateria(cantidadRecargaBateria);
        soundController.SeleccionAudio(6, 0.8f, false);
        Destroy(this.gameObject);
    }

}
