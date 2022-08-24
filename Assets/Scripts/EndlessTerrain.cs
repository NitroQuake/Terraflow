using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
    public const float maxChunkDistance = 240;
    public Transform viewer;
    public Material mapMaterial;
    
    public static Vector2 playerPosition;
    int chunkSize;
    int chunkVisibleInViewDistance;
    Dictionary<Vector2, TerrainChunk> chunkList = new Dictionary<Vector2, TerrainChunk>();
    List<TerrainChunk> chunksVisible = new List<TerrainChunk>();

    public static MapGenerator mapGenerator;

    // Start is called before the first frame update
    void Start()
    {
        mapGenerator = GetComponent<MapGenerator>();
        chunkSize = MapGenerator.mapSizeWL - 1;
        chunkVisibleInViewDistance = Mathf.RoundToInt(maxChunkDistance / chunkSize);
    }

    private void Update()
    {
        playerPosition = new Vector2(viewer.position.x, viewer.position.z);
        LoadChunks();
    }

    private void LoadChunks()
    {
        // Checks if chunk is visible again as the player may move out where it can't update terrain chunks given it can only check along chunk distance from player
        foreach(TerrainChunk chunk in chunksVisible)
        {
            chunk.UpdateTerrainChunk();
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

                    chunksVisible.Add(chunkList[ChunkOnDistance]);
                }
                // Create new chunk
                else
                {
                    chunkList.Add(ChunkOnDistance, new TerrainChunk(ChunkOnDistance, chunkSize, transform, mapMaterial));
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

        // Creates and positions terrain chunk
        public TerrainChunk(Vector2 coord, int size, Transform parent, Material mapMaterial)
        {
            position = coord * size;
            bounds = new Bounds(position, Vector3.one * size);
            Vector3 terrainChunkPosition = new Vector3(position.x, 0, position.y);

            chunk = new GameObject("Terrain Chunk");
            meshFilter = chunk.AddComponent<MeshFilter>();
            meshRenderer = chunk.AddComponent<MeshRenderer>();
            meshRenderer.material = mapMaterial;

            chunk.transform.position = terrainChunkPosition;
            // Sets plane to be parented by object holding this script
            chunk.transform.parent = parent;
            chunk.SetActive(false);

            mapGenerator.StartThreadMapData(OnMapDataRecieved);
        }

        private void OnMapDataRecieved(MapData mapData)
        {
            mapGenerator.StartThreadMeshData(mapData.noiseMap, OnMeshDataRecieved);
        }

        private void OnMeshDataRecieved(MeshData meshData)
        {
            meshFilter.mesh = meshData.CreateMesh();
        }

        // Within chunk distance set chunks as visible
        public void UpdateTerrainChunk()
        {
            // Gets distance from bounding box from player position and if it is less than max chunk distance show chunk
            float playerDstFromChunkBounds = Mathf.Sqrt(bounds.SqrDistance(playerPosition));
            bool isVisible = playerDstFromChunkBounds <= maxChunkDistance;
            chunk.SetActive(isVisible);
        }
    }
}
