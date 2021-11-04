using UnityEngine;

public class Block
{
    public string id;

    public int toptexturex;
    public int toptexturey;

    public int sidetexturex;
    public int sidetexturey;

    public int bottomtexturex;
    public int bottomtexturey;

    public Block(string id, int texturex, int texturey) : this(id, texturex, texturey, texturex, texturey, texturex, texturey) { }

    public Block(string id, int texturex, int texturey, int sidetexturex, int sidetexturey) : this(id, texturex, texturey, sidetexturex, sidetexturey, texturex, texturey) { }

    public Block(string id, int toptexturex, int toptexturey, int sidetexturex, int sidetexturey, int bottomtexturex, int bottomtexturey)
    {
        this.id = id;

        this.toptexturex = toptexturex;
        this.toptexturey = toptexturey;
        this.sidetexturex = sidetexturex;
        this.sidetexturey = sidetexturey;
        this.bottomtexturex = bottomtexturex;
        this.bottomtexturey = bottomtexturey;
    }
}

public class Blocks
{
    public static Block Air = new Block("air", -1, -1);
    public static Block Bedrock = new Block("bedrock", 0, 0);
    public static Block Stone = new Block("stone", 1, 0);
    public static Block Dirt = new Block("dirt", 2, 0);
    public static Block Grass = new Block("grass", 3, 0, 4, 0, 2, 0);
    public static Block Mud = new Block("mud", 5, 0);
    public static Block Log = new Block("log", 7, 0, 6, 0);
    public static Block Leaves = new Block("leaves", 8, 0);
    public static Block Sand = new Block("sand", 9, 0);
    public static Block Cactus = new Block("cactus", 11, 0, 10, 0);
    public static Block Gravel = new Block("gravel", 12, 0);
    public static Block Water = new Block("water", 13, 0);

    public static Block[] blocks = { Air, Bedrock, Stone, Dirt, Grass, Log, Leaves, Sand, Gravel, Water };

    public static Vector2[] GetUV(string side, Block block)
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

public class Tree
{
    public int height;
    public int variation;

    public Block trunk;
    public Block leaves;

    public Tree(int height, int variation, Block trunk, Block leaves)
    {
        this.height = height;
        this.variation = variation;

        this.trunk = trunk;
        this.leaves = leaves;
    }
}

public class Trees
{
    public static Tree None = new Tree(0, 0, Blocks.Air, Blocks.Air);
    public static Tree Oak = new Tree(6, 1, Blocks.Log, Blocks.Leaves);
    public static Tree Cactus = new Tree(3, 1, Blocks.Cactus, Blocks.Air);

    public static Tree[] trees = { None, Oak, Cactus };
}

public class Biome
{
    public int height;

    public int scale;
    public int scale2;

    public Block topblock;
    public Block middleblock;
    public Block bottomblock;

    public Tree tree;
    public int treeamount;

    public Biome(int height, int scale, int scale2, Block topblock, Block middleblock, Block bottomblock, Tree tree, int treeamount)
    {
        this.height = height;

        this.scale = scale;
        this.scale2 = scale2;

        this.topblock = topblock;
        this.middleblock = middleblock;
        this.bottomblock = bottomblock;

        this.tree = tree;
        this.treeamount = treeamount;
    }
}

public class Biomes
{
    public static Biome Void = new Biome(0, 0, 0, Blocks.Air, Blocks.Air, Blocks.Air, Trees.None, 0);
    public static Biome Plains = new Biome(70, 5, 1, Blocks.Grass, Blocks.Dirt, Blocks.Stone, Trees.None, 0);
    public static Biome Forest = new Biome(76, 8, 3, Blocks.Grass, Blocks.Dirt, Blocks.Stone, Trees.Oak, 3);
    public static Biome Swamp = new Biome(60, 4, 2, Blocks.Mud, Blocks.Dirt, Blocks.Stone, Trees.Oak, 1);
    public static Biome Desert = new Biome(68, 5, 1, Blocks.Sand, Blocks.Sand, Blocks.Stone, Trees.Cactus, 2);
    public static Biome Mountains = new Biome(86, 10, 8, Blocks.Stone, Blocks.Stone, Blocks.Stone, Trees.None, 0);
    public static Biome Ocean = new Biome(40, 4, 4, Blocks.Gravel, Blocks.Stone, Blocks.Stone, Trees.None, 0);

    public static Biome[] biomes = { Plains, Forest, Swamp, Desert, Mountains, Ocean };
}