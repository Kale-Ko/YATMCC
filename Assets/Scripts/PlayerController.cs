using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    public GameObject selectionBoxPrefab;

    public LayerMask ground;

    public float speed = 5f;
    public float sprintSpeed = 10f;
    public float jumpPower = 10f;
    public float gravity = 10f;

    Vector3 velocity;

    bool onGround;

    Vector3 spawnPos = new Vector3(0, 0, 0);
    bool spawned = false;

    GameObject selectionBox;

    public void Start()
    {
        PlayerController.Instance = this;

        transform.GetChild(0).GetComponent<Camera>().enabled = true;
        transform.GetChild(0).GetChild(0).GetComponent<Camera>().enabled = false;

        selectionBox = Instantiate(selectionBoxPrefab);
    }

    public void Update()
    {
        if (Main.Instance.paused) return;

        transform.GetComponent<CharacterController>().enabled = true;

        if (spawnPos != new Vector3(0, 0, 0) && !spawned)
        {
            transform.GetComponent<CharacterController>().enabled = false;

            transform.position = spawnPos;
            spawned = true;
            spawnPos = new Vector3(0, 0, 0);

            return;
        }

        onGround = (Physics.CheckSphere(new Vector3(transform.GetChild(1).transform.position.x + 0.25f, transform.GetChild(1).transform.position.y, transform.GetChild(1).transform.position.z + 0.25f), 0.1f, ground) || Physics.CheckSphere(new Vector3(transform.GetChild(1).transform.position.x - 0.25f, transform.GetChild(1).transform.position.y, transform.GetChild(1).transform.position.z - 0.25f), 0.1f, ground) || Physics.CheckSphere(new Vector3(transform.GetChild(1).transform.position.x + 0.25f, transform.GetChild(1).transform.position.y, transform.GetChild(1).transform.position.z - 0.25f), 0.1f, ground) || Physics.CheckSphere(new Vector3(transform.GetChild(1).transform.position.x - 0.25f, transform.GetChild(1).transform.position.y, transform.GetChild(1).transform.position.z + 0.25f), 0.1f, ground)) && velocity.y < 0;

        if (onGround)
        {
            velocity.y = -0.5f;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                velocity.y = jumpPower;
            }
        }

        Vector3 move = (transform.right * Input.GetAxis("Horizontal")) + (transform.forward * Input.GetAxis("Vertical"));

        if (Input.GetKey(KeyCode.LeftShift)) transform.GetComponent<CharacterController>().Move(move * sprintSpeed * Time.deltaTime);
        else transform.GetComponent<CharacterController>().Move(move * speed * Time.deltaTime);

        velocity.y += -gravity * Time.deltaTime;

        transform.GetComponent<CharacterController>().Move(velocity * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (transform.GetChild(0).GetComponent<Camera>().enabled)
            {
                transform.GetChild(0).GetComponent<Camera>().enabled = false;
                transform.GetChild(0).GetChild(0).GetComponent<Camera>().enabled = true;
            }
            else if (transform.GetChild(0).GetChild(0).GetComponent<Camera>().enabled)
            {
                transform.GetChild(0).GetComponent<Camera>().enabled = true;
                transform.GetChild(0).GetChild(0).GetComponent<Camera>().enabled = false;
            }
        }

        if (World.Instance.IsWater(new Vector3(Mathf.Floor(transform.position.x), Mathf.Floor(transform.position.y + 0.75f), Mathf.Floor(transform.position.z))))
        {
            transform.GetChild(0).GetComponent<Camera>().GetComponent<CameraController>().underwater = true;
            transform.GetChild(0).GetChild(0).GetComponent<Camera>().GetComponent<CameraController>().underwater = true;
        }
        else
        {
            transform.GetChild(0).GetComponent<Camera>().GetComponent<CameraController>().underwater = false;
            transform.GetChild(0).GetChild(0).GetComponent<Camera>().GetComponent<CameraController>().underwater = false;
        }

        RaycastHit hit;

        if (Physics.Raycast(transform.GetChild(0).GetComponent<Camera>().transform.position, transform.GetChild(0).GetComponent<Camera>().transform.forward, out hit, 4f, ground))
        {
            Vector3 position = hit.point;
            position += (hit.normal * -0.5f);

            int chunkx = Mathf.FloorToInt(position.x / 16);
            int chunky = Mathf.FloorToInt(position.z / 16);

            Vector3 localposition = new Vector3(Mathf.Floor(position.x - (chunkx * 16)), Mathf.Floor(position.y), Mathf.Floor(position.z - (chunky * 16)));

            selectionBox.transform.GetComponent<MeshRenderer>().enabled = true;
            selectionBox.transform.position = new Vector3(Mathf.Floor(position.x) + 0.5f, Mathf.Floor(position.y) + 0.5f, Mathf.Floor(position.z) + 0.5f);

            if (Mathf.Floor(position.y) != 0)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    World.Instance.RemoveBlock(new Vector3(Mathf.Floor(position.x), Mathf.Floor(position.y), Mathf.Floor(position.z)));

                    GameObject.Find("Chunk " + chunkx + ", " + chunky).GetComponent<Chunk>().Render();
                    if (localposition.x == 0) GameObject.Find("Chunk " + (chunkx - 1) + ", " + chunky).GetComponent<Chunk>().Render();
                    if (localposition.x == 15 || localposition.x == 16) GameObject.Find("Chunk " + (chunkx + 1) + ", " + chunky).GetComponent<Chunk>().Render();
                    if (localposition.z == 0) GameObject.Find("Chunk " + chunkx + ", " + (chunky - 1)).GetComponent<Chunk>().Render();
                    if (localposition.z == 15 || localposition.z == 16) GameObject.Find("Chunk " + chunkx + ", " + (chunky + 1)).GetComponent<Chunk>().Render();
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    position -= (hit.normal * -0.5f);
                    position += (hit.normal * 0.5f);

                    if (new Vector3(Mathf.Floor(position.x), Mathf.Floor(position.y), Mathf.Floor(position.z)) != new Vector3(Mathf.Floor(transform.position.x), Mathf.Floor(transform.position.y), Mathf.Floor(transform.position.z)) && new Vector3(Mathf.Floor(position.x), Mathf.Floor(position.y), Mathf.Floor(position.z)) != new Vector3(Mathf.Floor(transform.position.x), Mathf.Floor(transform.position.y + 1f), Mathf.Floor(transform.position.z)))
                    {
                        World.Instance.SetBlock(new Vector3(Mathf.Floor(position.x), Mathf.Floor(position.y), Mathf.Floor(position.z)), Blocks.Dirt);

                        GameObject.Find("Chunk " + chunkx + ", " + chunky).GetComponent<Chunk>().Render();
                        if (localposition.x == 0) GameObject.Find("Chunk " + (chunkx - 1) + ", " + chunky).GetComponent<Chunk>().Render();
                        if (localposition.x == 15 || localposition.x == 16) GameObject.Find("Chunk " + (chunkx + 1) + ", " + chunky).GetComponent<Chunk>().Render();
                        if (localposition.z == 0) GameObject.Find("Chunk " + chunkx + ", " + (chunky - 1)).GetComponent<Chunk>().Render();
                        if (localposition.z == 15 || localposition.z == 16) GameObject.Find("Chunk " + chunkx + ", " + (chunky + 1)).GetComponent<Chunk>().Render();
                    }
                }
            }
        }
        else selectionBox.transform.GetComponent<MeshRenderer>().enabled = false;
    }

    public void Spawn(Vector3 pos)
    {
        spawnPos = pos;
        spawned = false;
    }
}