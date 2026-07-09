using UnityEngine;
using TMPro;
using Unity.Netcode;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    
    [SerializeField] private Canvas mainHUD;
    [SerializeField] private Canvas pauseMenu;
    [SerializeField] private Canvas dialogBox;
    
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private Button dialogConfirmButton;
    [SerializeField] private Button dialogCancelButton;
    
    private bool isPaused = false;
    
    public static UIManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UIManager>();
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
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        dialogConfirmButton.onClick.AddListener(CloseDialog);
        dialogCancelButton.onClick.AddListener(CloseDialog);
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }
    
    private void TogglePauseMenu()
    {
        isPaused = !isPaused;
        pauseMenu.enabled = isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
    }
    
    public void ShowDialog(string message)
    {
        dialogText.text = message;
        dialogBox.enabled = true;
    }
    
    public void CloseDialog()
    {
        dialogBox.enabled = false;
    }
}
