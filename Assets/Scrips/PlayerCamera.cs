
using UnityEngine;
[AddComponentMenu("Camera-Control/Mouse Look")]
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
    }

    // Update is called once per frame
    void Update()
    {
        rotation.x = playerCamera.transform.localEulerAngles.y + Input.GetAxis("Mouse X") * lookSpeed;
        rotation.y = Mathf.Clamp(playerCamera.transform.localEulerAngles.z + Input.GetAxis("Mouse Y")* lookSpeed, -90f, 90f);

        playerCamera.transform.localEulerAngles = new Vector3(-rotation.y, rotation.x, 0);


        playerCamera.transform.position = player.transform.position;
    }
}
