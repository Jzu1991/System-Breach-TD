using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using SystemBreach.Data;

namespace SystemBreach.Core
{
    public enum GameState { Idle, Playing, Paused, Victory, Defeat }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        // Events
        public static event Action<float, float> OnPCHealthChanged; // (current, max)
        public static event Action               OnGameOver;
        public static event Action               OnVictory;
        public static event Action<GameState>    OnStateChanged;

        [Header("Level Config")]
        public LevelData currentLevel;

        [Header("Runtime State")]
        [SerializeField] private float pcCurrentHP;
        [SerializeField] private float pcMaxHP;
        [SerializeField] private GameState gameState = GameState.Idle;

        public float PCCurrentHP => pcCurrentHP;
        public float PCMaxHP     => pcMaxHP;
        public GameState State   => gameState;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        void Start()
        {
            if (currentLevel != null)
                InitLevel(currentLevel);
        }

        public void InitLevel(LevelData level)
        {
            currentLevel = level;
            pcMaxHP      = level.pcMaxHP;
            pcCurrentHP  = pcMaxHP;
            SetState(GameState.Idle);
            OnPCHealthChanged?.Invoke(pcCurrentHP, pcMaxHP);
        }

        public void StartGame()
        {
            if (gameState == GameState.Idle || gameState == GameState.Paused)
                SetState(GameState.Playing);
        }

        public void PauseGame()
        {
            if (gameState == GameState.Playing)
            {
                Time.timeScale = 0f;
                SetState(GameState.Paused);
            }
        }

        public void ResumeGame()
        {
            if (gameState == GameState.Paused)
            {
                Time.timeScale = 1f;
                SetState(GameState.Playing);
            }
        }

        // Called by enemies that reach the computer
        public void DamagePC(float amount)
        {
            if (gameState != GameState.Playing) return;

            pcCurrentHP = Mathf.Max(0f, pcCurrentHP - amount);
            OnPCHealthChanged?.Invoke(pcCurrentHP, pcMaxHP);

            EconomyManager.Instance?.AddScore(-(int)amount); // score penalty

            if (pcCurrentHP <= 0f)
                TriggerDefeat();
        }

        public void TriggerVictory()
        {
            if (gameState == GameState.Victory || gameState == GameState.Defeat) return;
            SetState(GameState.Victory);
            OnVictory?.Invoke();
            Progression.PlayerProgress.Instance?.CompleteLevel(
                currentLevel.levelIndex,
                EconomyManager.Instance?.Score ?? 0);
        }

        public void TriggerDefeat()
        {
            if (gameState == GameState.Victory || gameState == GameState.Defeat) return;
            SetState(GameState.Defeat);
            OnGameOver?.Invoke();
        }

        public void ReturnToMainMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
        }

        public void RestartLevel()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void SetState(GameState newState)
        {
            gameState = newState;
            OnStateChanged?.Invoke(newState);
        }
    }
}
