using UnityEngine;
using TMPro;
using Unity.Netcode;

public class HUDCanvas : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI balanceText;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI deployStatusText;
    [SerializeField] private TextMeshProUGUI tornadoInfoText;
    
    [SerializeField] private Button btnCompany;
    [SerializeField] private Button btnRadar;
    [SerializeField] private Button btnMap;
    [SerializeField] private Button btnGarage;
    [SerializeField] private Button btnChat;
    [SerializeField] private Button btnMainMenu;
    
    [SerializeField] private Canvas companyCanvas;
    [SerializeField] private Canvas radarCanvas;
    [SerializeField] private Canvas mapCanvas;
    [SerializeField] private Canvas garageCanvas;
    [SerializeField] private Canvas chatCanvas;
    
    private PlayerDataManager playerData;
    private VehicleController currentVehicle;
    
    private void Start()
    {
        playerData = PlayerDataManager.Instance;
        
        // Подключаем кнопки
        btnCompany.onClick.AddListener(() => ToggleCanvas(companyCanvas));
        btnRadar.onClick.AddListener(() => ToggleCanvas(radarCanvas));
        btnMap.onClick.AddListener(() => ToggleCanvas(mapCanvas));
        btnGarage.onClick.AddListener(() => ToggleCanvas(garageCanvas));
        btnChat.onClick.AddListener(() => ToggleCanvas(chatCanvas));
        btnMainMenu.onClick.AddListener(GoToMainMenu);
        
        // Находим текущую машину игрока
        currentVehicle = FindObjectOfType<VehicleController>();
    }
    
    private void Update()
    {
        UpdateHUD();
    }
    
    private void UpdateHUD()
    {
        // Баланс
        if (playerData != null)
        {
            balanceText.text = $"{LocalizationManager.Instance.GetText("balance")}{playerData.GetBalance():F0}";
        }
        
        // Скорость и статус деплоя
        if (currentVehicle != null)
        {
            // Здесь нужно добавить получение скорости из VehicleController
            speedText.text = $"{LocalizationManager.Instance.GetText("speed")}: 0 km/h";
            
            string deployStatus = currentVehicle.IsDeployActive() ? "ACTIVE" : "INACTIVE";
            deployStatusText.text = $"{LocalizationManager.Instance.GetText("deploy_status")}: {deployStatus}";
        }
    }
    
    private void ToggleCanvas(Canvas canvas)
    {
        if (canvas != null)
        {
            canvas.enabled = !canvas.enabled;
        }
    }
    
    private void GoToMainMenu()
    {
        // Отключаемся от сервера
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.Shutdown();
        }
        
        // Загружаем главное меню
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
