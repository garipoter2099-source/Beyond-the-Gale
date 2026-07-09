# Структура сетевых RPC вызовов в Beyond the Gale

## Архитектура Server-Authoritative

Вся игра работает на основе Server-Authoritative архитектуры:
- **Сервер** - источник истины для всех игровых данных
- **Клиенты** - отправляют запросы на сервер и получают подтверждения

## Основные RPC вызовы

### PlayerDataManager
```csharp
// Клиент -> Сервер
PlayerDataManager.Instance.AddBalance(100f);
PlayerDataManager.Instance.TrySpendBalance(50f);

// Автоматически синхронизируется через NetworkVariable
```

### CompanySystem
```csharp
// Создание компании (Клиент -> Сервер)
CompanySystem.Instance.CreateCompanyRpc("MyCompany", clientId);

// Присоединение к компании
CompanySystem.Instance.JoinCompanyRpc(playerId, companyId);

// Повышение ранга
CompanySystem.Instance.PromotePlayerRpc(issuerId, targetId, companyId);

// Добавление награды
CompanySystem.Instance.AddCompanyRewardRpc(companyId, amount);
```

### NetworkTornadoManager
```csharp
// Спавн торнадо (Автоматически на сервере каждые 4 минуты)
NetworkTornadoManager.Instance.GetActiveTornadoes();
```

### VehicleController
```csharp
// Требование награды
ClaimRewardRpc(amount); // Внутренний RPC

// Переключение деплоя
ToggleDeployRpc(state); // Внутренний RPC

// Сигналы поворота
UpdateTurnSignalRpc(signalType, state); // Внутренний RPC
```

### GarageManager
```csharp
// Построение машины
GarageManager.Instance.BuildVehicleRpc(
    playerId,
    "VehicleName",
    ArmorMaterial.Metal,
    WheelType.OffRoad,
    EngineType.V8,
    cameraLevel: 5,
    windSensorLevel: 3,
    windDirectionLevel: 4,
    sensorPHTLevel: 2,
    hydraulicCount: 2,
    HydraulicControllerType.Enhanced
);

// Спавн машины
GarageManager.Instance.SpawnVehicleRpc(vehicleId);
```

## NetworkVariable синхронизация

### Tornado
```csharp
networkPosition          // Vector3 - позиция торнадо
networkWindSpeed        // float - скорость ветра
networkCategory         // TornadoCategory - категория
```

### VehicleController
```csharp
networkVelocity         // Vector3 - скорость машины
networkDeployStatus     // bool - активен ли деплой
networkLeftTurnSignal   // int - левый поворотник
networkRightTurnSignal  // int - правый поворотник
networkEmergencySignal  // int - аварийка
```

### DeploySystem
```csharp
networkDeployState      // bool - состояние деплоя
```

## Примеры использования

### Пример 1: Создание компании
```csharp
public void CreatePlayerCompany(string companyName)
{
    var playerData = PlayerDataManager.Instance;
    var companySystem = CompanySystem.Instance;
    
    if (playerData.GetBalance() >= GameConstants.COMPANY_CREATION_COST)
    {
        companySystem.CreateCompanyRpc(companyName, playerData.GetPlayerId());
    }
}
```

### Пример 2: Построение машины
```csharp
public void BuildNewVehicle(string vehicleName)
{
    var garageManager = GarageManager.Instance;
    var playerData = PlayerDataManager.Instance;
    
    garageManager.BuildVehicleRpc(
        playerData.GetPlayerId(),
        vehicleName,
        ArmorMaterial.Titanium,
        WheelType.OffRoadReinforced,
        EngineType.V12,
        10, 10, 15, 20, 2,
        HydraulicControllerType.Enhanced
    );
}
```

### Пример 3: Получение информации о компании
```csharp
public void DisplayCompanyInfo()
{
    var companySystem = CompanySystem.Instance;
    var playerData = PlayerDataManager.Instance;
    var companyId = playerData.GetCompanyId();
    
    if (!string.IsNullOrEmpty(companyId))
    {
        var company = companySystem.GetCompanyData(companyId);
        var members = companySystem.GetCompanyMembers(companyId);
        
        Debug.Log($"Company: {company.CompanyName}");
        Debug.Log($"Members: {members.Count}");
        Debug.Log($"Tornados: {company.TornadosCaught}");
    }
}
```

## Обработка ошибок сетевых операций

```csharp
[Rpc(SendTo.Server)]
public void SafeOperationRpc(string data)
{
    if (!IsServer) return; // Проверяем, что это сервер
    
    try
    {
        // Выполняем операцию
    }
    catch (System.Exception ex)
    {
        Debug.LogError($"RPC Error: {ex.Message}");
    }
}
```

## Оптимизация сетевых передач

1. **Использование NetworkVariable для часто меняющихся данных**
2. **Использование RPC для редких событий**
3. **Не отправляй большие объемы данных часто**
4. **Кэшируй результаты запросов на клиенте**

## Debugging сетевых проблем

```csharp
// Добавь этот скрипт для отладки
public class NetworkDebugger : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F10))
        {
            Debug.Log($"Is Server: {NetworkManager.Singleton.IsServer}");
            Debug.Log($"Is Client: {NetworkManager.Singleton.IsClient}");
            Debug.Log($"Connected Clients: {NetworkManager.Singleton.ConnectedClientsIds.Count}");
        }
    }
}
```
