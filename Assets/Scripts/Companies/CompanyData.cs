using UnityEngine;
using System.Collections.Generic;

public enum PlayerRank
{
    Interceptor = 0,
    Senior = 1,
    Deputy = 2,
    Chief = 3
}

public class CompanyData
{
    public string CompanyId { get; set; }
    public string CompanyName { get; set; }
    public ulong CreatorClientId { get; set; }
    public long CreationTime { get; set; }
    public int TornadosCaught { get; set; }
    public float CompanyBalance { get; set; }
    
    public List<CompanyMember> Members { get; set; } = new List<CompanyMember>();
}

public class CompanyMember
{
    public ulong ClientId { get; set; }
    public string Nickname { get; set; }
    public PlayerRank Rank { get; set; }
    public long JoinTime { get; set; }
    public int TornadosCaught { get; set; }
}

public class VehicleData
{
    public string VehicleId { get; set; }
    public string VehicleName { get; set; }
    public ulong OwnerId { get; set; }
    public string OwnerCompanyId { get; set; }
    
    // Броня
    public ArmorMaterial ArmorType { get; set; }
    
    // Колеса
    public WheelType WheelType { get; set; }
    
    // Двигатель
    public EngineType EngineType { get; set; }
    
    // Оборудование
    public int CameraLevel { get; set; }
    public int WindSensorLevel { get; set; }
    public int WindDirectionLevel { get; set; }
    public int SensorPHTLevel { get; set; }
    
    // Гидравлические контроллеры
    public int HydraulicControllerCount { get; set; }
    public HydraulicControllerType ControllerType { get; set; }
    
    // Декаль
    public string CompanyNameDecal { get; set; }
    
    // Статистика
    public long CreationTime { get; set; }
    public float TotalMoneyEarned { get; set; }
    public int TornadosIntercepted { get; set; }
}
