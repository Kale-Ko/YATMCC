/**
    MIT License
    Copyright (c) 2021 Kale Ko
    See https://kaleko.ga/license.txt
*/

using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public float chunkx;
    public float chunky;

    public Dictionary<Vector3, BlockType> blocks = new Dictionary<Vector3, BlockType>();

    public bool rendered = false;

    Mesh[] GenerateMeshes()
    {
        Mesh landmesh = new Mesh();
        Mesh watermesh = new Mesh();

        List<Vector3> landvertices = new List<Vector3>();
        List<int> landtriangles = new List<int>();
        List<Vector2> landuvs = new List<Vector2>();

        List<Vector3> watervertices = new List<Vector3>();
        List<int> watertriangles = new List<int>();
        List<Vector2> wateruvs = new List<Vector2>();

        landmesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        watermesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        foreach (var block in blocks)
        {
            Vector3 blockpos = block.Key;

            if (World.Instance.IsBlock(blockpos))
            {
                int numFaces = 0;

                if (blockpos.y == 255 || !World.Instance.IsBlock(new Vector3(blockpos.x, blockpos.y + 1, blockpos.z)))
                {
                    landvertices.Add(blockpos + new Vector3(0, 1, 0));
                    landvertices.Add(blockpos + new Vector3(0, 1, 1));
                    landvertices.Add(blockpos + new Vector3(1, 1, 1));
                    landvertices.Add(blockpos + new Vector3(1, 1, 0));
                    numFaces++;

                    landuvs.AddRange(Blocks.GetUV("top", World.Instance.GetBlock(blockpos)));
                }

                if (blockpos.y == 0 || !World.Instance.IsBlock(new Vector3(blockpos.x, blockpos.y - 1, blockpos.z)))
                {
                    landvertices.Add(blockpos + new Vector3(0, 0, 0));
                    landvertices.Add(blockpos + new Vector3(1, 0, 0));
                    landvertices.Add(blockpos + new Vector3(1, 0, 1));
                    landvertices.Add(blockpos + new Vector3(0, 0, 1));
                    numFaces++;

                    landuvs.AddRange(Blocks.GetUV("bottom", World.Instance.GetBlock(blockpos)));
                }

                if (!World.Instance.IsBlock(new Vector3(blockpos.x, blockpos.y, blockpos.z - 1)))
                {
                    landvertices.Add(blockpos + new Vector3(0, 0, 0));
                    landvertices.Add(blockpos + new Vector3(0, 1, 0));
                    landvertices.Add(blockpos + new Vector3(1, 1, 0));
                    landvertices.Add(blockpos + new Vector3(1, 0, 0));
                    numFaces++;

                    landuvs.AddRange(Blocks.GetUV("side", World.Instance.GetBlock(blockpos)));
                }

                if (!World.Instance.IsBlock(new Vector3(blockpos.x, blockpos.y, blockpos.z + 1)))
                {
                    landvertices.Add(blockpos + new Vector3(1, 0, 1));
                    landvertices.Add(blockpos + new Vector3(1, 1, 1));
                    landvertices.Add(blockpos + new Vector3(0, 1, 1));
                    landvertices.Add(blockpos + new Vector3(0, 0, 1));
                    numFaces++;

                    landuvs.AddRange(Blocks.GetUV("side", World.Instance.GetBlock(blockpos)));
                }

                if (!World.Instance.IsBlock(new Vector3(blockpos.x - 1, blockpos.y, blockpos.z)))
                {
                    landvertices.Add(blockpos + new Vector3(0, 0, 1));
                    landvertices.Add(blockpos + new Vector3(0, 1, 1));
                    landvertices.Add(blockpos + new Vector3(0, 1, 0));
                    landvertices.Add(blockpos + new Vector3(0, 0, 0));
                    numFaces++;

                    landuvs.AddRange(Blocks.GetUV("side", World.Instance.GetBlock(blockpos)));
                }

                if (!World.Instance.IsBlock(new Vector3(blockpos.x + 1, blockpos.y, blockpos.z)))
                {
                    landvertices.Add(blockpos + new Vector3(1, 0, 0));
                    landvertices.Add(blockpos + new Vector3(1, 1, 0));
                    landvertices.Add(blockpos + new Vector3(1, 1, 1));
                    landvertices.Add(blockpos + new Vector3(1, 0, 1));
                    numFaces++;

                    landuvs.AddRange(Blocks.GetUV("side", World.Instance.GetBlock(blockpos)));
                }

                int tl = landvertices.Count - 4 * numFaces;
                for (int i = 0; i < numFaces; i++)
                {
                    landtriangles.AddRange(new int[] { tl + i * 4, tl + i * 4 + 1, tl + i * 4 + 2, tl + i * 4, tl + i * 4 + 2, tl + i * 4 + 3 });
                }
            }
            else if (World.Instance.IsWater(blockpos))
            {
                int numFaces = 0;

                if (!World.Instance.IsWater(new Vector3(blockpos.x, blockpos.y + 1, blockpos.z)))
                {
                    if (blockpos.y == 255 || !World.Instance.IsWater(new Vector3(blockpos.x, blockpos.y + 1, blockpos.z)))
                    {
                        watervertices.Add(blockpos + new Vector3(0, 0.875f, 0));
                        watervertices.Add(blockpos + new Vector3(0, 0.875f, 1));
                        watervertices.Add(blockpos + new Vector3(1, 0.875f, 1));
                        watervertices.Add(blockpos + new Vector3(1, 0.875f, 0));
                        numFaces++;

                        wateruvs.AddRange(Blocks.GetUV("top", World.Instance.GetBlock(blockpos)));

                        watervertices.Add(blockpos + new Vector3(0, 0.875f, 0));
                        watervertices.Add(blockpos + new Vector3(1, 0.875f, 0));
                        watervertices.Add(blockpos + new Vector3(1, 0.875f, 1));
                        watervertices.Add(blockpos + new Vector3(0, 0.875f, 1));
                        numFaces++;

                        wateruvs.AddRange(Blocks.GetUV("top", World.Instance.GetBlock(blockpos)));
                    }

                    if (blockpos.y == 0 || !World.Instance.IsWater(new Vector3(blockpos.x, blockpos.y - 1, blockpos.z)))
                    {
                        watervertices.Add(blockpos + new Vector3(0, 0, 0));
                        watervertices.Add(blockpos + new Vector3(1, 0, 0));
                        watervertices.Add(blockpos + new Vector3(1, 0, 1));
                        watervertices.Add(blockpos + new Vector3(0, 0, 1));
                        numFaces++;

                        wateruvs.AddRange(Blocks.GetUV("bottom", World.Instance.GetBlock(blockpos)));
                    }

                    if (!World.Instance.IsWater(new Vector3(blockpos.x, blockpos.y, blockpos.z - 1)))
                    {
                        watervertices.Add(blockpos + new Vector3(0, 0, 0));
                        watervertices.Add(blockpos + new Vector3(0, 0.875f, 0));
                        watervertices.Add(blockpos + new Vector3(1, 0.875f, 0));
                        watervertices.Add(blockpos + new Vector3(1, 0, 0));
                        numFaces++;

                        wateruvs.AddRange(Blocks.GetUV("side", World.Instance.GetBlock(blockpos)));
                    }

                    if (!World.Instance.IsWater(new Vector3(blockpos.x, blockpos.y, blockpos.z + 1)))
                    {
                        watervertices.Add(blockpos + new Vector3(1, 0, 1));
                        watervertices.Add(blockpos + new Vector3(1, 0.875f, 1));
                        watervertices.Add(blockpos + new Vector3(0, 0.875f, 1));
                        watervertices.Add(blockpos + new Vector3(0, 0, 1));
                        numFaces++;

                        wateruvs.AddRange(Blocks.GetUV("side", World.Instance.GetBlock(blockpos)));
                    }

                    if (!World.Instance.IsWater(new Vector3(blockpos.x - 1, blockpos.y, blockpos.z)))
                    {
                        watervertices.Add(blockpos + new Vector3(0, 0, 1));
                        watervertices.Add(blockpos + new Vector3(0, 0.875f, 1));
                        watervertices.Add(blockpos + new Vector3(0, 0.875f, 0));
                        watervertices.Add(blockpos + new Vector3(0, 0, 0));
                        numFaces++;

                        wateruvs.AddRange(Blocks.GetUV("side", World.Instance.GetBlock(blockpos)));
                    }

                    if (!World.Instance.IsWater(new Vector3(blockpos.x + 1, blockpos.y, blockpos.z)))
                    {
                        watervertices.Add(blockpos + new Vector3(1, 0, 0));
                        watervertices.Add(blockpos + new Vector3(1, 0.875f, 0));
                        watervertices.Add(blockpos + new Vector3(1, 0.875f, 1));
                        watervertices.Add(blockpos + new Vector3(1, 0, 1));
                        numFaces++;

                        wateruvs.AddRange(Blocks.GetUV("side", World.Instance.GetBlock(blockpos)));
                    }
                }
                else
                {
                    if (blockpos.y == 255 || !World.Instance.IsWater(new Vector3(blockpos.x, blockpos.y + 1, blockpos.z)))
                    {
                        watervertices.Add(blockpos + new Vector3(0, 1, 0));
                        watervertices.Add(blockpos + new Vector3(0, 1, 1));
                        watervertices.Add(blockpos + new Vector3(1, 1, 1));
                        watervertices.Add(blockpos + new Vector3(1, 1, 0));
                        numFaces++;

                        wateruvs.AddRange(Blocks.GetUV("top", World.Instance.GetBlock(blockpos)));
                    }

                    if (blockpos.y == 0 || !World.Instance.IsWater(new Vector3(blockpos.x, blockpos.y - 1, blockpos.z)))
                    {
                        watervertices.Add(blockpos + new Vector3(0, 0, 0));
                        watervertices.Add(blockpos + new Vector3(1, 0, 0));
                        watervertices.Add(blockpos + new Vector3(1, 0, 1));
                        watervertices.Add(blockpos + new Vector3(0, 0, 1));
                        numFaces++;

                        wateruvs.AddRange(Blocks.GetUV("bottom", World.Instance.GetBlock(blockpos)));
                    }

                    if (!World.Instance.IsWater(new Vector3(blockpos.x, blockpos.y, blockpos.z - 1)))
                    {
                        watervertices.Add(blockpos + new Vector3(0, 0, 0));
                        watervertices.Add(blockpos + new Vector3(0, 1, 0));
                        watervertices.Add(blockpos + new Vector3(1, 1, 0));
                        watervertices.Add(blockpos + new Vector3(1, 0, 0));
                        numFaces++;

                        wateruvs.AddRange(Blocks.GetUV("side", World.Instance.GetBlock(blockpos)));
                    }

                    if (!World.Instance.IsWater(new Vector3(blockpos.x, blockpos.y, blockpos.z + 1)))
                    {
                        watervertices.Add(blockpos + new Vector3(1, 0, 1));
                        watervertices.Add(blockpos + new Vector3(1, 1, 1));
                        watervertices.Add(blockpos + new Vector3(0, 1, 1));
                        watervertices.Add(blockpos + new Vector3(0, 0, 1));
                        numFaces++;

                        wateruvs.AddRange(Blocks.GetUV("side", World.Instance.GetBlock(blockpos)));
                    }

                    if (!World.Instance.IsWater(new Vector3(blockpos.x - 1, blockpos.y, blockpos.z)))
                    {
                        watervertices.Add(blockpos + new Vector3(0, 0, 1));
                        watervertices.Add(blockpos + new Vector3(0, 1, 1));
                        watervertices.Add(blockpos + new Vector3(0, 1, 0));
                        watervertices.Add(blockpos + new Vector3(0, 0, 0));
                        numFaces++;

                        wateruvs.AddRange(Blocks.GetUV("side", World.Instance.GetBlock(blockpos)));
                    }

                    if (!World.Instance.IsWater(new Vector3(blockpos.x + 1, blockpos.y, blockpos.z)))
                    {
                        watervertices.Add(blockpos + new Vector3(1, 0, 0));
                        watervertices.Add(blockpos + new Vector3(1, 1, 0));
                        watervertices.Add(blockpos + new Vector3(1, 1, 1));
                        watervertices.Add(blockpos + new Vector3(1, 0, 1));
                        numFaces++;

                        wateruvs.AddRange(Blocks.GetUV("side", World.Instance.GetBlock(blockpos)));
                    }
                }

                int tl = watervertices.Count - 4 * numFaces;
                for (int i = 0; i < numFaces; i++)
                {
                    watertriangles.AddRange(new int[] { tl + i * 4, tl + i * 4 + 1, tl + i * 4 + 2, tl + i * 4, tl + i * 4 + 2, tl + i * 4 + 3 });
                }
            }
        }

        landmesh.name = "Land " + chunkx + " " + chunky;
        landmesh.vertices = landvertices.ToArray();
        landmesh.triangles = landtriangles.ToArray();
        landmesh.uv = landuvs.ToArray();
        landmesh.RecalculateNormals();

        watermesh.name = "Water " + chunkx + " " + chunky;
        watermesh.vertices = watervertices.ToArray();
        watermesh.triangles = watertriangles.ToArray();
        watermesh.uv = wateruvs.ToArray();
        watermesh.RecalculateNormals();

        return new Mesh[] { landmesh, watermesh };
    }

    public void Render()
    {
        Mesh[] meshes = GenerateMeshes();

        transform.GetChild(0).GetComponent<MeshFilter>().mesh = meshes[0];
        transform.GetChild(0).GetComponent<MeshCollider>().sharedMesh = meshes[0];

        transform.GetChild(1).GetComponent<MeshFilter>().mesh = meshes[1];

        rendered = true;
    }

    public void Enable()
    {
        transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
        transform.GetChild(1).GetComponent<MeshRenderer>().enabled = true;
    }

    public void Disable()
    {
        transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
        transform.GetChild(1).GetComponent<MeshRenderer>().enabled = false;
    }
}