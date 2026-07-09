using UnityEngine;

public class PriceCalculator
{
    /// <summary>
    /// Рассчитывает цену оборудования на основе уровня и базовой цены
    /// </summary>
    public static float CalculateEquipmentPrice(float basePrice, int level, float multiplier)
    {
        if (level <= 1) return basePrice;
        
        float price = basePrice;
        for (int i = 1; i < level; i++)
        {
            price *= multiplier;
        }
        return price;
    }
    
    /// <summary>
    /// Рассчитывает цену камеры
    /// </summary>
    public static float GetCameraPrice(int level)
    {
        return CalculateEquipmentPrice(
            GameConstants.EquipmentPrices.CAMERA_LVL1,
            level,
            GameConstants.EquipmentPrices.CAMERA_MULTIPLIER
        );
    }
    
    /// <summary>
    /// Рассчитывает цену датчика ветра
    /// </summary>
    public static float GetWindSensorPrice(int level)
    {
        return CalculateEquipmentPrice(
            GameConstants.EquipmentPrices.WIND_SENSOR_LVL1,
            level,
            GameConstants.EquipmentPrices.WIND_SENSOR_MULTIPLIER
        );
    }
    
    /// <summary>
    /// Рассчитывает цену датчика направления ветра
    /// </summary>
    public static float GetWindDirectionPrice(int level)
    {
        return CalculateEquipmentPrice(
            GameConstants.EquipmentPrices.WIND_DIRECTION_LVL1,
            level,
            GameConstants.EquipmentPrices.WIND_DIRECTION_MULTIPLIER
        );
    }
    
    /// <summary>
    /// Рассчитывает цену сенсора давления/влажности/температуры
    /// </summary>
    public static float GetSensorPHTPrice(int level)
    {
        return CalculateEquipmentPrice(
            GameConstants.EquipmentPrices.SENSOR_PHT_LVL1,
            level,
            GameConstants.EquipmentPrices.SENSOR_PHT_MULTIPLIER
        );
    }
    
    /// <summary>
    /// Получает цену брони по типу материала
    /// </summary>
    public static float GetArmorPrice(ArmorMaterial material)
    {
        return material switch
        {
            ArmorMaterial.Plywood => GameConstants.ArmorPrices.PLYWOOD,
            ArmorMaterial.Metal => GameConstants.ArmorPrices.METAL,
            ArmorMaterial.Titanium => GameConstants.ArmorPrices.TITANIUM,
            _ => 0f
        };
    }
    
    /// <summary>
    /// Получает цену колес по типу
    /// </summary>
    public static float GetWheelsPrice(WheelType type)
    {
        return type switch
        {
            WheelType.Standard => GameConstants.ArmorPrices.WHEELS_STANDARD,
            WheelType.OffRoad => GameConstants.ArmorPrices.WHEELS_OFF_ROAD,
            WheelType.OffRoadReinforced => GameConstants.ArmorPrices.WHEELS_OFF_ROAD_REINFORCED,
            _ => 0f
        };
    }
    
    /// <summary>
    /// Получает цену двигателя по типу
    /// </summary>
    public static float GetEnginePrice(EngineType type)
    {
        return type switch
        {
            EngineType.V2 => GameConstants.ArmorPrices.ENGINE_V2,
            EngineType.V4 => GameConstants.ArmorPrices.ENGINE_V4,
            EngineType.V6 => GameConstants.ArmorPrices.ENGINE_V6,
            EngineType.V8 => GameConstants.ArmorPrices.ENGINE_V8,
            EngineType.V12 => GameConstants.ArmorPrices.ENGINE_V12,
            _ => 0f
        };
    }
    
    /// <summary>
    /// Получает цену контроллера гидравлической юбки
    /// </summary>
    public static float GetHydraulicControllerPrice(HydraulicControllerType type)
    {
        return type switch
        {
            HydraulicControllerType.Normal => GameConstants.ArmorPrices.HYDRAULIC_CONTROLLER_NORMAL,
            HydraulicControllerType.Enhanced => GameConstants.ArmorPrices.HYDRAULIC_CONTROLLER_ENHANCED,
            _ => 0f
        };
    }
    
    /// <summary>
    /// Получает общую стоимость машины на основе компонентов
    /// </summary>
    public static float GetTotalVehiclePrice(
        ArmorMaterial armor,
        WheelType wheels,
        EngineType engine,
        HydraulicControllerType[] controllers,
        int cameraLevel,
        int windSensorLevel,
        int windDirectionLevel,
        int sensorPHTLevel
    )
    {
        float total = 0f;
        
        // Броня
        total += GetArmorPrice(armor);
        
        // Колеса
        total += GetWheelsPrice(wheels);
        
        // Двигатель
        total += GetEnginePrice(engine);
        
        // Контроллеры
        foreach (var controller in controllers)
        {
            total += GetHydraulicControllerPrice(controller);
        }
        
        // Оборудование
        total += GetCameraPrice(cameraLevel);
        total += GetWindSensorPrice(windSensorLevel);
        total += GetWindDirectionPrice(windDirectionLevel);
        total += GetSensorPHTPrice(sensorPHTLevel);
        
        return total;
    }
}

public enum ArmorMaterial
{
    Plywood,
    Metal,
    Titanium
}

public enum WheelType
{
    Standard,
    OffRoad,
    OffRoadReinforced
}

public enum EngineType
{
    V2,
    V4,
    V6,
    V8,
    V12
}

public enum HydraulicControllerType
{
    Normal,
    Enhanced
}
