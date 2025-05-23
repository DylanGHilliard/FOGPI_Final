
using UnityEngine;
using System.Collections;
using UnityEditor.EditorTools;
using UnityEngine.UIElements;



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
    [SerializeField] private float maxSnapDistance = 0.1f;
    
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

        
        movement.y += verticalVelocity.y;
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
        Vector3 position = transform.position;

        while (remainingDistance > 0.0001)
        {
            // Check for collisions
            RaycastHit hit;


            if (CapsuleCast(position, direction, remainingDistance, out hit))
            {
                

                float fraction = hit.distance / remainingDistance;

                //float hitDistance = hit.distance;


                // Move up to the hit point
                position += (remainingDistance * fraction) * direction;

                // Calculate sliding direction
                Vector3 slide = Vector3.ProjectOnPlane(direction, hit.normal).normalized;
                direction = slide;
                remainingDistance *= (1 - fraction);
            }
            else
            {
                // No collision, move the full remaining distance
                movement = direction * remainingDistance;
                remainingDistance = 0;
            
            }
            SnapPlayerDown();
        }
    }

    private bool CapsuleCast( Vector3 position, Vector3 direction, float distance, out RaycastHit hit)
    {

        Quaternion rot = transform.rotation;

        Vector3 center = rot * capsule.center + position;
        float radius = CapsuleRadius;
        float height = CapsuleHeight;

        Vector3 bottom =  center + rot * Vector3.down * (height * 0.5f - radius);
        Vector3 top = center + rot * Vector3.up * (height * 0.5f - radius);
        Vector3 p1 = transform.position +  Vector3.up * (CapsuleHeight * 0.5f);
        Vector3 p2 = transform.position - Vector3.up * (CapsuleHeight * 0.5f);

        return Physics.CapsuleCast(top, bottom, radius, direction, out hit, 
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

                transform.position = new Vector3(transform.position.x + moveDirection.x, 
                                                transform.position.y + maxStepHeight,
                                                transform.position.z + moveDirection.z);
                return true;
            
        }
    }

    return false;
}

    private void SnapPlayerDown()
    {
        bool closeToGround = CapsuleCast(transform.position, Vector3.down, maxSnapDistance, out RaycastHit hit);

        if(closeToGround && hit.distance > 0 && !groundingStatus.snappingPrevented)
        {
            transform.position += Vector3.down * (hit.distance - 0.001f);
        }
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
        isJumping = false;
        groundingStatus.onGround = false;
        groundingStatus.snappingPrevented = false;

    }





}

}
