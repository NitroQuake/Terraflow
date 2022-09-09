using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class MeshGenerator
{
    public static MeshData GenerateTerrainMesh(MapData mapData, float multipliar, AnimationCurve meshCurve, int levelOfDetail, Gradient normGradient, Gradient redGradient)
    {
        // Creates own mesh curve for each thread
        AnimationCurve _meshCurve = new AnimationCurve(meshCurve.keys);

        int width =  mapData.noiseMap.GetLength(0);
        int length = mapData.noiseMap.GetLength(1);
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
        for(int y = 0; y < length; y += meshTriangleSimpleification)
        {
            for(int x = 0; x < width; x += meshTriangleSimpleification)
            {
                // Creates vertices
                // meshCurve.Evaluate returns a y value from the horizontal axis or x which in this case is noiseMap[x, y] 
                meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, _meshCurve.Evaluate(mapData.noiseMap[x, y]) * multipliar, topLeftZ - y);
                // Creates a uv map where 0,0 represents the bottom left corner of the texture while 1,1 represents the top right corner of the texture
                meshData.uvs[vertexIndex] = new Vector2(y / (float)length, x / (float)width);

                if (mapData.noiseMap[x, y] < 0.45)
                {
                    meshData.colors[vertexIndex] = normGradient.Evaluate(mapData.noiseMap[x, y]);
                }
                else if (mapData.noiseMap[x, y] < 0.5)
                {
                    if (mapData.moistureMap[x, y] < 0.4)
                    {
                        meshData.colors[vertexIndex] = redGradient.Evaluate(0.5f);
                    }
                    else
                    {
                        meshData.colors[vertexIndex] = normGradient.Evaluate(mapData.noiseMap[x, y]);
                    }
                }
                else if (mapData.noiseMap[x, y] < 0.55)
                {
                    if (mapData.moistureMap[x, y] < 0.4)
                    {
                        meshData.colors[vertexIndex] = redGradient.Evaluate(0.55f);
                    }
                    else
                    {
                        meshData.colors[vertexIndex] = normGradient.Evaluate(mapData.noiseMap[x, y]);
                    }
                }
                else if (mapData.noiseMap[x, y] < 0.65)
                {
                    if (mapData.moistureMap[x, y] < 0.4)
                    {
                        meshData.colors[vertexIndex] = redGradient.Evaluate(0.65f);
                    }
                    else
                    {
                        meshData.colors[vertexIndex] = normGradient.Evaluate(mapData.noiseMap[x, y]);
                    }
                }
                else if (mapData.noiseMap[x, y] < 0.75)
                {
                    if (mapData.moistureMap[x, y] < 0.4)
                    {
                        meshData.colors[vertexIndex] = redGradient.Evaluate(0.75f);
                    }
                    else
                    {
                        meshData.colors[vertexIndex] = normGradient.Evaluate(mapData.noiseMap[x, y]);
                    }
                }
                else if (mapData.noiseMap[x, y] > 0.75)
                {
                    if (mapData.moistureMap[x, y] < 0.5)
                    {
                        meshData.colors[vertexIndex] = redGradient.Evaluate(1);
                    }
                    else
                    {
                        meshData.colors[vertexIndex] = normGradient.Evaluate(mapData.noiseMap[x, y]);
                    }
                }

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
    public Color[] colors;

    int triangleIndex;

    // Creates a variable that holds the array of verices and the array of triangles
    public MeshData(int width, int length)
    {
        vertices = new Vector3[width * length];
        uvs = new Vector2[width * length];
        // numbers of squares 
        triangles = new int[(width - 1) * (length - 1) * 6];
        colors = new Color[width * length];
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
        mesh.colors = colors;
        mesh.RecalculateNormals();
        return mesh;
    }
}
