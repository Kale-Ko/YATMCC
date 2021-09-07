using System.Collections.Generic;
using UnityEngine;
using FastNoiseLite;

public class Chunk : MonoBehaviour
{
    public bool transparent;

    public Dictionary<Vector3, int> blocks = new Dictionary<Vector3, int>();

    public Dictionary<Vector3, int> waterblocks = new Dictionary<Vector3, int>();

    int chunksize = 16;
    int chunkheight = 128;

    float x;
    float y;

    public void Generate(int seed, float x, float y, int chunksize, int chunkheight, int waterlevel)
    {
        this.x = x;
        this.y = y;

        this.chunksize = chunksize;
        this.chunkheight = chunkheight;

        Noise noise = new Noise(seed);
        noise.SetNoiseType(Noise.NoiseType.Perlin);

        for (var chunkx = 0; chunkx < chunksize; chunkx++)
        {
            for (var chunky = 0; chunky < chunksize; chunky++)
            {
                int ylevel = waterlevel + Mathf.RoundToInt(noise.GetNoise((x * chunksize) + chunkx, (y * chunksize) + chunky) * 20f);

                blocks.Add(new Vector3(chunkx, ylevel, chunky), 2);
                for (var indexy = ylevel - 1; indexy >= ylevel - 4; indexy--) blocks.Add(new Vector3(chunkx, indexy, chunky), 1);
                for (var indexy = ylevel - 5; indexy > 0; indexy--) blocks.Add(new Vector3(chunkx, indexy, chunky), 0);
                blocks.Add(new Vector3(chunkx, 0, chunky), 3);

                if (ylevel < waterlevel) for (var indexy = waterlevel; indexy > ylevel; indexy--) waterblocks.Add(new Vector3(chunkx, indexy, chunky), 4);
            }
        }

        Render();
    }

