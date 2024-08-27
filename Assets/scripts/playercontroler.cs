using UnityEngine;
using Cinemachine;
using Photon.Pun.UtilityScripts;
using Photon.Pun.Demo.Cockpit;

public class PlayerController : MonoBehaviour {
    CharacterController controller;
    CinemachineVirtualCamera playerCamera;
    
    [Header("Player Value")]
    public float playerspeed = 3;
    [Header("Player Smooth Turn")]
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    [Header("Mouse Sensitivity")]
    public float mouseSensitivity = 100f;
    float mouseX, mouseY;

    void Start() {
        controller = GetComponent<CharacterController>();
        playerCamera = FindObjectOfType<CinemachineVirtualCamera>();

        if (playerCamera == null) {
            Debug.LogError("No Cinemachine Virtual Camera found in the scene.");
        }

        // Lock the cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    void Update() {
        if (playerCamera == null) {
            return; // Exit the Update method if no Cinemachine Virtual Camera is found
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        // Get mouse input
        mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotate the player based on mouse input
        transform.Rotate(Vector3.up * mouseX);

        if (direction.magnitude >= 0.01f) {
            float targetangle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + playerCamera.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetangle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0, angle, 0);

            Vector3 moveDir = Quaternion.Euler(0, targetangle, 0) * Vector3.forward;
            controller.Move(moveDir.normalized * playerspeed * Time.deltaTime);

            // if player is holding down the shift key, increase the player speed to 6
            if (Input.GetKey(KeyCode.LeftShift)) {
                controller.Move(moveDir.normalized * playerspeed * 2 * Time.deltaTime);
            }
        }
    }
}