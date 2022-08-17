using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public const int mapSizeWL = 241;
    [Range(1, 6)]
    public int levelOfDetail;
    public float noiseScale;
    public float amplitude;
    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;
    public int seed;
    public Vector2 offset;
    public float power;
    public float multipliar;
    public AnimationCurve meshCurve;

    public bool autoUpdate;

    //Generates perlin noise map
    public void GenerateMap()
    {
        float[,] mapGen = Noise.GenerateNoiseMapMyVersion(mapSizeWL, mapSizeWL, seed, noiseScale, amplitude, octaves, persistance, lacunarity, offset, power);

        MapDisplay mapDisplay = GetComponent<MapDisplay>();
        mapDisplay.BiomeLevel(mapGen);
        mapDisplay.DrawMesh(MeshGenerator.GenerateTerrainMesh(mapGen, multipliar, meshCurve, levelOfDetail));
    }

    void OnValidate()
    {
        if(lacunarity < 1)
        {
            lacunarity = 1;
        }
        if(octaves < 0)
        {
            octaves = 0;
        }
        if(power < 1)
        {
            power = 1;
        }
    }
}
