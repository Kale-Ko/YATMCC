public enum Blocks
{
    Bedrock = 0,
    Stone = 1,
    Dirt = 2,
    Grass = 3,
    Sand = 4,
    Gravel = 5,
    Water = 6
}

public enum Trees
{
    None = 0,
    Oak = 1,
    Spruce = 2,
    Cactus = 3
}

public class Biome
{
    public float height;
    public float temperature;

    public float scale;

    public Blocks topblock;
    public Blocks middleblock;
    public Blocks bottomblock;

    public Trees tree;
    public float treeamount;

    public Biome(float height, float temperature, float scale, Blocks topblock, Blocks middleblock, Blocks bottomblock, Trees tree, float treeamount)
    {
        this.height = height;
        this.temperature = temperature;

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
    public static Biome Plains = new Biome(70, 5, 5, Blocks.Grass, Blocks.Dirt, Blocks.Stone, Trees.None, 0);
    public static Biome Forest = new Biome(76, 4, 8, Blocks.Grass, Blocks.Dirt, Blocks.Stone, Trees.Oak, 3);
    public static Biome Desert = new Biome(70, 8, 5, Blocks.Sand, Blocks.Sand, Blocks.Stone, Trees.Cactus, 1);
    public static Biome Mountains = new Biome(100, 3, 20, Blocks.Stone, Blocks.Stone, Blocks.Stone, Trees.None, 0);
    public static Biome Ocean = new Biome(40, 4, 4, Blocks.Gravel, Blocks.Stone, Blocks.Stone, Trees.None, 0);

    public static Biome GetBiome(int height, float temp)
    {
        return Biomes.Plains;
    }
}