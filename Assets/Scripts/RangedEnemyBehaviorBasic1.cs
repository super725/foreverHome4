using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class RangedEnemyBehaviorSight : MonoBehaviour
{
    public float wanderRadius = 5f;
    public float walkSpeed = 2f;
    public float runSpeed = 4f;
    public float wanderDuration = 5f; 
    public float maxpersistence = 3f;
    private float persistence = 0f;
    public float orbitDistance = 12f;
    public GameObject projectile;
    public float health = 100f;
    private float maxHealth = 0;
    public float panickNum = 20f;
    public Transform firePoint;

    bool panicked = false;
    public Transform target;
    private Vector3 initialPosition;
    private Vector3 destination;
    private float wanderDelay;
    private float persistenceTimer = 0f;

    private NavMeshAgent agent;
    private EnemySight enemySight;
    private Vector3 lastKnownPosition;
<<<<<<< HEAD
    
=======
    [HideInInspector]
    UIHealthBar healthBar;
    Animator animator; 

    Ragdoll ragdoll;
>>>>>>> origin/RayBranch
    private float cooldown = 3f;

    int randomNumber = 0;

    void Start()
    {
<<<<<<< HEAD
=======
        ragdoll = GetComponent<Ragdoll>();
        animator = GetComponent<Animator>();
        healthBar = GetComponentInChildren<UIHealthBar>();
>>>>>>> origin/RayBranch
        maxHealth = health;
        initialPosition = transform.position;
        destination = initialPosition;
        agent = GetComponent<NavMeshAgent>();
        enemySight = GetComponent<EnemySight>();
<<<<<<< HEAD
=======
        target = GameObject.FindGameObjectWithTag("Player").transform;
        var rigidBodies = GetComponentsInChildren<Rigidbody>();
        foreach(var rigidBody in rigidBodies){
            Hitbox hitBox = rigidBody.gameObject.AddComponent<Hitbox>();
            hitBox.health =  this.health;
        }
>>>>>>> origin/RayBranch
    }

void Update()
{
<<<<<<< HEAD
=======
    animator.SetFloat("Speed", agent.velocity.magnitude);
>>>>>>> origin/RayBranch
    if (health <= panickNum && panicked == false)
    {
        randomNumber = Random.Range(1, 6);
        if (randomNumber == 1)
        {
            Panick();
            randomNumber = 0;
        }
    }
    if (enemySight.canSeePlayer)
    {
        persistence = maxpersistence;
        lastKnownPosition = target.transform.position;
        Pursue();
    }
    else if (persistence > 0)
    {
        Search();
    }
    else
    {
        Wander();
    }
}

void Wander()
{
    if (Vector3.Distance(transform.position, destination) <= 1f)
    {
        destination = GetNewSearchDestination();
        wanderDelay = 0f;
    }
    agent.speed = walkSpeed;
    StartCoroutine(WaitForWanderDuration());
    agent.destination = destination;
}

void Search()
{
    Debug.Log("Search Mode.");
    destination = GetNewSearchDestination();
    wanderDelay = 0f;
    agent.speed = walkSpeed;
    agent.destination = destination;
    StartCoroutine(SearchDelay());
    persistence--;
}

IEnumerator WaitForWanderDuration()
{
    yield return new WaitForSeconds(wanderDuration);
}

IEnumerator SearchDelay()
{
    Debug.Log("Waiting");
    yield return new WaitForSeconds(3);
}

private float timer = 0f;
    void Pursue()
    {
        agent.speed = runSpeed;
        if (Time.time % 5f == 0f) 
        {
            randomNumber = Random.Range(1, 6);
        }
        // Get the distance to the target
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget <= orbitDistance)
        {
            //Start of code provided by mixandjam
            Vector3 dir = (target.transform.position - transform.position).normalized;
            Vector3 pDir = Quaternion.AngleAxis(90, Vector3.up) * dir;
            Vector3 movedir = Vector3.zero;

            Vector3 finalDirection = Vector3.zero;
            //End of code by mixandjam
            agent.destination = finalDirection;

            agent.speed = 0f;
            if (randomNumber == 5)
            {
                RangedAttack();
                return;
            }
        }
        else
        {
            agent.destination = target.position;
        }
    }
void RangedAttack()
    {
        agent.speed = 0f;
        Invoke("Pursue", 2.5f);
    }

    void ShootProjectiles()
    {
        Vector3 spawnPosition = firePoint.position;
        spawnPosition.x += 4f;
        for (int i = 0; i < 3; i++)
        {
            Instantiate(projectile, spawnPosition, Quaternion.identity);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            target = other.transform;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            target = null;
            persistenceTimer = 0f;
        }
    }

    Vector3 GetNewWanderDestination()
    {
        Vector3 newDestination = Random.insideUnitSphere * (wanderRadius/2);
        newDestination += initialPosition;
        NavMeshHit navMeshHit;
        NavMesh.SamplePosition(newDestination, out navMeshHit, wanderRadius, NavMesh.AllAreas);
        return navMeshHit.position;
    }

    Vector3 GetNewSearchDestination()
    {
        Vector3 direction = lastKnownPosition - transform.position;
        float distance = Random.Range(3, 8);
        Vector3 randomPoint = lastKnownPosition + direction.normalized * distance;
        NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, distance, NavMesh.AllAreas);
        return hit.position;
    }

    private bool isPanicking = false;
    private float panicTimer = 0f;
    private float panicDuration = 10f;
    private float panicDirectionChangeInterval = 2f;
    private float panicStopDuration = 5f;
    private float panicHealthRegenRate = 2.5f;
    private Vector3 panicDestination;
    private float originalSpeed;

    void Panick()
    {
        // Set the panic destination as the opposite direction from the player's position
        panicDestination = transform.position - (target.position - transform.position);

        // Start the panic behavior
        isPanicking = true;
        originalSpeed = agent.speed;
        agent.speed = runSpeed;

        StartCoroutine(PanicRoutine());
    }

    IEnumerator PanicRoutine()
    {
        while (isPanicking)
        {
            // Check if the player is too close or health has decreased
            float distanceToPlayer = Vector3.Distance(transform.position, target.position);
            if (distanceToPlayer <= 10f || health < maxHealth)
            {
                // Stop panicking and return to normal behavior
                StopPanic();
                yield break;
            }

            // Flee from the player's position
            agent.destination = panicDestination;

            // Randomly change the panic direction every panicDirectionChangeInterval seconds
            if (panicTimer % panicDirectionChangeInterval == 0f)
            {
                Vector3 randomDirection = Random.insideUnitSphere * 5f;
                randomDirection.y = 0f;
                panicDestination = transform.position - randomDirection;
            }

            // Increase health by panicHealthRegenRate every second
            health += panicHealthRegenRate * Time.deltaTime;

            // Check if the panic duration has ended
            if (panicTimer >= panicDuration)
            {
                // Stop panicking and hold still for panicStopDuration seconds
                agent.speed = 0f;
                yield return new WaitForSeconds(panicStopDuration);
                StopPanic();
                yield break;
            }

            panicTimer += Time.deltaTime;
            yield return null;
        }
    }

    void StopPanic()
    {
        isPanicking = false;
        agent.speed = originalSpeed;
        panicTimer = 0f;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        Destroy(gameObject);
    }
}