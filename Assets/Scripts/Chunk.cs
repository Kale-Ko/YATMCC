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

        Dictionary<Vector3, Block> iteratorblocks = world.GetIteratorBlocks();
        Dictionary<Vector3, Block> blocks = world.GetBlocks();
        Dictionary<Vector3, Block> waterblocks = world.GetWaterBlocks();

        foreach (KeyValuePair<Vector3, Block> block in iteratorblocks)
        {
            if (block.Key.x < chunkx * 16 || block.Key.x > (chunkx + 1) * 16 || block.Key.z < chunky * 16 || block.Key.z > (chunky + 1) * 16) continue;

            if (block.Value != Blocks.Water)
            {
                int numFaces = 0;

                if (block.Key.y < 128 && !blocks.ContainsKey(new Vector3(block.Key.x, block.Key.y + 1, block.Key.z)))
                {
                    vertices.Add(block.Key + new Vector3(0, 1, 0));
                    vertices.Add(block.Key + new Vector3(0, 1, 1));
                    vertices.Add(block.Key + new Vector3(1, 1, 1));
                    vertices.Add(block.Key + new Vector3(1, 1, 0));
                    numFaces++;

                    uvs.AddRange(GetUV("top", block.Value));
                }

                if (block.Key.y > 0 && !blocks.ContainsKey(new Vector3(block.Key.x, block.Key.y - 1, block.Key.z)))
                {
                    vertices.Add(block.Key + new Vector3(0, 0, 0));
                    vertices.Add(block.Key + new Vector3(1, 0, 0));
                    vertices.Add(block.Key + new Vector3(1, 0, 1));
                    vertices.Add(block.Key + new Vector3(0, 0, 1));
                    numFaces++;

                    uvs.AddRange(GetUV("bottom", block.Value));
                }

                if (!blocks.ContainsKey(new Vector3(block.Key.x, block.Key.y, block.Key.z - 1)))
                {
                    vertices.Add(block.Key + new Vector3(0, 0, 0));
                    vertices.Add(block.Key + new Vector3(0, 1, 0));
                    vertices.Add(block.Key + new Vector3(1, 1, 0));
                    vertices.Add(block.Key + new Vector3(1, 0, 0));
                    numFaces++;

                    uvs.AddRange(GetUV("side", block.Value));
                }

                if (!blocks.ContainsKey(new Vector3(block.Key.x, block.Key.y, block.Key.z + 1)))
                {
                    vertices.Add(block.Key + new Vector3(1, 0, 1));
                    vertices.Add(block.Key + new Vector3(1, 1, 1));
                    vertices.Add(block.Key + new Vector3(0, 1, 1));
                    vertices.Add(block.Key + new Vector3(0, 0, 1));
                    numFaces++;

                    uvs.AddRange(GetUV("side", block.Value));
                }

                if (!blocks.ContainsKey(new Vector3(block.Key.x - 1, block.Key.y, block.Key.z)))
                {
                    vertices.Add(block.Key + new Vector3(0, 0, 1));
                    vertices.Add(block.Key + new Vector3(0, 1, 1));
                    vertices.Add(block.Key + new Vector3(0, 1, 0));
                    vertices.Add(block.Key + new Vector3(0, 0, 0));
                    numFaces++;

                    uvs.AddRange(GetUV("side", block.Value));
                }

                if (!blocks.ContainsKey(new Vector3(block.Key.x + 1, block.Key.y, block.Key.z)))
                {
                    vertices.Add(block.Key + new Vector3(1, 0, 0));
                    vertices.Add(block.Key + new Vector3(1, 1, 0));
                    vertices.Add(block.Key + new Vector3(1, 1, 1));
                    vertices.Add(block.Key + new Vector3(1, 0, 1));
                    numFaces++;

                    uvs.AddRange(GetUV("side", block.Value));
                }

                int tl = vertices.Count - 4 * numFaces;
                for (int i = 0; i < numFaces; i++)
                {
                    triangles.AddRange(new int[] { tl + i * 4, tl + i * 4 + 1, tl + i * 4 + 2, tl + i * 4, tl + i * 4 + 2, tl + i * 4 + 3 });
                }
            }
            else
            {
                int numFaces = 0;

                if (block.Key.y < 128 && !waterblocks.ContainsKey(new Vector3(block.Key.x, block.Key.y + 1, block.Key.z)))
                {
                    watervertices.Add(block.Key + new Vector3(0, 1, 0));
                    watervertices.Add(block.Key + new Vector3(0, 1, 1));
                    watervertices.Add(block.Key + new Vector3(1, 1, 1));
                    watervertices.Add(block.Key + new Vector3(1, 1, 0));
                    numFaces++;

                    wateruvs.AddRange(GetUV("top", block.Value));
                }

                if (block.Key.y > 0 && !waterblocks.ContainsKey(new Vector3(block.Key.x, block.Key.y - 1, block.Key.z)))
                {
                    watervertices.Add(block.Key + new Vector3(0, 0, 0));
                    watervertices.Add(block.Key + new Vector3(1, 0, 0));
                    watervertices.Add(block.Key + new Vector3(1, 0, 1));
                    watervertices.Add(block.Key + new Vector3(0, 0, 1));
                    numFaces++;

                    wateruvs.AddRange(GetUV("bottom", block.Value));
                }

                if (!waterblocks.ContainsKey(new Vector3(block.Key.x, block.Key.y, block.Key.z - 1)))
                {
                    watervertices.Add(block.Key + new Vector3(0, 0, 0));
                    watervertices.Add(block.Key + new Vector3(0, 1, 0));
                    watervertices.Add(block.Key + new Vector3(1, 1, 0));
                    watervertices.Add(block.Key + new Vector3(1, 0, 0));
                    numFaces++;

                    wateruvs.AddRange(GetUV("side", block.Value));
                }

                if (!waterblocks.ContainsKey(new Vector3(block.Key.x, block.Key.y, block.Key.z + 1)))
                {
                    watervertices.Add(block.Key + new Vector3(1, 0, 1));
                    watervertices.Add(block.Key + new Vector3(1, 1, 1));
                    watervertices.Add(block.Key + new Vector3(0, 1, 1));
                    watervertices.Add(block.Key + new Vector3(0, 0, 1));
                    numFaces++;

                    wateruvs.AddRange(GetUV("side", block.Value));
                }

                if (!waterblocks.ContainsKey(new Vector3(block.Key.x - 1, block.Key.y, block.Key.z)))
                {
                    watervertices.Add(block.Key + new Vector3(0, 0, 1));
                    watervertices.Add(block.Key + new Vector3(0, 1, 1));
                    watervertices.Add(block.Key + new Vector3(0, 1, 0));
                    watervertices.Add(block.Key + new Vector3(0, 0, 0));
                    numFaces++;

                    wateruvs.AddRange(GetUV("side", block.Value));
                }

                if (!waterblocks.ContainsKey(new Vector3(block.Key.x + 1, block.Key.y, block.Key.z)))
                {
                    watervertices.Add(block.Key + new Vector3(1, 0, 0));
                    watervertices.Add(block.Key + new Vector3(1, 1, 0));
                    watervertices.Add(block.Key + new Vector3(1, 1, 1));
                    watervertices.Add(block.Key + new Vector3(1, 0, 1));
                    numFaces++;

                    wateruvs.AddRange(GetUV("side", block.Value));
                }

                int tl = watervertices.Count - 4 * numFaces;
                for (int i = 0; i < numFaces; i++)
                {
                    watertriangles.AddRange(new int[] { tl + i * 4, tl + i * 4 + 1, tl + i * 4 + 2, tl + i * 4, tl + i * 4 + 2, tl + i * 4 + 3 });
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

    public Vector2[] GetUV(string side, Block block)
    {
        int x = 1;
        int y = 0;

        if (side == "top") { x = block.toptexturex; y = block.toptexturey; }
        else if (side == "bottom") { x = block.bottomtexturex; y = block.bottomtexturey; }
        else if (side == "side") { x = block.sidetexturex; y = block.sidetexturey; }

        return new Vector2[] {
            new Vector2(x / 16f + .001f, y / 16f + .001f),
            new Vector2(x / 16f + .001f, (y + 1) / 16f - .001f),
            new Vector2((x + 1) / 16f - .001f, (y + 1) / 16f - .001f),
            new Vector2((x + 1) / 16f - .001f, y / 16f + .001f)
        };
    }
}