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

    public Dictionary<Vector3, Block> blocks = new Dictionary<Vector3, Block>();

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

    public Dictionary<Vector3, Block> GetBlocks()
    {
        Dictionary<Vector3, Block> blocks = new Dictionary<Vector3, Block>();

        foreach (KeyValuePair<Vector3, Block> kvp in this.blocks) if (kvp.Value != Blocks.Water) blocks.Add(kvp.Key, kvp.Value);

        return blocks;
    }

    public Dictionary<Vector3, Block> GetWaterBlocks()
    {
        Dictionary<Vector3, Block> waterblocks = new Dictionary<Vector3, Block>();

        foreach (KeyValuePair<Vector3, Block> kvp in this.blocks) if (kvp.Value == Blocks.Water) waterblocks.Add(kvp.Key, kvp.Value);

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

        for (var x = centerx - (distance + 1); x < centerx + 2 + distance; x++)
        {
            for (var y = centery - (distance + 1); y < centery + 2 + distance; y++)
            {
                if (exists.Contains("Chunk " + x + ", " + y)) continue;

                Generate(x, y);

                if (x < centerx - distance || x > centerx + distance || y < centery - distance || y > centery + distance) continue;

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

    public void SetBlock(Vector3 pos, Block block) { if (!blocks.ContainsKey(pos)) blocks.Add(pos, block); }

    public void Generate(int chunkx, int chunky)
    {
        Noise heightmap = new Noise(seed);
        heightmap.SetNoiseType(Noise.NoiseType.Perlin);

        Noise tempmap = new Noise(seed + 1);
        heightmap.SetNoiseType(Noise.NoiseType.Perlin);

        for (var x = chunkx * 16; x < (chunkx + 1) * 16; x++)
        {
            for (var y = chunky * 16; y < (chunky + 1) * 16; y++)
            {
                Biome biome = Biomes.GetBiome(Mathf.RoundToInt(heightmap.GetNoise(x, y) * 128), Mathf.Round(tempmap.GetNoise(x, y) * 10));

                float ylevel = Mathf.Round(biome.height + (heightmap.GetNoise(x, y) * biome.scale));

                SetBlock(new Vector3(x, ylevel, y), biome.topblock);
                for (var newy = ylevel - 1; newy > ylevel - 5; newy--) SetBlock(new Vector3(x, newy, y), biome.middleblock);
                for (var newy = ylevel - 5; newy > 0; newy--) SetBlock(new Vector3(x, newy, y), biome.bottomblock);
                SetBlock(new Vector3(x, 0, y), Blocks.Bedrock);

                for (var newy = 64; newy > ylevel; newy--) SetBlock(new Vector3(x, newy, y), Blocks.Water);
            }
        }
    }
}