using UnityEngine;

public class WorldTime : MonoBehaviour
{
    public float time = 0;

    void Start()
    {
        InvokeRepeating("Tick", 0, 0.05f);
    }

    void Update()
    {
        transform.GetChild(0).rotation = Quaternion.Euler(((time / 6000) * 360) - 90, 0, 0);
        transform.GetChild(1).rotation = Quaternion.Euler(((time / 6000) * 360) + 90, 0, 0);
    }

    public void Tick()
    {
        time += 1;
    }
}