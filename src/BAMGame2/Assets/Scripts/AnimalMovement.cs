using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class AnimalMovement : MonoBehaviour
{
    public float moveSpeed = 1.5f;
    public float wanderRadius = 3f;
    public float idleTimeMin = 1f;
    public float idleTimeMax = 3f;
    public float targetRange = 12f; // how far they search for collectibles

    [HideInInspector] public AnimalDefinition definition;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    private Vector2 movement;
    private Vector3 wanderTarget;

    private float idleTimer;
    private bool isIdle = false;

    private Transform currentTarget; // collectible target

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        PickNewWanderTarget();
    }

    private void Update()
    {
        // 1. Check for a target collectible
        FindTargetCollectible();

        if (currentTarget != null)
        {
            MoveTowardTarget(currentTarget.position);
        }
        else
        {
            Wander();
        }

        // Flip sprite based on movement
        if (movement.x > 0.1f)
            spriteRenderer.flipX = false;
        else if (movement.x < -0.1f)
            spriteRenderer.flipX = true;

        // Update animator
        animator.SetFloat("MoveX", movement.x);
        animator.SetFloat("MoveY", movement.y);
    }

    private void FixedUpdate()
    {
        Vector2 newPos = rb.position + movement * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPos);
    }

    // ---------------------------
    // WANDERING
    // ---------------------------
    private void Wander()
    {
        if (isIdle)
        {
            movement = Vector2.zero;

            idleTimer -= Time.deltaTime;
            if (idleTimer <= 0)
            {
                isIdle = false;
                PickNewWanderTarget();
            }
            return;
        }

        Vector3 dir = (wanderTarget - transform.position);
        if (dir.magnitude < 0.2f)
        {
            // reached wander point → go idle
            isIdle = true;
            idleTimer = Random.Range(idleTimeMin, idleTimeMax);
            movement = Vector2.zero;
            return;
        }

        movement = dir.normalized;
    }

    private void PickNewWanderTarget()
    {
        wanderTarget = transform.position + new Vector3(
            Random.Range(-wanderRadius, wanderRadius),
            Random.Range(-wanderRadius, wanderRadius),
            0
        );
    }

    // ---------------------------
    // COLLECTIBLE SEARCH
    // ---------------------------
    private void FindTargetCollectible()
    {
        Collectible[] all = FindObjectsOfType<Collectible>();
        float bestDist = Mathf.Infinity;
        Transform best = null;

        foreach (var col in all)
        {
            float d = Vector2.Distance(transform.position, col.transform.position);
            if (d < bestDist && d <= targetRange)
            {
                bestDist = d;
                best = col.transform;
            }
        }

        currentTarget = best;
    }

    private void MoveTowardTarget(Vector3 targetPos)
    {
        Vector3 dir = targetPos - transform.position;

        // reached collectible → collect it
        if (dir.magnitude < 0.3f)
        {
            movement = Vector2.zero;

            Collectible c = currentTarget.GetComponent<Collectible>();
            if (c != null)
                c.CollectByAnimal();

            currentTarget = null;
            return;
        }

        movement = dir.normalized;
    }
}