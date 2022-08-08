using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    public Renderer textureRender;

    //Puts perlin noiseMap texture

    // Sets color based on values in the perlin noise map
    public void BiomeLevel(float[,] levels)
    {
        int width = levels.GetLength(0);
        int length = levels.GetLength(1);

        Color[] colors = new Color[width * length];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < length; y++)
            {
                // colors[x * length + y] = Colors.lerp[Color.black, color.white, levels[x, y] changes it back to black and white
                if (levels[x, y] < 0.4)
                {
                    colors[x * length + y] = Color.blue;
                }
                if (levels[x, y] > 0.4)
                {
                    colors[x * length + y] = Color.green;
                }
            }
        }
        DrawNoiseMap(colors, width, length);
    }

    // Creates and sets texture
    public void DrawNoiseMap(Color[] colors, int width, int length)
    {
        Texture2D texture2D = new Texture2D(width, length);

        //removes repeated color border outline of the texture
        texture2D.filterMode = FilterMode.Point;
        texture2D.wrapMode = TextureWrapMode.Clamp;

        texture2D.SetPixels(colors);
        texture2D.Apply();

        textureRender.sharedMaterial.mainTexture = texture2D;
        textureRender.transform.localScale = new Vector3(width, 1, length);
    }
}
