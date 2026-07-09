using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class NetworkTornadoManager : NetworkBehaviour
{
    private static NetworkTornadoManager instance;
    
    [SerializeField] private GameObject tornadoPrefab;
    [SerializeField] private float spawnInterval = GameConstants.TORNADO_SPAWN_INTERVAL;
    [SerializeField] private List<Transform> spawnLocations = new List<Transform>();
    
    private float timeSinceLastSpawn = 0f;
    private NetworkList<ulong> activeTornadoes;
    
    public static NetworkTornadoManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<NetworkTornadoManager>();
            }
            return instance;
        }
    }
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        activeTornadoes = new NetworkList<ulong>();
    }
    
    private void Update()
    {
        if (!IsServer) return;
        
        timeSinceLastSpawn += Time.deltaTime;
        
        if (timeSinceLastSpawn >= spawnInterval)
        {
            SpawnRandomTornadoRpc();
            timeSinceLastSpawn = 0f;
        }
    }
    
    /// <summary>
    /// Спавнит случайное торнадо на карте
    /// </summary>
    [Rpc(SendTo.Server)]
    private void SpawnRandomTornadoRpc()
    {
        if (!IsServer) return;
        
        // Выбираем случайную локацию
        Vector3 spawnPos = GetRandomSpawnLocation();
        
        // Выбираем случайную категорию торнадо
        GameConstants.TornadoCategory category = (GameConstants.TornadoCategory)Random.Range(0, 7);
        
        // Проверяем модификатор локации
        float tornadoChanceModifier = GetLocationTornadoChanceModifier(spawnPos);
        float spawnChance = Random.Range(0f, 1f);
        
        if (spawnChance > (1f - tornadoChanceModifier))
        {
            return; // Не спавним на этой локации
        }
        
        // Спавним торнадо
        GameObject tornadoInstance = Instantiate(tornadoPrefab, spawnPos, Quaternion.identity);
        var tornadoScript = tornadoInstance.GetComponent<Tornado>();
        
        if (tornadoScript != null)
        {
            // Устанавливаем категорию
            // (Нужно добавить setter в Tornado скрипт)
        }
        
        // Регистрируем на сети
        var networkObject = tornadoInstance.GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            networkObject.Spawn();
            activeTornadoes.Add(networkObject.NetworkObjectId);
        }
        
        // Уведомляем клиентов
        NotifyTornadoSpawnedClientRpc(spawnPos, (int)category);
    }
    
    /// <summary>
    /// Получает случайное место спавна на карте
    /// </summary>
    private Vector3 GetRandomSpawnLocation()
    {
        if (spawnLocations.Count > 0)
        {
            return spawnLocations[Random.Range(0, spawnLocations.Count)].position;
        }
        
        // Fallback: случайная позиция
        return new Vector3(
            Random.Range(-500f, 500f),
            5f,
            Random.Range(-500f, 500f)
        );
    }
    
    /// <summary>
    /// Получает модификатор шанса торнадо для локации
    /// </summary>
    private float GetLocationTornadoChanceModifier(Vector3 position)
    {
        // Примерно определяем локацию по позиции
        float x = position.x;
        float z = position.z;
        
        if (x < -400) return GameConstants.LocationTornadoChance.GALEVIEW;
        if (x > -200 && x < -50) return GameConstants.LocationTornadoChance.VORTEX_CITY;
        if (x > -50 && x < 150) return GameConstants.LocationTornadoChance.DUSTY_RIDGE;
        if (z < -300) return GameConstants.LocationTornadoChance.WINDY_WILLOW;
        if (z > 0 && z < 200) return GameConstants.LocationTornadoChance.TWISTER_CREEK;
        if (z > 200 && z < 400) return GameConstants.LocationTornadoChance.STORMVILLE;
        if (x > 300) return GameConstants.LocationTornadoChance.SIRENHILL;
        
        return 1.0f;
    }
    
    /// <summary>
    /// Регистрирует локации для спавна
    /// </summary>
    public void RegisterSpawnLocation(Transform location)
    {
        if (!spawnLocations.Contains(location))
        {
            spawnLocations.Add(location);
        }
    }
    
    /// <summary>
    /// Получает список активных торнадо
    /// </summary>
    public List<ulong> GetActiveTornadoes()
    {
        return new List<ulong>(activeTornadoes);
    }
    
    [Rpc(SendTo.ClientsAndHost)]
    private void NotifyTornadoSpawnedClientRpc(Vector3 position, int category)
    {
        Debug.Log($"[CLIENT] Tornado spawned at {position} with category {category}");
    }
}
