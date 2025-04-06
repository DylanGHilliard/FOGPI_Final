using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [Header("Player Movement Variables")]
    [Tooltip("The base speed at which the player moves")]
    public float moveSpeed = 0f;
    public float gravity = 4.0f;

    private Vector3 velocity;

    [Header("Player Jump Variables")]
    public float jumpForce = 0;

    [Header("Slope Options")]
    [Tooltip("The angle at which the player can walk up a slope")]
    public float maxSlopeAngle = 45f;

    // Grounding Variables
    private RaycastHit groundHit;
    private float currentGroundSlope;
    private bool isGrounded;


    private PlayerInputController playerInput;

    void Start()
    {
        playerInput = this.GetComponent<PlayerInputController>();        
    }

    // Update is called once per frame
    void Update()
    {
        //Gravity();
        FirstMovement();
        Jump();
        FinalMovement();
    }

    private void FirstMovement()
    {
        velocity += new Vector3(playerInput.currentInput.moveInput.x, 0, playerInput.currentInput.moveInput.z);
    }

    private void FinalMovement(){

        Vector3 vel = new Vector3(velocity.x, velocity.y, velocity.z)* moveSpeed;

        transform.position += vel * Time.deltaTime;
        velocity = Vector3.zero;
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
}
