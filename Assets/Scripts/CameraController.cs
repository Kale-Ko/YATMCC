using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public enum CameraType
{
    FirstPerson,
    ThirdPerson,
    MainMenu
}

public class CameraController : MonoBehaviour
{
    public PostProcessProfile defaultlayer;
    public PostProcessProfile underwaterlayer;

    public bool underwater = false;

    public CameraType type = CameraType.FirstPerson;

    public float fieldOfView = 110f;
    public float zoomedFieldOfView = 30f;

    float xRotation = 0f;

    void Update()
    {
        if (type == CameraType.FirstPerson && !Main.Instance.paused)
        {
            transform.parent.Rotate(Vector3.up * Input.GetAxis("Mouse X") * Config.sensitivity);

            xRotation = xRotation - Input.GetAxis("Mouse Y") * Config.sensitivity;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

            if (Input.GetKey(KeyCode.Z))
            {
                transform.GetComponent<Camera>().fieldOfView = zoomedFieldOfView;
            }
            else
            {
                transform.GetComponent<Camera>().fieldOfView = fieldOfView;
            }
        }
        else if (type == CameraType.MainMenu)
        {
            xRotation += 8 * Time.deltaTime;
            transform.rotation = Quaternion.Euler(30, xRotation, 0);
        }

        if (underwater) transform.GetComponent<PostProcessVolume>().profile = underwaterlayer;
        else transform.GetComponent<PostProcessVolume>().profile = defaultlayer;
    }
}