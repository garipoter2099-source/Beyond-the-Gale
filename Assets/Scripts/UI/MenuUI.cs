using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitButton;
    
    [SerializeField] private Canvas settingsCanvas;
    [SerializeField] private TMP_InputField nicknameInput;
    [SerializeField] private Dropdown languageDropdown;
    [SerializeField] private Dropdown graphicsDropdown;
    [SerializeField] private Button settingsBackButton;
    
    private void Start()
    {
        playButton.onClick.AddListener(Play);
        settingsButton.onClick.AddListener(OpenSettings);
        exitButton.onClick.AddListener(Exit);
        settingsBackButton.onClick.AddListener(CloseSettings);
        
        // Загружаем сохраненные настройки
        LoadSettings();
    }
    
    private void Play()
    {
        // Сохраняем никнейм
        PlayerPrefs.SetString("PlayerNickname", nicknameInput.text);
        PlayerPrefs.Save();
        
        // Загружаем лобби
        UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby");
    }
    
    private void OpenSettings()
    {
        settingsCanvas.enabled = true;
    }
    
    private void CloseSettings()
    {
        // Сохраняем настройки
        SaveSettings();
        settingsCanvas.enabled = false;
    }
    
    private void Exit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    private void SaveSettings()
    {
        PlayerPrefs.SetString("PlayerNickname", nicknameInput.text);
        PlayerPrefs.SetInt("Language", languageDropdown.value);
        PlayerPrefs.SetInt("GraphicsQuality", graphicsDropdown.value);
        PlayerPrefs.Save();
        
        // Применяем качество графики
        QualitySettings.SetQualityLevel(graphicsDropdown.value);
        
        // Устанавливаем язык
        LocalizationManager.Instance.SetLanguage((LocalizationManager.Language)languageDropdown.value);
    }
    
    private void LoadSettings()
    {
        if (PlayerPrefs.HasKey("PlayerNickname"))
        {
            nicknameInput.text = PlayerPrefs.GetString("PlayerNickname");
        }
        
        if (PlayerPrefs.HasKey("Language"))
        {
            languageDropdown.value = PlayerPrefs.GetInt("Language");
        }
        
        if (PlayerPrefs.HasKey("GraphicsQuality"))
        {
            graphicsDropdown.value = PlayerPrefs.GetInt("GraphicsQuality");
            QualitySettings.SetQualityLevel(graphicsDropdown.value);
        }
        
        LocalizationManager.Instance.LoadSettings();
    }
}
