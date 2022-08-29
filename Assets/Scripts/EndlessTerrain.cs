using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
    // a values that rounds up with 240 will unload and load chunks properly
    public const float maxChunkDistance = 450;
    public Transform viewer;
    public Material mapMaterial;
    
    public static Vector2 playerPosition;
    int chunkSize;
    public int chunkVisibleInViewDistance;
    Dictionary<Vector2, TerrainChunk> chunkList = new Dictionary<Vector2, TerrainChunk>();
    List<TerrainChunk> chunksVisible = new List<TerrainChunk>();

    public static MapGenerator mapGenerator;
    private Vector2 oldViewerPosition = new Vector2(0, 0);
    private float distanceUntilUpdate;

    // Start is called before the first frame update
    void Start()
    {
        mapGenerator = GetComponent<MapGenerator>();
        chunkSize = MapGenerator.mapSizeWL - 1;
        chunkVisibleInViewDistance = Mathf.RoundToInt(maxChunkDistance / chunkSize);

        LoadChunks();
    }

    private void Update()
    {
        playerPosition = new Vector2(viewer.position.x, viewer.position.z);
        distanceUntilUpdate = Vector2.Distance(oldViewerPosition, playerPosition);
        if(distanceUntilUpdate > 25f)
        {
            LoadChunks();
            oldViewerPosition = playerPosition;
        }
    }

    private void LoadChunks()
    {

        // Checks if chunk is visible again as the player may move out where it can't update terrain chunks given it can only check along chunk distance from player
        for  (int i = 0; i < chunksVisible.Count; i++)
        {
            chunksVisible[i].UpdateTerrainChunk();
        }
        chunksVisible.Clear();

        // Offsets for player position so it could add active chunks based on player position
        int chunkOnX = Mathf.RoundToInt(playerPosition.x / chunkSize);
        int chunkOnY = Mathf.RoundToInt(playerPosition.y / chunkSize);

        // Loops through to collect chunks cord along chunk distance
        for (int y = -chunkVisibleInViewDistance; y <= chunkVisibleInViewDistance; y++)
        {
            for (int x = -chunkVisibleInViewDistance; x <= chunkVisibleInViewDistance; x++)
            {
                Vector2 ChunkOnDistance = new Vector2(y + chunkOnX, x + chunkOnY);

                // Uses the chunk in chunklist
                if (chunkList.ContainsKey(ChunkOnDistance))
                {
                    // Checks if chunk is visible along chunk distance
                    chunkList[ChunkOnDistance].UpdateTerrainChunk();
                    chunkList[ChunkOnDistance].CheckLOD(6);

                    chunksVisible.Add(chunkList[ChunkOnDistance]);
                }
                // Create new chunk
                else
                {
                    chunkList.Add(ChunkOnDistance, new TerrainChunk(ChunkOnDistance, chunkSize, transform, mapMaterial));

                    chunkList[ChunkOnDistance].UpdateTerrainChunk();
                    chunkList[ChunkOnDistance].CheckLOD(6);
                }
            }
        }
    }

    // Holds a terrain chunk with methods that can be used
    public class TerrainChunk
    {
        GameObject chunk;
        Vector2 position;
        Bounds bounds;
        MeshFilter meshFilter;
        MeshRenderer meshRenderer;
        MapDisplay mapDisplay;
        MapData mapDataSaved;
        int levelOfDetail = 1;
        float playerDstFromChunkBounds;
        bool mapDataRecieved;

        // Creates and positions terrain chunk
        public TerrainChunk(Vector2 coord, int size, Transform parent, Material mapMaterial)
        {
            position = coord * size;
            bounds = new Bounds(position, Vector3.one * size);
            Vector3 terrainChunkPosition = new Vector3(position.x, 0, position.y);

            chunk = new GameObject("Terrain Chunk");
            mapDisplay = chunk.AddComponent<MapDisplay>();
            meshFilter = chunk.AddComponent<MeshFilter>();
            meshRenderer = chunk.AddComponent<MeshRenderer>();
            meshRenderer.material = mapMaterial;

            chunk.transform.position = terrainChunkPosition;
            // Sets plane to be parented by object holding this script
            chunk.transform.parent = parent;
            chunk.SetActive(false);

            mapGenerator.StartThreadMapData(OnMapDataRecieved, terrainChunkPosition);
            mapDataRecieved = false;
        }

        private void OnMapDataRecieved(MapData mapData)
        {
            mapDataSaved = mapData;
            Texture2D texture = mapDisplay.BiomeLevel(mapData.noiseMap);
            meshRenderer.material.mainTexture = texture;
            for (int i = 6; i > 0; i--)
            {
                if (playerDstFromChunkBounds <= maxChunkDistance / i)
                {
                    levelOfDetail = 6 + 1 - i;
                    mapGenerator.StartThreadMeshData(mapData.noiseMap, OnMeshDataRecieved, levelOfDetail);
                    break;
                }
            }
            mapDataRecieved = true;
        }

        private void OnMeshDataRecieved(MeshData meshData)
        {
            meshFilter.mesh = meshData.CreateMesh();
        }

        // Within chunk distance set chunks as visible
        public void UpdateTerrainChunk()
        {
            // Gets distance from bounding box from player position and if it is less than max chunk distance show chunk
            playerDstFromChunkBounds = Mathf.Sqrt(bounds.SqrDistance(playerPosition));

            bool isVisible = playerDstFromChunkBounds <= maxChunkDistance;
            chunk.SetActive(isVisible);
        }

        public void CheckLOD(int performanceLOD)
        {
            if (mapDataRecieved)
            {
                for (int i = performanceLOD; i > 0; i--)
                {
                    if (playerDstFromChunkBounds <= maxChunkDistance / i)
                    {
                        levelOfDetail = performanceLOD + 1 - i;
                        mapGenerator.StartThreadMeshData(mapDataSaved.noiseMap, OnMeshDataRecieved, levelOfDetail);
                        break;
                    }
                }
            }
        }
    }
}
