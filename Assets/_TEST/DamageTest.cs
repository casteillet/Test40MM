using UnityEngine;

public class DamageTest : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private AmmoDefinition ammoDefinition;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Fire();
        }
    }

    private void Fire()
    {
        var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out var hit)) return;
        
        var damageable = hit.collider.GetComponentInParent<IDamageable>();
        damageable.TakeDamage(ammoDefinition);

        Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.red, 1f);
    }
}