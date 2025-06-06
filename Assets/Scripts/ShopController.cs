using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopController : MonoBehaviour
{
    private RoombaScript roombaScript;

    public GameObject shopUIPanel;
    private bool isPlayer = false;
    private bool shopOpen = false;

    [System.Serializable]
    public struct ShopItem
    {
        public string nombre;
        public int coste;
        public enum TipoMejora { Velocidad, Bateria, BateriaMaxima, Desbloqueo, RecargaCombustible, CombustibleMaximo, Vida, VidaMaxima }
        public TipoMejora tipo;
        public float valor;
        public int maxPurchases;
        public Button shopButton;
    }

    public ShopItem[] itemsEnVenta;
    private Dictionary<string, int> itemPurchaseCounts = new Dictionary<string, int>();

    private SoundController soundController;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            roombaScript = player.GetComponent<RoombaScript>();
        }
        if (shopUIPanel != null)
        {
            shopUIPanel.SetActive(false);
        }
        foreach (ShopItem item in itemsEnVenta)
        {
            if (!itemPurchaseCounts.ContainsKey(item.nombre))
            {
                itemPurchaseCounts[item.nombre] = 0;
            }
        }
        soundController = FindFirstObjectByType<SoundController>();

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && (isPlayer == true))
        {
            if (shopOpen)
            {
                shopUIPanel.SetActive(false);
                shopOpen = false;
                Time.timeScale = 1f;
                if (roombaScript != null)
                {
                    roombaScript.sePuedeMover = true;
                }
            }
            else
            {
                shopUIPanel.SetActive(true);
                shopOpen = true;
                Time.timeScale = 0f;
                if (roombaScript != null)
                {
                    roombaScript.sePuedeMover = false;
                }
                soundController.SeleccionAudio(5, 0.6f, false); 
            }
    }
    if (Input.GetKeyDown(KeyCode.Escape) && shopOpen)
    {
        shopUIPanel.SetActive(false);
        shopOpen = false;
        Time.timeScale = 1f;
        if (roombaScript != null)
        {
            roombaScript.sePuedeMover = true;
        }
    }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayer = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayer = false;
            if (shopOpen)
            {
                shopUIPanel.SetActive(false);
                shopOpen = false;
                Time.timeScale = 1f;
                if (roombaScript != null)
                {
                    roombaScript.sePuedeMover = true;
                }
            }
        }
    }

    public void ComprarItem(int indiceItem)
    {

        if (indiceItem >= 0 && indiceItem < itemsEnVenta.Length)
        {
            ShopItem itemAComprar = itemsEnVenta[indiceItem];

            int currentPurchases = itemPurchaseCounts.ContainsKey(itemAComprar.nombre) ? itemPurchaseCounts[itemAComprar.nombre] : 0;


            if (currentPurchases < itemAComprar.maxPurchases && roombaScript.money >= itemAComprar.coste)
            {
                roombaScript.money -= itemAComprar.coste;
                AplicarMejora(itemAComprar);
                itemPurchaseCounts[itemAComprar.nombre]++;
                UpdateButtonState(itemAComprar);
            }
        }
    }

    void AplicarMejora(ShopItem item)
    {

        int currentLevelForStat = itemPurchaseCounts.ContainsKey(item.nombre) ? itemPurchaseCounts[item.nombre] + 1 : 1;
        switch (item.tipo)
        {
            case ShopItem.TipoMejora.Velocidad:
                roombaScript.AumentarVelocidad(item.valor);
                break;
            case ShopItem.TipoMejora.Bateria:
                roombaScript.RecargarBateria(item.valor);

                break;
            case ShopItem.TipoMejora.BateriaMaxima:
                roombaScript.AumentarBateriaMax(item.valor);
                break;
            case ShopItem.TipoMejora.Desbloqueo:
                if (item.nombre == "Jetpack")
                {
                    roombaScript.hasJetpack = true;
                }
                break;
            case ShopItem.TipoMejora.RecargaCombustible:
                roombaScript.RecargarFuel(item.valor);
                break;
            case ShopItem.TipoMejora.CombustibleMaximo:
                roombaScript.AumentarCombustibleMax(item.valor);
                break;
            case ShopItem.TipoMejora.Vida:
                roombaScript.RecargarVida(item.valor);
                break;
            case ShopItem.TipoMejora.VidaMaxima:
                roombaScript.AumentarVidaMax(item.valor);
                break;
            default:
                Debug.LogError("Tipo de mejora desconocido: " + item.tipo);
                break;
        }
    }

    void UpdateButtonState(ShopItem item)
    {
        if (item.shopButton == null) return; 

        int currentPurchases = itemPurchaseCounts.ContainsKey(item.nombre) ? itemPurchaseCounts[item.nombre] : 0;

        bool reachedMaxPurchases = currentPurchases >= item.maxPurchases;
        bool hasEnoughMoney = (roombaScript != null && roombaScript.money >= item.coste);

        item.shopButton.interactable = !reachedMaxPurchases && hasEnoughMoney;

        // Cambiar el color del botón
        Image buttonImage = item.shopButton.GetComponent<Image>();
        if (buttonImage != null)
        {
            if (reachedMaxPurchases)
            {
                buttonImage.color = Color.gray;
            }
            else if (!hasEnoughMoney)
            {
                buttonImage.color = new Color(0.7f, 0.7f, 0.7f); 
            }
            else
            {
                buttonImage.color = Color.white;
            }
        }
    }
    
    void UpdateAllButtonsState()
    {
        foreach (ShopItem item in itemsEnVenta)
        {
            UpdateButtonState(item); 
        }
        
    }
}