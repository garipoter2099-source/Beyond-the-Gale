# Инструкция по настройке проекта Beyond the Gale

## Шаг 1: Создание структуры папок

В папке `Assets/` создай следующую структуру:

```
Assets/
├── Scripts/
│   ├── Network/
│   │   ├── NetworkManager.cs
│   │   ├── NetworkSpawner.cs
│   │   └── SyncManager.cs
│   ├── Managers/
│   │   ├── GameManager.cs
│   │   ├── MapManager.cs
│   │   ├── TornadoSpawner.cs
│   │   └── PlayerDataManager.cs
│   ├── Vehicles/
│   │   ├── VehicleController.cs
│   │   ├── VehicleStats.cs
│   │   └── DeploySystem.cs
│   ├── UI/
│   │   ├── UIManager.cs
│   │   ├── MenuUI.cs
│   │   ├── HUDCanvas.cs
│   │   ├── CompanyMenuUI.cs
│   │   ├── GarageUI.cs
│   │   ├── RadarUI.cs
│   │   └── ChatUI.cs
│   ├── Tornadoes/
│   │   ├── Tornado.cs
│   │   ├── TornadoPhysics.cs
│   │   ├── NetworkTornadoManager.cs
│   │   └── TornadoDestruction.cs
│   ├── Companies/
│   │   ├── CompanySystem.cs
│   │   ├── CompanyData.cs
│   │   └── CompanyRoles.cs
│   └── Utilities/
│       ├── PriceCalculator.cs
│       ├── LocalizationManager.cs
│       └── GameConstants.cs
├── Scenes/
│   ├── SplashScreen.unity
│   ├── MainMenu.unity
│   ├── Lobby.unity
│   ├── GameScene.unity
│   └── CharacterSelect.unity
├── Prefabs/
│   ├── Vehicles/
│   ├── Tornadoes/
│   ├── Buildings/
│   └── UI/
├── Resources/
│   ├── Localization/
│   ├── Characters/
│   └── Settings/
└── StreamingAssets/
```

## Шаг 2: Установка пакетов Unity

Отворить Window > TextMesh Pro > Import TMP Essential Resources

Установить через Package Manager:
- Netcode for GameObjects
- DOTween (для анимаций)
- JSON.NET (для сохранений)

## Шаг 3: Создание сцен

1. **SplashScreen** - заставка 3 секунды
2. **MainMenu** - главное меню
3. **Lobby** - лобби ожидания
4. **GameScene** - основная игровая сцена
5. **CharacterSelect** - выбор персонажа

## Шаг 4: Конфигурирование сетевого менеджера

1. Создать пустой GameObject "NetworkManager"
2. Добавить компонент Netcode NetworkManager
3. Настроить:
   - Connection Approval
   - Tick Rate: 60
   - Max Players: 64

## Шаг 5: Создание 3D моделей и ассетов

**Примечание:** Используй простые формы (Cube, Cylinder) для тестирования, затем замени на реальные модели.

### Дома:
- 1-этажный дом (Cube)
- 2-этажный дом (вытянутый Cube)
- 3-этажный дом (высокий Cube)

### Машина:
- Базовая модель (Cube с колесами из Sphere)
- Добавить Rigidbody и Collider

### Торнадо:
- Конус (Cone) с вращением
- Цилиндр с эффектами частиц

## Шаг 6: Настройка InputManager

Edit > Project Settings > Input Manager

Добавить оси:
- Horizontal (WASD)
- Vertical (WASD)
- Deploy (Q)
- Handbrake (E)
- EnterExit (F)
- LeftTurnSignal (1)
- RightTurnSignal (2)
- EmergencySignal (3)

## Шаг 7: Настройка Build Settings

1. Добавить все сцены в Build Settings в нужном порядке
2. Установить SplashScreen сценой 0
3. Настроить сетевые настройки для готовой игры

## Готово к разработке!

Теперь ты можешь начать работать с предоставленными скриптами.
