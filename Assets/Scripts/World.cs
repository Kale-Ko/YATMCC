using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using FastNoiseLite;

public class World : MonoBehaviour
{
    public static World Instance;

    public GameObject chunkPrefab;

    public bool titleScreen = false;
    public int seed = 0;

    Dictionary<Vector2, Chunk> chunks = new Dictionary<Vector2, Chunk>();

    Dictionary<Vector2, Texture2D> noiseData = new Dictionary<Vector2, Texture2D>();
    Dictionary<Vector2, Texture2D> noiseData2 = new Dictionary<Vector2, Texture2D>();
    Dictionary<Vector2, Texture2D> biomeNoiseData = new Dictionary<Vector2, Texture2D>();

    Noise noise;
    Noise noise2;

    Noise biomeNoise;

    void Start()
    {
        World.Instance = this;

        if (!titleScreen) seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);

        noise = new Noise(seed);
        noise.SetNoiseType(Noise.NoiseType.Perlin);

        noise2 = new Noise(seed + 1);
        noise2.SetNoiseType(Noise.NoiseType.Cellular);
        noise2.SetFrequency(0.025f);

        biomeNoise = new Noise(seed - 1);
        biomeNoise.SetNoiseType(Noise.NoiseType.Cellular);
        biomeNoise.SetCellularDistanceFunction(Noise.CellularDistanceFunction.Hybrid);
        biomeNoise.SetCellularReturnType(Noise.CellularReturnType.CellValue);
        biomeNoise.SetFrequency(0.015f);

