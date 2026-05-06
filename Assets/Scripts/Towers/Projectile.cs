using UnityEngine;
using SystemBreach.Enemies;
using SystemBreach.Core;

namespace SystemBreach.Towers
{
    // Simple homing projectile — follows target until hit or target dies.
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float speed = 10f;
        [SerializeField] private float lifetime = 5f;

        private Enemy target;
        private int   damage;

        public void Initialize(Enemy enemy, int dmg)
        {
            target = enemy;
            damage = dmg;
            Destroy(gameObject, lifetime);
        }

        void Update()
        {
            if (target == null || !target.IsAlive)
            {
                Destroy(gameObject);
                return;
            }

            Vector3 dir = (target.transform.position - transform.position).normalized;
            transform.position += dir * speed * Time.deltaTime;

            float dist = Vector3.Distance(transform.position, target.transform.position);
            if (dist < 0.15f)
                HitTarget();
        }

        private void HitTarget()
        {
            if (target != null && target.IsAlive)
            {
                target.TakeDamage(damage);
                EconomyManager.Instance?.AddScore(damage);
            }
            Destroy(gameObject);
        }
    }
}
