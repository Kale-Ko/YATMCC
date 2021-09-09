using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public World world;

    public int chunkx;
    public int chunky;

    public void Render()
    {
        Mesh mesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        Mesh watermesh = new Mesh();

        List<Vector3> watervertices = new List<Vector3>();
        List<int> watertriangles = new List<int>();
        List<Vector2> wateruvs = new List<Vector2>();

        Dictionary<Vector3, Blocks> blocks = world.GetBlocks();
        Dictionary<Vector3, Blocks> waterblock = world.GetWaterBlocks();

        for (var x = chunkx * 16; x < (chunkx + 1) * 16; x++)
        {
            for (var z = chunky * 16; z < (chunky + 1) * 16; z++)
            {
                for (var y = 0; y < 128; y++)
                {
                    Vector3 blockPos = new Vector3(x, y, z);

                    int numFaces = 0;

                    if (blocks.ContainsKey(blockPos))
                    {
                        if (y < 128 && !blocks.ContainsKey(new Vector3(x, y + 1, z)))
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

                    if (waterblock.ContainsKey(blockPos))
                    {
                        if (y < 128 && !waterblock.ContainsKey(new Vector3(x, y + 1, z)))
                        {
                            watervertices.Add(blockPos + new Vector3(0, 1, 0));
                            watervertices.Add(blockPos + new Vector3(0, 1, 1));
                            watervertices.Add(blockPos + new Vector3(1, 1, 1));
                            watervertices.Add(blockPos + new Vector3(1, 1, 0));
                            numFaces++;

                            wateruvs.AddRange(GetUV("top", waterblock[blockPos]));
                        }

                        if (y > 0 && !waterblock.ContainsKey(new Vector3(x, y - 1, z)))
                        {
                            watervertices.Add(blockPos + new Vector3(0, 0, 0));
                            watervertices.Add(blockPos + new Vector3(1, 0, 0));
                            watervertices.Add(blockPos + new Vector3(1, 0, 1));
                            watervertices.Add(blockPos + new Vector3(0, 0, 1));
                            numFaces++;

                            wateruvs.AddRange(GetUV("bottom", waterblock[blockPos]));
                        }

                        if (!waterblock.ContainsKey(new Vector3(x, y, z - 1)))
                        {
                            watervertices.Add(blockPos + new Vector3(0, 0, 0));
                            watervertices.Add(blockPos + new Vector3(0, 1, 0));
                            watervertices.Add(blockPos + new Vector3(1, 1, 0));
                            watervertices.Add(blockPos + new Vector3(1, 0, 0));
                            numFaces++;

                            wateruvs.AddRange(GetUV("front", waterblock[blockPos]));
                        }

                        if (!waterblock.ContainsKey(new Vector3(x, y, z + 1)))
                        {
                            watervertices.Add(blockPos + new Vector3(1, 0, 1));
                            watervertices.Add(blockPos + new Vector3(1, 1, 1));
                            watervertices.Add(blockPos + new Vector3(0, 1, 1));
                            watervertices.Add(blockPos + new Vector3(0, 0, 1));
                            numFaces++;

                            wateruvs.AddRange(GetUV("back", waterblock[blockPos]));
                        }

                        if (!waterblock.ContainsKey(new Vector3(x - 1, y, z)))
                        {
                            watervertices.Add(blockPos + new Vector3(0, 0, 1));
                            watervertices.Add(blockPos + new Vector3(0, 1, 1));
                            watervertices.Add(blockPos + new Vector3(0, 1, 0));
                            watervertices.Add(blockPos + new Vector3(0, 0, 0));
                            numFaces++;

                            wateruvs.AddRange(GetUV("left", waterblock[blockPos]));
                        }

                        if (!waterblock.ContainsKey(new Vector3(x + 1, y, z)))
                        {
                            watervertices.Add(blockPos + new Vector3(1, 0, 0));
                            watervertices.Add(blockPos + new Vector3(1, 1, 0));
                            watervertices.Add(blockPos + new Vector3(1, 1, 1));
                            watervertices.Add(blockPos + new Vector3(1, 0, 1));
                            numFaces++;

                            wateruvs.AddRange(GetUV("right", waterblock[blockPos]));
                        }

                        int tl = watervertices.Count - 4 * numFaces;
                        for (int i = 0; i < numFaces; i++)
                        {
                            watertriangles.AddRange(new int[] { tl + i * 4, tl + i * 4 + 1, tl + i * 4 + 2, tl + i * 4, tl + i * 4 + 2, tl + i * 4 + 3 });
                        }
                    }
                }
            }
        }

        mesh.name = "Chunk " + chunkx + " " + chunky;
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();

        mesh.RecalculateNormals();

        transform.GetChild(0).GetComponent<MeshFilter>().mesh = mesh;
        transform.GetChild(0).GetComponent<MeshCollider>().sharedMesh = mesh;

        watermesh.name = "Water Chunk " + chunkx + " " + chunky;
        watermesh.vertices = watervertices.ToArray();
        watermesh.triangles = watertriangles.ToArray();
        watermesh.uv = wateruvs.ToArray();

        watermesh.RecalculateNormals();

        transform.GetChild(1).GetComponent<MeshFilter>().mesh = watermesh;
    }

    public Vector2[] GetUV(string side, Blocks id)
    {
        int x = (int)id;
        int y = 0;

        while (x > 16) { x -= 16; y++; }

        return new Vector2[] {
            new Vector2(x / 16f + .001f, y / 16f + .001f),
            new Vector2(x / 16f + .001f, (y + 1) / 16f - .001f),
            new Vector2((x + 1) / 16f - .001f, (y + 1) / 16f - .001f),
            new Vector2((x + 1) / 16f - .001f, y / 16f + .001f)
        };
    }
}