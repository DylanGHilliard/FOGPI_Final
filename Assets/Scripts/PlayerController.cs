
using UnityEngine;

namespace KinematicCharacterController{
    public struct CharacterGroundingReport
    {
        public bool foundAnyGround;
        public bool onGround;
        public bool snappingPrevented;
        public Vector3 groundNormal;

        public Collider groundCollider;
        public Vector3 groundPoint;
    }
public class PlayerController : MonoBehaviour
{
    [Header("Components")]

    public CapsuleCollider capsule;
     [Header("Capsule Settings")]

    [SerializeField]
    [Tooltip("Radius of the Character Capsule")]
    private float CapsuleRadius = 0.5f;
    
    [SerializeField]
    [Tooltip("Height of the Character Capsule")]
    private float CapsuleHeight = 2f;

    [Header("Grounding settings")]
    [Range(0f, 89f)]
    [Tooltip("Maximum slope angle on which the character can be stable")]
    public float MaxStableSlopeAngle = 60f;
    [Tooltip("Maximum height of a step which the character can climb")]
    public float MaxStepHeight = 0.5f;
    
    [HideInInspector]
    public CharacterGroundingReport groundingStatus = new CharacterGroundingReport();

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float groundCheckDistance = 0.1f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Terrain Handling")]
    [SerializeField] private float stepCheckDistance = 0.3f;
    [SerializeField] private int stepSmoothingIterations = 3;
    [SerializeField] private float groundedOffset = 0.1f;

    [Header("Gravity Settings")]
    [SerializeField] private float gravity = 5.0f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float terminalVelocity = -20.0f;
    private Vector3 verticalVelocity;

    private float currentStepOffset;
 
    private int maxIterations = 10;
    public LayerMask collisionLayers;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        capsule = GetComponent<CapsuleCollider>();
        if (capsule == null)
        {
            Debug.LogError("CapsuleCollider not found on the GameObject.");
        }
        else
        {
            CapsuleRadius = capsule.radius;
            CapsuleHeight = capsule.height;
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckGrounding();
        Vector3 inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Move(inputDirection, Time.deltaTime);
    }
    
    void Move(Vector3 moveInput, float deltaTime)
    {
        Vector3 targetPosition = transform.position;
        Vector3 movement = moveInput *moveSpeed * deltaTime;

        if (groundingStatus.onGround)
        {
            verticalVelocity.y = 0f;
        }
        else
        {
            verticalVelocity.y -= gravity * deltaTime;
            verticalVelocity.y = Mathf.Max(verticalVelocity.y, terminalVelocity);
        }

        
        movement.y += verticalVelocity.y * deltaTime;
        movement = transform.TransformDirection(movement);
       

        
        if (movement.magnitude > 0)
        {
            // Check ground state
            CheckGrounding();

            // Handle movement collisions
            HandleMovement(ref movement);

            // Apply final movement
            targetPosition += movement;
        }
        transform.position = targetPosition;
    }




     private void HandleMovement(ref Vector3 movement)
    {
        int iterations = 0;
        float remainingDistance = movement.magnitude;
        Vector3 direction = movement.normalized;

        while (remainingDistance > 0 && iterations < maxIterations)
        {
            // Check for collisions
            RaycastHit hit;
            if (CapsuleCast(direction, remainingDistance, out hit))
            {
             
                float hitDistance = hit.distance;
                remainingDistance -= hitDistance;

                // Move up to the hit point
                movement = direction * hitDistance;

                // Calculate sliding direction
                Vector3 slide = Vector3.ProjectOnPlane(direction, hit.normal).normalized;
                direction = slide;
            }
            else
            {
                // No collision, move the full remaining distance
                movement = direction * remainingDistance;
                remainingDistance = 0;
            }

            iterations++;
        }
    }

    private bool CapsuleCast(Vector3 direction, float distance, out RaycastHit hit)
    {
        Vector3 p1 = transform.position + capsule.center + Vector3.up * (CapsuleHeight * 0.5f - CapsuleRadius);
        Vector3 p2 = transform.position + capsule.center + Vector3.down * (CapsuleHeight * 0.5f - CapsuleRadius);

        return Physics.CapsuleCast(p1, p2, CapsuleRadius, direction, out hit, 
            distance, collisionLayers, QueryTriggerInteraction.Ignore);
    }

        // Apply downward force when in air or moving down slopes

    private bool CheckForStep(Vector3 moveDirection)
    {
        RaycastHit hitLow, hitHigh;
        Vector3 rayStart = transform.position + Vector3.up * 0.1f;
        Vector3 rayStartHigh = transform.position + Vector3.up * MaxStepHeight;

        // Check if there's an obstacle at foot level
        if (Physics.Raycast(rayStart, moveDirection, out hitLow, stepCheckDistance))
        {
            // Check if there's space above the obstacle
            if (!Physics.Raycast(rayStartHigh, moveDirection, out hitHigh, stepCheckDistance))
            {
                // Check if the height difference is within step limit
                currentStepOffset = hitLow.point.y - transform.position.y;
                return currentStepOffset <= MaxStepHeight;
            }
        }

        currentStepOffset = 0f;
        return false;
    }

   

     private void CheckGrounding()
    {
        Vector3 capsuleBottom = transform.position + Vector3.up * (CapsuleRadius);
        Vector3 capsuleTop = transform.position + Vector3.up * (CapsuleHeight - CapsuleRadius);

        RaycastHit hit;
        groundingStatus.foundAnyGround = Physics.CapsuleCast(
            capsuleBottom, capsuleTop, CapsuleRadius,
            Vector3.down, out hit, groundCheckDistance, groundLayer
        );

        if (groundingStatus.foundAnyGround)
        {
            groundingStatus.groundNormal = hit.normal;
            groundingStatus.groundPoint = hit.point;
            groundingStatus.groundCollider = hit.collider;
            groundingStatus.onGround = Vector3.Angle(Vector3.up, hit.normal) <= MaxStableSlopeAngle;
        }
        else
        {
            groundingStatus.onGround = false;
        }

    }
     private void HandleGroundSnapping()
    {
        if (!groundingStatus.onGround || groundingStatus.snappingPrevented)
            return;

        float distanceToGround = Vector3.Distance(transform.position, groundingStatus.groundPoint);
        if (distanceToGround > groundCheckDistance)
        {
            transform.position = Vector3.Lerp(
                transform.position,
                groundingStatus.groundPoint + Vector3.up * CapsuleRadius,
                Time.deltaTime * 15f
            );
        }
    }



}

}
