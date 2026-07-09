using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class GarageManager : NetworkBehaviour
{
    private static GarageManager instance;
    
    [SerializeField] private GameObject vehiclePrefab;
    [SerializeField] private Transform spawnPoint;
    
    // Данные о построенных машинах
    private Dictionary<string, VehicleData> playerVehicles = new Dictionary<string, VehicleData>();
    
    public static GarageManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GarageManager>();
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
    
    /// <summary>
    /// Создает новую машину с заданными компонентами
    /// </summary>
    [Rpc(SendTo.Server)]
    public void BuildVehicleRpc(
        ulong playerId,
        string vehicleName,
        ArmorMaterial armor,
        WheelType wheels,
        EngineType engine,
        int cameraLevel,
        int windSensorLevel,
        int windDirectionLevel,
        int sensorPHTLevel,
        int hydraulicCount,
        HydraulicControllerType controllerType
    )
    {
        if (!IsServer) return;
        
        // Проверка баланса
        var playerData = FindObjectOfType<PlayerDataManager>();
        if (playerData == null)
        {
            Debug.LogError("PlayerDataManager not found");
            return;
        }
        
        // Рассчитываем общую стоимость
        float totalPrice = PriceCalculator.GetTotalVehiclePrice(
            armor, wheels, engine,
            new HydraulicControllerType[] { controllerType },
            cameraLevel, windSensorLevel, windDirectionLevel, sensorPHTLevel
        );
        
        if (playerData.GetBalance() < totalPrice)
        {
            Debug.LogWarning("Insufficient funds to build vehicle");
            NotifyBuildFailedClientRpc("Insufficient funds");
            return;
        }
        
        // Списываем деньги
        playerData.TrySpendBalance(totalPrice);
        
        // Создаем данные машины
        string vehicleId = System.Guid.NewGuid().ToString();
        var vehicleData = new VehicleData
        {
            VehicleId = vehicleId,
            VehicleName = vehicleName,
            OwnerId = playerId,
            OwnerCompanyId = playerData.GetCompanyId(),
            ArmorType = armor,
            WheelType = wheels,
            EngineType = engine,
            CameraLevel = cameraLevel,
            WindSensorLevel = windSensorLevel,
            WindDirectionLevel = windDirectionLevel,
            SensorPHTLevel = sensorPHTLevel,
            HydraulicControllerCount = hydraulicCount,
            ControllerType = controllerType,
            CompanyNameDecal = playerData.GetCompanyId(),
            CreationTime = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            TotalMoneyEarned = 0f,
            TornadosIntercepted = 0
        };
        
        // Сохраняем данные
        playerVehicles[vehicleId] = vehicleData;
        playerData.AddVehicleId(vehicleId);
        
        // Уведомляем клиентов
        NotifyVehicleBuiltClientRpc(vehicleId, vehicleName);
        
        Debug.Log($"Vehicle '{vehicleName}' built for player {playerId} (ID: {vehicleId})");
    }
    
    /// <summary>
    /// Спавнит построенную машину на карту
    /// </summary>
    [Rpc(SendTo.Server)]
    public void SpawnVehicleRpc(string vehicleId)
    {
        if (!IsServer) return;
        
        if (!playerVehicles.TryGetValue(vehicleId, out var vehicleData))
        {
            Debug.LogError($"Vehicle {vehicleId} not found");
            return;
        }
        
        // Спавним машину
        Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : Vector3.zero;
        GameObject vehicleInstance = Instantiate(vehiclePrefab, spawnPos, Quaternion.identity);
        
        // Настраиваем машину
        var vehicleController = vehicleInstance.GetComponent<VehicleController>();
        if (vehicleController == null)
        {
            vehicleController = vehicleInstance.AddComponent<VehicleController>();
        }
        
        // Применяем визуальные изменения (цвет, декаль и т.д.)
        ApplyVehicleVisuals(vehicleInstance, vehicleData);
        
        // Регистрируем на сети
        var networkObject = vehicleInstance.GetComponent<NetworkObject>();
        if (networkObject == null)
        {
            networkObject = vehicleInstance.AddComponent<NetworkObject>();
        }
        networkObject.Spawn();
        
        // Уведомляем клиентов
        NotifyVehicleSpawnedClientRpc(vehicleId, spawnPos);
        
        Debug.Log($"Vehicle {vehicleId} spawned at {spawnPos}");
    }
    
    /// <summary>
    /// Применяет визуальные параметры к машине
    /// </summary>
    private void ApplyVehicleVisuals(GameObject vehicleInstance, VehicleData vehicleData)
    {
        // Меняем цвет на основе типа брони
        Renderer renderer = vehicleInstance.GetComponent<Renderer>();
        if (renderer != null)
        {
            Color armorColor = vehicleData.ArmorType switch
            {
                ArmorMaterial.Plywood => new Color(0.8f, 0.6f, 0.3f),
                ArmorMaterial.Metal => Color.gray,
                ArmorMaterial.Titanium => new Color(0.7f, 0.7f, 0.8f),
                _ => Color.white
            };
            
            renderer.material.color = armorColor;
        }
        
        // Добавляем 3D-текст с названием компании
        if (!string.IsNullOrEmpty(vehicleData.CompanyNameDecal))
        {
            // Создаем текстовое меш-поле для названия компании
            // (В реальной игре используй TextMesh Pro 3D)
            Debug.Log($"Company decal: {vehicleData.CompanyNameDecal}");
        }
    }
    
    /// <summary>
    /// Получает данные машины
    /// </summary>
    public VehicleData GetVehicleData(string vehicleId)
    {
        if (playerVehicles.TryGetValue(vehicleId, out var vehicleData))
        {
            return vehicleData;
        }
        return null;
    }
    
    /// <summary>
    /// Получает все машины игрока
    /// </summary>
    public List<VehicleData> GetPlayerVehicles(ulong playerId)
    {
        var result = new List<VehicleData>();
        foreach (var vehicle in playerVehicles.Values)
        {
            if (vehicle.OwnerId == playerId)
            {
                result.Add(vehicle);
            }
        }
        return result;
    }
    
    // ========== RPC NOTIFICATIONS ==========
    
    [Rpc(SendTo.ClientsAndHost)]
    private void NotifyVehicleBuiltClientRpc(string vehicleId, string vehicleName)
    {
        Debug.Log($"[CLIENT] Vehicle built: {vehicleName} (ID: {vehicleId})");
    }
    
    [Rpc(SendTo.ClientsAndHost)]
    private void NotifyVehicleSpawnedClientRpc(string vehicleId, Vector3 position)
    {
        Debug.Log($"[CLIENT] Vehicle spawned: {vehicleId} at {position}");
    }
    
    [Rpc(SendTo.Client)]
    private void NotifyBuildFailedClientRpc(string reason)
    {
        Debug.LogError($"[CLIENT] Build failed: {reason}");
    }
}
