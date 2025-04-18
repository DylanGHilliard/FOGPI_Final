
using UnityEngine;

namespace KinematicCharacterController{
public class PlayerController : MonoBehaviour
{
    [Header("Player Model Variables")]
    public float playerHeight = 2.0f;
    public float playerRadius = 1.5f;

    public LayerMask discludePlayer;

    [Header("Player Movement Variables")]

    public float walkSpeed = 0f;
    public float sprintSpeed = 0f;

    public bool canSprint = false;
 

    private Vector3 velocity;

    [Header("Player Jump Variables")]
    public float jumpForce = 0;
    public bool isGrounded;
    public float gravity = 4.0f;

    [Header("Slope Options")]
 
    public float maxSlopeAngle = 45f;

    [Header("Camera Options")]

    public Camera playerCamera;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;
    public bool cursorVisable = false;
    
    // Grounding Variables
    private RaycastHit groundHit;
    private float currentGroundSlope;
    [Tooltip("Sets the distance off the ground the player snaps onto the ground")]
    public float snapDistance = 0.1f;
    
    private float rotationX;

    private Rigidbody rigidBody;
    public LayerMask groundLayer;
    public Vector3 groundNormal;

    




    void Start()
    {
        if (cursorVisable){
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else{
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        rigidBody = GetComponent<Rigidbody>();
 
    }

    void Update()
    {
        Gravity();
        Jump();
        Movement();
        GroundCheck();
    }



    private void Movement(){

        velocity += Vector3.ProjectOnPlane(transform.TransformDirection(PlayerInputController.currentInput.moveInput), groundHit.normal);
        //velocity += new Vector3(PlayerInputController.currentInput.moveInput.x, 0, PlayerInputController.currentInput.moveInput.z);

        Vector3 vel = new Vector3();

      
        if (PlayerInputController.currentInput.sprintInput && canSprint)
        {
            vel = new Vector3(velocity.x, velocity.y, velocity.z)* sprintSpeed;
        }
        else
        {
            vel = new Vector3(velocity.x, velocity.y, velocity.z)* walkSpeed;
        }
        
       // vel = transform.TransformDirection(vel);

        rigidBody.MovePosition(transform.position + (vel * Time.deltaTime));
        velocity = Vector3.zero;
        SnapeToGround();

         rotationX += playerCamera.transform.localEulerAngles.y + Input.GetAxisRaw("Mouse Y") * lookSpeed;
                rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
                playerCamera.transform.localRotation = Quaternion.Euler(-rotationX, 0, 0);
                transform.rotation *= Quaternion.Euler(0, Input.GetAxisRaw("Mouse X") * lookSpeed, 0);
    }

    private void Gravity(){
        if (!isGrounded){
            if (velocity.y < gravity*4){
                velocity.y -= gravity/20;

            }
        }
    }

    private void Jump(){
        if (isGrounded && PlayerInputController.currentInput.jumpInput)
        {
            velocity.y = jumpForce;
        }
    }

    private void GroundCheck(){
       RaycastHit _hit;
     
        if(Physics.Raycast(transform.position, Vector3.down, out _hit, (playerHeight/2) + 0.1f, ~discludePlayer))
        {
            isGrounded = true;
            currentGroundSlope = Vector3.Angle(_hit.normal, Vector3.up);
            groundHit = _hit;
            groundNormal = _hit.normal;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void SnapeToGround(){
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, snapDistance, groundLayer))
    
        {
            Vector3 targetPosition = new Vector3(rigidBody.position.x, hit.point.y + 0.1f, rigidBody.position.z);
            rigidBody.MovePosition(targetPosition);
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(playerRadius, playerHeight, playerRadius));
    }
}


}
