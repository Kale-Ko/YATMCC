using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject selectionBoxPrefab;

    public Camera firstPersonCamera;

    public Camera thirdPersonCamera;

    public CharacterController controller;

    public Transform groundChecker;

    public LayerMask ground;

    public float speed = 5f;

    public float sprintSpeed = 10f;

    public float jumpPower = 7f;

    public float gravity = 9.8f;

    GameObject selectionBox;

    Vector3 velocity;

    bool onGround;

    public void Start()
    {
        firstPersonCamera.enabled = true;
        thirdPersonCamera.enabled = false;

        selectionBox = Instantiate(selectionBoxPrefab);
    }

    public void Update()
    {
        onGround = Physics.CheckSphere(groundChecker.position, 0.2f, ground) && velocity.y < 0;

        if (onGround)
        {
            velocity.y = -0.5f;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                velocity.y = jumpPower;
            }
        }

        Vector3 move = (transform.right * Input.GetAxis("Horizontal")) + (transform.forward * Input.GetAxis("Vertical"));

        if (Input.GetKey(KeyCode.LeftShift)) controller.Move(move * sprintSpeed * Time.deltaTime);
        else controller.Move(move * speed * Time.deltaTime);

        velocity.y += -gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (firstPersonCamera.enabled)
            {
                firstPersonCamera.enabled = false;
                thirdPersonCamera.enabled = true;
            }
            else if (thirdPersonCamera.enabled)
            {
                firstPersonCamera.enabled = true;
                thirdPersonCamera.enabled = false;
            }
        }

        RaycastHit hit;

        if (Physics.Raycast(firstPersonCamera.transform.position, firstPersonCamera.transform.forward, out hit, 10f))
        {
            Vector3 position = hit.point;
            position += (hit.normal * -0.5f);

            selectionBox.transform.GetComponent<MeshRenderer>().enabled = true;
            selectionBox.transform.position = new Vector3(Mathf.Round(position.x) + 0.5f, Mathf.Round(position.y) + 0.5f, Mathf.Round(position.z) + 0.5f);

            if (position.x < 0) position.x = -position.x;
            if (position.z < 0) position.z = -position.z;

            while (position.x > 16) position.x -= 16;
            while (position.z > 16) position.z -= 16;

            if (Input.GetMouseButtonDown(0))
            {
                hit.transform.GetComponent<Chunk>().blocks.Remove(new Vector3(Mathf.Round(position.x), Mathf.Round(position.y), Mathf.Round(position.z)));

                hit.transform.GetComponent<Chunk>().Render();
            }
        }
        else selectionBox.transform.GetComponent<MeshRenderer>().enabled = false;
    }
}