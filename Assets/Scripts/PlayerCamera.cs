
using UnityEngine;
public class PlayerCamera : MonoBehaviour
{

    public Camera playerCamera;
    private GameObject player;
    public float lookSpeed = 700.0f;
    private Vector2 rotation = Vector2.zero;


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
    }

    // Update is called once per frame
    void LateUpdate()
    {
        /*
        rotation.y = playerCamera.transform.localEulerAngles.y + Input.GetAxisRaw("Mouse X") * lookSpeed *Time.deltaTime;
        rotation.x = playerCamera.transform.localEulerAngles.x + Input.GetAxisRaw("Mouse Y")* lookSpeed *Time.deltaTime;
          playerCamera.transform.localEulerAngles = new Vector3(-rotation.x, rotation.y, 0);
        Vector3 playerRotation = player.transform.localEulerAngles+ new Vector3(0, Input.GetAxisRaw("Mouse X") * lookSpeed, 0);
        player.transform.localEulerAngles = new Vector3(playerRotation.x, rotation.x, playerRotation.z);
        */
        playerCamera.transform.Rotate(-Input.GetAxisRaw("Mouse Y") * lookSpeed * Time.deltaTime, 0f, 0f, Space.Self);
        player.transform.Rotate(0f, Input.GetAxisRaw("Mouse X") * lookSpeed * Time.deltaTime, 0f, Space.Self);
        playerCamera.transform.position = player.transform.position;
    }
}