    public void Render()
    {
        Mesh mesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        for (var x = 0; x < chunksize; x++)
        {
            for (var z = 0; z < chunksize; z++)
            {
                for (var y = 0; y < chunkheight; y++)
                {
                    Vector3 blockPos = new Vector3(x, y, z);
                    int numFaces = 0;

                    if (!transparent)
                    {
                        if (blocks.ContainsKey(blockPos))
                        {
                            if (y < chunkheight && !blocks.ContainsKey(new Vector3(x, y + 1, z)))
                            {
                                vertices.Add(blockPos + new Vector3(0, 1, 0));
                                vertices.Add(blockPos + new Vector3(0, 1, 1));
                                vertices.Add(blockPos + new Vector3(1, 1, 1));
                                vertices.Add(blockPos + new Vector3(1, 1, 0));
                                numFaces++;

                                uvs.AddRange(GetUV("top", blocks[blockPos]));
                            }

                            if (y > 0 && !blocks.ContainsKey(new Vector3(x, y - 1, z)))
                            {
                                vertices.Add(blockPos + new Vector3(0, 0, 0));
                                vertices.Add(blockPos + new Vector3(1, 0, 0));
                                vertices.Add(blockPos + new Vector3(1, 0, 1));
                                vertices.Add(blockPos + new Vector3(0, 0, 1));
                                numFaces++;

                                uvs.AddRange(GetUV("bottom", blocks[blockPos]));
                            }

                            if (!blocks.ContainsKey(new Vector3(x, y, z - 1)))
                            {
                                vertices.Add(blockPos + new Vector3(0, 0, 0));
                                vertices.Add(blockPos + new Vector3(0, 1, 0));
                                vertices.Add(blockPos + new Vector3(1, 1, 0));
                                vertices.Add(blockPos + new Vector3(1, 0, 0));
                                numFaces++;

                                uvs.AddRange(GetUV("front", blocks[blockPos]));
                            }

                            if (!blocks.ContainsKey(new Vector3(x, y, z + 1)))
                            {
                                vertices.Add(blockPos + new Vector3(1, 0, 1));
                                vertices.Add(blockPos + new Vector3(1, 1, 1));
                                vertices.Add(blockPos + new Vector3(0, 1, 1));
                                vertices.Add(blockPos + new Vector3(0, 0, 1));
                                numFaces++;

                                uvs.AddRange(GetUV("back", blocks[blockPos]));
                            }

                            if (!blocks.ContainsKey(new Vector3(x - 1, y, z)))
                            {
                                vertices.Add(blockPos + new Vector3(0, 0, 1));
                                vertices.Add(blockPos + new Vector3(0, 1, 1));
                                vertices.Add(blockPos + new Vector3(0, 1, 0));
                                vertices.Add(blockPos + new Vector3(0, 0, 0));
                                numFaces++;

                                uvs.AddRange(GetUV("left", blocks[blockPos]));
                            }

                            if (!blocks.ContainsKey(new Vector3(x + 1, y, z)))
                            {
                                vertices.Add(blockPos + new Vector3(1, 0, 0));
                                vertices.Add(blockPos + new Vector3(1, 1, 0));
                                vertices.Add(blockPos + new Vector3(1, 1, 1));
                                vertices.Add(blockPos + new Vector3(1, 0, 1));
                                numFaces++;

                                uvs.AddRange(GetUV("right", blocks[blockPos]));
                            }

                            int tl = vertices.Count - 4 * numFaces;
                            for (int i = 0; i < numFaces; i++)
                            {
                                triangles.AddRange(new int[] { tl + i * 4, tl + i * 4 + 1, tl + i * 4 + 2, tl + i * 4, tl + i * 4 + 2, tl + i * 4 + 3 });
                            }
                        }
                    }
                    else
                    {
                        if (waterblocks.ContainsKey(blockPos))
                        {
                            if (y < chunkheight && !waterblocks.ContainsKey(new Vector3(x, y + 1, z)))
                            {
                                vertices.Add(blockPos + new Vector3(0, 1, 0));
                                vertices.Add(blockPos + new Vector3(0, 1, 1));
                                vertices.Add(blockPos + new Vector3(1, 1, 1));
                                vertices.Add(blockPos + new Vector3(1, 1, 0));
                                numFaces++;

                                uvs.AddRange(GetUV("top", waterblocks[blockPos]));
                            }

                            if (y > 0 && !waterblocks.ContainsKey(new Vector3(x, y - 1, z)))
                            {
                                vertices.Add(blockPos + new Vector3(0, 0, 0));
                                vertices.Add(blockPos + new Vector3(1, 0, 0));
                                vertices.Add(blockPos + new Vector3(1, 0, 1));
                                vertices.Add(blockPos + new Vector3(0, 0, 1));
                                numFaces++;

                                uvs.AddRange(GetUV("bottom", waterblocks[blockPos]));
                            }

                            if (!waterblocks.ContainsKey(new Vector3(x, y, z - 1)))
                            {
                                vertices.Add(blockPos + new Vector3(0, 0, 0));
                                vertices.Add(blockPos + new Vector3(0, 1, 0));
                                vertices.Add(blockPos + new Vector3(1, 1, 0));
                                vertices.Add(blockPos + new Vector3(1, 0, 0));
                                numFaces++;

                                uvs.AddRange(GetUV("front", waterblocks[blockPos]));
                            }

                            if (!waterblocks.ContainsKey(new Vector3(x, y, z + 1)))
                            {
                                vertices.Add(blockPos + new Vector3(1, 0, 1));
                                vertices.Add(blockPos + new Vector3(1, 1, 1));
                                vertices.Add(blockPos + new Vector3(0, 1, 1));
                                vertices.Add(blockPos + new Vector3(0, 0, 1));
                                numFaces++;

                                uvs.AddRange(GetUV("back", waterblocks[blockPos]));
                            }

                            if (!waterblocks.ContainsKey(new Vector3(x - 1, y, z)))
                            {
                                vertices.Add(blockPos + new Vector3(0, 0, 1));
                                vertices.Add(blockPos + new Vector3(0, 1, 1));
                                vertices.Add(blockPos + new Vector3(0, 1, 0));
                                vertices.Add(blockPos + new Vector3(0, 0, 0));
                                numFaces++;

                                uvs.AddRange(GetUV("left", waterblocks[blockPos]));
                            }

                            if (!waterblocks.ContainsKey(new Vector3(x + 1, y, z)))
                            {
                                vertices.Add(blockPos + new Vector3(1, 0, 0));
                                vertices.Add(blockPos + new Vector3(1, 1, 0));
                                vertices.Add(blockPos + new Vector3(1, 1, 1));
                                vertices.Add(blockPos + new Vector3(1, 0, 1));
                                numFaces++;

                                uvs.AddRange(GetUV("right", waterblocks[blockPos]));
                            }

                            int tl = vertices.Count - 4 * numFaces;
                            for (int i = 0; i < numFaces; i++)
                            {
                                triangles.AddRange(new int[] { tl + i * 4, tl + i * 4 + 1, tl + i * 4 + 2, tl + i * 4, tl + i * 4 + 2, tl + i * 4 + 3 });
                            }
                        }
                    }
                }
            }
        }

        mesh.name = "Chunk " + x + " " + y;
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();

        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;
        if (!transparent) GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    public Vector2[] GetUV(string side, int id)
    {
        if (id > 256) id -= 256;

        return new Vector2[] {
            new Vector2(id / 16f + .001f, 0 / 16f + .001f),
            new Vector2(id / 16f + .001f, 1 / 16f - .001f),
            new Vector2((id + 1) / 16f - .001f, 1 / 16f - .001f),
            new Vector2((id + 1) / 16f - .001f, 0 / 16f + .001f)
        };
    }
}