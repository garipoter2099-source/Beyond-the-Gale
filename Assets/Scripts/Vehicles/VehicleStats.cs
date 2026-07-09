using UnityEngine;

public class VehicleStats
{
    public string VehicleId { get; set; }
    public string VehicleName { get; set; }
    
    // Броня
    public ArmorMaterial ArmorType { get; set; }
    public float ArmorHP { get; set; }
    
    // Колеса
    public WheelType WheelType { get; set; }
    public float WheelDurability { get; set; }
    
    // Двигатель
    public EngineType EngineType { get; set; }
    public float MaxSpeed { get; set; }
    public float Acceleration { get; set; }
    
    // Оборудование
    public int CameraLevel { get; set; }
    public int WindSensorLevel { get; set; }
    public int WindDirectionLevel { get; set; }
    public int SensorPHTLevel { get; set; }
    
    // Гидравлика
    public int HydraulicControllerCount { get; set; }
    public HydraulicControllerType ControllerType { get; set; }
    public float DeployWeight { get; set; }
    
    // Статистика
    public float TotalMoneyEarned { get; set; }
    public int TornadosIntercepted { get; set; }
    public long CreationTime { get; set; }
    
    public VehicleStats()
    {
        ArmorHP = 100f;
        WheelDurability = 100f;
        TotalMoneyEarned = 0f;
        TornadosIntercepted = 0;
        CreationTime = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        DeployWeight = 50f; // Базовый вес юбки
    }
    
    /// <summary>
    /// Применяет урон брони
    /// </summary>
    public void TakeDamage(float damage)
    {
        ArmorHP = Mathf.Max(0, ArmorHP - damage);
    }
    
    /// <summary>
    /// Проверяет, машина уничтожена
    /// </summary>
    public bool IsDestroyed() => ArmorHP <= 0;
    
    /// <summary>
    /// Получает множитель скорости на основе типа колес
    /// </summary>
    public float GetWheelSpeedMultiplier()
    {
        return WheelType switch
        {
            WheelType.Standard => 1f,
            WheelType.OffRoad => 1.1f,
            WheelType.OffRoadReinforced => 1.2f,
            _ => 1f
        };
    }
    
    /// <summary>
    /// Получает максимальную скорость на основе двигателя
    /// </summary>
    public float GetEngineMaxSpeed()
    {
        float baseSpeed = EngineType switch
        {
            EngineType.V2 => 50f,
            EngineType.V4 => 70f,
            EngineType.V6 => 90f,
            EngineType.V8 => 120f,
            EngineType.V12 => 150f,
            _ => 50f
        };
        
        return baseSpeed * GetWheelSpeedMultiplier();
    }
    
    /// <summary>
    /// Получает множитель защиты на основе брони
    /// </summary>
    public float GetArmorDefenseMultiplier()
    {
        return ArmorType switch
        {
            ArmorMaterial.Plywood => 0.8f,
            ArmorMaterial.Metal => 1.2f,
            ArmorMaterial.Titanium => 1.8f,
            _ => 1f
        };
    }
}
