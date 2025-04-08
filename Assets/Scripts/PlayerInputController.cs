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
        currentInput.moveInput = move;
        currentInput.mouseInput = new Vector2(mouse.x, mouse.y);
        currentInput.jumpInput = Input.GetButtonDown("Jump");
        currentInput.sprintInput = Keyboard.current.leftShiftKey.isPressed;
       // Debug.Log(currentInput.moveInput);


    }
}

public struct PlayerInput {
    public Vector3 moveInput;
    public Vector2 mouseInput;
    public bool jumpInput;
    public bool sprintInput;
}
