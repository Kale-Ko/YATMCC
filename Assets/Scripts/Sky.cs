using UnityEngine;

public class Sky : MonoBehaviour
{
    public float time = 0;
    public int day = 0;

    void Start()
    {
        InvokeRepeating("Tick", 0, 0.05f);
    }

    void Update()
    {
        transform.position = (PlayerController.Instance != null ? PlayerController.Instance.transform.position : new Vector3(0, 0, 0));

        transform.GetChild(0).rotation = Quaternion.Euler(((time / 24000) * 360) - 90, 0, 0);
        transform.GetChild(1).rotation = Quaternion.Euler(((time / 24000) * 360) + 90, 0, 0);
        transform.GetComponent<MeshRenderer>().material.mainTextureOffset = new Vector2(time / 24000, 0);
    }

    public void Tick()
    {
        time += 1;

        if (time > 24000)
        {
            time = 0;

            day++;
        }
    }
}