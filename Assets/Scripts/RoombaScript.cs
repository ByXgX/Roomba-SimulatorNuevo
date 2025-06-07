using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoombaScript : MonoBehaviour
{
    public float speed = 3f;
    public float rotationSpeed = 100f;
    public TextMeshProUGUI moneyText;
    public float money = 50;
    public float points = 0;
    public float bateriaMax = 100f;
    public float bateriaActual = 100f;
    public float maxHeal = 100f;
    public float currHeal;
    public float perdidaBateriaPorDistancia = 0.1f;
    //private Vector3 posicionAnterior;
    public bool sePuedeMover = true;

    //---------------- win ------------------
    public float puntosGanar = 14500;
    public GameObject winScreenPanel;
    //---------- jetpack --------
    public float maxFuel = 50f;
    public float currFuel;
    public float jForce = 0.5f;
    public Rigidbody rb;
    public Transform groundTransform;
    //public ParticleSystem effect;
    public bool hasJetpack = false;
    private bool jumpInputHeld; //poner en 10 y max 12

    //--------------Die--------------
    public Renderer playerRenderer;
    public Color damageColor = Color.red;
    public Color originalColor;
    public GameObject deathScreenPanel;

    //--------------------- bars ---------------------
    public Slider batteryUI;
    public Slider fuelUI;
    public Slider healthUI;

    //------------------------- powerUp --------------------

    private float pointMultiplier = 1f;       
    private Coroutine puntosX2Coroutine;
    public GameObject powerUpBox;
    public TextMeshProUGUI powerUpTime;
    public TextMeshProUGUI powerUpTimeBlan;
    private float powerUpTimeRemaining;
    private bool isPowerUp = false;

    //------------sonidos--------------------

    private SoundController soundController;
    bool reproduciendoSonido = false;

    public float Velocidad
    {
        get { return speed; }
        set { speed = Mathf.Max(0f, value); }
    }

    public float Vida
    {
        get { return currHeal; }
        set { currHeal = Mathf.Clamp(value, 0f, maxHeal); }
    }

    public float Bateria
    {
        get { return bateriaActual; }
        set { bateriaActual = Mathf.Clamp(value, 0f, bateriaMax); }
    }

    public float Combustible
    {
        get { return currFuel; }
        set { currFuel = Mathf.Clamp(value, 0f, maxFuel); }
    }
    void Start()
    {
        //posicionAnterior = transform.position;
        currFuel = maxFuel;
        if (deathScreenPanel != null)
        {
            deathScreenPanel.SetActive(false);
        }
        if (powerUpBox != null)
        {
            powerUpBox.SetActive(false);
        }
        soundController = FindFirstObjectByType<SoundController>();
        originalColor = playerRenderer.material.color;
        sePuedeMover = true;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        currFuel = maxFuel;
        Vida = maxHeal;
        bateriaActual = bateriaMax;
        
    }

    void Update()
    {
        jumpInputHeld = Input.GetAxis("Jump") > 0f;
        UpdateSlider();
        moneyText.text = "Money: " + money.ToString();
        powerUpTimeRemaining -= Time.deltaTime;
        if (powerUpTimeRemaining < 0)
        {
            powerUpTimeRemaining = 0;
        }
        UpdatePowerUp();
        if (points >= puntosGanar)
        {
            GanarJuego();
        }
    }

    void FixedUpdate()
    {
        if (sePuedeMover)
        {
            // --- Movimiento Horizontal ---
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            float verticalInput = Input.GetAxisRaw("Vertical");

            Vector3 desiredMoveDirection = transform.right * (horizontalInput * -1) + transform.forward * (verticalInput * -1);
            desiredMoveDirection.y = 0f;
            rb.linearVelocity = new Vector3(desiredMoveDirection.normalized.x * Velocidad, rb.linearVelocity.y, desiredMoveDirection.normalized.z * Velocidad);

            bool seMueve = desiredMoveDirection.magnitude > 0.1f;

            if (seMueve && !reproduciendoSonido)
            {
                soundController.SeleccionAudio(4, 0.1f, true);
                reproduciendoSonido = true;
            }
            else if (!seMueve && reproduciendoSonido)
            {
                soundController.DetenerAudio();
                reproduciendoSonido = false;
            }

            // --- Pérdida de Bateria Útil ---
            if (rb.linearVelocity.magnitude > 0.01f)
            {
                float distanciaDesplazada = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z).magnitude * Time.fixedDeltaTime;
                PerderBateria(distanciaDesplazada);
            }

            // --- Rotación con Q y E ---
            float rotationInput = 0f;
            if (Input.GetKey(KeyCode.E))
            {
                rotationInput = 1f;
            }
            else if (Input.GetKey(KeyCode.Q))
            {
                rotationInput = -1f;
            }
            transform.Rotate(Vector3.up * rotationInput * rotationSpeed * Time.fixedDeltaTime);

            if (hasJetpack)
            {

                // --- Lógica del Jetpack  ---
                if (jumpInputHeld && currFuel > 0f)
                {
                    currFuel -= Time.fixedDeltaTime;
                    rb.AddForce(Vector3.up * jForce, ForceMode.Acceleration);

                    //if (effect != null && !effect.isPlaying) effect.Play();
                }
                else
                {
                    //if (effect != null && effect.isPlaying) effect.Stop();


                    bool isGrounded = Physics.Raycast(groundTransform.position, Vector3.down, 0.2f, LayerMask.GetMask("Grounded"));

                    if (isGrounded && currFuel < maxFuel)
                    {
                        currFuel += Time.fixedDeltaTime;
                    }
                }
            }
        }
        else
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            //if (effect != null && effect.isPlaying) effect.Stop();
            soundController.DetenerAudio();
        }
        currFuel = Mathf.Clamp(currFuel, 0f, maxFuel);
    }

    //--------------- x2 -------------------
    
    public void ActivatePuntosPowerUp(float duration, float multiplierValue)
    {
        if (puntosX2Coroutine != null)
        {
            StopCoroutine(puntosX2Coroutine);
        }
        puntosX2Coroutine = StartCoroutine(PuntosX2PowerUp(duration, multiplierValue));
    }

    // --- CORRUTINA  ---
    System.Collections.IEnumerator PuntosX2PowerUp(float duration, float multiplierValue)
    {
        pointMultiplier = multiplierValue;
        isPowerUp = true;
        powerUpTimeRemaining = duration;
        
        if (powerUpBox != null)
        {
            powerUpBox.SetActive(true);
        }

        while (powerUpTimeRemaining > 0)
        {
            yield return null;
        }

        //yield return new WaitForSeconds(duration); 
        pointMultiplier = 1f;
        isPowerUp = false;
        if (powerUpBox != null)
        {
            powerUpBox.SetActive(false);
        }

        puntosX2Coroutine = null;
    }

    void UpdatePowerUp()
    {
        if (powerUpTime != null)
        {
            int seconds = Mathf.CeilToInt(powerUpTimeRemaining);
            powerUpTime.text = "x2: " + seconds.ToString() + " sec";
            powerUpTimeBlan.text = "x2: " + seconds.ToString() + " sec";
        }
    }

    public float GetCurrentPointMultiplier()
    {
        return pointMultiplier;
    }

    public void AddPointsAndMoney(int basePointsToAdd, int baseMoneyToAdd)
    {
        points += basePointsToAdd * pointMultiplier;
        money += baseMoneyToAdd * pointMultiplier;
    }

    //-------------vida--------------
    public void RecibirDano(float cantidad)
    {

        Vida -= cantidad;
        playerRenderer.material.color = damageColor;
        Invoke("ResetColor", 0.2f);

        if (Vida <= 0)
        {
            Vida = 0;
            Morir();
        }
    }
    void ResetColor()
    {
        playerRenderer.material.color = originalColor;
    }

    public void GanarJuego()
    {
        if (!sePuedeMover) return;
        sePuedeMover = false;
        Time.timeScale = 0f;

        if (soundController != null)
        {
            soundController.DetenerAudio();
        }
        SceneManager.LoadScene("WinScene");
    }

    //-----------bateria-------------
    void PerderBateria(float distancia)
    {
        bateriaActual -= distancia * perdidaBateriaPorDistancia;

        if (bateriaActual < 0f)
        {
            bateriaActual = 0f;
            Morir();
        }

    }
    public void AumentarVelocidad(float incremento)
    {
        Velocidad += incremento;
        Debug.Log("Velocidad aumentada a: " + Velocidad);
        soundController.SeleccionAudio(2, 0.8f,false);
    }
    public void RecargarBateria(float incremento)
    {
        float oldBateria = bateriaActual;
        bateriaActual = Mathf.Min(bateriaActual + incremento, bateriaMax);
        soundController.SeleccionAudio(2, 0.8f,false);

    }
    public void AumentarBateriaMax(float incremento)
    {
        bateriaMax += incremento;
        Bateria = bateriaMax;
        soundController.SeleccionAudio(2, 0.8f,false);

    }
    public void RecargarFuel(float incremento)
    {
        float oldFuel = currFuel;
        currFuel = Mathf.Min(currFuel + incremento, maxFuel);
        soundController.SeleccionAudio(2, 0.8f,false);

    }
    public void AumentarCombustibleMax(float incremento)
    {
        maxFuel += incremento;
        Combustible = maxFuel;
        soundController.SeleccionAudio(2, 0.8f,false);
    }
    public void RecargarVida(float incremento)
    {
        float oldHeal = currHeal;
        currHeal = Math.Min(currHeal + incremento, maxHeal);
        soundController.SeleccionAudio(2, 0.8f,false);
    }
    public void AumentarVidaMax(float incremento)
    {
        maxHeal += incremento;
        Vida = maxHeal;
        soundController.SeleccionAudio(2, 0.8f,false);
    }

    //------------------ Sliders -----------------------

    public void UpdateSlider()
    {
        fuelUI.value = currFuel / maxFuel;
        batteryUI.value = bateriaActual / bateriaMax;
        healthUI.value = currHeal / maxHeal;
    }

    public void Morir()
    {
        sePuedeMover = false;
        deathScreenPanel.SetActive(true);
        Time.timeScale = 0f;
        soundController.SeleccionAudio(3, 0.5f,false);
        soundController.SeleccionAudio(4, 0,false);
    }
}

