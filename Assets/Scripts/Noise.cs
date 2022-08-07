using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{

    //Sets a perlin noise map in an array using perlinValues
    public static float[,] GenerateNoiseMap(int width, int length, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
    {
        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffset = new Vector2[octaves];
        
        for(int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) + offset.y;
            octaveOffset[i] = new Vector2(offsetX, offsetY);
        }

        float[,] noiseMap = new float[width, length];

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = width / 2f;
        float halfLength = length / 2f;
        
        if(scale < 0)
        {
            scale = 0.001f;
        }

        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < length; y++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for(int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - halfWidth) / scale + octaveOffset[i].x;
                    float sampleY = (y - halfLength) / scale + octaveOffset[i].y;

                    //The * 2 - 1 makes it so there can be a value below 0
                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight = amplitude * perlinValue;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if(noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }

                noiseMap[x, y] = noiseHeight;
            }
        }
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < length; y++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }
        return noiseMap;
    }
}
