using UnityEngine;
using System.Collections;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode { NoiseMap, ColourMap, Mesh };
    public DrawMode drawMode;

    [SerializeField] private MapDisplay mapDisplay; // Serialized field for MapDisplay component
    [SerializeField] private int mapChunkSize = 241; // Size of the map chunk
    [Range(0, 6)] public int levelOfDetail; // Level of detail for mesh generation
    public float noiseScale = 30f; // Scale of the noise

    public int octaves = 4; // Number of octaves for noise generation
    [Range(0, 1)] public float persistence = 0.5f; // Persistence parameter for noise
    public float lacunarity = 2f; // Lacunarity parameter for noise

    [SerializeField] private int seed; // Seed for random generation
    [SerializeField] private Vector2 offset; // Offset for noise generation

    public float meshHeightMultiplier = 10f; // Multiplier for mesh height
    public AnimationCurve meshHeightCurve; // Curve for mesh height adjustment

    public TerrainType[] regions; // Array of TerrainType for defining regions

    private void Start()
    {
        InitializeRandomValues(); // Initialize random values for noiseScale, seed, and offset
        GenerateMap(); // Generate the map
    }

    private void InitializeRandomValues()
    {
        noiseScale = Random.Range(20f, 50f); // Random noise scale within range
        seed = Random.Range(0, 100000); // Random seed within range
        offset = new Vector2(Random.Range(-10000f, 10000f), Random.Range(-10000f, 10000f)); // Random offset within range
    }

    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed, noiseScale, octaves, persistence, lacunarity, offset); // Generate noise map

        Color[] colourMap = new Color[mapChunkSize * mapChunkSize]; // Initialize color map array
        for (int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {
                float currentHeight = noiseMap[x, y]; // Current height from noise map
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].height)
                    {
                        colourMap[y * mapChunkSize + x] = regions[i].colour; // Assign color based on height
                        break;
                    }
                }
            }
        }

        switch (drawMode)
        {
            case DrawMode.NoiseMap:
                mapDisplay.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap)); // Draw noise map texture
                break;
            case DrawMode.ColourMap:
                mapDisplay.DrawTexture(TextureGenerator.TextureFromColourMap(colourMap, mapChunkSize, mapChunkSize)); // Draw color map texture
                break;
            case DrawMode.Mesh:
                MeshData meshData = MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplier, meshHeightCurve, levelOfDetail); // Generate terrain mesh
                mapDisplay.DrawMesh(meshData, TextureGenerator.TextureFromColourMap(colourMap, mapChunkSize, mapChunkSize)); // Draw mesh with color map texture
                break;
        }
    }

    private void OnValidate()
    {
        if (lacunarity < 1)
        {
            lacunarity = 1; // Ensure lacunarity is not less than 1
        }
        if (octaves < 0)
        {
            octaves = 0; // Ensure octaves is not less than 0
        }
    }
}
[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color colour;
}