        if (!titleScreen) InvokeRepeating("GenerateWorld", 0f, 0.1f);
#if UNITY_EDITOR
        else
        {
            GenerateWorld();

            List<CombineInstance> toCombine = new List<CombineInstance>();

            foreach (Transform child in transform)
            {
                if (!child.name.Contains("Chunk")) continue;

                CombineInstance newCombine = new CombineInstance();
                newCombine.mesh = child.GetComponent<MeshFilter>().mesh;
                newCombine.transform = child.localToWorldMatrix;

                toCombine.Add(newCombine);

                Destroy(child.gameObject);
            }

            Mesh fullWorldMesh = new Mesh();
            fullWorldMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            fullWorldMesh.CombineMeshes(toCombine.ToArray());

            transform.GetComponent<MeshFilter>().mesh = fullWorldMesh;

            AssetDatabase.CreateAsset(fullWorldMesh, "Assets/Assets/TitleMenuWorld.asset");
            AssetDatabase.SaveAssets();
        }
#endif
    }

    public void GenerateWorld()
    {
        Vector2 pos = (PlayerController.Instance != null ? new Vector2(Mathf.FloorToInt(PlayerController.Instance.transform.position.x / 16), Mathf.FloorToInt(PlayerController.Instance.transform.position.z / 16)) : new Vector2(0, 0));

        foreach (Transform child in transform)
        {
            if (!child.name.Contains("Chunk")) continue;

            if (child.GetComponent<Chunk>().chunkx < pos.x - Config.distance || child.GetComponent<Chunk>().chunkx > pos.x + Config.distance || child.GetComponent<Chunk>().chunky < pos.y - Config.distance || child.GetComponent<Chunk>().chunky > pos.y + Config.distance) child.GetComponent<MeshRenderer>().enabled = false;
        }

        for (var x = pos.x - Config.distance; x < pos.x + 1 + Config.distance; x++)
        {
            for (var y = pos.y - Config.distance; y < pos.y + 1 + Config.distance; y++)
            {
                if (chunks.ContainsKey(new Vector2(x, y))) continue;

                GameObject newchunk = Instantiate(chunkPrefab);
                newchunk.name = "Chunk " + x + ", " + y;
                newchunk.transform.parent = transform;
                newchunk.transform.GetComponent<Chunk>().chunkx = x;
                newchunk.transform.GetComponent<Chunk>().chunky = y;
                newchunk.transform.GetComponent<MeshRenderer>().enabled = false;

                chunks.Add(new Vector2(x, y), newchunk.GetComponent<Chunk>());

                GenerateChunk(x, y);
            }
        }

        for (var x = pos.x - Config.distance; x < pos.x + 1 + Config.distance; x++)
        {
            for (var y = pos.y - Config.distance; y < pos.y + 1 + Config.distance; y++)
            {
                if (chunks[new Vector2(x, y)].rendered) chunks[new Vector2(x, y)].GetComponent<MeshRenderer>().enabled = true;
                else chunks[new Vector2(x, y)].Render();
            }
        }
    }

    public void GenereateNoise(float x, float y)
    {
        noiseData.Add(new Vector2(x, y), new Texture2D(16, 16, TextureFormat.RGB24, false));
        noiseData2.Add(new Vector2(x, y), new Texture2D(16, 16, TextureFormat.RGB24, false));
        biomeNoiseData.Add(new Vector2(x, y), new Texture2D(16, 16, TextureFormat.RGB24, false));

        for (var blockx = 0; blockx < 16; blockx++)
        {
            for (var blocky = 0; blocky < 16; blocky++)
            {
                float data1 = (noise.GetNoise(blockx + (x * 16), blocky + (y * 16))) + 1;
                float data2 = (noise2.GetNoise(blockx + (x * 16), blocky + (y * 16))) + 1;
                float data3 = (biomeNoise.GetNoise(blockx + (x * 16), blocky + (y * 16))) + 1;

                noiseData[new Vector2(x, y)].SetPixel(Mathf.RoundToInt(blockx), Mathf.RoundToInt(blocky), new Color(data1, data1, data1));
                noiseData2[new Vector2(x, y)].SetPixel(Mathf.RoundToInt(blockx), Mathf.RoundToInt(blocky), new Color(data2, data2, data2));
                biomeNoiseData[new Vector2(x, y)].SetPixel(Mathf.RoundToInt(blockx), Mathf.RoundToInt(blocky), new Color(data3, data3, data3));
            }
        }

        noiseData[new Vector2(x, y)].Apply();
        noiseData2[new Vector2(x, y)].Apply();
        biomeNoiseData[new Vector2(x, y)].Apply();
    }

    public float GetNoise(int layer, float x, float y)
    {
        if (!noiseData.ContainsKey(new Vector2(Mathf.FloorToInt(x / 16), Mathf.FloorToInt(y / 16)))) GenereateNoise(Mathf.FloorToInt(x / 16), Mathf.FloorToInt(y / 16));

        if (layer == 0) return (noiseData[new Vector2(Mathf.FloorToInt(x / 16), Mathf.FloorToInt(y / 16))].GetPixel(Mathf.RoundToInt(x), Mathf.RoundToInt(y)).grayscale) - 1;
        else if (layer == 1) return (noiseData2[new Vector2(Mathf.FloorToInt(x / 16), Mathf.FloorToInt(y / 16))].GetPixel(Mathf.RoundToInt(x), Mathf.RoundToInt(y)).grayscale) - 1;
        else if (layer == 2) return (biomeNoiseData[new Vector2(Mathf.FloorToInt(x / 16), Mathf.FloorToInt(y / 16))].GetPixel(Mathf.RoundToInt(x), Mathf.RoundToInt(y)).grayscale) - 1;
        else return 0;
    }

    public Biome GetBiome(float x, float y) { return Biomes.biomes[Mathf.FloorToInt((GetNoise(2, Mathf.RoundToInt(x), Mathf.RoundToInt(y)) * (Biomes.biomes.Length / 2))) + (Biomes.biomes.Length / 2)]; }

    public float GetYLevel(float x, float y)
    {
        Vector2[] sample = {
            new Vector2(0, 0),

            new Vector2(1, 0), new Vector2(0, 1), new Vector2(-1, 0), new Vector2(0, -1),
            new Vector2(2, 0), new Vector2(0, 2), new Vector2(-2, 0), new Vector2(0, -2),
            new Vector2(3, 0), new Vector2(0, 3), new Vector2(-3, 0), new Vector2(0, -3),
            new Vector2(6, 0), new Vector2(0, 6), new Vector2(-6, 0), new Vector2(0, -6),
            new Vector2(10, 0), new Vector2(0, 10), new Vector2(-10, 0), new Vector2(0, -10),
            new Vector2(12, 0), new Vector2(0, 12), new Vector2(-12, 0), new Vector2(0, -12),
            new Vector2(15, 0), new Vector2(0, 15), new Vector2(-15, 0), new Vector2(0, -15),

            new Vector2(1, 1), new Vector2(-1, -1), new Vector2(1, -1), new Vector2(-1, 1),
            new Vector2(2, 1), new Vector2(2, -1), new Vector2(-2, 1), new Vector2(-2, -1), new Vector2(1, 2), new Vector2(-1, 2), new Vector2(1, -2), new Vector2(-1, -2),
            new Vector2(3, 3), new Vector2(-3, -3), new Vector2(3, -3), new Vector2(-3, 3),
            new Vector2(6, 6), new Vector2(-6, -6), new Vector2(6, -6), new Vector2(-6, 6),
            new Vector2(10, 10), new Vector2(-10, -10), new Vector2(10, -10), new Vector2(-10, 10),
            new Vector2(12, 12), new Vector2(-12, -12), new Vector2(12, -12), new Vector2(-12, 12),
            new Vector2(15, 15), new Vector2(-15, -15), new Vector2(15, -15), new Vector2(-15, 15)
        };

        int amount = 0;
        float total = 0;

        foreach (var pos in sample)
        {
            amount++;
            total += GetRawYLevel(x + pos.x, y + pos.y);
        }

        return Mathf.Round(total / amount);
    }

    public float GetRawYLevel(float x, float y)
    {
        Biome biome = GetBiome(x, y);

        return Mathf.Round(biome.height + ((GetNoise(0, x, y) * biome.scale) * (GetNoise(1, x, y) * biome.scale2)));
    }

    public void GenerateChunk(float x, float y)
    {
        for (var blockx = x * 16; blockx < (x + 1) * 16; blockx++)
        {
            for (var blocky = y * 16; blocky < (y + 1) * 16; blocky++)
            {
                Biome biome = GetBiome(blockx, blocky);

                float ylevel = GetYLevel(blockx, blocky);

                SetBlock(new Vector3(blockx, ylevel, blocky), biome.topblock);
                for (var newy = ylevel - 1; newy > ylevel - 5; newy--) SetBlock(new Vector3(blockx, newy, blocky), biome.middleblock);
                for (var newy = ylevel - 5; newy > 0; newy--) SetBlock(new Vector3(blockx, newy, blocky), biome.bottomblock);
                SetBlock(new Vector3(blockx, 0, blocky), Blocks.Bedrock);

                for (var newy = 64; newy > ylevel; newy--) SetBlock(new Vector3(blockx, newy, blocky), Blocks.Water);

                Random.InitState((seed + Mathf.RoundToInt(blockx)) - (Mathf.RoundToInt(blockx) * Mathf.RoundToInt(blocky)) - Mathf.RoundToInt(blocky));

                if (ylevel > 64 && biome.tree != Trees.None && Random.Range(0, 32 - (biome.treeamount * 2)) == 0)
                {
                    int height = Mathf.RoundToInt(ylevel) + biome.tree.height + Mathf.RoundToInt(Random.Range(-biome.tree.variation, biome.tree.variation));
                    for (var newy = ylevel; newy < height; newy++) SetBlock(new Vector3(blockx, newy, blocky), biome.tree.trunk);

                    if (biome.tree.leaves == Blocks.Air) continue;

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

    public bool ChunkExists(Vector3 pos)
    {
        if (!ValidPos(pos)) return false;

        return chunks.ContainsKey(new Vector2(Mathf.Floor(pos.x / 16), Mathf.Floor(pos.z / 16)));
    }

    public Chunk GetChunk(Vector3 pos)
    {
        if (!ValidPos(pos)) return null;

        if (!ChunkExists(pos)) return null;
        else return chunks[new Vector2(Mathf.Floor(pos.x / 16), Mathf.Floor(pos.z / 16))];
    }

    public bool ValidPos(Vector3 pos)
    {
        if (pos.y < 0 || pos.y > 255) return false;
        else return true;
    }

    public bool BlockExists(Vector3 pos)
    {
        if (!ValidPos(pos)) return false;

        if (!ChunkExists(pos)) return false;
        else return GetChunk(pos).blocks.ContainsKey(pos);
    }

    public Block GetBlock(Vector3 pos)
    {
        if (!ValidPos(pos)) return null;

        if (!BlockExists(pos)) return null;
        else return GetChunk(pos).blocks[pos];
    }

    public void SetBlock(Vector3 pos, Block block)
    {
        if (!ValidPos(pos)) return;

        if (BlockExists(pos)) RemoveBlock(pos);

        if (ChunkExists(pos)) GetChunk(pos).blocks.Add(pos, block);
    }

    public void RemoveBlock(Vector3 pos)
    {
        if (!ValidPos(pos)) return;

        if (!BlockExists(pos)) return;
        else if (ChunkExists(pos)) GetChunk(pos).blocks.Remove(pos);
    }

    public bool IsBlock(Vector3 pos)
    {
        if (!BlockExists(pos)) return false;
        else return GetBlock(pos) != Blocks.Water;
    }

    public bool IsWater(Vector3 pos)
    {
        if (!BlockExists(pos)) return false;
        else return GetBlock(pos) == Blocks.Water;
    }
}