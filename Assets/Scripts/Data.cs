using UnityEngine;

public class Block
{
    public int id;

    public int toptexturex;
    public int toptexturey;

    public int sidetexturex;
    public int sidetexturey;

    public int bottomtexturex;
    public int bottomtexturey;

    public Block(int id, int texturex, int texturey) : this(id, texturex, texturey, texturex, texturey, texturex, texturey) { }

    public Block(int id, int texturex, int texturey, int sidetexturex, int sidetexturey) : this(id, texturex, texturey, sidetexturex, sidetexturey, texturex, texturey) { }

    public Block(int id, int toptexturex, int toptexturey, int sidetexturex, int sidetexturey, int bottomtexturex, int bottomtexturey)
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
    public static Block Air = new Block(0, -1, -1);
    public static Block Bedrock = new Block(1, 0, 0);
    public static Block Stone = new Block(2, 1, 0);
    public static Block Dirt = new Block(3, 2, 0);
    public static Block Grass = new Block(4, 3, 0, 2, 0, 2, 0);
    public static Block Log = new Block(5, 5, 0, 4, 0);
    public static Block Leaves = new Block(6, 6, 0);
    public static Block Sand = new Block(7, 7, 0);
    public static Block Gravel = new Block(8, 8, 0);
    public static Block Water = new Block(9, 9, 0);

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
    public static Tree Cactus = new Tree(2, 1, Blocks.Leaves, Blocks.Air);

    public static Tree[] trees = { None, Oak, Cactus };
}

public class Biome
{
    public int height;
    public int temperature;
    public int moisture;

    public int scale;

    public Block topblock;
    public Block middleblock;
    public Block bottomblock;

    public Tree tree;
    public int treeamount;

    public Biome(int height, int temperature, int moisture, int scale, Block topblock, Block middleblock, Block bottomblock, Tree tree, int treeamount)
    {
        this.height = height;
        this.temperature = temperature;
        this.moisture = moisture;

        this.scale = scale;

        this.topblock = topblock;
        this.middleblock = middleblock;
        this.bottomblock = bottomblock;

        this.tree = tree;
        this.treeamount = treeamount;
    }
}

public class Biomes
{
    public static Biome Void = new Biome(0, 0, 0, 0, Blocks.Air, Blocks.Air, Blocks.Air, Trees.None, 0);
    public static Biome Plains = new Biome(70, 5, 3, 5, Blocks.Grass, Blocks.Dirt, Blocks.Stone, Trees.None, 0);
    public static Biome Forest = new Biome(76, 4, 5, 8, Blocks.Grass, Blocks.Dirt, Blocks.Stone, Trees.Oak, 3);
    public static Biome Swamp = new Biome(64, 6, 7, 8, Blocks.Grass, Blocks.Dirt, Blocks.Stone, Trees.Oak, 1);
    public static Biome Desert = new Biome(68, 8, 1, 5, Blocks.Sand, Blocks.Sand, Blocks.Stone, Trees.Cactus, 2);
    public static Biome Mountains = new Biome(100, 2, 4, 20, Blocks.Stone, Blocks.Stone, Blocks.Stone, Trees.None, 0);
    public static Biome Ocean = new Biome(40, 3, 9, 4, Blocks.Gravel, Blocks.Stone, Blocks.Stone, Trees.None, 0);

    public static Biome[] biomes = { Plains, Forest, Swamp, Desert, Mountains, Ocean };

    public static BVP[] heights = { new BVP(Plains, Plains.height), new BVP(Forest, Forest.height), new BVP(Swamp, Swamp.height), new BVP(Desert, Desert.height), new BVP(Mountains, Mountains.height), new BVP(Ocean, Ocean.height) };
    public static BVP[] temps = { new BVP(Plains, Plains.temperature), new BVP(Forest, Forest.temperature), new BVP(Swamp, Swamp.temperature), new BVP(Desert, Desert.temperature), new BVP(Mountains, Mountains.temperature), new BVP(Ocean, Ocean.temperature) };
    public static BVP[] moistures = { new BVP(Plains, Plains.moisture), new BVP(Forest, Forest.moisture), new BVP(Swamp, Swamp.moisture), new BVP(Desert, Desert.moisture), new BVP(Mountains, Mountains.moisture), new BVP(Ocean, Ocean.moisture) };

    public static Biome GetBiome(int height, int temp, int moisture)
    {
        BVP closestheight = FindClosest(heights, height);
        BVP closesttemp = FindClosest(temps, temp);
        BVP closestmoisture = FindClosest(moistures, moisture);

        return closestheight.key;
    }

    public struct BVP
    {
        public Biome key;
        public int value;

        public BVP(Biome key, int value)
        {
            this.key = key;
            this.value = value;
        }
    }

    public static BVP FindClosest(BVP[] arr, int target)
    {
        if (target <= arr[0].value) return arr[0];
        if (target >= arr[arr.Length - 1].value) return arr[arr.Length - 1];

        int i = 0, j = arr.Length, mid = 0;
        while (i < j)
        {
            mid = (i + j) / 2;

            if (arr[mid].value == target) return arr[mid];

            if (target < arr[mid].value)
            {
                if (mid > 0 && target > arr[mid - 1].value) return GetClosest(arr[mid - 1], arr[mid], target);

                j = mid;
            }
            else
            {
                if (mid < arr.Length - 1 && target < arr[mid + 1].value) return GetClosest(arr[mid], arr[mid + 1], target);

                i = mid + 1;
            }
        }

        return arr[mid];
    }

    public static BVP GetClosest(BVP x, BVP y, int t) { return t - x.value >= y.value - t ? y : x; }
}