using UnityEngine;

public class CamaraController : MonoBehaviour
{
    [Header("Referencias")]
    public Transform target;    public LayerMask obstacleLayer; 

    [Header("Configuración de la Cámara")]
    public float defaultDistance = 4f; 
    public float minDistance = 0.5f; 
    public float heightOffset = 0.5f; 
    public float smoothing = 5f;
    public float rotationSpeed = 5f; 

    private Vector3 currentVelocity = Vector3.zero;
    private float currentDistance; 
    
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
            }
            else
            {
                enabled = false; 
                return;
            }
        }

        currentDistance = defaultDistance;
    }

    void LateUpdate()
    {
        if (target == null) return;

       
        Vector3 desiredCameraPosition = target.position + target.up * heightOffset - target.forward * defaultDistance;

       
        Vector3 rayStartPoint = target.position + Vector3.up * heightOffset;
        RaycastHit hit;
        
        if (Physics.Linecast(rayStartPoint, desiredCameraPosition, out hit, obstacleLayer))
        {
          
            currentDistance = Mathf.Max(minDistance, hit.distance - 0.1f);
            desiredCameraPosition = target.position + target.up * heightOffset - target.forward * currentDistance;

            if (showDebugLines)
            {
                Debug.DrawLine(rayStartPoint, hit.point, Color.green, debugLineDuration);
                Debug.DrawLine(hit.point, desiredCameraPosition, Color.yellow, debugLineDuration);
            }            
            transform.position = Vector3.Lerp(transform.position, desiredCameraPosition, Time.deltaTime * smoothing);
        }
        else
        {
            currentDistance = defaultDistance;
            if (showDebugLines)
            {
                Debug.DrawLine(rayStartPoint, desiredCameraPosition, Color.red, debugLineDuration);
            }
            transform.position = Vector3.Lerp(transform.position, desiredCameraPosition, Time.deltaTime * smoothing);
        }
        transform.LookAt(target.position + Vector3.up * heightOffset / 2);
    }
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
    [ContextMenu("Log Tutorial Seen Status")]
    void LogTutorialStatus()
    {
        bool tutorialSeen = PlayerPrefs.GetInt("HasPlayedNonIntrusiveTutorial", 0) == 1;
    }
}
