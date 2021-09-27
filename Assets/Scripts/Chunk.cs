using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public World world;

    public float chunkx;
    public float chunky;

    Mesh GenerateMesh(bool water)
    {
        Mesh mesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        Mesh watermesh = new Mesh();

        List<Vector3> watervertices = new List<Vector3>();
        List<int> watertriangles = new List<int>();
        List<Vector2> wateruvs = new List<Vector2>();

        for (var x = chunkx * 16; x < (chunkx + 1) * 16; x++)
        {
            for (var z = chunky * 16; z < (chunky + 1) * 16; z++)
            {
                for (var y = 0; y < 128; y++)
                {
                    Vector3 blockpos = new Vector3(x, y, z);

                    if (IsBlock(blockpos) && !water)
                    {
                        int numFaces = 0;

                        if (y == 128 || !IsBlock(new Vector3(x, y + 1, z)))
                        {
                            vertices.Add(blockpos + new Vector3(0, 1, 0));
                            vertices.Add(blockpos + new Vector3(0, 1, 1));
                            vertices.Add(blockpos + new Vector3(1, 1, 1));
                            vertices.Add(blockpos + new Vector3(1, 1, 0));
                            numFaces++;

                            uvs.AddRange(Blocks.GetUV("top", world.blocks[blockpos]));
                        }

                        if (y == 0 || !IsBlock(new Vector3(x, y - 1, z)))
                        {
                            vertices.Add(blockpos + new Vector3(0, 0, 0));
                            vertices.Add(blockpos + new Vector3(1, 0, 0));
                            vertices.Add(blockpos + new Vector3(1, 0, 1));
                            vertices.Add(blockpos + new Vector3(0, 0, 1));
                            numFaces++;

                            uvs.AddRange(Blocks.GetUV("bottom", world.blocks[blockpos]));
                        }

                        if (!IsBlock(new Vector3(x, y, z - 1)))
                        {
                            vertices.Add(blockpos + new Vector3(0, 0, 0));
                            vertices.Add(blockpos + new Vector3(0, 1, 0));
                            vertices.Add(blockpos + new Vector3(1, 1, 0));
                            vertices.Add(blockpos + new Vector3(1, 0, 0));
                            numFaces++;

                            uvs.AddRange(Blocks.GetUV("side", world.blocks[blockpos]));
                        }

                        if (!IsBlock(new Vector3(x, y, z + 1)))
                        {
                            vertices.Add(blockpos + new Vector3(1, 0, 1));
                            vertices.Add(blockpos + new Vector3(1, 1, 1));
                            vertices.Add(blockpos + new Vector3(0, 1, 1));
                            vertices.Add(blockpos + new Vector3(0, 0, 1));
                            numFaces++;

                            uvs.AddRange(Blocks.GetUV("side", world.blocks[blockpos]));
                        }

                        if (!IsBlock(new Vector3(x - 1, y, z)))
                        {
                            vertices.Add(blockpos + new Vector3(0, 0, 1));
                            vertices.Add(blockpos + new Vector3(0, 1, 1));
                            vertices.Add(blockpos + new Vector3(0, 1, 0));
                            vertices.Add(blockpos + new Vector3(0, 0, 0));
                            numFaces++;

                            uvs.AddRange(Blocks.GetUV("side", world.blocks[blockpos]));
                        }

                        if (!IsBlock(new Vector3(x + 1, y, z)))
                        {
                            vertices.Add(blockpos + new Vector3(1, 0, 0));
                            vertices.Add(blockpos + new Vector3(1, 1, 0));
                            vertices.Add(blockpos + new Vector3(1, 1, 1));
                            vertices.Add(blockpos + new Vector3(1, 0, 1));
                            numFaces++;

                            uvs.AddRange(Blocks.GetUV("side", world.blocks[blockpos]));
                        }

                        int tl = vertices.Count - 4 * numFaces;
                        for (int i = 0; i < numFaces; i++)
                        {
                            triangles.AddRange(new int[] { tl + i * 4, tl + i * 4 + 1, tl + i * 4 + 2, tl + i * 4, tl + i * 4 + 2, tl + i * 4 + 3 });
                        }
                    }
                    else if (IsWater(blockpos) && water)
                    {
                        int numFaces = 0;

                        if (y == 128 || !IsWater(new Vector3(x, y + 1, z)))
                        {
                            watervertices.Add(blockpos + new Vector3(0, 1, 0));
                            watervertices.Add(blockpos + new Vector3(0, 1, 1));
                            watervertices.Add(blockpos + new Vector3(1, 1, 1));
                            watervertices.Add(blockpos + new Vector3(1, 1, 0));
                            numFaces++;

                            wateruvs.AddRange(Blocks.GetUV("top", world.blocks[blockpos]));
                        }

                        if (y == 0 || !IsWater(new Vector3(x, y - 1, z)))
                        {
                            watervertices.Add(blockpos + new Vector3(0, 0, 0));
                            watervertices.Add(blockpos + new Vector3(1, 0, 0));
                            watervertices.Add(blockpos + new Vector3(1, 0, 1));
                            watervertices.Add(blockpos + new Vector3(0, 0, 1));
                            numFaces++;

                            wateruvs.AddRange(Blocks.GetUV("bottom", world.blocks[blockpos]));
                        }

                        if (!IsWater(new Vector3(x, y, z - 1)))
                        {
                            watervertices.Add(blockpos + new Vector3(0, 0, 0));
                            watervertices.Add(blockpos + new Vector3(0, 1, 0));
                            watervertices.Add(blockpos + new Vector3(1, 1, 0));
                            watervertices.Add(blockpos + new Vector3(1, 0, 0));
                            numFaces++;

                            wateruvs.AddRange(Blocks.GetUV("side", world.blocks[blockpos]));
                        }

                        if (!IsWater(new Vector3(x, y, z + 1)))
                        {
                            watervertices.Add(blockpos + new Vector3(1, 0, 1));
                            watervertices.Add(blockpos + new Vector3(1, 1, 1));
                            watervertices.Add(blockpos + new Vector3(0, 1, 1));
                            watervertices.Add(blockpos + new Vector3(0, 0, 1));
                            numFaces++;

                            wateruvs.AddRange(Blocks.GetUV("side", world.blocks[blockpos]));
                        }

                        if (!IsWater(new Vector3(x - 1, y, z)))
                        {
                            watervertices.Add(blockpos + new Vector3(0, 0, 1));
                            watervertices.Add(blockpos + new Vector3(0, 1, 1));
                            watervertices.Add(blockpos + new Vector3(0, 1, 0));
                            watervertices.Add(blockpos + new Vector3(0, 0, 0));
                            numFaces++;

                            wateruvs.AddRange(Blocks.GetUV("side", world.blocks[blockpos]));
                        }

                        if (!IsWater(new Vector3(x + 1, y, z)))
                        {
                            watervertices.Add(blockpos + new Vector3(1, 0, 0));
                            watervertices.Add(blockpos + new Vector3(1, 1, 0));
                            watervertices.Add(blockpos + new Vector3(1, 1, 1));
                            watervertices.Add(blockpos + new Vector3(1, 0, 1));
                            numFaces++;

                            wateruvs.AddRange(Blocks.GetUV("side", world.blocks[blockpos]));
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

        watermesh.name = "Water Chunk " + chunkx + " " + chunky;
        watermesh.vertices = watervertices.ToArray();
        watermesh.triangles = watertriangles.ToArray();
        watermesh.uv = wateruvs.ToArray();

        watermesh.RecalculateNormals();

        if (!water) return mesh;
        else return watermesh;
    }

    public void Render()
    {
        Mesh mesh = GenerateMesh(false);
        Mesh watermesh = GenerateMesh(true);

        transform.GetChild(0).GetComponent<MeshFilter>().mesh = mesh;
        transform.GetChild(0).GetComponent<MeshCollider>().sharedMesh = mesh;

        transform.GetChild(1).GetComponent<MeshFilter>().mesh = watermesh;
    }

    bool IsBlock(Vector3 pos)
    {
        bool exists = world.blocks.ContainsKey(pos);

        if (!exists) return exists;
        else return world.blocks[pos] != Blocks.Water;
    }

    bool IsWater(Vector3 pos)
    {
        bool exists = world.blocks.ContainsKey(pos);

        if (!exists) return exists;
        else return world.blocks[pos] == Blocks.Water;
    }
}