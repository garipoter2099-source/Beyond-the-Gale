using UnityEngine;
using Unity.Netcode;

public class Tornado : NetworkBehaviour
{
    [SerializeField] private GameConstants.TornadoCategory category = GameConstants.TornadoCategory.EFU;
    [SerializeField] private float windSpeed = 50f;
    [SerializeField] private float tornadoSize = 10f;
    [SerializeField] private float lifespan = GameConstants.TORNADO_LIFESPAN;
    
    private float elapsedTime = 0f;
    private Vector3 currentVelocity = Vector3.zero;
    private Vector3 targetPosition;
    private float moveSpeed = 15f;
    
    private NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>();
    private NetworkVariable<float> networkWindSpeed = new NetworkVariable<float>();
    private NetworkVariable<GameConstants.TornadoCategory> networkCategory = new NetworkVariable<GameConstants.TornadoCategory>();
    
    private Collider tornadoCollider;
    
    private void Start()
    {
        tornadoCollider = GetComponent<Collider>();
        if (tornadoCollider == null)
        {
            gameObject.AddComponent<SphereCollider>();
            tornadoCollider = GetComponent<Collider>();
        }
        
        // Устанавливаем размер на основе категории
        UpdateTornadoSize();
        
        // Случайная целевая позиция
        targetPosition = transform.position + Random.insideUnitSphere * 100f;
    }
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        if (IsServer)
        {
            networkCategory.Value = category;
            networkWindSpeed.Value = windSpeed;
            networkPosition.Value = transform.position;
        }
    }
    
    private void Update()
    {
        if (!IsServer) return;
        
        elapsedTime += Time.deltaTime;
        
        // Движение торнадо
        MoveTornado();
        
        // Синхронизация позиции
        networkPosition.Value = transform.position;
        
        // Проверка жизненного цикла
        if (elapsedTime >= lifespan)
        {
            DespawnTornadoRpc();
        }
    }
    
    /// <summary>
    /// Движение торнадо по карте
    /// </summary>
    private void MoveTornado()
    {
        // Если достигли целевой позиции, выбираем новую
        if (Vector3.Distance(transform.position, targetPosition) < 5f)
        {
            targetPosition = GetRandomLocationOnMap();
        }
        
        // Движемся к целевой позиции
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
    }
    
    /// <summary>
    /// Получает случайную позицию на карте
    /// </summary>
    private Vector3 GetRandomLocationOnMap()
    {
        // Пример: случайная позиция в диапазоне -500 до 500
        return new Vector3(
            Random.Range(-500f, 500f),
            transform.position.y,
            Random.Range(-500f, 500f)
        );
    }
    
    /// <summary>
    /// Обновляет размер торнадо в зависимости от категории
    /// </summary>
    private void UpdateTornadoSize()
    {
        float sizeMultiplier = 1f + ((int)category * GameConstants.EF_SCALE_MULTIPLIER);
        tornadoSize = GameConstants.TORNADO_MIN_SIZE * sizeMultiplier;
        
        // Масштабируем объект
        transform.localScale = Vector3.one * (tornadoSize / 10f);
        
        // Обновляем коллайдер
        if (tornadoCollider is SphereCollider sphereCollider)
        {
            sphereCollider.radius = tornadoSize / 2f;
        }
    }
    
    /// <summary>
    /// Получает мощность торнадо (для расчета награды)
    /// </summary>
    public float GetTornadoPower()
    {
        return 1f + ((int)category * 0.3f);
    }
    
    /// <summary>
    /// Получает ветровую скорость
    /// </summary>
    public float GetWindSpeed() => networkWindSpeed.Value;
    
    /// <summary>
    /// Получает категорию торнадо
    /// </summary>
    public GameConstants.TornadoCategory GetCategory() => networkCategory.Value;
    
    /// <summary>
    /// Получает оставшееся время жизни
    /// </summary>
    public float GetRemainingLifespan() => Mathf.Max(0, lifespan - elapsedTime);
    
    [Rpc(SendTo.Server)]
    private void DespawnTornadoRpc()
    {
        if (IsServer)
        {
            // Уведомляем клиентов
            NotifyTornadoDespawnedClientRpc();
            
            // Удаляем торнадо
            GetComponent<NetworkObject>().Despawn();
        }
    }
    
    [Rpc(SendTo.ClientsAndHost)]
    private void NotifyTornadoDespawnedClientRpc()
    {
        Debug.Log("Tornado despawned");
    }
}
