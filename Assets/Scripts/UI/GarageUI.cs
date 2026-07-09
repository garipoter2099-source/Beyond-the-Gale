using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GarageUI : MonoBehaviour
{
    [SerializeField] private Button buildVehicleButton;
    [SerializeField] private Button spawnVehicleButton;
    [SerializeField] private Canvas buildCanvas;
    [SerializeField] private Canvas spawnCanvas;
    [SerializeField] private Button backButton;
    
    // Build Canvas Elements
    [SerializeField] private TMP_InputField vehicleNameInput;
    [SerializeField] private Dropdown armorDropdown;
    [SerializeField] private Dropdown wheelsDropdown;
    [SerializeField] private Dropdown engineDropdown;
    [SerializeField] private Slider cameraSlider;
    [SerializeField] private Slider windSensorSlider;
    [SerializeField] private Slider windDirectionSlider;
    [SerializeField] private Slider sensorPHTSlider;
    [SerializeField] private TextMeshProUGUI totalCostText;
    [SerializeField] private Button confirmBuildButton;
    
    private GarageManager garageManager;
    private PlayerDataManager playerData;
    
    private void Start()
    {
        garageManager = GarageManager.Instance;
        playerData = PlayerDataManager.Instance;
        
        buildVehicleButton.onClick.AddListener(() => buildCanvas.enabled = true);
        spawnVehicleButton.onClick.AddListener(() => spawnCanvas.enabled = true);
        backButton.onClick.AddListener(() => gameObject.SetActive(false));
        confirmBuildButton.onClick.AddListener(BuildVehicle);
        
        // Подключаем обновление цены при изменении слайдеров
        cameraSlider.onValueChanged.AddListener(_ => UpdateTotalCost());
        windSensorSlider.onValueChanged.AddListener(_ => UpdateTotalCost());
        windDirectionSlider.onValueChanged.AddListener(_ => UpdateTotalCost());
        sensorPHTSlider.onValueChanged.AddListener(_ => UpdateTotalCost());
        armorDropdown.onValueChanged.AddListener(_ => UpdateTotalCost());
        wheelsDropdown.onValueChanged.AddListener(_ => UpdateTotalCost());
        engineDropdown.onValueChanged.AddListener(_ => UpdateTotalCost());
    }
    
    private void UpdateTotalCost()
    {
        float totalPrice = PriceCalculator.GetTotalVehiclePrice(
            (ArmorMaterial)armorDropdown.value,
            (WheelType)wheelsDropdown.value,
            (EngineType)engineDropdown.value,
            new HydraulicControllerType[] { HydraulicControllerType.Normal },
            (int)cameraSlider.value,
            (int)windSensorSlider.value,
            (int)windDirectionSlider.value,
            (int)sensorPHTSlider.value
        );
        
        totalCostText.text = $"Total Cost: ${totalPrice:F0}";
    }
    
    private void BuildVehicle()
    {
        if (string.IsNullOrWhiteSpace(vehicleNameInput.text))
        {
            Debug.LogWarning("Vehicle name is empty");
            return;
        }
        
        garageManager.BuildVehicleRpc(
            playerData.GetPlayerId(),
            vehicleNameInput.text,
            (ArmorMaterial)armorDropdown.value,
            (WheelType)wheelsDropdown.value,
            (EngineType)engineDropdown.value,
            (int)cameraSlider.value,
            (int)windSensorSlider.value,
            (int)windDirectionSlider.value,
            (int)sensorPHTSlider.value,
            1,
            HydraulicControllerType.Normal
        );
        
        vehicleNameInput.text = "";
        buildCanvas.enabled = false;
    }
}
