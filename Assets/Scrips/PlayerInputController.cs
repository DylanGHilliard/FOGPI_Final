using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
    

public class PlayerInputController : MonoBehaviour
{
    public PlayerInput currentInput;
    void Start()
    {
     currentInput = new PlayerInput();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 move = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector2 mousePos = Input.mousePosition;
        Vector2 mouse = Camera.main.ScreenToWorldPoint(mousePos);


    }
}

public struct PlayerInput {
    public Vector3 moveInput;
    public Vector3 mouseInput;
    public bool jumpInput;
}
