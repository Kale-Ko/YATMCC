using System.Collections.Generic;
using UnityEngine;
using FastNoiseLite;

public class World : MonoBehaviour
{
    public static World Instance;

    public GameObject chunkPrefab;

    public int distance = 2;

    Dictionary<Vector2, GameObject> chunks = new Dictionary<Vector2, GameObject>();
    Dictionary<Vector3, Block> blocks = new Dictionary<Vector3, Block>();

    int seed;

    Noise noise;
    Noise noise2;

    Noise heightmap;
    Noise tempmap;
    Noise moisturemap;

    void Start()
    {
        World.Instance = this;

        seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);

        noise = new Noise(seed);
        noise.SetNoiseType(Noise.NoiseType.Perlin);

        noise2 = new Noise(seed + 1);
        noise2.SetNoiseType(Noise.NoiseType.Perlin);
        noise2.SetFrequency(0.02f);

        heightmap = new Noise(seed + 2);
        heightmap.SetNoiseType(Noise.NoiseType.Perlin);
        heightmap.SetFrequency(0.05f);

        tempmap = new Noise(seed + 3);
        tempmap.SetNoiseType(Noise.NoiseType.Perlin);
        tempmap.SetFrequency(0.05f);

        moisturemap = new Noise(seed + 4);
        moisturemap.SetNoiseType(Noise.NoiseType.Perlin);
        moisturemap.SetFrequency(0.05f);

        InvokeRepeating("GenerateWorld", 0.5f, 0.1f);
    }

    public void GenerateWorld()
    {
        Vector2 pos = new Vector2(Mathf.FloorToInt(PlayerController.Instance.transform.position.x / 16), Mathf.FloorToInt(PlayerController.Instance.transform.position.z / 16));

        foreach (Transform child in transform)
        {
            if (!child.name.Contains("Chunk")) continue;

            if (child.GetComponent<Chunk>().chunkx < pos.x - distance || child.GetComponent<Chunk>().chunkx > pos.x + distance || child.GetComponent<Chunk>().chunky < pos.y - distance || child.GetComponent<Chunk>().chunky > pos.y + distance) child.GetComponent<MeshRenderer>().enabled = false;
        }

        for (var x = pos.x - distance; x < pos.x + 1 + distance; x++)
        {
            for (var y = pos.y - distance; y < pos.y + 1 + distance; y++)
            {
                GenerateChunk(x, y);
            }
        }

        for (var x = pos.x - distance; x < pos.x + 1 + distance; x++)
        {
            for (var y = pos.y - distance; y < pos.y + 1 + distance; y++)
            {
                if (chunks.ContainsKey(new Vector2(x, y)))
                {
                    chunks[new Vector2(x, y)].GetComponent<MeshRenderer>().enabled = true;

                    continue;
                }

                GameObject newchunk = Instantiate(chunkPrefab);
                newchunk.name = "Chunk " + x + ", " + y;
                newchunk.transform.parent = transform;
                newchunk.transform.GetComponent<Chunk>().chunkx = x;
                newchunk.transform.GetComponent<Chunk>().chunky = y;
                newchunk.transform.GetComponent<Chunk>().Render();

                chunks.Add(new Vector2(x, y), newchunk);
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
                Biome biome = Biomes.Forest; // Biomes.GetBiome(Mathf.RoundToInt(heightmap.GetNoise(blockx, blocky) * 64) + 64, Mathf.RoundToInt(tempmap.GetNoise(blockx, blocky) * 5) + 5, Mathf.RoundToInt(moisturemap.GetNoise(blockx, blocky) * 5) + 5);

                float ylevel = Mathf.Round(biome.height + ((noise.GetNoise(blockx, blocky) * biome.scale) * (noise2.GetNoise(blockx, blocky) * 2)));

                SetBlock(new Vector3(blockx, ylevel, blocky), biome.topblock);
                for (var newy = ylevel - 1; newy > ylevel - 5; newy--) SetBlock(new Vector3(blockx, newy, blocky), biome.middleblock);
                for (var newy = ylevel - 5; newy > 0; newy--) SetBlock(new Vector3(blockx, newy, blocky), biome.bottomblock);
                SetBlock(new Vector3(blockx, 0, blocky), Blocks.Bedrock);

                for (var newy = 64; newy > ylevel; newy--) SetBlock(new Vector3(blockx, newy, blocky), Blocks.Water);

                if (Random.Range(0, 32 - (biome.treeamount * 2)) == 0 && ylevel > 64)
                {
                    int height = Mathf.RoundToInt(ylevel) + biome.tree.height + Mathf.RoundToInt(Random.Range(-biome.tree.variation, biome.tree.variation));
                    for (var newy = ylevel; newy < height; newy++) SetBlock(new Vector3(blockx, newy, blocky), biome.tree.trunk);

                    for (var newx = -2; newx <= 2; newx++)
                    {
                        for (var newy = -2; newy <= 2; newy++)
                        {
                            SetBlock(new Vector3(blockx + newx, height, blocky + newy), biome.tree.leaves);
                            SetBlock(new Vector3(blockx + newx, height - 1, blocky + newy), biome.tree.leaves);

                            if ((newx == 2 || newy == 2 || newx == -2 || newy == -2) && Random.Range(0, 1) == 1) RemoveBlock(new Vector3(blockx + newx, height, blocky + newy));
                            if ((newx == 2 || newy == 2 || newx == -2 || newy == -2) && Random.Range(0, 1) == 1) RemoveBlock(new Vector3(blockx + newx, height - 1, blocky + newy));
                        }
                    }
                }
            }
        }
    }

    public Block GetBlock(Vector3 pos) { return blocks[pos]; }

    public void SetBlock(Vector3 pos, Block block)
    {
        if (blocks.ContainsKey(pos)) return;

        blocks.Add(pos, block);
    }

    public void RemoveBlock(Vector3 pos) { blocks.Remove(pos); }

    public bool IsBlock(Vector3 pos)
    {
        bool exists = blocks.ContainsKey(pos);

        if (!exists) return exists;
        else return GetBlock(pos) != Blocks.Water;
    }

    public bool IsWater(Vector3 pos)
    {
        bool exists = blocks.ContainsKey(pos);

        if (!exists) return exists;
        else return GetBlock(pos) == Blocks.Water;
    }
}