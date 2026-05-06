using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SystemBreach.Enemies;
using SystemBreach.Core;

namespace SystemBreach.Towers
{
    // Handles targeting, firing, and damage for all attack-type towers.
    [RequireComponent(typeof(CircleCollider2D))]
    public class AttackTower : Tower
    {
        [Header("Attack")]
        [SerializeField] private FocusMode focusMode = FocusMode.Nearest;
        [SerializeField] private GameObject projectilePrefab; // optional — null = instant hit

        [Header("Visuals")]
        [SerializeField] private Transform turretPivot; // rotates to face target

        private CircleCollider2D rangeCollider;
        private float attackTimer = 0f;
        private readonly List<Enemy> enemiesInRange = new();

        public FocusMode Focus { get => focusMode; set => focusMode = value; }

        void Awake()
        {
            rangeCollider = GetComponent<CircleCollider2D>();
            rangeCollider.isTrigger = true;
        }

        protected override void OnLevelChanged()
        {
            if (rangeCollider != null)
                rangeCollider.radius = Range;
        }

        void Update()
        {
            if (GameManager.Instance == null || GameManager.Instance.State != GameState.Playing) return;

            attackTimer += Time.deltaTime;
            float attackInterval = Dexterity > 0f ? 1f / Dexterity : float.MaxValue;

            if (attackTimer >= attackInterval)
            {
                attackTimer = 0f;
                var target = SelectTarget();
                if (target != null)
                    Fire(target);
            }

            // Rotate turret toward best target
            if (turretPivot != null)
            {
                var t = SelectTarget();
                if (t != null)
                {
                    Vector3 dir = t.transform.position - turretPivot.position;
                    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    turretPivot.rotation = Quaternion.Euler(0f, 0f, angle);
                }
            }
        }

        private Enemy SelectTarget()
        {
            enemiesInRange.RemoveAll(e => e == null || !e.IsAlive);
            if (enemiesInRange.Count == 0) return null;

            return focusMode switch
            {
                FocusMode.Nearest  => enemiesInRange.OrderByDescending(e => e.PathProgress).FirstOrDefault(),
                FocusMode.Farthest => enemiesInRange.OrderBy(e => e.PathProgress).FirstOrDefault(),
                FocusMode.MostHP   => enemiesInRange.OrderByDescending(e => e.CurrentHP).FirstOrDefault(),
                FocusMode.LeastHP  => enemiesInRange.OrderBy(e => e.CurrentHP).FirstOrDefault(),
                FocusMode.Fastest  => enemiesInRange.OrderByDescending(e => e.MoveSpeed).FirstOrDefault(),
                _                  => enemiesInRange[0]
            };
        }

        private void Fire(Enemy target)
        {
            int dmg = CalculateDamage();

            if (projectilePrefab != null)
            {
                var go = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                var proj = go.GetComponent<Projectile>();
                proj?.Initialize(target, dmg);
            }
            else
            {
                // Instant-hit
                target.TakeDamage(dmg);
                EconomyManager.Instance?.AddScore(dmg);
            }
        }

        // Trigger callbacks track which enemies are in range
        void OnTriggerEnter2D(Collider2D other)
        {
            var enemy = other.GetComponent<Enemy>();
            if (enemy != null && !enemiesInRange.Contains(enemy))
                enemiesInRange.Add(enemy);
        }

        void OnTriggerExit2D(Collider2D other)
        {
            var enemy = other.GetComponent<Enemy>();
            if (enemy != null) enemiesInRange.Remove(enemy);
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, data != null ? Range : 1f);
        }
    }
}
