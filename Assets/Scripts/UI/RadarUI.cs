using UnityEngine;
using TMPro;
using Unity.Netcode;

public class RadarUI : MonoBehaviour
{
    [SerializeField] private RawImage radarImage;
    [SerializeField] private Texture2D radarTexture;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float radarRange = 500f;
    
    private int textureSize = 512;
    private Texture2D currentTexture;
    
    private void Start()
    {
        if (radarTexture == null)
        {
            radarTexture = new Texture2D(textureSize, textureSize, TextureFormat.RGB24, false);
        }
        
        currentTexture = radarTexture;
        radarImage.texture = currentTexture;
    }
    
    private void Update()
    {
        UpdateRadar();
    }
    
    private void UpdateRadar()
    {
        // Очищаем текстуру
        Color[] pixels = new Color[textureSize * textureSize];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.black;
        }
        
        // Рисуем игрока в центре
        DrawPixel(textureSize / 2, textureSize / 2, Color.green);
        
        // Рисуем торнадо
        var tornadoManager = NetworkTornadoManager.Instance;
        if (tornadoManager != null)
        {
            var tornadoes = tornadoManager.GetActiveTornadoes();
            // Здесь нужно получить позиции торнадо и нарисовать их на радаре
        }
        
        currentTexture.SetPixels(pixels);
        currentTexture.Apply();
    }
    
    private void DrawPixel(int x, int y, Color color)
    {
        if (x >= 0 && x < textureSize && y >= 0 && y < textureSize)
        {
            currentTexture.SetPixel(x, y, color);
        }
    }
}
