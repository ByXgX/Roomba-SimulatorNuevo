using UnityEngine;

public class CamaraController : MonoBehaviour
{
    [Header("Referencias")]
    public Transform target; // Asigna tu GameObject "Pivot" aquí en el Inspector
    public LayerMask obstacleLayer; // Qué capas la cámara debe considerar como obstáculos

    [Header("Configuración de la Cámara")]
    public float defaultDistance = 4f; // Distancia normal de la cámara al target
    public float minDistance = 0.5f; // Distancia mínima al target cuando choca con un obstáculo
    public float heightOffset = 0.5f; // Altura a la que se sitúa la cámara por encima del target
    public float smoothing = 5f; // Velocidad de suavizado del movimiento de la cámara
    public float rotationSpeed = 5f; // Velocidad de rotación de la cámara (si la gestionas aquí)

    private Vector3 currentVelocity = Vector3.zero; // Para SmoothDamp (no usado en este ejemplo)
    private float currentDistance; // La distancia actual de la cámara al target
    
    // --- NUEVAS VARIABLES DE DEPURACIÓN ---
    public bool showDebugLines = true;
    public float debugLineDuration = 0.1f;

    void Awake()
    {
        if (target == null)
        {
            GameObject pivotObject = GameObject.Find("Pivot");
            if (pivotObject != null)
            {
                target = pivotObject.transform;
                Debug.Log("Target 'Pivot' encontrado automáticamente.");
            }
            else
            {
                Debug.LogError("ERROR: No se encontró un GameObject llamado 'Pivot'. Asigna el target en el Inspector.");
                enabled = false; 
                return;
            }
        }

        currentDistance = defaultDistance;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Calculamos la posición base donde estaría la cámara si no hubiera obstáculos, manteniendo la altura.
        // Siempre usamos target.up para la altura y -target.forward para la distancia detrás.
        Vector3 desiredCameraPosition = target.position + target.up * heightOffset - target.forward * defaultDistance;

        // Punto de inicio del rayo para la detección de obstáculos
        // Este punto debe estar en una posición que asegure que el rayo no se inicia dentro del collider de la pared.
        // A menudo es el propio target, o el target ligeramente elevado para evitar el suelo.
        // Para este caso, vamos a usar el target, que está dentro del Roomba, un poco elevado.
        Vector3 rayStartPoint = target.position + Vector3.up * heightOffset; // El rayo sale del pivot, elevado.
        
        RaycastHit hit;
        
        // El rayo va desde el punto de inicio del rayo hasta la posición deseada de la cámara
        if (Physics.Linecast(rayStartPoint, desiredCameraPosition, out hit, obstacleLayer))
        {
            // --- DETECCIÓN DE COLISIÓN ---
            // Si el rayo golpea un obstáculo
            // Ajustamos la distancia basándonos en la colisión, manteniendo un margen mínimo.
            currentDistance = Mathf.Max(minDistance, hit.distance - 0.1f);
            
            // Recalculamos la posición final deseada usando la distancia ajustada,
            // pero crucialmente, manteniendo la altura fija con 'heightOffset'.
            // La cámara se moverá a 'currentDistance' detrás del target, manteniendo la altura.
            desiredCameraPosition = target.position + target.up * heightOffset - target.forward * currentDistance;

            // --- DEPURACIÓN: Rayo que GOLPEA ---
            if (showDebugLines)
            {
                Debug.DrawLine(rayStartPoint, hit.point, Color.green, debugLineDuration);
                Debug.DrawLine(hit.point, desiredCameraPosition, Color.yellow, debugLineDuration);
            }
            Debug.Log($"RAYCAST HIT: Hit point: {hit.point}, Distance: {hit.distance}, Current Distance: {currentDistance}, Obstáculo: {hit.collider.gameObject.name}, Layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)}");
            
            // Suaviza el movimiento hacia la posición final ajustada
            transform.position = Vector3.Lerp(transform.position, desiredCameraPosition, Time.deltaTime * smoothing);
        }
        else
        {
            // --- NO HAY COLISIÓN ---
            // Si no hay obstáculo, la cámara vuelve a su distancia por defecto
            currentDistance = defaultDistance;
            
            // La posición deseada ya está calculada como 'desiredCameraPositionNoObstacle' al inicio de LateUpdate.
            // Opcional: Para mayor claridad, podrías recalcularla aquí, aunque no es estrictamente necesario:
            // desiredCameraPosition = target.position + target.up * heightOffset - target.forward * defaultDistance;

            // --- DEPURACIÓN: Rayo que NO GOLPEA ---
            if (showDebugLines)
            {
                Debug.DrawLine(rayStartPoint, desiredCameraPosition, Color.red, debugLineDuration);
            }
            Debug.Log($"RAYCAST NO HIT: Distancia por defecto: {defaultDistance}");

            // Suaviza el movimiento hacia la posición por defecto
            transform.position = Vector3.Lerp(transform.position, desiredCameraPosition, Time.deltaTime * smoothing);
        }

        // --- Rotación de la Cámara (Para que siempre mire al target) ---
        // Mira al target, ajustando un poco la altura para una mejor vista
        transform.LookAt(target.position + Vector3.up * heightOffset / 2);
    }

    // Para ver el raycast en el editor (persistente)
    void OnDrawGizmos()
    {
        if (target != null && showDebugLines)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(target.position, 0.1f); // Posición del Pivot
            Gizmos.DrawWireSphere(target.position + Vector3.up * heightOffset, 0.15f); // Punto de inicio del rayo
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position, 0.1f); // Posición actual de la cámara
        }
    }

    // Para depurar el progreso del tutorial (si tienes el script TutorialController)
    [ContextMenu("Log Tutorial Seen Status")]
    void LogTutorialStatus()
    {
        bool tutorialSeen = PlayerPrefs.GetInt("HasPlayedNonIntrusiveTutorial", 0) == 1;
        Debug.Log($"Estado del Tutorial 'HasPlayedNonIntrusiveTutorial': {tutorialSeen}");
    }
}
