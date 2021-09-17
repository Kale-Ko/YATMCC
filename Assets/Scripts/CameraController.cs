using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class CameraController : MonoBehaviour
{
    public PostProcessProfile defaultlayer;
    public PostProcessProfile underwaterlayer;

    public bool underwater = false;

    public string type = "firstPerson";

    public float sensitivity = 200f;

    public float fieldOfView = 110f;

    public float zoomedFieldOfView = 30f;

    float xRotation = 0f;

    public void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Update()
    {
        if (type == "firstPerson")
        {
            transform.parent.Rotate(Vector3.up * Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime);

            xRotation = xRotation - Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            if (!GetComponent<Camera>().enabled)
            {
                xRotation = 0f;
            }

            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
        else if (type == "thirdPerson")
        {
            transform.LookAt(transform.parent);
        }

        if (underwater) transform.GetComponent<PostProcessVolume>().profile = underwaterlayer;
        else transform.GetComponent<PostProcessVolume>().profile = defaultlayer;

        if (Input.GetKey(KeyCode.Z) && type == "firstPerson" && GetComponent<Camera>().enabled)
        {
            transform.GetComponent<Camera>().fieldOfView = zoomedFieldOfView;
        }
        else
        {
            transform.GetComponent<Camera>().fieldOfView = fieldOfView;
        }
    }
}