using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

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
    public Queue<MapDataAndMethod<MapData>> mapDataAndMethodQueue = new Queue<MapDataAndMethod<MapData>>();
    public Queue<MeshDataAndMethod<MeshData>> meshDataAndMethodQueue = new Queue<MeshDataAndMethod<MeshData>>();

    public void drawMesh()
    {
        // stores value in mapData
        MapData mapData = GenerateMapData(Vector3.one);

        MapDisplay mapDisplay = GetComponent<MapDisplay>();
        mapDisplay.BiomeLevel(mapData.noiseMap, mapData.moistureMap);
        mapDisplay.DrawMesh(MeshGenerator.GenerateTerrainMesh(mapData.noiseMap, multipliar, meshCurve, levelOfDetail));
    }

    //Generates perlin noise map and saves it in MapData
    public MapData GenerateMapData(Vector3 terrainoffset)
    {
        float[,] mapGen = Noise.GenerateNoiseMapMyVersion(mapSizeWL, mapSizeWL, seed, noiseScale, amplitude, octaves, persistance, lacunarity, terrainoffset, power);

        float[,] moistureGen = Noise.GenerateNoiseMapMyVersion(mapSizeWL, mapSizeWL, seed, noiseScale + 75, amplitude, octaves + 1, persistance, lacunarity, terrainoffset + Vector3.one * 100, power);

        return new MapData(mapGen, moistureGen);
    }

    // Starts new thread in AddMethodToQueue
    public void StartThreadMapData(Action<MapData> method, Vector3 terrainOffset)
    {
        // The method where the thread starts
        ThreadStart mapN = delegate 
        {
            AddMethodToQueue(method, terrainOffset);
        };

        new Thread(mapN).Start();
    }

    public void StartThreadMeshData(float[,] noiseMap, Action<MeshData> method, int LOD)
    {
        ThreadStart meshDataThread = delegate
        {
            AddMethodToQueueMesh(noiseMap, method, LOD);
        };

        new Thread(meshDataThread).Start();
    }

    // Adds OnMapDataRecieved and mapData to queue
    public void AddMethodToQueue(Action<MapData> method, Vector3 terrainOffset)
    {
        // Add method
        MapData mapData = GenerateMapData(terrainOffset);

        // Add method to list
        lock (mapDataAndMethodQueue)
        { 
            mapDataAndMethodQueue.Enqueue(new MapDataAndMethod<MapData>(method, mapData));
        }
    }

    public void AddMethodToQueueMesh(float[,] noiseMap, Action<MeshData> method, int LOD)
    {
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(noiseMap, multipliar, meshCurve, LOD);
        lock (meshDataAndMethodQueue)
        {
            meshDataAndMethodQueue.Enqueue(new MeshDataAndMethod<MeshData>(method, meshData));
        }
    }

    // Goes back to main thread to execute OnMapDataRecieved 
    private void Update()
    {
        // remove it from the queue
        if(mapDataAndMethodQueue.Count > 0)
        {
            for(int i = 0; i < mapDataAndMethodQueue.Count; i++)
            {
                // Gets the oldest OnMapDataRecieved method and parameter from the script
                MapDataAndMethod<MapData> values = mapDataAndMethodQueue.Dequeue();
                // Calls OnMapDataRecieved function in the endless terrain script
                values.callOnMapDataRecieved(values.parameter);
            }
        }
        // Adds the method

        if(meshDataAndMethodQueue.Count >= 1)
        {
            for (int i = 0; i < meshDataAndMethodQueue.Count; i++)
            {
                MeshDataAndMethod<MeshData> method = meshDataAndMethodQueue.Dequeue();
                method.method(method.parameter);
            }
        }
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

// Holds unchangeable data
public struct MapData
{
    public readonly float[,] noiseMap;
    public readonly float[,] moistureMap;

    public MapData(float[,] noiseMap, float[,] moistureMap)
    {
        this.noiseMap = noiseMap;
        this.moistureMap = moistureMap;
    }
}

public struct MapDataAndMethod<noiseMap>
{
    public readonly Action<noiseMap> callOnMapDataRecieved;
    public readonly noiseMap parameter;

    public MapDataAndMethod(Action<noiseMap> callOnMapDataRecieved, noiseMap parameter)
    {
        this.callOnMapDataRecieved = callOnMapDataRecieved;
        this.parameter = parameter;
    }
}

public struct MeshDataAndMethod<param>
{
    public readonly Action<param> method;
    public readonly param parameter;

    public MeshDataAndMethod(Action<param> method, param parameter)
    {
        this.method = method;
        this.parameter = parameter;
    }
}
