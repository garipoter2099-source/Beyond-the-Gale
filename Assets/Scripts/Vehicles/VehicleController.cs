using UnityEngine;
using Unity.Netcode;

public class VehicleController : NetworkBehaviour
{
    [SerializeField] private float acceleration = GameConstants.VEHICLE_ACCELERATION;
    [SerializeField] private float maxSpeed = GameConstants.VEHICLE_BASE_SPEED;
    [SerializeField] private float brakeForce = GameConstants.VEHICLE_BRAKE_FORCE;
    [SerializeField] private float handbrakeForce = GameConstants.VEHICLE_HANDBRAKE_FORCE;
    [SerializeField] private float turnSpeed = 5f;
    
    private Rigidbody vehicleRigidbody;
    private float currentSpeed = 0f;
    private float currentTurnInput = 0f;
    private bool isDeployActive = false;
    private bool isHandbrakeActive = false;
    
    private NetworkVariable<Vector3> networkVelocity = new NetworkVariable<Vector3>();
    private NetworkVariable<bool> networkDeployStatus = new NetworkVariable<bool>();
    private NetworkVariable<int> networkLeftTurnSignal = new NetworkVariable<int>(0);
    private NetworkVariable<int> networkRightTurnSignal = new NetworkVariable<int>(0);
    private NetworkVariable<int> networkEmergencySignal = new NetworkVariable<int>(0);
    
    private float rewardTimer = 0f;
    private bool isInTornado = false;
    private Tornado currentTornado = null;
    
    private void Start()
    {
        vehicleRigidbody = GetComponent<Rigidbody>();
        if (vehicleRigidbody == null)
        {
            vehicleRigidbody = gameObject.AddComponent<Rigidbody>();
            vehicleRigidbody.mass = 2000f;
            vehicleRigidbody.drag = 0.5f;
            vehicleRigidbody.angularDrag = 0.5f;
        }
        
        gameObject.tag = "Vehicle";
    }
    
    private void Update()
    {
        if (!IsOwner) return;
        
        HandleInput();
        HandleTurnsignals();
    }
    
    private void FixedUpdate()
    {
        if (!IsOwner) return;
        
        ApplyMovement();
        CheckTornadoProximity();
        UpdateReward();
        
        // Синхронизация
        networkVelocity.Value = vehicleRigidbody.velocity;
        networkDeployStatus.Value = isDeployActive;
    }
    
    /// <summary>
    /// Обрабатывает ввод от игрока
    /// </summary>
    private void HandleInput()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        currentTurnInput = horizontal;
        
