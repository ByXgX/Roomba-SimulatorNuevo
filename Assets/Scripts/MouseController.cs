using UnityEngine;
using UnityEngine.AI;

public class MouseController : MonoBehaviour
{
    public NavMeshAgent agent;
    //public Animator animator;

    public float wanderRadius = 10f;
    public float wanderTimer = 3f;
    public float minDistanceFromObstacle = 0.5f; 
    private float timer;

    public float damageAmount = 5f;
    public float damageCooldown = 1f;
    private float lastDamageTime = -Mathf.Infinity;

    public string movementSpeedParameterName = "Speed";

    void Awake()
    {
        timer = wanderTimer;
    }
    void Update()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.1f)
        {
            timer += Time.deltaTime;
        }

        if (timer >= wanderTimer)
        {
            Vector3 newPos = FindValidRandomNavPoint(transform.position, wanderRadius, minDistanceFromObstacle, 10);
            if (newPos != Vector3.zero)
            {
                agent.SetDestination(newPos);
            }
            timer = 0;
        }
        Vector3 horizontalVelocity = new Vector3(agent.velocity.x, 0, agent.velocity.z);
        float currentSpeed = horizontalVelocity.magnitude;

        

    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && Time.time >= lastDamageTime + damageCooldown)
        {
            RoombaScript roomba = collision.gameObject.GetComponent<RoombaScript>();

            if (roomba != null)
            {
                roomba.RecibirDano(damageAmount);
                lastDamageTime = Time.time;
            }
        }
    }


    Vector3 FindValidRandomNavPoint(Vector3 origin, float searchRadius, float minDistanceToEdge, int maxAttempts)
    {
        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere * searchRadius;
            randomDirection += origin;

            NavMeshHit navHit;
            if (NavMesh.SamplePosition(randomDirection, out navHit, searchRadius, NavMesh.AllAreas))
            {
                if (NavMesh.SamplePosition(navHit.position, out NavMeshHit hitFromCenter, minDistanceToEdge + agent.radius, NavMesh.AllAreas))
                {
                    NavMeshHit edgeHit;
                    if (!NavMesh.FindClosestEdge(navHit.position, out edgeHit, NavMesh.AllAreas) || edgeHit.distance > minDistanceToEdge)
                    {
                        return navHit.position;
                    }
                }
            }
        }
        return Vector3.zero; 
    }
}
