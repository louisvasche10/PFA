using UnityEngine;
namespace PFACombat
{
    public class PistolBullet : MonoBehaviour
    {
        Vector3 direction;
        float speed = 65f;
        float life = 3f;
        public void Initialize(Vector3 travelDirection)
        {
            direction = travelDirection.normalized;
        }
        void Update()
        {
            float distance = speed * Time.deltaTime;
            if (Physics.SphereCast(transform.position, 0.045f, direction, out RaycastHit hit, distance, ~(1 << 2), QueryTriggerInteraction.Ignore))
            {
                MeleeEnemy enemy = hit.collider.GetComponentInParent<MeleeEnemy>();
                if (enemy != null) enemy.TakeDamage();
                Destroy(gameObject);
                return;
            }
            transform.position += direction * distance;
            life -= Time.deltaTime;
            if (life <= 0) Destroy(gameObject);
        }
    }
}
