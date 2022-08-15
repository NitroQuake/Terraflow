using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    public static MeshData GenerateTerrainMesh(float[,] noiseMap, float multipliar, AnimationCurve meshCurve, int levelOfDetail)
    {
        int width = noiseMap.GetLength(0);
        int length = noiseMap.GetLength(1);
        //Values to center the mesh
        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (length - 1) / 2f;

        // Reduces the amount of vertices
        int meshTriangleSimpleification = levelOfDetail * 2;
        // Amount of vertices for width and length
        int amountOfVertices = (width-1) / meshTriangleSimpleification + 1;

        MeshData meshData = new MeshData(amountOfVertices, amountOfVertices);
        int vertexIndex = 0;

        // Important to do y first before x, allowing the triangles to be in a clockwise direction
        for(int y = 0; y < width; y+= meshTriangleSimpleification)
        {
            for(int x = 0; x < length; x+= meshTriangleSimpleification)
            {
                // Creates vertices
                // meshCurve.Evaluate returns a y value from the horizontal axis or x which in this case is noiseMap[x, y] 
                meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, meshCurve.Evaluate(noiseMap[x, y]) * multipliar, topLeftZ - y);
                // Creates a uv map where 0,0 represents the bottom left corner of the texture while 1,1 represents the top right corner of the texture
                meshData.uvs[vertexIndex] = new Vector2(y / (float)length, x / (float)width);

                // Adds triangles
                if (x < width - 1 && y < length - 1)
                {
                    meshData.AddTriangle(vertexIndex, vertexIndex + amountOfVertices + 1, vertexIndex + amountOfVertices);
                    meshData.AddTriangle(vertexIndex + amountOfVertices + 1, vertexIndex, vertexIndex + 1);
                }

                vertexIndex++;
            }
        }
        return meshData;
    }
}

public class MeshData
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;

    int triangleIndex;

    // Creates a variable that holds the array of verices and the array of triangles
    public MeshData(int width, int length)
    {
        vertices = new Vector3[width * length];
        uvs = new Vector2[width * length];
        // numbers of squares 
        triangles = new int[(width - 1) * (length - 1) * 6];
    }

    // To make a triangle in mesh it needs 3 values/points that references to the vertices like (0, 0, 1) is 0 where (0, 0, 1) is the vertice and 0 is the one value/points of the triangle
    public void AddTriangle(int a, int b, int c)
    {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;

        triangleIndex += 3;
    }

    // Creates mesh 
    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        return mesh;
    }
}
