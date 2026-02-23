using UnityEngine;

namespace InterdimensionalGroceries.Rendering
{
    public class VHSNoiseTextureGenerator : MonoBehaviour
    {
        public static Texture2D GenerateNoiseTexture(int width = 512, int height = 512)
        {
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            texture.wrapMode = TextureWrapMode.Repeat;
            texture.filterMode = FilterMode.Bilinear;

            Color[] pixels = new Color[width * height];
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float noise = Random.Range(0f, 1f);
                    
                    float perlinNoise = Mathf.PerlinNoise(x * 0.1f, y * 0.1f);
                    noise = (noise + perlinNoise) * 0.5f;
                    
                    pixels[y * width + x] = new Color(noise, noise, noise, 1f);
                }
            }
            
            texture.SetPixels(pixels);
            texture.Apply();
            
            return texture;
        }
    }
}
