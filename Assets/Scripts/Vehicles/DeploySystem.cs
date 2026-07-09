using UnityEngine;
using Unity.Netcode;

public class DeploySystem : NetworkBehaviour
{
    [SerializeField] private Transform deployAnchor; // Точка крепления юбки
    [SerializeField] private float deploySpeed = 1f;
    [SerializeField] private float retractSpeed = 1f;
    
    private bool isDeployed = false;
    private float deployProgress = 0f;
    private Rigidbody vehicleRigidbody;
    
    private NetworkVariable<bool> networkDeployState = new NetworkVariable<bool>();
    
    private void Start()
    {
        vehicleRigidbody = GetComponent<Rigidbody>();
        if (vehicleRigidbody == null)
        {
            vehicleRigidbody = gameObject.AddComponent<Rigidbody>();
        }
    }
    
    private void Update()
    {
        if (!IsOwner) return;
        
        UpdateDeployAnimation();
    }
    
    /// <summary>
    /// Активирует деплой (опускает юбку)
    /// </summary>
    public void ActivateDeploy()
    {
        if (!IsOwner) return;
        
        isDeployed = true;
        ActivateDeployRpc();
    }
    
    /// <summary>
    /// Деактивирует деплой (поднимает юбку)
    /// </summary>
    public void RetractDeploy()
    {
        if (!IsOwner) return;
        
        isDeployed = false;
        RetractDeployRpc();
    }
    
    /// <summary>
    /// Обновляет анимацию деплоя
    /// </summary>
    private void UpdateDeployAnimation()
    {
        if (isDeployed)
        {
            deployProgress = Mathf.Min(deployProgress + deploySpeed * Time.deltaTime, 1f);
        }
        else
        {
            deployProgress = Mathf.Max(deployProgress - retractSpeed * Time.deltaTime, 0f);
        }
        
        // Применяем визуальные изменения
        if (deployAnchor != null)
        {
            // Опускаем якорь юбки
            deployAnchor.localPosition = Vector3.Lerp(
                Vector3.zero,
                new Vector3(0, -2f, 0),
                deployProgress
            );
        }
        
        // Если юбка активна, закрепляем машину
        if (isDeployed && deployProgress > 0.5f)
        {
            // Применяем огромный вес для фиксации
            vehicleRigidbody.mass = 10000f; // Гидравлическая система держит
        }
        else
        {
            vehicleRigidbody.mass = 2000f; // Обычный вес
        }
    }
    
    // ========== RPC CALLS ==========
    
    [Rpc(SendTo.Server)]
    private void ActivateDeployRpc()
    {
        networkDeployState.Value = true;
        NotifyDeployStateChangedClientRpc(true);
    }
    
    [Rpc(SendTo.Server)]
    private void RetractDeployRpc()
    {
        networkDeployState.Value = false;
        NotifyDeployStateChangedClientRpc(false);
    }
    
    [Rpc(SendTo.ClientsAndHost)]
    private void NotifyDeployStateChangedClientRpc(bool state)
    {
        Debug.Log($"Deploy state changed to: {state}");
    }
}
