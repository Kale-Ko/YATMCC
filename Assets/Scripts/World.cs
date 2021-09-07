using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public int distance = 2;

    public int chunksize = 16;
    public int chunkheight = 128;

    public int waterlevel = 64;

    public GameObject chunkprefab;

    public GameObject player;

    Dictionary<Vector2, GameObject> chunkmap = new Dictionary<Vector2, GameObject>();

    int seed;

    Vector2 prevpos = new Vector2(float.MaxValue, float.MaxValue);

    void Start()
    {
        seed = Random.Range(int.MinValue, int.MaxValue);
    }

    void Update()
    {
        Vector2 playerpos = new Vector2(Mathf.Round(player.transform.position.x / 16), Mathf.Round(player.transform.position.z / 16));

        if (playerpos != prevpos)
        {
            prevpos = playerpos;

            Generate();
        }
    }

    public void Generate()
    {
        GameObject world = GameObject.Find("World");

        for (var index = 0; index < world.transform.childCount; index++)
        {
            GameObject child = world.transform.GetChild(index).gameObject;

            bool shouldexist = false;

            for (var x = -distance + prevpos.x; x <= distance + prevpos.x; x++)
            {
                for (var y = -distance + prevpos.y; y <= distance + prevpos.y; y++)
                {
                    if (child.name == "Chunk " + x + " " + y || !child.name.Contains("Chunk")) shouldexist = true;
                }
            }

            if (!shouldexist)
            {
                chunkmap.Remove(new Vector2(int.Parse(child.name.Replace("Chunk ", "").Split(" ".ToCharArray())[0]), int.Parse(child.name.Replace("Chunk ", "").Split(" ".ToCharArray())[1])));

                Destroy(child);
            }
        }

        for (var x = -distance + prevpos.x; x <= distance + prevpos.x; x++)
        {
            for (var y = -distance + prevpos.y; y <= distance + prevpos.y; y++)
            {
                if (GameObject.Find("Chunk " + x + " " + y) == null)
                {
                    GameObject chunk = Instantiate(chunkprefab);
                    chunk.name = "Chunk " + x + " " + y;
                    chunk.transform.parent = world.transform;
                    chunk.transform.position = new Vector3(x * chunksize, 0, y * chunksize);
                    chunk.transform.GetChild(0).GetComponent<Chunk>().Generate(seed, x, y, chunksize, chunkheight, waterlevel);
                    chunk.transform.GetChild(1).GetComponent<Chunk>().Generate(seed, x, y, chunksize, chunkheight, waterlevel);

                    chunkmap.Add(new Vector2(x, y), chunk);
                }
            }
        }
    }
}