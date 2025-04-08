using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Model Variables")]
    public float playerHeight = 2.0f;
    public float playerRadius = 1.5f;
    [Tooltip("Set to Current LayerMask for player For correct Ground Checking")]
    public LayerMask discludePlayer;

    [Header("Player Movement Variables")]
    [Tooltip("The base speed at which the player moves")]
    public float walkSpeed = 0f;
    public float sprintSpeed = 0f;
[Tooltip("Enables Ability To Sprint/Run")]
    public bool canSprint = false;
    public float gravity = 4.0f;

    private Vector3 velocity;

    [Header("Player Jump Variables")]
    public float jumpForce = 0;
    public bool isGrounded;

    [Header("Slope Options")]
    [Tooltip("The angle at which the player can walk up a slope")]
    public float maxSlopeAngle = 45f;

    [Header("Camera Options")]

    [Tooltip("The camera that will follow the player")]
    public Camera playerCamera;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;
    public bool cursorVisable = false;
    // Grounding Variables
    private RaycastHit groundHit;
    private float currentGroundSlope;
    
    private float rotationX;




    private PlayerInputController playerInput;

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
        // Init Player input
        playerInput = this.GetComponent<PlayerInputController>();        
    }

    void Update()
    {
        //Gravity();
        FirstMovement();
        Jump();
        GroundCheck();
        FinalMovement();
    }

    private void FirstMovement()
    {
        velocity += new Vector3(playerInput.currentInput.moveInput.x, 0, playerInput.currentInput.moveInput.z);
    }

    private void FinalMovement(){

        Vector3 vel = new Vector3();
        
        if (playerInput.currentInput.sprintInput && canSprint)
        {
            vel = new Vector3(velocity.x, velocity.y, velocity.z)* sprintSpeed;
        }
        else
        {
            vel = new Vector3(velocity.x, velocity.y, velocity.z)* walkSpeed;
        }
        

        transform.position += vel * Time.deltaTime;
        velocity = Vector3.zero;

         rotationX += playerCamera.transform.localEulerAngles.y + Input.GetAxisRaw("Mouse Y") * lookSpeed;
                rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
                playerCamera.transform.localRotation = Quaternion.Euler(-rotationX, 0, 0);
                transform.rotation *= Quaternion.Euler(0, Input.GetAxisRaw("Mouse X") * lookSpeed, 0);
    }

    private void Gravity(){
        if (!isGrounded){
            velocity.y -= gravity;
        }
    }

    private void Jump(){
        if (isGrounded && playerInput.currentInput.jumpInput)
        {
            velocity.y = jumpForce;
        }
    }

    private void GroundCheck(){
        Ray ray = new Ray(transform.position, Vector3.down);

        RaycastHit tempHit = new RaycastHit();
        if (Physics.SphereCast(ray, playerRadius, out tempHit, playerHeight*2, discludePlayer)){
            groundHit = tempHit;
            isGrounded = true;
            currentGroundSlope = Vector3.Angle(Vector3.up, groundHit.normal);
        }
        else
        {
            isGrounded = false;
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(playerRadius, playerHeight, playerRadius));
    }
}