        // Движение
        if (vertical > 0)
        {
            currentSpeed = Mathf.Min(currentSpeed + acceleration * Time.deltaTime, maxSpeed);
        }
        else if (vertical < 0)
        {
            currentSpeed = Mathf.Max(currentSpeed - brakeForce * Time.deltaTime, -maxSpeed * 0.5f);
        }
        else
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0, Time.deltaTime * 5f);
        }
        
        // Ручной тормоз (E)
        if (Input.GetKeyDown(KeyCode.E))
        {
            isHandbrakeActive = !isHandbrakeActive;
        }
        
        // Деплой (Q)
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ToggleDeploy();
        }
        
        // Вход/Выход из машины (F)
        if (Input.GetKeyDown(KeyCode.F))
        {
            HandleEnterExit();
        }
    }
    
    /// <summary>
    /// Обрабатывает поворотники и аварийку
    /// </summary>
    private void HandleTurnsignals()
    {
        // Левый поворотник (1)
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            networkLeftTurnSignal.Value = (networkLeftTurnSignal.Value + 1) % 2;
            UpdateTurnSignalRpc(0, networkLeftTurnSignal.Value);
        }
        
        // Правый поворотник (2)
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            networkRightTurnSignal.Value = (networkRightTurnSignal.Value + 1) % 2;
            UpdateTurnSignalRpc(1, networkRightTurnSignal.Value);
        }
        
        // Аварийка (3)
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            networkEmergencySignal.Value = (networkEmergencySignal.Value + 1) % 2;
            UpdateTurnSignalRpc(2, networkEmergencySignal.Value);
        }
    }
    
    /// <summary>
    /// Применяет движение к машине
    /// </summary>
    private void ApplyMovement()
    {
        if (isDeployActive && currentSpeed != 0)
        {
            // Деплой активен - машина зафиксирована
            vehicleRigidbody.velocity = Vector3.zero;
            vehicleRigidbody.angularVelocity = Vector3.zero;
            return;
        }
        
        // Движение вперед/назад
        Vector3 moveDirection = transform.forward * currentSpeed;
        vehicleRigidbody.velocity = new Vector3(moveDirection.x, vehicleRigidbody.velocity.y, moveDirection.z);
        
        // Поворот
        float rotation = currentTurnInput * turnSpeed;
        transform.Rotate(0, rotation, 0);
        
        // Ручной тормоз
        if (isHandbrakeActive)
        {
            vehicleRigidbody.velocity *= 0.8f;
        }
    }
    
    /// <summary>
    /// Проверяет близость к торнадо
    /// </summary>
    private void CheckTornadoProximity()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, GameConstants.TORNADO_DETECTION_RADIUS);
        
        isInTornado = false;
        currentTornado = null;
        
        foreach (Collider col in colliders)
        {
            var tornado = col.GetComponent<Tornado>();
            if (tornado != null)
            {
                isInTornado = true;
                currentTornado = tornado;
                break;
            }
        }
    }
    
    /// <summary>
    /// Обновляет награду за перехват торнадо
    /// </summary>
    private void UpdateReward()
    {
        if (!isInTornado || currentTornado == null)
        {
            rewardTimer = 0f;
            return;
        }
        
        rewardTimer += Time.deltaTime;
        
        if (rewardTimer >= GameConstants.REWARD_INTERVAL)
        {
            // Рассчитываем награду на основе мощности торнадо
            float power = currentTornado.GetTornadoPower();
            float reward = Mathf.Lerp(GameConstants.REWARD_MIN, GameConstants.REWARD_MAX, power);
            
            // Отправляем награду игроку
            ClaimRewardRpc(reward);
            rewardTimer = 0f;
        }
    }
    
    /// <summary>
    /// Переключает статус деплоя
    /// </summary>
    private void ToggleDeploy()
    {
        isDeployActive = !isDeployActive;
        ToggleDeployRpc(isDeployActive);
    }
    
    /// <summary>
    /// Проверяет, активирован ли деплой
    /// </summary>
    public bool IsDeployActive() => isDeployActive;
    
    /// <summary>
    /// Обработка входа/выхода из машины
    /// </summary>
    private void HandleEnterExit()
    {
        // Здесь можно добавить логику для входа/выхода из машины
        Debug.Log("Player entered/exited vehicle");
    }
    
    // ========== RPC CALLS ==========
    
    [Rpc(SendTo.Server)]
    private void ClaimRewardRpc(float amount)
    {
        var playerData = PlayerDataManager.Instance;
        if (playerData != null)
        {
            playerData.AddBalance(amount);
            
            // Уведомляем всех клиентов
            NotifyRewardClaimedClientRpc(OwnerClientId, amount);
        }
    }
    
    [Rpc(SendTo.ClientsAndHost)]
    private void ToggleDeployRpc(bool state)
    {
        Debug.Log($"Deploy toggled: {state}");
    }
    
    [Rpc(SendTo.ClientsAndHost)]
    private void UpdateTurnSignalRpc(int signalType, int state)
    {
        // 0 = Left, 1 = Right, 2 = Emergency
        Debug.Log($"Turn signal {signalType} set to {state}");
    }
    
    [Rpc(SendTo.ClientsAndHost)]
    private void NotifyRewardClaimedClientRpc(ulong playerId, float amount)
    {
        Debug.Log($"[CLIENT] Player {playerId} claimed reward: ${amount}");
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, GameConstants.TORNADO_DETECTION_RADIUS);
    }
}
