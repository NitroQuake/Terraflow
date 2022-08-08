using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int mapWidth;
    public int mapLength;
    public float noiseScale;
    public float amplitude;
    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;
    public int seed;
    public Vector2 offset;
    public float power;

    public bool autoUpdate;

    //Generates perlin noise map
    public void GenerateMap()
    {
        float[,] mapGen = Noise.GenerateNoiseMapMyVersion(mapWidth, mapLength, seed, noiseScale, amplitude, octaves, persistance, lacunarity, offset, power);

        MapDisplay mapDisplay = GetComponent<MapDisplay>();
        mapDisplay.BiomeLevel(mapGen);
    }

    void OnValidate()
    {
        if(mapWidth < 1)
        {
            mapWidth = 1;
        }
        if(mapLength < 1)
        {
            mapLength = 1;
        }
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
