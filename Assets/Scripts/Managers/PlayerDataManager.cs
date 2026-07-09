using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class PlayerDataManager : NetworkBehaviour
{
    private static PlayerDataManager instance;
    
    [SerializeField] private string playerNickname = "Player";
    [SerializeField] private int selectedCharacterSkin = 0;
    private float playerBalance = 0f;
    private ulong playerId;
    
    // Компания
    private string playerCompanyId = "";
    private int playerRank = 0; // 0=Interceptor, 1=Senior, 2=Deputy, 3=Chief
    
    // Оборудование
    private int cameraLevel = 0;
    private int windSensorLevel = 0;
    private int windDirectionLevel = 0;
    private int sensorPHTLevel = 0;
    
    // Построенные машины (список ID)
    private List<string> vehicleIds = new List<string>();
    
    public static PlayerDataManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PlayerDataManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("PlayerDataManager");
                    instance = go.AddComponent<PlayerDataManager>();
                }
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
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        if (IsOwner)
        {
            playerId = OwnerClientId;
            LoadPlayerData();
        }
    }
    
    /// <summary>
    /// Загружает данные игрока с диска
    /// </summary>
    private void LoadPlayerData()
    {
        if (PlayerPrefs.HasKey("PlayerNickname"))
        {
            playerNickname = PlayerPrefs.GetString("PlayerNickname");
        }
        
        if (PlayerPrefs.HasKey("Balance"))
        {
            playerBalance = PlayerPrefs.GetFloat("Balance");
        }
        
        if (PlayerPrefs.HasKey("CameraLevel"))
        {
            cameraLevel = PlayerPrefs.GetInt("CameraLevel");
        }
        
        if (PlayerPrefs.HasKey("WindSensorLevel"))
        {
            windSensorLevel = PlayerPrefs.GetInt("WindSensorLevel");
        }
        
        if (PlayerPrefs.HasKey("WindDirectionLevel"))
        {
            windDirectionLevel = PlayerPrefs.GetInt("WindDirectionLevel");
        }
        
        if (PlayerPrefs.HasKey("SensorPHTLevel"))
        {
            sensorPHTLevel = PlayerPrefs.GetInt("SensorPHTLevel");
        }
    }
    
    /// <summary>
    /// Сохраняет данные игрока на диск
    /// </summary>
    public void SavePlayerData()
    {
        PlayerPrefs.SetString("PlayerNickname", playerNickname);
        PlayerPrefs.SetFloat("Balance", playerBalance);
        PlayerPrefs.SetInt("CameraLevel", cameraLevel);
        PlayerPrefs.SetInt("WindSensorLevel", windSensorLevel);
        PlayerPrefs.SetInt("WindDirectionLevel", windDirectionLevel);
        PlayerPrefs.SetInt("SensorPHTLevel", sensorPHTLevel);
        PlayerPrefs.Save();
    }
    
    // ========== GETTERS & SETTERS ==========
    
    public string GetNickname() => playerNickname;
    public void SetNickname(string nickname)
    {
        playerNickname = nickname;
        SavePlayerData();
    }
    
    public int GetCharacterSkin() => selectedCharacterSkin;
    public void SetCharacterSkin(int skinId)
    {
        selectedCharacterSkin = Mathf.Clamp(skinId, 0, GameConstants.MALE_CHARACTER_VARIANTS - 1);
    }
    
    public float GetBalance() => playerBalance;
    public void AddBalance(float amount)
    {
        playerBalance += amount;
        SavePlayerData();
    }
    
    public bool TrySpendBalance(float amount)
    {
        if (playerBalance >= amount)
        {
            playerBalance -= amount;
            SavePlayerData();
            return true;
        }
        return false;
    }
    
    public void SetBalance(float amount)
    {
        playerBalance = Mathf.Max(0, amount);
        SavePlayerData();
    }
    
    public string GetCompanyId() => playerCompanyId;
    public void SetCompanyId(string companyId)
    {
        playerCompanyId = companyId;
    }
    
    public int GetRank() => playerRank;
    public void SetRank(int rank)
    {
        playerRank = Mathf.Clamp(rank, 0, 3);
    }
    
    public int GetCameraLevel() => cameraLevel;
    public void SetCameraLevel(int level)
    {
        cameraLevel = Mathf.Clamp(level, 0, GameConstants.EquipmentPrices.CAMERA_MAX_LEVEL);
        SavePlayerData();
    }
    
    public int GetWindSensorLevel() => windSensorLevel;
    public void SetWindSensorLevel(int level)
    {
        windSensorLevel = Mathf.Clamp(level, 0, GameConstants.EquipmentPrices.WIND_SENSOR_MAX_LEVEL);
        SavePlayerData();
    }
    
    public int GetWindDirectionLevel() => windDirectionLevel;
    public void SetWindDirectionLevel(int level)
    {
        windDirectionLevel = Mathf.Clamp(level, 0, GameConstants.EquipmentPrices.WIND_DIRECTION_MAX_LEVEL);
        SavePlayerData();
    }
    
    public int GetSensorPHTLevel() => sensorPHTLevel;
    public void SetSensorPHTLevel(int level)
    {
        sensorPHTLevel = Mathf.Clamp(level, 0, GameConstants.EquipmentPrices.SENSOR_PHT_MAX_LEVEL);
        SavePlayerData();
    }
    
    public void AddVehicleId(string vehicleId)
    {
        if (!vehicleIds.Contains(vehicleId))
        {
            vehicleIds.Add(vehicleId);
        }
    }
    
    public List<string> GetVehicleIds() => new List<string>(vehicleIds);
    
    public ulong GetPlayerId() => playerId;
}
