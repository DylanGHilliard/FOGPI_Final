
using UnityEngine;
public class PlayerCamera : MonoBehaviour
{

    public Camera playerCamera;
    private GameObject player;
    public float lookSpeed = 700.0f;
    private Vector2 rotation = Vector2.zero;
    private float verticalRotation = 0.0f;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player not found");
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerCamera.gameObject.transform.localEulerAngles = player.transform.localEulerAngles;
        verticalRotation = playerCamera.transform.localEulerAngles.x;
    }

    // Update is called once per frame
    void LateUpdate()
    {

        verticalRotation  -= Input.GetAxis("Mouse Y") * lookSpeed * Time.deltaTime;
        verticalRotation = Mathf.Clamp(verticalRotation, -80f, 80f);
        
        //playerCamera.transform.Rotate(-rotY, 0f, 0f, Space.Self);
        player.transform.Rotate(0f, Input.GetAxisRaw("Mouse X") * lookSpeed * Time.deltaTime, 0f, Space.Self);
        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        playerCamera.transform.position = player.transform.position;
    }
}
