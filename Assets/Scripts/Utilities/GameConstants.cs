using UnityEngine;

public static class GameConstants
{
    // ========== TORNADO CONSTANTS ==========
    public const float TORNADO_SPAWN_INTERVAL = 240f; // 4 минуты
    public const float TORNADO_LIFESPAN = 600f; // 10 минут
    public const float TORNADO_MIN_SIZE = 5f;
    public const float TORNADO_MAX_SIZE = 50f;
    
    // EF Scale multiplier
    public const float EF_SCALE_MULTIPLIER = 1.5f;
    
    // Tornado Pull Force
    public const float BASE_PULL_FORCE = 100f;
    public const float MAX_PULL_FORCE = 1000f;
    
    // ========== LOCATION MODIFIERS ==========
    public static class LocationTornadoChance
    {
        public const float GALEVIEW = 1.0f;           // Спавн (базовый)
        public const float VORTEX_CITY = 1.4f;        // +40%
        public const float DUSTY_RIDGE = 1.1f;        // +10%
        public const float WINDY_WILLOW = 1.2f;       // +20%
        public const float TWISTER_CREEK = 1.15f;     // +15%
        public const float STORMVILLE = 1.25f;        // +25%
        public const float SIRENHILL = 1.75f;         // +75%
    }
    
    // ========== VEHICLE CONSTANTS ==========
    public const float VEHICLE_BASE_SPEED = 20f;
    public const float VEHICLE_ACCELERATION = 10f;
    public const float VEHICLE_BRAKE_FORCE = 15f;
    public const float VEHICLE_HANDBRAKE_FORCE = 20f;
    public const float DEPLOY_WEIGHT_MULTIPLIER = 100f; // Юбка фиксирует машину
    
    // ========== REWARD CONSTANTS ==========
    public const float REWARD_INTERVAL = 10f; // каждые 10 секунд
    public const float REWARD_MIN = 250f;     // EFU
    public const float REWARD_MAX = 2000f;    // EF5
    public const float TORNADO_DETECTION_RADIUS = 50f;
    
    // ========== COMPANY SYSTEM ==========
    public const float COMPANY_CREATION_COST = 100000f;
    public const int MAX_COMPANY_MEMBERS = 50;
    
    // ========== EQUIPMENT PRICES ==========
    public static class EquipmentPrices
    {
        // Камера (×3 за уровень)
        public const float CAMERA_LVL1 = 2000f;
        public const float CAMERA_MULTIPLIER = 3f;
        public const int CAMERA_MAX_LEVEL = 10;
        
        // Датчик ветра (×1.5 за уровень)
        public const float WIND_SENSOR_LVL1 = 800f;
        public const float WIND_SENSOR_MULTIPLIER = 1.5f;
        public const int WIND_SENSOR_MAX_LEVEL = 10;
        
        // Датчик направления ветра (×2 за уровень)
        public const float WIND_DIRECTION_LVL1 = 700f;
        public const float WIND_DIRECTION_MULTIPLIER = 2f;
        public const int WIND_DIRECTION_MAX_LEVEL = 15;
        
        // Датчик давления/влажности/температуры (×2.5 за уровень)
        public const float SENSOR_PHT_LVL1 = 900f;
        public const float SENSOR_PHT_MULTIPLIER = 2.5f;
        public const int SENSOR_PHT_MAX_LEVEL = 20;
    }
    
    // ========== ARMOR PRICES ==========
    public static class ArmorPrices
    {
        // Материалы корпуса за 10 листов
        public const float PLYWOOD = 1000f;
        public const float METAL = 10000f;
        public const float TITANIUM = 40000f;
        
        // Колеса за пару (2 шт)
        public const float WHEELS_STANDARD = 2500f;
        public const float WHEELS_OFF_ROAD = 10000f;
        public const float WHEELS_OFF_ROAD_REINFORCED = 20000f;
        
        // Двигатели
        public const float ENGINE_V2 = 4000f;
        public const float ENGINE_V4 = 6000f;
        public const float ENGINE_V6 = 10000f;
        public const float ENGINE_V8 = 20000f;
        public const float ENGINE_V12 = 50000f;
        
        // Контроллеры гидравлической юбки
        public const float HYDRAULIC_CONTROLLER_NORMAL = 3000f;
        public const float HYDRAULIC_CONTROLLER_ENHANCED = 10000f;
    }
    
    // ========== UI LAYERS ==========
    public const int UI_LAYER_HUD = 5;
    public const int UI_LAYER_MENU = 10;
    public const int UI_LAYER_DIALOG = 15;
    
    // ========== NETWORK CONSTANTS ==========
    public const int MAX_PLAYERS = 64;
    public const float NETWORK_TICK_RATE = 60f;
    public const float POSITION_SYNC_INTERVAL = 0.1f;
    
    // ========== CHARACTER SKINS ==========
    public const int MALE_CHARACTER_VARIANTS = 5;
    
    // ========== TORNADO CATEGORIES ==========
    public enum TornadoCategory
    {
        EFU = 0,    // 39-73 mph
        EF0 = 1,    // 73-112 mph
        EF1 = 2,    // 112-157 mph
        EF2 = 3,    // 157-207 mph
        EF3 = 4,    // 207-260 mph
        EF4 = 5,    // 260-318 mph
        EF5 = 6     // 318+ mph
    }
    
    // ========== WIND SPEED THRESHOLDS ==========
    public static class WindThresholds
    {
        public const float EFU_MAX = 73f;
        public const float EF0_MAX = 112f;
        public const float EF1_MAX = 157f;
        public const float EF2_MAX = 207f;
        public const float EF3_MAX = 260f;
        public const float EF4_MAX = 318f;
        public const float EF5_MAX = 400f;
    }
}
