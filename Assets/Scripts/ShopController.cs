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
        public Button shopButton; // Necesitas asignar el botón de UI correspondiente a cada ShopItem en el Inspector
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
        else
        {
            Debug.LogError("ShopController: No se encontró un GameObject con la etiqueta 'Player'. Asegúrate de que tu jugador tiene esta etiqueta.");
        }

        if (shopUIPanel != null)
        {
            shopUIPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("ShopController: 'shopUIPanel' no está asignado en el Inspector.");
        }

        // Inicializa los conteos de compra para cada item
        foreach (ShopItem item in itemsEnVenta)
        {
            if (!itemPurchaseCounts.ContainsKey(item.nombre))
            {
                itemPurchaseCounts[item.nombre] = 0;
            }
        }

        // Busca el SoundController en la escena
        soundController = FindFirstObjectByType<SoundController>();
        if (soundController == null)
        {
            Debug.LogWarning("ShopController: No se encontró un SoundController en la escena. Los sonidos de la tienda no se reproducirán.");
        }

        // Establece el estado inicial de todos los botones de la tienda
        UpdateAllButtonsState();
    }

    void Update()
    {
        // Alternar la tienda con la tecla 'F'
        if (Input.GetKeyDown(KeyCode.F) && isPlayer)
        {
            ToggleShop();
        }
        // Cerrar la tienda con la tecla 'Escape'
        if (Input.GetKeyDown(KeyCode.Escape) && shopOpen)
        {
            ToggleShop();
        }
    }

    // Función para abrir/cerrar la tienda
    void ToggleShop()
    {
        shopOpen = !shopOpen; // Invierte el estado de shopOpen
        shopUIPanel.SetActive(shopOpen); // Activa o desactiva el panel de UI de la tienda

        if (shopOpen) // Si la tienda se está abriendo
        {
            Time.timeScale = 0f; // Pausa el juego
            if (roombaScript != null)
            {
                roombaScript.sePuedeMover = false; // Deshabilita el movimiento del jugador
            }
            // Sonido al abrir la tienda (ej. índice 5, volumen 0.6f)
            // if (soundController != null) soundController.SeleccionAudio(5, 0.6f, false);
            
            // Actualiza el estado de todos los botones cada vez que la tienda se abre
            UpdateAllButtonsState();
        }
        else // Si la tienda se está cerrando
        {
            Time.timeScale = 1f; // Reanuda el juego
            if (roombaScript != null)
            {
                roombaScript.sePuedeMover = true; // Habilita el movimiento del jugador
            }
            // Opcional: Sonido al cerrar la tienda
            // if (soundController != null) soundController.SeleccionAudio(6, 0.6f, false);
        }
    }

    // Detecta cuando el jugador entra en el área de la tienda (Collider con Is Trigger activado)
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayer = true;
        }
    }

    // Detecta cuando el jugador sale del área de la tienda
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayer = false;
            // Si la tienda está abierta y el jugador sale del trigger, cierra la tienda
            if (shopOpen)
            {
                ToggleShop();
            }
        }
    }

    // Método llamado por los botones de la UI de la tienda (en el OnClick del botón)
    public void ComprarItem(int indiceItem)
    {
        if (indiceItem >= 0 && indiceItem < itemsEnVenta.Length)
        {
            ShopItem itemAComprar = itemsEnVenta[indiceItem];

            // Obtener el conteo actual de compras para este item
            int currentPurchases = itemPurchaseCounts.ContainsKey(itemAComprar.nombre) ? itemPurchaseCounts[itemAComprar.nombre] : 0;

            // Verificar condiciones generales para la compra
            bool canPurchase = true;
            string purchaseMessage = "";

            if (roombaScript == null)
            {
                Debug.LogError("RoombaScript no encontrado al intentar comprar un item.");
                return;
            }

            if (currentPurchases >= itemAComprar.maxPurchases)
            {
                canPurchase = false;
                purchaseMessage = "Has alcanzado el límite de compras para " + itemAComprar.nombre;
                // if (soundController != null) soundController.SeleccionAudio(indiceSonidoError, 0.6f, false);
            }
            else if (roombaScript.money < itemAComprar.coste)
            {
                canPurchase = false;
                purchaseMessage = "No tienes suficiente dinero para " + itemAComprar.nombre;
                // if (soundController != null) soundController.SeleccionAudio(indiceSonidoError, 0.6f, false);
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
                    // if (soundController != null) soundController.SeleccionAudio(indiceSonidoError, 0.6f, false);
                }
            }
            
            // Realizar la compra si todas las condiciones se cumplen
            if (canPurchase)
            {
                roombaScript.money -= itemAComprar.coste; // Resta el dinero
                AplicarMejora(itemAComprar); // Aplica el efecto del item
                itemPurchaseCounts[itemAComprar.nombre]++; // Incrementa el conteo de compras
                // Sonido de compra exitosa (ej. índice 7, volumen 0.8f)
                // if (soundController != null) soundController.SeleccionAudio(7, 0.8f, false);
                Debug.Log("Compra exitosa de " + itemAComprar.nombre);
            }
            else
            {
                Debug.LogWarning("Fallo al comprar " + itemAComprar.nombre + ": " + purchaseMessage);
            }

            // Después de cualquier intento de compra (exitoso o fallido), actualiza el estado de todos los botones
            UpdateAllButtonsState();
        }
    }

    // Aplica la mejora al Roomba
    void AplicarMejora(ShopItem item)
    {
        if (roombaScript == null)
        {
            Debug.LogError("RoombaScript no encontrado al aplicar mejora.");
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
                if (item.nombre == "Jetpack") // Asumo que el desbloqueo del Jetpack se llama "Jetpack"
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

    // Actualiza el estado visual y de interactividad de un solo botón
    void UpdateButtonState(ShopItem item)
    {
        if (item.shopButton == null) return; // Asegura que el botón está asignado

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
                    isRechargeItemAtMax = false; // Otros tipos de items no tienen esta condición
                    break;
            }
        }

        // Condición final para la interactividad del botón
        item.shopButton.interactable = !reachedMaxPurchases && hasEnoughMoney && !isRechargeItemAtMax;

        // Cambiar el color del botón
        Image buttonImage = item.shopButton.GetComponent<Image>();
        if (buttonImage != null)
        {
            if (reachedMaxPurchases)
            {
                buttonImage.color = Color.gray; // Gris: Límite de compras alcanzado
            }
            else if (isRechargeItemAtMax)
            {
                // Un color que indique que la estadística está al máximo (ej. un azul/púrpura suave)
                buttonImage.color = new Color(0.5f, 0.5f, 0.7f);
            }
            else if (!hasEnoughMoney)
            {
                // Un gris más claro: No hay dinero
                buttonImage.color = new Color(0.7f, 0.7f, 0.7f);
            }
            else
            {
                buttonImage.color = Color.white; // Blanco: Comprable
            }
        }
    }

    // Actualiza el estado de interactividad y color de TODOS los botones en la tienda
    void UpdateAllButtonsState()
    {
        foreach (ShopItem item in itemsEnVenta)
        {
            UpdateButtonState(item);
        }
    }
}