using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SystemBreach.Core;

namespace SystemBreach.UI
{
    // Shown on victory or defeat.
    public class GameOverPanel : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private TMP_Text   titleText;
        [SerializeField] private TMP_Text   scoreText;
        [SerializeField] private TMP_Text   coinsEarnedText;
        [SerializeField] private Button     retryButton;
        [SerializeField] private Button     menuButton;

        void OnEnable()
        {
            GameManager.OnVictory  += ShowVictory;
            GameManager.OnGameOver += ShowDefeat;
        }

        void OnDisable()
        {
            GameManager.OnVictory  -= ShowVictory;
            GameManager.OnGameOver -= ShowDefeat;
        }

        void Start()
        {
            if (panel != null) panel.SetActive(false);
            if (retryButton != null) retryButton.onClick.AddListener(() => GameManager.Instance?.RestartLevel());
            if (menuButton  != null) menuButton.onClick.AddListener(() => GameManager.Instance?.ReturnToMainMenu());
        }

        private void ShowVictory()
        {
            Show("SYSTEM SECURED", true);
        }

        private void ShowDefeat()
        {
            Show("SYSTEM BREACHED", false);
        }

        private void Show(string title, bool isVictory)
        {
            if (panel != null) panel.SetActive(true);
            if (titleText != null)
            {
                titleText.text  = title;
                titleText.color = isVictory ? Color.green : Color.red;
            }

            int score  = EconomyManager.Instance?.Score ?? 0;
            int coins  = EconomyManager.Instance?.GetImprovementCoins() ?? 0;

            if (scoreText      != null) scoreText.text      = $"Score: {score}";
            if (coinsEarnedText != null) coinsEarnedText.text = $"+{coins} Improvement Coins";
        }
    }
}
