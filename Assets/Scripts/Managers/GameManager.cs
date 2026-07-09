using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    private static GameManager instance;
    
    [SerializeField] private float gameStartDelay = 2f;
    [SerializeField] private float maxGameDuration = 3600f; // 1 час
    
    private float gameStartTime = 0f;
    private bool gameStarted = false;
    
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("GameManager");
                    instance = go.AddComponent<GameManager>();
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
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        if (IsServer)
        {
            StartGameRpc();
        }
    }
    
    private void Update()
    {
        if (!IsServer) return;
        
        if (gameStarted)
        {
            float elapsedTime = Time.time - gameStartTime;
            
            // Проверяем достижение максимальной длительности игры
            if (elapsedTime >= maxGameDuration)
            {
                EndGameRpc();
            }
        }
    }
    
    /// <summary>
    /// Инициирует начало игры
    /// </summary>
    [Rpc(SendTo.Server)]
    private void StartGameRpc()
    {
        if (!IsServer) return;
        
        gameStartTime = Time.time;
        gameStarted = true;
        
        NotifyGameStartedClientRpc();
        Debug.Log("Game started!");
    }
    
    /// <summary>
    /// Завершает игровую сессию
    /// </summary>
    [Rpc(SendTo.Server)]
    private void EndGameRpc()
    {
        if (!IsServer) return;
        
        gameStarted = false;
        NotifyGameEndedClientRpc();
        Debug.Log("Game ended!");
    }
    
    /// <summary>
    /// Возвращает время игры с начала
    /// </summary>
    public float GetGameElapsedTime()
    {
        if (gameStarted)
        {
            return Time.time - gameStartTime;
        }
        return 0f;
    }
    
    /// <summary>
    /// Проверяет, запущена ли игра
    /// </summary>
    public bool IsGameStarted() => gameStarted;
    
    [Rpc(SendTo.ClientsAndHost)]
    private void NotifyGameStartedClientRpc()
    {
        Debug.Log("[CLIENT] Game started!");
    }
    
    [Rpc(SendTo.ClientsAndHost)]
    private void NotifyGameEndedClientRpc()
    {
        Debug.Log("[CLIENT] Game ended!");
    }
}
