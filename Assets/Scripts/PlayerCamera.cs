
using UnityEngine;
public class PlayerCamera : MonoBehaviour
{

    public Camera playerCamera;
    private GameObject player;
    public float lookSpeed = 2.0f;
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
    void Update()
    {
        rotation.x = playerCamera.transform.localEulerAngles.y + Input.GetAxisRaw("Mouse X") * lookSpeed;
        rotation.y = Mathf.Clamp(playerCamera.transform.localEulerAngles.z + Input.GetAxis("Mouse Y")* lookSpeed, -90f, 90f);

        playerCamera.transform.localEulerAngles = new Vector3(-rotation.y, rotation.x, 0);
        Vector3 playerRotation = player.transform.localEulerAngles+ new Vector3(0, Input.GetAxisRaw("Mouse X") * lookSpeed, 0);
        player.transform.localEulerAngles = new Vector3(playerRotation.x, playerRotation.y, playerRotation.z);

        playerCamera.transform.position = player.transform.position;
    }
}
