using System.Collections.Generic;
using UnityEngine;
using FastNoiseLite;

public class World : MonoBehaviour
{
    public GameObject chunk;
    public GameObject player;

    Vector2 lastpos = new Vector2(int.MaxValue, int.MaxValue);

    public int distance = 2;

    public int seed;

    public Dictionary<Vector3, Blocks> blocks = new Dictionary<Vector3, Blocks>();

    void Start()
    {
        seed = Random.Range(int.MinValue, int.MaxValue);
    }

    void Update()
    {
        Vector2 pos = new Vector2(Mathf.RoundToInt(player.transform.position.x / 16), Mathf.RoundToInt(player.transform.position.z / 16));

        if (pos != lastpos)
        {
            lastpos = pos;

            GenerateWorld((int)pos.x, (int)pos.y);
        }
    }

    public Dictionary<Vector3, Blocks> GetBlocks()
    {
        Dictionary<Vector3, Blocks> blocks = new Dictionary<Vector3, Blocks>();

        foreach (KeyValuePair<Vector3, Blocks> kvp in this.blocks) if (kvp.Value != Blocks.Water) blocks.Add(kvp.Key, kvp.Value);

        return blocks;
    }

    public Dictionary<Vector3, Blocks> GetWaterBlocks()
    {
        Dictionary<Vector3, Blocks> waterblocks = new Dictionary<Vector3, Blocks>();

        foreach (KeyValuePair<Vector3, Blocks> kvp in this.blocks) if (kvp.Value == Blocks.Water) waterblocks.Add(kvp.Key, kvp.Value);

        return waterblocks;
    }

    public void GenerateWorld(int centerx, int centery)
    {
        List<string> exists = new List<string>();

        foreach (Transform child in transform)
        {
            if (!child.name.Contains("Chunk")) continue;

            bool shouldexist = false;

            for (var x = centerx - distance; x < centerx + 1 + distance; x++)
            {
                for (var y = centery - distance; y < centery + 1 + distance; y++)
                {
                    if (child.name == "Chunk " + x + ", " + y) shouldexist = true;
                }
            }

            if (!shouldexist) Destroy(child.gameObject);
            else exists.Add(child.name);
        }

        for (var x = centerx - distance; x < centerx + 1 + distance; x++)
        {
            for (var y = centery - distance; y < centery + 1 + distance; y++)
            {
                if (exists.Contains("Chunk " + x + ", " + y)) continue;

                Generate(x, y);

                GameObject newchunk = Instantiate(chunk);
                newchunk.name = "Chunk " + x + ", " + y;
                newchunk.transform.parent = transform;
                newchunk.transform.GetComponent<Chunk>().world = this;
                newchunk.transform.GetComponent<Chunk>().chunkx = x;
                newchunk.transform.GetComponent<Chunk>().chunky = y;
                newchunk.transform.GetComponent<Chunk>().Render();
            }
        }
    }

    public void SetBlock(Vector3 pos, Blocks block) { if (!blocks.ContainsKey(pos)) blocks.Add(pos, block); }

    public void Generate(int chunkx, int chunky)
    {
        Noise noise = new Noise(seed);
        noise.SetNoiseType(Noise.NoiseType.Perlin);

        Biome biome = Biomes.Plains;

        for (var x = chunkx * 16; x < (chunkx + 1) * 16; x++)
        {
            for (var y = chunky * 16; y < (chunky + 1) * 16; y++)
            {
                float ylevel = Mathf.Round(biome.height + (noise.GetNoise(x, y) * biome.scale));

                SetBlock(new Vector3(x, ylevel, y), biome.topblock);
                for (var newy = ylevel - 1; newy > ylevel - 5; newy--) SetBlock(new Vector3(x, newy, y), biome.middleblock);
                for (var newy = ylevel - 5; newy > 0; newy--) SetBlock(new Vector3(x, newy, y), biome.bottomblock);
                SetBlock(new Vector3(x, 0, y), Blocks.Bedrock);

                for (var newy = 64; newy > ylevel; newy--) SetBlock(new Vector3(x, newy, y), Blocks.Water);
            }
        }
    }
}