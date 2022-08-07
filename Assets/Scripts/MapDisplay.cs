using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    public Renderer textureRender;

    //Puts perlin noiseMap texture
    public void DrawNoiseMap(float[,] noiseMap)
    {
        int width = noiseMap.GetLength(0);
        int length = noiseMap.GetLength(1);

        Texture2D texture2D = new Texture2D(width, length);

        Color[] colors = new Color[width * length];

        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < length; y++)
            {
                // Makes the 1D array have the same parameter as the 2D array
                colors[x * length + y] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
            }
        }
        texture2D.SetPixels(colors);
        texture2D.Apply();

        textureRender.sharedMaterial.mainTexture = texture2D;
        textureRender.transform.localScale = new Vector3(width, 1, length);
    }
}
