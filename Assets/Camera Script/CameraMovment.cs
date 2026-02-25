using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovment : MonoBehaviour
{
    public float sensitivity;
    private float mouseX;
    private float mouseY;

    private float xRotation = 0f;
    [SerializeField] Transform tf;

    public void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Update()
    {
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // makes it so you can't look past legs and head

        tf.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        tf.parent.Rotate(Vector3.up * mouseX);
    }

    public void Look(InputAction.CallbackContext ctx)
    {
        mouseX = ctx.ReadValue<Vector2>().x * sensitivity;
        mouseY = ctx.ReadValue<Vector2>().y * sensitivity;
    }
}
