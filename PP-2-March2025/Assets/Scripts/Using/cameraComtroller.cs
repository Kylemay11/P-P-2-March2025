using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Rendering.UI;
using UnityEngine.SceneManagement;

public class cameraComtroller : MonoBehaviour

{
    public static cameraComtroller instance;

    [SerializeField] public float sens = 200;
    [SerializeField] int lockVertMin, lockVertMax;
    [SerializeField] bool invertY;

    public bool canLook = true;

    float rotX;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
        if (SceneManager.GetActiveScene().name == "level_1")
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (canLook)
        {
            float mouseX = Input.GetAxis("Mouse X") * sens * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * sens * Time.deltaTime;

            if (invertY)
                rotX += mouseY;
            else
                rotX -= mouseY;

            rotX = Mathf.Clamp(rotX, lockVertMin, lockVertMax);

            transform.localRotation = Quaternion.Euler(rotX, 0, 0);

            transform.parent.Rotate(Vector3.up * mouseX);
        }
    }
}
