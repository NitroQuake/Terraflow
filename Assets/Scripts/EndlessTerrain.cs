using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
    public const float maxChunkDistance = 240;
    public Transform viewer;
    
    public static Vector2 playerPosition;
    int chunkSize;
    int chunkVisibleInViewDistance;
    Dictionary<Vector2, TerrainChunk> chunkList = new Dictionary<Vector2, TerrainChunk>();
    List<TerrainChunk> chunksVisible = new List<TerrainChunk>();

    // Start is called before the first frame update
    void Start()
    {
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
                    chunkList.Add(ChunkOnDistance, new TerrainChunk(ChunkOnDistance, chunkSize, transform));
                }
            }
        }
    }

    public class TerrainChunk
    {
        GameObject plane;
        Vector2 position;
        Bounds bounds;

        // Creates and positions terrain chunk
        public TerrainChunk(Vector2 coord, int size, Transform parent)
        {
            position = coord * size;
            bounds = new Bounds(position, Vector3.one * size);
            Vector3 terrainChunkPosition = new Vector3(position.x, 0, position.y);

            plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            plane.transform.position = terrainChunkPosition;
            plane.transform.localScale = Vector3.one * size / 10;
            // Sets plane to be parented by object holding this script
            plane.transform.parent = parent;
            plane.SetActive(false);
        }

        // Within chunk distance set chunks as visible
        public void UpdateTerrainChunk()
        {
            // Gets distance from bounding box from player position and if it is less than max chunk distance show chunk
            float playerDstFromChunkBounds = Mathf.Sqrt(bounds.SqrDistance(playerPosition));
            bool isVisible = playerDstFromChunkBounds <= maxChunkDistance;
            plane.SetActive(isVisible);
        }
    }
}
