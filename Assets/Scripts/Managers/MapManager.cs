using UnityEngine;
using Unity.Netcode;

public class MapManager : MonoBehaviour
{
    private static MapManager instance;
    
    [SerializeField] private Transform[] locationSpawns;
    
    // Локации
    public enum Location
    {
        Galeview = 0,
        VortexCity = 1,
        DustyRidge = 2,
        WindyWillow = 3,
        TwisterCreek = 4,
        Stormville = 5,
        Sirenhill = 6
    }
    
    public static MapManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<MapManager>();
            }
            return instance;
        }
    }
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        // Регистрируем локации как точки спавна торнадо
        var tornadoManager = NetworkTornadoManager.Instance;
        if (tornadoManager != null && locationSpawns.Length > 0)
        {
            foreach (var spawn in locationSpawns)
            {
                tornadoManager.RegisterSpawnLocation(spawn);
            }
        }
    }
    
    /// <summary>
    /// Получает трансформ локации по типу
    /// </summary>
    public Transform GetLocationSpawn(Location location)
    {
        if ((int)location < locationSpawns.Length)
        {
            return locationSpawns[(int)location];
        }
        return null;
    }
    
    /// <summary>
    /// Получает все точки спавна
    /// </summary>
    public Transform[] GetAllSpawns() => locationSpawns;
}
