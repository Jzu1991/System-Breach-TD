using System;
using UnityEngine;
using SystemBreach.Data;
using SystemBreach.Core;
using SystemBreach.Map;

namespace SystemBreach.Enemies
{
    public class Enemy : MonoBehaviour
    {
        // Static events consumed by WaveManager and others
        public static event Action<Enemy> OnEnemyDied;
        public static event Action<Enemy> OnEnemyReached; // reached the computer

        [Header("Runtime (read-only in inspector)")]
        [SerializeField] private float currentHP;
        [SerializeField] private float maxHP;
        [SerializeField] private float moveSpeed;
        [SerializeField] private int   killReward;
        [SerializeField] private EnemyType enemyType;

        // Path traversal
        private Transform[] waypoints;
        private int         waypointIndex = 0;
        private float       pathProgress  = 0f; // 0=start, 1=end

        private bool isAlive = true;

        public float     CurrentHP    => currentHP;
        public float     MaxHP        => maxHP;
        public float     MoveSpeed    => moveSpeed;
        public EnemyType EnemyType    => enemyType;
        public bool      IsAlive      => isAlive;
        public float     PathProgress => pathProgress; // used by targeting (Nearest = highest progress)

        // Health bar (assign in prefab)
        [SerializeField] private UnityEngine.UI.Slider healthBar;

        public void Initialize(EnemyData data, int levelIndex, bool isBoss)
        {
            enemyType  = data.enemyType;
            maxHP      = data.GetHP(levelIndex, isBoss);
            currentHP  = maxHP;
            moveSpeed  = data.moveSpeed;
            killReward = data.killReward;
            isAlive    = true;
            waypointIndex = 0;
            pathProgress  = 0f;

            if (healthBar != null) healthBar.value = 1f;

            // Fetch waypoints from the scene path
            if (WaypointPath.Instance != null)
                waypoints = WaypointPath.Instance.Waypoints;

            // Position at spawn waypoint
            if (waypoints != null && waypoints.Length > 0)
                transform.position = waypoints[0].position;
        }

        void Update()
        {
            if (!isAlive) return;
            if (GameManager.Instance == null || GameManager.Instance.State != GameState.Playing) return;

            MoveAlongPath();
        }

        private void MoveAlongPath()
        {
            if (waypoints == null || waypointIndex >= waypoints.Length) return;

            Transform target = waypoints[waypointIndex];
            Vector3   dir    = (target.position - transform.position).normalized;
            transform.position += dir * moveSpeed * Time.deltaTime;

            // Update path progress (0-1 across total waypoints)
            pathProgress = (float)waypointIndex / Mathf.Max(1, waypoints.Length - 1);

            // Rotate to face movement direction (2D top-down)
            if (dir != Vector3.zero)
            {
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
                transform.rotation = Quaternion.Euler(0f, 0f, angle);
            }

            float distToNext = Vector3.Distance(transform.position, target.position);
            if (distToNext < 0.1f)
            {
                waypointIndex++;
                if (waypointIndex >= waypoints.Length)
                    ReachComputer();
            }
        }

        public void TakeDamage(float amount)
        {
            if (!isAlive) return;
            currentHP -= amount;

            if (healthBar != null)
                healthBar.value = currentHP / maxHP;

            if (currentHP <= 0f)
                Die();
        }

        private void Die()
        {
            isAlive = false;
            EconomyManager.Instance?.AddMoney(killReward);
            EconomyManager.Instance?.AddScore(killReward * 2);
            AudioManager.Instance?.PlaySFX(AudioManager.Instance.enemyDeath);
            OnEnemyDied?.Invoke(this);
            Destroy(gameObject, 0.1f);
        }

        private void ReachComputer()
        {
            isAlive = false;
            // Damage PC by remaining HP
            GameManager.Instance?.DamagePC(currentHP);
            AudioManager.Instance?.PlaySFX(AudioManager.Instance.pcDamage);
            OnEnemyReached?.Invoke(this);
            Destroy(gameObject, 0.05f);
        }
    }
}
