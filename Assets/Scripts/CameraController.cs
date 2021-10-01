using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public enum CameraType
{
    FirstPerson,
    ThirdPerson
}

public class CameraController : MonoBehaviour
{
    public PostProcessProfile defaultlayer;
    public PostProcessProfile underwaterlayer;

    public bool underwater = false;

    public CameraType type = CameraType.FirstPerson;

    public float sensitivity = 8f;

    public float fieldOfView = 110f;
    public float zoomedFieldOfView = 30f;

    float xRotation = 0f;

    public void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Update()
    {
        if (type == CameraType.FirstPerson)
        {
            transform.parent.Rotate(Vector3.up * Input.GetAxis("Mouse X") * sensitivity);

            xRotation = xRotation - Input.GetAxis("Mouse Y") * sensitivity;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }

        if (underwater) transform.GetComponent<PostProcessVolume>().profile = underwaterlayer;
        else transform.GetComponent<PostProcessVolume>().profile = defaultlayer;

        if (Input.GetKey(KeyCode.Z) && type == CameraType.FirstPerson && GetComponent<Camera>().enabled)
        {
            transform.GetComponent<Camera>().fieldOfView = zoomedFieldOfView;
        }
        else
        {
            transform.GetComponent<Camera>().fieldOfView = fieldOfView;
        }
    }
}