using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    public Renderer textureRender;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public bool meshRender;

    // Sets color based on values in the perlin noise map
    public Texture2D BiomeLevel(float[,] levels, float[,] moisture)
    {
        int width = levels.GetLength(0);
        int length = levels.GetLength(1);

        Color[] colors = new Color[width * length];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < length; y++)
            {
                // colors[x * length + y] = Color.Lerp(Color.black, Color.white, levels[x, y]); changes it back to black and white
                // This is where regions are set based on height
                if (levels[x, y] < 0.3)
                {
                    colors[x * length + y] = new Color32(7, 25, 126, 255); // Deep Ocean
                }
                else if (levels[x, y] < 0.4)
                {
                    colors[x * length + y] = new Color32(7, 25, 188, 255); // Ocean
                }
                // Sand
                else if (levels[x, y] < 0.45)
                {
                    colors[x * length + y] = new Color32(191, 192, 100, 255); // Beach
                }
                // Ground
                else if (levels[x, y] < 0.5)
                {
                    if (moisture[x, y] < 0.25)
                    {
                        colors[x * length + y] = new Color32(252, 174, 39, 255); // orange grass
                    }
                    else if (moisture[x, y] < 0.4)
                    {
                        colors[x * length + y] = new Color32(247, 69, 95, 255); // red grass
                    }
                    else
                    {
                        colors[x * length + y] = new Color32(52, 177, 34, 255); // Grass
                    }
                }
                else if (levels[x, y] < 0.55)
                {
                    if (moisture[x, y] < 0.25)
                    {
                        colors[x * length + y] = new Color32(222, 153, 35, 255); // Dark orange stone
                    }
                    else if (moisture[x, y] < 0.4)
                    {
                        colors[x * length + y] = new Color32(220, 30, 58, 255); // Dark red grass
                    }
                    else
                    {
                        colors[x * length + y] = new Color32(52, 106, 34, 255); // Dark Grass
                    }
                }
                else if (levels[x, y] < 0.65)
                {
                    if (moisture[x, y] < 0.25)
                    {
                        colors[x * length + y] = new Color32(191, 124, 11, 255); // Light orange stone
                    }
                    else if (moisture[x, y] < 0.4)
                    {
                        colors[x * length + y] = new Color32(190, 12, 38, 255); // Light red stone
                    }
                    else
                    {
                        colors[x * length + y] = new Color32(159, 146, 149, 255); // Light stone
                    }
                }
                else if (levels[x, y] < 0.75)
                {
                    if (moisture[x, y] < 0.25)
                    {
                        colors[x * length + y] = new Color32(147, 96, 9, 255); // Dark brown stone
                    }
                    else if (moisture[x, y] < 0.4)
                    {
                        colors[x * length + y] = new Color32(134, 8, 27, 255); // Dark red stone
                    }
                    else
                    {
                        colors[x * length + y] = new Color32(68, 55, 58, 255); // Dark Stone
                    }
                }
                else if (levels[x, y] > 0.75)
                {
                    if (moisture[x, y] < 0.1)
                    {
                        colors[x * length + y] = new Color32(85, 85, 85, 255); // Scorched 
                    }
                    else if (moisture[x, y] < 0.2)
                    {
                        colors[x * length + y] = new Color32(136, 136, 136, 255); // Bare
                    }
                    else if (moisture[x, y] < 0.5)
                    {
                        colors[x * length + y] = new Color32(187, 187, 170, 255); // Tundra
                    }
                    else
                    {
                        colors[x * length + y] = Color.white; // Snow
                    }
                }
            }
        }
        Texture2D texture2D = new Texture2D(width, length);

        //removes repeated color border outline of the texture
        texture2D.filterMode = FilterMode.Point;
        texture2D.wrapMode = TextureWrapMode.Clamp;

        texture2D.SetPixels(colors);
        texture2D.Apply();

        return texture2D;
    }

    //
    // Creates and sets texture
    public void DrawNoiseMap(Color[] colors, int width, int length)
    {
        Texture2D texture2D = new Texture2D(width, length);

        //removes repeated color border outline of the texture
        texture2D.filterMode = FilterMode.Point;
        texture2D.wrapMode = TextureWrapMode.Clamp;

        texture2D.SetPixels(colors);
        texture2D.Apply();

        if (meshRender)
        {
            meshRenderer.sharedMaterial.mainTexture = texture2D;
        }
        else
        {
            textureRender.sharedMaterial.mainTexture = texture2D;
            textureRender.transform.localScale = new Vector3(width, 1, length);
        }
    }

    public void DrawMesh(MeshData meshData)
    {
        meshFilter.sharedMesh = meshData.CreateMesh();
    }
}
