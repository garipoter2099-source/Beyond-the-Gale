using UnityEngine;

public class TornadoDestruction : MonoBehaviour
{
    [SerializeField] private GameObject destructionPrefab;
    [SerializeField] private bool useRagdoll = false;
    [SerializeField] private float destructionForce = 500f;
    
    private Rigidbody cachedRigidbody;
    
    private void Start()
    {
        cachedRigidbody = GetComponent<Rigidbody>();
    }
    
    /// <summary>
    /// Уничтожает здание или дерево
    /// </summary>
    public void Destroy()
    {
        if (useRagdoll)
        {
            // Активируем Ragdoll на всех дочерних Rigidbodies
            ActivateRagdoll();
            
            // Применяем случайную силу
            ApplyDestructionForce();
        }
        else if (destructionPrefab != null)
        {
            // Спавним модель обломков
            Instantiate(destructionPrefab, transform.position, transform.rotation);
            Destroy(gameObject);
        }
        else
        {
            // Просто удаляем объект
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// Активирует Ragdoll эффект на объекте
    /// </summary>
    private void ActivateRagdoll()
    {
        // Отключаем основный Animator если есть
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.enabled = false;
        }
        
        // Включаем все Rigidbodies для физики
        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = false;
        }
        
        // Удаляем главный Rigidbody коллайдер если есть
        if (cachedRigidbody != null)
        {
            cachedRigidbody.isKinematic = true;
        }
    }
    
    /// <summary>
    /// Применяет силу для уничтожения
    /// </summary>
    private void ApplyDestructionForce()
    {
        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rigidbodies)
        {
            if (rb.isKinematic) continue;
            
            Vector3 randomDirection = Random.insideUnitSphere.normalized;
            rb.AddForce(randomDirection * destructionForce, ForceMode.Impulse);
        }
    }
}
