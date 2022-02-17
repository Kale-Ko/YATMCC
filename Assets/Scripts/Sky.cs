using UnityEngine;

public class Sky : MonoBehaviour
{
    public static Sky Instance;

    public float time = 0;
    public int day = 0;

    void Start()
    {
        Sky.Instance = this;
        InvokeRepeating("Tick", 0, 0.05f);
    }

    void Update()
    {

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