using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SystemBreach.Data;

namespace SystemBreach.Core
{
    public class WaveManager : MonoBehaviour
    {
        public static WaveManager Instance { get; private set; }

        // Events
        public static event Action<int, int> OnWaveStarted;   // (waveIndex, totalWaves)
        public static event Action<int>      OnWaveCompleted; // (waveIndex)
        public static event Action           OnAllWavesCompleted;

        [Header("Spawn")]
        public Transform spawnPoint;

        [Header("State")]
        [SerializeField] private int currentWaveIndex = -1;
        [SerializeField] private int activeEnemyCount = 0;
        [SerializeField] private bool waveInProgress   = false;
        [SerializeField] private bool autoSkipWave     = false;

        private WaveData[]      waves;
        private List<Enemies.Enemy> aliveEnemies = new();

        public int  CurrentWaveIndex => currentWaveIndex;
        public int  TotalWaves       => waves?.Length ?? 0;
        public bool WaveInProgress   => waveInProgress;
        public bool AutoSkipWave     { get => autoSkipWave; set => autoSkipWave = value; }

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        void OnEnable()
        {
            Enemies.Enemy.OnEnemyDied    += HandleEnemyDied;
            Enemies.Enemy.OnEnemyReached += HandleEnemyReached;
            GameManager.OnStateChanged   += HandleStateChanged;
        }

        void OnDisable()
        {
            Enemies.Enemy.OnEnemyDied    -= HandleEnemyDied;
            Enemies.Enemy.OnEnemyReached -= HandleEnemyReached;
            GameManager.OnStateChanged   -= HandleStateChanged;
        }

        public void Initialize(WaveData[] levelWaves)
        {
            waves = levelWaves;
            currentWaveIndex = -1;
            waveInProgress   = false;
            activeEnemyCount = 0;
            aliveEnemies.Clear();
        }

        // Called by HUD button or auto-skip
        public void StartNextWave()
        {
            if (waveInProgress) return;
            if (currentWaveIndex + 1 >= waves.Length) return;
            if (GameManager.Instance.State != GameState.Playing &&
                GameManager.Instance.State != GameState.Idle) return;

            GameManager.Instance.StartGame();
            currentWaveIndex++;
            StartCoroutine(SpawnWave(waves[currentWaveIndex]));
        }

        // Player manually triggers early wave — grants bonus money
        public void TriggerEarlyWave()
        {
            if (waveInProgress) return;
            EconomyManager.Instance?.AddMoney(GameManager.Instance.currentLevel.earlyWaveBonus);
            StartNextWave();
        }

        private IEnumerator SpawnWave(WaveData wave)
        {
            waveInProgress = true;
            activeEnemyCount = 0;
            OnWaveStarted?.Invoke(currentWaveIndex, waves.Length);

            // Farm towers generate money at wave start
            Towers.FarmTower.NotifyWaveStart();

            foreach (var group in wave.spawnGroups)
            {
                if (group.delayBefore > 0f) yield return new WaitForSeconds(group.delayBefore);
                if (group.enemyData?.prefab == null) continue;

                for (int i = 0; i < group.count; i++)
                {
                    SpawnEnemy(group.enemyData);
                    if (i < group.count - 1) yield return new WaitForSeconds(group.spawnInterval);
                }
            }

            // Wait until all enemies are gone
            yield return new WaitUntil(() => activeEnemyCount <= 0);

            waveInProgress = false;
            OnWaveCompleted?.Invoke(currentWaveIndex);

            if (currentWaveIndex + 1 >= waves.Length)
            {
                OnAllWavesCompleted?.Invoke();
                GameManager.Instance.TriggerVictory();
            }
            else if (autoSkipWave)
            {
                yield return new WaitForSeconds(1f);
                StartNextWave();
            }
        }

        private void SpawnEnemy(EnemyData data)
        {
            if (spawnPoint == null) return;
            var go = Instantiate(data.prefab, spawnPoint.position, Quaternion.identity);
            var enemy = go.GetComponent<Enemies.Enemy>();
            if (enemy == null) return;

            int levelIdx = GameManager.Instance.currentLevel.levelIndex;
            bool isBoss  = data.enemyType == EnemyType.Boss;
            enemy.Initialize(data, levelIdx, isBoss);
            aliveEnemies.Add(enemy);
            activeEnemyCount++;
        }

        private void HandleEnemyDied(Enemies.Enemy enemy)
        {
            aliveEnemies.Remove(enemy);
            activeEnemyCount = Mathf.Max(0, activeEnemyCount - 1);
        }

        private void HandleEnemyReached(Enemies.Enemy enemy)
        {
            aliveEnemies.Remove(enemy);
            activeEnemyCount = Mathf.Max(0, activeEnemyCount - 1);
        }

        private void HandleStateChanged(GameState state)
        {
            if (state == GameState.Defeat || state == GameState.Victory)
                StopAllCoroutines();
        }
    }
}
