using System.Collections.Generic;
using UnityEngine;
using FastNoiseLite;

public class World : MonoBehaviour
{
    public GameObject chunk;
    public GameObject block;
    public GameObject waterblock;
    public GameObject player;

    public int distance = 2;

    public int seed;

    public Dictionary<Vector3, Block> blocks = new Dictionary<Vector3, Block>();

    Noise noise;

    Noise heightmap;
    Noise tempmap;
    Noise moisturemap;

    void Start()
    {
        seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);

        noise = noise = new Noise(seed);
        noise.SetNoiseType(Noise.NoiseType.Perlin);

        heightmap = new Noise(seed + 1);
        heightmap.SetNoiseType(Noise.NoiseType.Perlin);
        heightmap.SetFrequency(0.05f);

        tempmap = new Noise(seed + 2);
        tempmap.SetNoiseType(Noise.NoiseType.Perlin);
        tempmap.SetFrequency(0.05f);

        moisturemap = new Noise(seed + 3);
        moisturemap.SetNoiseType(Noise.NoiseType.Perlin);
        moisturemap.SetFrequency(0.05f);

        GenerateWorld(true);

        InvokeRepeating("UpdateWorld", 0.5f, 0.1f);
    }

    void UpdateWorld() { GenerateWorld(false); }

    public void GenerateWorld(bool full)
    {
        Vector2 pos = new Vector2(Mathf.FloorToInt(player.transform.position.x / 16), Mathf.FloorToInt(player.transform.position.z / 16));

        foreach (Transform child in transform)
        {
            if (!child.name.Contains("Chunk")) continue;

            if (child.GetComponent<Chunk>().chunkx < pos.x - distance || child.GetComponent<Chunk>().chunkx > pos.x + distance || child.GetComponent<Chunk>().chunky < pos.y - distance || child.GetComponent<Chunk>().chunky > pos.y + distance) Destroy(child.gameObject);
        }

        for (var x = pos.x - (distance + 1); x < pos.x + 2 + distance; x++)
        {
            for (var y = pos.y - (distance + 1); y < pos.y + 2 + distance; y++)
            {
                GenerateChunk(x, y);

                if (x < pos.x - distance || x > pos.x + distance || y < pos.y - distance || y > pos.y + distance || GameObject.Find("/World/Chunk " + x + ", " + y) != null) continue;

                GameObject newchunk = Instantiate(chunk);
                newchunk.name = "Chunk " + x + ", " + y;
                newchunk.transform.parent = transform;
                newchunk.transform.GetComponent<Chunk>().world = this;
                newchunk.transform.GetComponent<Chunk>().chunkx = x;
                newchunk.transform.GetComponent<Chunk>().chunky = y;
                newchunk.transform.GetComponent<Chunk>().Render();

                if (!full) return;
            }
        }
    }

    public void GenerateChunk(float x, float y)
    {
        if (blocks.ContainsKey(new Vector3(x * 16, 0, y * 16))) return;

        for (var blockx = x * 16; blockx < (x + 1) * 16; blockx++)
        {
            for (var blocky = y * 16; blocky < (y + 1) * 16; blocky++)
            {
                Biome biome = Biomes.GetBiome(Mathf.RoundToInt(heightmap.GetNoise(blockx, blocky) * 64) + 64, Mathf.RoundToInt(tempmap.GetNoise(blockx, blocky) * 5) + 5, Mathf.RoundToInt(moisturemap.GetNoise(blockx, blocky) * 5) + 5);

                float ylevel = Mathf.Round(biome.height + (noise.GetNoise(blockx, blocky) * biome.scale));

                SetBlock(new Vector3(blockx, ylevel, blocky), biome.topblock);
                for (var newy = ylevel - 1; newy > ylevel - 5; newy--) SetBlock(new Vector3(blockx, newy, blocky), biome.middleblock);
                for (var newy = ylevel - 5; newy > 0; newy--) SetBlock(new Vector3(blockx, newy, blocky), biome.bottomblock);
                SetBlock(new Vector3(blockx, 0, blocky), Blocks.Bedrock);

                for (var newy = 64; newy > ylevel; newy--) SetBlock(new Vector3(blockx, newy, blocky), Blocks.Water);
            }
        }
    }

    public Block GetBlock(Vector3 pos) { return blocks[pos]; }

    public void SetBlock(Vector3 pos, Block block)
    {
        if (blocks.ContainsKey(pos)) return;

        blocks.Add(pos, block);
    }

    public void RemoveBlock(Vector3 pos) { blocks.Remove(pos); SetBlock(pos, Blocks.Air); }
}