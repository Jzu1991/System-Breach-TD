using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SystemBreach.Core;
using SystemBreach.Map;

namespace SystemBreach.UI
{
    // In-game HUD: PC HP, wave counter, money display, wave start button.
    public class GameHUD : MonoBehaviour
    {
        [Header("PC Health")]
        [SerializeField] private Slider   pcHealthBar;
        [SerializeField] private TMP_Text pcHealthText;

        [Header("Wave Info")]
        [SerializeField] private TMP_Text waveCounterText;
        [SerializeField] private Button   nextWaveButton;
        [SerializeField] private TMP_Text nextWaveButtonText;

        [Header("Economy")]
        [SerializeField] private TMP_Text moneyText;
        [SerializeField] private TMP_Text scoreText;

        [Header("Panels")]
        [SerializeField] private GameObject towerShopPanel;
        [SerializeField] private GameObject towerInfoPanel;
        [SerializeField] private GameObject pausePanel;

        void OnEnable()
        {
            GameManager.OnPCHealthChanged += UpdatePCHealth;
            GameManager.OnStateChanged    += HandleStateChanged;
            EconomyManager.OnMoneyChanged += UpdateMoney;
            EconomyManager.OnScoreChanged += UpdateScore;
            WaveManager.OnWaveStarted     += HandleWaveStarted;
            WaveManager.OnWaveCompleted   += HandleWaveCompleted;
            GridManager.OnTowerSelected   += _ => ShowTowerInfo();
            GridManager.OnTowerDeselected += HideTowerInfo;
        }

        void OnDisable()
        {
            GameManager.OnPCHealthChanged -= UpdatePCHealth;
            GameManager.OnStateChanged    -= HandleStateChanged;
            EconomyManager.OnMoneyChanged -= UpdateMoney;
            EconomyManager.OnScoreChanged -= UpdateScore;
            WaveManager.OnWaveStarted     -= HandleWaveStarted;
            WaveManager.OnWaveCompleted   -= HandleWaveCompleted;
        }

        void Start()
        {
            if (nextWaveButton != null)
                nextWaveButton.onClick.AddListener(OnNextWavePressed);

            HideTowerInfo();
            if (pausePanel != null) pausePanel.SetActive(false);

            // Refresh initial state
            if (GameManager.Instance != null)
                UpdatePCHealth(GameManager.Instance.PCCurrentHP, GameManager.Instance.PCMaxHP);
            if (EconomyManager.Instance != null)
            {
                UpdateMoney(EconomyManager.Instance.Money);
                UpdateScore(EconomyManager.Instance.Score);
            }
            UpdateWaveCounter();
        }

        private void UpdatePCHealth(float current, float max)
        {
            if (pcHealthBar != null)
                pcHealthBar.value = max > 0 ? current / max : 0f;
            if (pcHealthText != null)
                pcHealthText.text = $"{Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";
        }

        private void UpdateMoney(int money)
        {
            if (moneyText != null) moneyText.text = $"${money}";
        }

        private void UpdateScore(int score)
        {
            if (scoreText != null) scoreText.text = $"{score} pts";
        }

        private void HandleWaveStarted(int waveIdx, int total)
        {
            UpdateWaveCounter();
            SetNextWaveButtonInteractable(false);
        }

        private void HandleWaveCompleted(int waveIdx)
        {
            UpdateWaveCounter();
            bool hasMore = waveIdx + 1 < (WaveManager.Instance?.TotalWaves ?? 0);
            SetNextWaveButtonInteractable(hasMore);
        }

        private void UpdateWaveCounter()
        {
            if (waveCounterText == null || WaveManager.Instance == null) return;
            int cur   = WaveManager.Instance.CurrentWaveIndex + 1;
            int total = WaveManager.Instance.TotalWaves;
            waveCounterText.text = $"Wave {Mathf.Max(cur, 0)} / {total}";
        }

        private void SetNextWaveButtonInteractable(bool state)
        {
            if (nextWaveButton != null) nextWaveButton.interactable = state;
        }

        private void OnNextWavePressed()
        {
            if (WaveManager.Instance == null) return;

            if (!WaveManager.Instance.WaveInProgress)
                WaveManager.Instance.TriggerEarlyWave();
        }

        private void HandleStateChanged(GameState state)
        {
            if (state == GameState.Idle)
                SetNextWaveButtonInteractable(true);
        }

        private void ShowTowerInfo()
        {
            if (towerInfoPanel != null) towerInfoPanel.SetActive(true);
            if (towerShopPanel != null) towerShopPanel.SetActive(false);
        }

        private void HideTowerInfo()
        {
            if (towerInfoPanel != null) towerInfoPanel.SetActive(false);
            if (towerShopPanel != null) towerShopPanel.SetActive(true);
        }

        public void TogglePause()
        {
            if (GameManager.Instance == null) return;
            if (GameManager.Instance.State == GameState.Playing)
            {
                GameManager.Instance.PauseGame();
                if (pausePanel != null) pausePanel.SetActive(true);
            }
            else if (GameManager.Instance.State == GameState.Paused)
            {
                GameManager.Instance.ResumeGame();
                if (pausePanel != null) pausePanel.SetActive(false);
            }
        }

        public void ReturnToMenu() => GameManager.Instance?.ReturnToMainMenu();
    }
}
