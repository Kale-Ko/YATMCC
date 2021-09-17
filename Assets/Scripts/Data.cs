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
    public static Block Log = new Block(5, 4, 0, 5, 0);
    public static Block Leaves = new Block(6, 6, 0);
    public static Block Sand = new Block(7, 7, 0);
    public static Block Gravel = new Block(8, 8, 0);
    public static Block Water = new Block(9, 9, 0);

    public static Block[] blocks = { Air, Bedrock, Stone, Dirt, Grass, Log, Leaves, Sand, Gravel, Water };
}

public class Tree
{
    public int height;
    public int variation;

    public Block trunk;
    public Block leafs;

    public Tree(int height, int variation, Block trunk, Block leafs)
    {
        this.height = height;
        this.variation = variation;

        this.trunk = trunk;
        this.leafs = leafs;
    }
}

public class Trees
{
    public static Tree None = new Tree(0, 0, Blocks.Air, Blocks.Air);
    public static Tree Oak = new Tree(8, 1, Blocks.Log, Blocks.Leaves);
    public static Tree Spruce = new Tree(12, 3, Blocks.Log, Blocks.Leaves);
    public static Tree Cactus = new Tree(2, 1, Blocks.Leaves, Blocks.Air);

    public static Tree[] trees = { None, Oak, Spruce, Cactus };
}

public class Biome
{
    public float height;
    public float temperature;
    public float moisture;

    public float scale;

    public Block topblock;
    public Block middleblock;
    public Block bottomblock;

    public Tree tree;
    public float treeamount;

    public Biome(float height, float temperature, float moisture, float scale, Block topblock, Block middleblock, Block bottomblock, Tree tree, float treeamount)
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
    public static Biome Void = new Biome(0, -1, -1, 0, Blocks.Air, Blocks.Air, Blocks.Air, Trees.None, 0);
    public static Biome Plains = new Biome(70, 5, 3, 5, Blocks.Grass, Blocks.Dirt, Blocks.Stone, Trees.None, 0);
    public static Biome Forest = new Biome(76, 4, 5, 8, Blocks.Grass, Blocks.Dirt, Blocks.Stone, Trees.Oak, 3);
    public static Biome Swamp = new Biome(64, 2, 8, 8, Blocks.Grass, Blocks.Dirt, Blocks.Stone, Trees.Oak, 3);
    public static Biome Desert = new Biome(70, 8, 1, 5, Blocks.Sand, Blocks.Sand, Blocks.Stone, Trees.Cactus, 1);
    public static Biome Mountains = new Biome(100, 3, 3, 20, Blocks.Stone, Blocks.Stone, Blocks.Stone, Trees.None, 0);
    public static Biome Ocean = new Biome(40, 4, 10, 4, Blocks.Gravel, Blocks.Stone, Blocks.Stone, Trees.None, 0);

    public static Biome[] biomes = { Plains, Forest, Swamp, Desert, Mountains, Ocean };

    public static Biome GetBiome(float height, float temp, float moisture)
    {
        if (height == Biomes.Plains.height || (height > 64 && height < 76)) return Biomes.Plains;
        else if (height == Biomes.Forest.height || (height > 70 && height < 100)) return Biomes.Forest;
        else if (height == Biomes.Swamp.height || (height > 40 && height < 70)) return Biomes.Swamp;
        else if (height == Biomes.Desert.height || (height > 64 && height < 76)) return Biomes.Desert;
        else if (height == Biomes.Mountains.height || height > 76) return Biomes.Mountains;
        else if (height == Biomes.Ocean.height || height < 64) return Biomes.Ocean;
        else return Biomes.Void;
    }
}