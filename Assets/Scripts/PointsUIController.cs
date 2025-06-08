using UnityEngine;
using TMPro;

public class PointsUIController : MonoBehaviour
{
    [SerializeField] private RoombaScript roomba; 
    [SerializeField] private TMP_Text textoPuntos;
    [SerializeField] private TMP_Text textoPuntosBlan;
    [SerializeField] private TMP_Text textoDinero;
    [SerializeField] private TMP_Text textoDineroBlan;
    [SerializeField] private TMP_Text textoStats;
    private string jet;

    void Update()
    {
        if (roomba.hasJetpack == true)
        {
            jet = "Yes";
        }
        if (roomba != null && textoPuntos != null)
        {
            textoPuntos.text = "Points: " + roomba.points.ToString() + " / 15000";
            textoPuntosBlan.text = "Points: " + roomba.points.ToString()+ " / 15000";

        }
        if (roomba != null && textoDinero != null)
        {
            textoDinero.text = "Money: " + roomba.money.ToString();
            textoDineroBlan.text = "Money: " + roomba.money.ToString();
        }
        if (roomba != null && textoStats != null)
        {
            textoStats.text = "Speed: " + roomba.Velocidad.ToString()+ "\nMax. Batery: " + roomba.bateriaMax.ToString() + "\nBatery: "+ roomba.bateriaActual.ToString("F2") + "\nMax. Health: " + roomba.maxHeal.ToString() + "\nHealth: " + roomba.maxHeal.ToString() + "\nJetpack: "+ jet + "\nMax.Fuel: "+ roomba.maxFuel.ToString()+"\nFuel:"+ roomba.currFuel.ToString() ;
            
        }
    }
}
