# КРАТКОЕ РУКОВОДСТВО ПО ВНЕДРЕНИЮ Beyond the Gale

## Шаг 1: Подготовка сцен

### SplashScreen
1. Создай новую сцену `SplashScreen.unity`
2. Добавь Panel с логотипом "Yes!Product technologies"
3. Создай скрипт SplashScreenController:

```csharp
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreenController : MonoBehaviour
{
    [SerializeField] private float displayDuration = 3f;
    
    private void Start()
    {
        Invoke(nameof(LoadMainMenu), displayDuration);
    }
    
    private void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
```

### MainMenu
1. Создай сцену MainMenu.unity
2. Добавь Canvas с кнопками:
   - Play Button
   - Settings Button
   - Exit Button
3. Добавь скрипт MenuUI.cs
4. Подключи все кнопки через Inspector

### Lobby
1. Создай сцену Lobby.unity
2. Добавь UI для показа подключенных игроков
3. Добавь кнопку "Start Game"

### GameScene
1. Создай сцену GameScene.unity с 3D окружением
2. Добавь NetworkManager с компонентом Netcode
3. Расставь 7 локаций (Galeview, Vortex City и т.д.)
4. Добавь пустой GameObject "TornadoSpawner" с NetworkTornadoManager
5. Расставь спавн-пойнты для машин (spawn point)
6. Добавь HUD Canvas с необходимыми панелями

## Шаг 2: Конфигурация Prefabs

### Vehicle Prefab (Assets/Prefabs/Vehicles/Vehicle.prefab)
```
Vehicle (Root)
├── Body (Cube) - материал для брони
├── Wheels (4x Cylinder)
├── Rigidbody
├── BoxCollider
├── VehicleController (скрипт)
├── DeploySystem (скрипт)
├── NetworkObject (компонент)
└── NetworkTransform (компонент)
```

### Tornado Prefab (Assets/Prefabs/Tornadoes/Tornado.prefab)
```
Tornado (Root)
├── Visual (Cone + Particles)
├── SphereCollider (is Trigger)
├── Tornado (скрипт)
├── TornadoPhysics (скрипт)
├── NetworkObject (компонент)
└── NetworkTransform (компонент)
```

### Building Prefab (Assets/Prefabs/Buildings/Building.prefab)
```
Building (Root)
├── Visual (Cube)
├── BoxCollider
├── Rigidbody
├── TornadoDestruction (скрипт)
└── Tag: "Building"
```

## Шаг 3: Network Setup

1. Создай пустой GameObject "NetworkManager"
2. Добавь компонент `Unity.Netcode.NetworkManager`
3. Конфигурируй:
   - Connection Approval: enabled
   - Tick Rate: 60
   - Transport: выбери Netcode Transport

4. Создай пустой GameObject "PlayerSpawner" с компонентом:
```csharp
using Unity.Netcode;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform[] spawnPoints;
    
    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += SpawnPlayers;
    }
    
    private void SpawnPlayers()
    {
        // Спавним игроков на сервере
        foreach (var spawnPoint in spawnPoints)
        {
            var player = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);
            var networkObject = player.GetComponent<NetworkObject>();
            networkObject.Spawn();
        }
    }
}
```

## Шаг 4: Сохранение данных

### PlayerDataManager автоматически использует PlayerPrefs:
```csharp
// Сохранение
PlayerPrefs.SetString("PlayerNickname", "Player");
PlayerPrefs.SetFloat("Balance", 10000f);
PlayerPrefs.Save();

// Загрузка
string nickname = PlayerPrefs.GetString("PlayerNickname");
float balance = PlayerPrefs.GetFloat("Balance");
```

## Шаг 5: Input Manager Configuration

Edit > Project Settings > Input Manager

Добавить оси:
```
Horizontal  -> A/D (для поворота)
Vertical    -> W/S (для ускорения)
Deploy      -> Q (для деплоя)
Handbrake   -> E (для ручного тормоза)
EnterExit   -> F (для входа/выхода)
```

## Шаг 6: Исправление ошибок компиляции

### Ошибка: "Button не найден"
```csharp
// Добавь using
using UnityEngine.UI;
```

### Ошибка: "RawImage не найден"
```csharp
// Добавь using
using UnityEngine.UI;
```

### Ошибка: "ScrollRect не найден"
```csharp
// Добавь using
using UnityEngine.UI;
```

## Шаг 7: Финальная настройка

1. **Build Settings:**
   - Добавь все сцены в порядке: SplashScreen -> MainMenu -> Lobby -> GameScene -> CharacterSelect

2. **Quality Settings:**
   - Настрой 5 уровней качества (Potato, Low, Medium, High, Ultra)

3. **Physics:**
   - Gravity: -9.81
   - Default Solver Iterations: 6
   - Default Solver Velocity Iterations: 2

4. **Time:**
   - Time Scale: 1.0
   - Fixed Timestep: 0.02

## Шаг 8: Тестирование

1. Запусти сцену SplashScreen
2. Проверь загрузку меню
3. Создай тестовую компанию
4. Построй тестовую машину
5. Спавни её на карте
6. Проверь физику торнадо и взаимодействие

## Полезные команды для отладки

```csharp
// Добавить деньги
PlayerDataManager.Instance.AddBalance(100000);

// Получить информацию о компании
var company = CompanySystem.Instance.GetCompanyData(companyId);

// Получить активные торнадо
var tornadoes = NetworkTornadoManager.Instance.GetActiveTornadoes();
```

## Проверка списка

- [ ] Все сцены созданы и добавлены в Build Settings
- [ ] NetworkManager настроен
- [ ] Prefabs созданы и настроены
- [ ] UI Canvas созданы с нужными элементами
- [ ] Все скрипты скомпилированы без ошибок
- [ ] Input Manager настроен
- [ ] Игра запускается и загружает меню
- [ ] Сетевое подключение работает
- [ ] Торнадо спавнятся на карте
- [ ] Машины могут быть построены и заспавнены
- [ ] Чат работает для всех игроков

## Контакты для поддержки

При возникновении проблем проверь:
1. Консоль Unity на предмет ошибок
2. Логи сетевого подключения
3. Правильность конфигурации Prefabs
4. Версию Netcode for GameObjects
