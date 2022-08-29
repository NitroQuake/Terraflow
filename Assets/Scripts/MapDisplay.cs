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
    public Texture2D BiomeLevel(float[,] levels)
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
                    colors[x * length + y] = new Color32(7, 25, 126, 255);
                }
                else if (levels[x, y] < 0.4)
                {
                    colors[x * length + y] = new Color32(7, 25, 188, 255); ;
                }
                // Sand
                else if (levels[x, y] < 0.45)
                {
                    colors[x * length + y] = new Color32(191, 192, 100, 255); 
                }
                // Ground
                else if (levels[x, y] < 0.5)
                {
                    colors[x * length + y] = new Color32(52, 177, 34, 255);
                }
                else if (levels[x, y] < 0.55)
                {
                    colors[x * length + y] = new Color32(52, 106, 34, 255);
                }
                else if (levels[x, y] < 0.65)
                {
                    colors[x * length + y] = new Color32(106, 68, 34, 255);
                }
                else if (levels[x, y] < 0.75)
                {
                    colors[x * length + y] = new Color32(91, 50, 25, 255);
                }
                else if (levels[x, y] > 0.75)
                {
                    colors[x * length + y] = Color.white;
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
