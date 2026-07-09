using UnityEngine;
using Unity.Netcode;

public class TornadoPhysics : MonoBehaviour
{
    [SerializeField] private Tornado tornadoScript;
    [SerializeField] private float pullForceMultiplier = GameConstants.BASE_PULL_FORCE;
    [SerializeField] private float pullRadius = GameConstants.TORNADO_DETECTION_RADIUS;
    
    private Collider tornadoCollider;
    
    private void Start()
    {
        tornadoScript = GetComponent<Tornado>();
        tornadoCollider = GetComponent<Collider>();
    }
    
    private void FixedUpdate()
    {
        // Применяем силу к объектам в радиусе торнадо
        ApplyTornadoForce();
    }
    
    /// <summary>
    /// Применяет подъемную силу к объектам вблизи торнадо
    /// </summary>
    private void ApplyTornadoForce()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, pullRadius);
        
        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Vehicle"))
            {
                // Проверяем, активирован ли деплой
                var vehicleController = col.GetComponent<VehicleController>();
                if (vehicleController != null && vehicleController.IsDeployActive())
                {
                    // Машина защищена
                    continue;
                }
                
                // Применяем силу
                Rigidbody rb = col.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    ApplyUpwardForce(rb);
                }
            }
            else if (col.CompareTag("Building") || col.CompareTag("Tree"))
            {
                // Уничтожаем строения
                var destruction = col.GetComponent<TornadoDestruction>();
                if (destruction != null)
                {
                    destruction.Destroy();
                }
            }
        }
    }
    
    /// <summary>
    /// Применяет вертикальную силу к объекту
    /// </summary>
    private void ApplyUpwardForce(Rigidbody rb)
    {
        float distance = Vector3.Distance(rb.position, transform.position);
        float force = pullForceMultiplier * tornadoScript.GetTornadoPower() * (1f - (distance / pullRadius));
        
        Vector3 forceDirection = (transform.position - rb.position).normalized + Vector3.up;
        rb.AddForce(forceDirection * force, ForceMode.Force);
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, pullRadius);
    }
}
