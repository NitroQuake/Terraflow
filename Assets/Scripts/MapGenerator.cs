using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int mapWidth;
    public int mapLength;
    public float noiseFrequency;

    public float amplitude;

    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;

    public bool autoUpdate;

    //Generates perlin noise map
    public void GenerateMap()
    {
        float[,] mapGen = Noise.GenerateNoiseMapMyVersion(mapWidth, mapLength, noiseFrequency, amplitude, octaves, persistance);

        MapDisplay mapDisplay = GetComponent<MapDisplay>();
        mapDisplay.DrawNoiseMap(mapGen);
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
    }
}
