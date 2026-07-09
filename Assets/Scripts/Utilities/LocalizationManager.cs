using UnityEngine;
using System.Collections.Generic;

public class LocalizationManager : MonoBehaviour
{
    private static LocalizationManager instance;
    
    public enum Language
    {
        English,
        Spanish,
        Russian
    }
    
    private Language currentLanguage = Language.English;
    
    private Dictionary<Language, Dictionary<string, string>> translations = new();
    
    public static LocalizationManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<LocalizationManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("LocalizationManager");
                    instance = go.AddComponent<LocalizationManager>();
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
            InitializeTranslations();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    private void InitializeTranslations()
    {
        // ========== ENGLISH ==========
        translations[Language.English] = new Dictionary<string, string>
        {
            // Menu
            { "btn_play", "Play" },
            { "btn_exit", "Exit" },
            { "btn_settings", "Settings" },
            { "btn_back", "Back" },
            
            // Settings
            { "label_nickname", "Nickname" },
            { "label_language", "Language" },
            { "label_graphics", "Graphics Quality" },
            { "quality_potato", "Ultra-Low" },
            { "quality_low", "Low" },
            { "quality_medium", "Medium" },
            { "quality_high", "High" },
            { "quality_ultra", "Ultra" },
            
            // Game UI
            { "balance", "Balance: $" },
            { "company", "Company" },
            { "radar", "Radar" },
            { "map", "Map" },
            { "garage", "Garage" },
            { "chat", "Chat" },
            { "speed", "Speed" },
            { "deploy_status", "Deploy" },
            
            // Company
            { "btn_create_company", "Create Company" },
            { "btn_join_company", "Join Company" },
            { "label_company_name", "Company Name" },
            { "label_members", "Members" },
            { "label_tornados_caught", "Tornados Caught" },
            { "label_rank", "Rank" },
            { "rank_interceptor", "Interceptor" },
            { "rank_senior", "Senior" },
            { "rank_deputy", "Deputy" },
            { "rank_chief", "Chief" },
            
            // Garage
            { "btn_buy_equipment", "Buy Equipment" },
            { "btn_build_vehicle", "Build Vehicle" },
            { "btn_spawn_vehicle", "Spawn Vehicle" },
            { "label_camera", "Camera" },
            { "label_wind_sensor", "Wind Sensor" },
            { "label_wind_direction", "Wind Direction Sensor" },
            { "label_pressure_sensor", "Pressure/Humidity/Temperature Sensor" },
            { "label_armor", "Armor Material" },
            { "label_wheels", "Wheels" },
            { "label_engine", "Engine" },
            
            // Tornado
            { "tornado_efu", "EFU" },
            { "tornado_ef0", "EF0" },
            { "tornado_ef1", "EF1" },
            { "tornado_ef2", "EF2" },
            { "tornado_ef3", "EF3" },
            { "tornado_ef4", "EF4" },
            { "tornado_ef5", "EF5" },
        };
        
        // ========== SPANISH ==========
        translations[Language.Spanish] = new Dictionary<string, string>
        {
            // Menu
            { "btn_play", "Jugar" },
            { "btn_exit", "Salir" },
            { "btn_settings", "Configuración" },
            { "btn_back", "Atrás" },
            
            // Settings
            { "label_nickname", "Apodo" },
            { "label_language", "Idioma" },
            { "label_graphics", "Calidad de Gráficos" },
            { "quality_potato", "Ultra-Bajo" },
            { "quality_low", "Bajo" },
            { "quality_medium", "Medio" },
            { "quality_high", "Alto" },
            { "quality_ultra", "Ultra" },
            
            // Game UI
            { "balance", "Saldo: $" },
            { "company", "Empresa" },
            { "radar", "Radar" },
            { "map", "Mapa" },
            { "garage", "Garaje" },
            { "chat", "Chat" },
            { "speed", "Velocidad" },
            { "deploy_status", "Desplegar" },
            
            // Company
            { "btn_create_company", "Crear Empresa" },
            { "btn_join_company", "Unirse a Empresa" },
            { "label_company_name", "Nombre de Empresa" },
            { "label_members", "Miembros" },
            { "label_tornados_caught", "Tornados Capturados" },
            { "label_rank", "Rango" },
            { "rank_interceptor", "Interceptor" },
            { "rank_senior", "Senior" },
            { "rank_deputy", "Diputado" },
            { "rank_chief", "Jefe" },
            
            // Garage
            { "btn_buy_equipment", "Comprar Equipo" },
            { "btn_build_vehicle", "Construir Vehículo" },
            { "btn_spawn_vehicle", "Desplegar Vehículo" },
            { "label_camera", "Cámara" },
            { "label_wind_sensor", "Sensor de Viento" },
            { "label_wind_direction", "Sensor de Dirección del Viento" },
            { "label_pressure_sensor", "Sensor de Presión/Humedad/Temperatura" },
            { "label_armor", "Material de Armadura" },
            { "label_wheels", "Ruedas" },
            { "label_engine", "Motor" },
        };
        
        // ========== RUSSIAN ==========
        translations[Language.Russian] = new Dictionary<string, string>
        {
            // Menu
            { "btn_play", "Играть" },
            { "btn_exit", "Выход" },
            { "btn_settings", "Настройки" },
            { "btn_back", "Назад" },
            
            // Settings
            { "label_nickname", "Никнейм" },
            { "label_language", "Язык" },
            { "label_graphics", "Качество графики" },
            { "quality_potato", "Картошка (Очень низкое)" },
            { "quality_low", "Низкое" },
            { "quality_medium", "Среднее" },
            { "quality_high", "Высокое" },
            { "quality_ultra", "Ультра" },
            
            // Game UI
            { "balance", "Баланс: $" },
            { "company", "Компания" },
            { "radar", "Радар" },
            { "map", "Карта" },
            { "garage", "Гараж" },
            { "chat", "Чат" },
            { "speed", "Скорость" },
            { "deploy_status", "Деплой" },
            
            // Company
            { "btn_create_company", "Создать компанию" },
            { "btn_join_company", "Присоединиться" },
            { "label_company_name", "Название компании" },
            { "label_members", "Члены" },
            { "label_tornados_caught", "Перехвачено торнадо" },
            { "label_rank", "Нин" },
            { "rank_interceptor", "Перехватчик" },
            { "rank_senior", "Старший" },
            { "rank_deputy", "Заместитель" },
            { "rank_chief", "Главный" },
            
            // Garage
            { "btn_buy_equipment", "Купить оборудование" },
            { "btn_build_vehicle", "Составить машину" },
            { "btn_spawn_vehicle", "Навзыть машину" },
            { "label_camera", "Камера" },
            { "label_wind_sensor", "Датчик ветра" },
            { "label_wind_direction", "Датчик направления ветра" },
            { "label_pressure_sensor", "Датчик давления/влажности/температуры" },
            { "label_armor", "Материал брони" },
            { "label_wheels", "Колеса" },
            { "label_engine", "Двигатель" },
        };
    }
    
    public void SetLanguage(Language language)
    {
        currentLanguage = language;
        PlayerPrefs.SetInt("Language", (int)language);
    }
    
    public Language GetLanguage() => currentLanguage;
    
    public string GetText(string key)
    {
        if (translations[currentLanguage].TryGetValue(key, out var text))
        {
            return text;
        }
        
        Debug.LogWarning($"Translation key not found: {key}");
        return key;
    }
    
    public void LoadSettings()
    {
        if (PlayerPrefs.HasKey("Language"))
        {
            currentLanguage = (Language)PlayerPrefs.GetInt("Language");
        }
    }
}
