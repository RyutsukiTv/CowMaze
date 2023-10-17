using UnityEngine;

public class FreeCameraController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float sensitivity = 2f;

    private Vector3 rotation = Vector3.zero;
    private bool isMouseVisible = true;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Masquer la souris lorsque la touche Alt est relâchée
        if (Input.GetKeyUp(KeyCode.LeftAlt))
        {
            isMouseVisible = !isMouseVisible;
            Cursor.lockState = isMouseVisible ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = isMouseVisible;
        }

        if (!isMouseVisible)
        {
            // Rotation de la caméra avec la souris
            rotation.x -= Input.GetAxis("Mouse Y") * sensitivity;
            rotation.y += Input.GetAxis("Mouse X") * sensitivity;
            rotation.x = Mathf.Clamp(rotation.x, -90f, 90f); // Limitez l'angle de rotation en vertical
            transform.eulerAngles = rotation;
        }

        // Déplacement de la caméra avec les touches WASD
        Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        moveDirection = transform.TransformDirection(moveDirection);
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }
}
