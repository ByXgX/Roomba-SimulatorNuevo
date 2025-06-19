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
        UpdateAllButtonsState();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && isPlayer)
        {
            ToggleShop();
        }
        if (Input.GetKeyDown(KeyCode.Escape) && shopOpen)
        {
            ToggleShop();
        }
    }

    void ToggleShop()
    {
        shopOpen = !shopOpen; 
        shopUIPanel.SetActive(shopOpen); 

        if (shopOpen)
        {
            Time.timeScale = 0f;
            soundController.SeleccionAudio(5, 0.6f, false);
            if (roombaScript != null)
            {
                roombaScript.sePuedeMover = false;
            } 
            UpdateAllButtonsState();
        }
        else
        {
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
                ToggleShop();
            }
        }
    }
    public void ComprarItem(int indiceItem)
    {
        if (indiceItem >= 0 && indiceItem < itemsEnVenta.Length)
        {
            ShopItem itemAComprar = itemsEnVenta[indiceItem];

            int currentPurchases = itemPurchaseCounts.ContainsKey(itemAComprar.nombre) ? itemPurchaseCounts[itemAComprar.nombre] : 0;

            bool canPurchase = true;
            string purchaseMessage = "";

            if (roombaScript == null)
            {
                return;
            }

            if (currentPurchases >= itemAComprar.maxPurchases)
            {
                canPurchase = false;
                purchaseMessage = "Has alcanzado el límite de compras para " + itemAComprar.nombre;
            }
            else if (roombaScript.money < itemAComprar.coste)
            {
                canPurchase = false;
                purchaseMessage = "No tienes suficiente dinero para " + itemAComprar.nombre;
            }

            // Verificación adicional para items de recarga si ya están al máximo
            bool isRechargeItemAtMax = false;
            if (canPurchase) // Solo verifica si la compra es posible hasta ahora
            {
                switch (itemAComprar.tipo)
                {
                    case ShopItem.TipoMejora.Bateria:
                        // Asegúrate de que roombaScript.currBattery y roombaScript.maxBattery sean accesibles y correctos
                        isRechargeItemAtMax = (roombaScript.bateriaActual >= roombaScript.bateriaMax);
                        break;
                    case ShopItem.TipoMejora.RecargaCombustible:
                        // Asegúrate de que roombaScript.currFuel y roombaScript.maxFuel sean accesibles y correctos
                        isRechargeItemAtMax = (roombaScript.currFuel >= roombaScript.maxFuel);
                        break;
                    case ShopItem.TipoMejora.Vida:
                        isRechargeItemAtMax = (roombaScript.currHeal >= roombaScript.maxHeal);
                        break;
                    default:
                        // Para otros tipos de items, no se aplica la condición de "estar al máximo"
                        isRechargeItemAtMax = false;
                        break;
                }

                if (isRechargeItemAtMax)
                {
                    canPurchase = false;
                    purchaseMessage = "¡Ya estás al máximo de " + itemAComprar.nombre + "!";
                }
            } 
            if (canPurchase)
            {
                roombaScript.money -= itemAComprar.coste; 
                AplicarMejora(itemAComprar); 
                itemPurchaseCounts[itemAComprar.nombre]++;
            }
            else
            {
                Debug.LogWarning("Fallo al comprar " + itemAComprar.nombre + ": " + purchaseMessage);
            }
            UpdateAllButtonsState();
        }
    }

    void AplicarMejora(ShopItem item)
    {
        if (roombaScript == null)
        {
            return;
        }

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

        bool isRechargeItemAtMax = false;
        if (roombaScript != null)
        {
            switch (item.tipo)
            {
                case ShopItem.TipoMejora.Bateria:
                    isRechargeItemAtMax = (roombaScript.bateriaActual >= roombaScript.bateriaMax);
                    break;
                case ShopItem.TipoMejora.RecargaCombustible:
                    isRechargeItemAtMax = (roombaScript.currFuel >= roombaScript.maxFuel);
                    break;
                case ShopItem.TipoMejora.Vida:
                    isRechargeItemAtMax = (roombaScript.currHeal >= roombaScript.maxHeal);
                    break;
                default:
                    isRechargeItemAtMax = false;
                    break;
            }
        }

        item.shopButton.interactable = !reachedMaxPurchases && hasEnoughMoney && !isRechargeItemAtMax;
        Image buttonImage = item.shopButton.GetComponent<Image>();
        if (buttonImage != null)
        {
            if (reachedMaxPurchases)
            {
                buttonImage.color = Color.gray;
            }
            else if (isRechargeItemAtMax)
            {
                buttonImage.color = new Color(0.5f, 0.5f, 0.7f);
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