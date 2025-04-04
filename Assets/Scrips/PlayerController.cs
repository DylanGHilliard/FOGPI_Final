using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [Header("Player Movement Variables")]
    [Tooltip("The base speed at which the player moves")]
    public float moveSpeed = 0f;

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
        
    }
}
