
using UnityEngine;
using System.Collections;
using UnityEditor.EditorTools;

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
    public float maxStepHeight = 0.5f;
    [SerializeField] private float groundedOffset = 0.1f;
    [SerializeField] private LayerMask groundLayer;
    
    [HideInInspector]
    public CharacterGroundingReport groundingStatus = new CharacterGroundingReport();

    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float groundCheckDistance = 0.1f;
    [SerializeField] private float jumpForce = 5f;
    [Tooltip("Time to reach the peak of the jump")]
    [SerializeField] private float jumpTime = 0.5f;


 

    [Header("Gravity Settings")]
    [SerializeField] private float gravity = 5.0f;
    [SerializeField] private float terminalVelocity = -20.0f;

    //private IEnumerator Jumping;
    private Vector3 verticalVelocity;

    private float currentStepOffset;

    public LayerMask collisionLayers;
    public bool isGrounded;
    public bool isJumping;
    private float time;


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
        isGrounded = groundingStatus.onGround;
    }
    
    void Move(Vector3 moveInput, float deltaTime)
    {
        Vector3 movement = Vector3.zero;
        Vector3 targetPosition = transform.position;

        // Check If player is Moving/ Spriting
        if ( PlayerInputController.currentInput.sprintInput){
            movement = transform.TransformDirection((moveInput *sprintSpeed) * deltaTime);
        }
        else
        {
         movement = transform.TransformDirection((moveInput *walkSpeed) * deltaTime);
        }
        // Tries to simulate smooth jumping
        if(isJumping)
        {
            time += Time.deltaTime;
            verticalVelocity.y += jumpForce;
            if (time > jumpTime)
            {
                isJumping = false;
                time = 0;
                groundingStatus.snappingPrevented = false;
            }
            
        }

        if (groundingStatus.onGround)
        {
            
            if (PlayerInputController.currentInput.jumpInput)
            {
               Jump();
            }

        }
        else{
            verticalVelocity.y -= gravity * deltaTime;
            verticalVelocity.y = Mathf.Clamp(verticalVelocity.y, terminalVelocity, 0.0f);
        }

        
        movement.y += verticalVelocity.y * deltaTime;
        //movement = transform.TransformDirection(movement);
       

        
        if (movement.magnitude > 0)
        {
            // Check ground state

            // Handle movement collisions
            HandleMovement(ref movement);

            // Apply final movement
            targetPosition += movement;
        }
        transform.position = targetPosition;
        //HandleGroundSnapping();
    }




     private void HandleMovement(ref Vector3 movement)
    {
       
        float remainingDistance = movement.magnitude;
        Vector3 direction = movement.normalized;

        while (remainingDistance > 0)
        {
            // Check for collisions
            RaycastHit hit;

             if (CheckForStep(direction, ref movement))
            {
                break;
            }

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

        }
    }

    private bool CapsuleCast(Vector3 direction, float distance, out RaycastHit hit)
    {
        Vector3 p1 = transform.position +  Vector3.up * (CapsuleHeight * 0.5f);
        Vector3 p2 = transform.position - Vector3.up * (CapsuleHeight * 0.5f);

        return Physics.CapsuleCast(p1, p2, CapsuleRadius, direction, out hit, 
                                distance, collisionLayers, QueryTriggerInteraction.Ignore);
    }

    private bool CheckForStep(Vector3 moveDirection, ref Vector3 movement)
    {
        RaycastHit hitLow, hitHigh;
        // exit if not grounded or jumping
    if (!groundingStatus.onGround || isJumping)
        return false;

    Vector3 rayStart = transform.position + Vector3.down * (CapsuleHeight * 0.5f);
    Vector3 rayStartHigh = rayStart + Vector3.up * maxStepHeight;

    Debug.DrawRay(rayStart, moveDirection * maxStepHeight, Color.yellow);
    Debug.DrawRay(rayStartHigh, moveDirection * maxStepHeight, Color.cyan);
    

    // Check for obstacle at foot level
    if (Physics.Raycast(rayStart, moveDirection, out hitLow, capsule.radius + 1, collisionLayers))
    {
        // check at max step height
        if (!Physics.Raycast(rayStartHigh, moveDirection, out hitHigh, capsule.radius + 1, collisionLayers))
        {

                transform.position = new Vector3(transform.position.x, 
                                                transform.position.y + maxStepHeight,
                                                transform.position.z);
                return true;
            
        }
    }

    return false;
}
    
   

     private void CheckGrounding()
    {
        Vector3 capsuleBottom = transform.position - Vector3.up * (CapsuleHeight * 0.5f);
    

        RaycastHit hit;
        Debug.DrawRay(capsuleBottom, Vector3.down * groundCheckDistance, Color.red);
        groundingStatus.foundAnyGround = Physics.Raycast(capsuleBottom, Vector3.down, out hit,
            groundCheckDistance, groundLayer, QueryTriggerInteraction.Ignore);
    

        if (groundingStatus.foundAnyGround)
        {
            groundingStatus.groundNormal = hit.normal;
            groundingStatus.groundPoint = hit.point;
            groundingStatus.groundCollider = hit.collider;
            groundingStatus.onGround = true;
            if (transform.position.y-(CapsuleHeight * 0.4f)+ groundedOffset < hit.point.y && !groundingStatus.snappingPrevented)
            {
                transform.position = new Vector3(transform.position.x, hit.point.y + groundedOffset + (CapsuleHeight * 0.4f), transform.position.z);
            }
            verticalVelocity.y = 0f;
            //transform.position = new Vector3(transform.position.x, hit.point.y + groundedOffset + (CapsuleHeight * 0.5f), transform.position.z);
        }
        else
        {
            groundingStatus.onGround = false;
        }

    }

    private void Jump()
    {
        verticalVelocity += Vector3.up *jumpForce;
        isJumping = true;
        groundingStatus.onGround = false;
        groundingStatus.snappingPrevented = true;

    }





}

}
