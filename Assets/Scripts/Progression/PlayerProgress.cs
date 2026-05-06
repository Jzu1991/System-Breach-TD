using UnityEngine;

namespace SystemBreach.Progression
{
    // Persistent player data: level unlock/completion, high scores, improvement coins.
    public class PlayerProgress : MonoBehaviour
    {
        public static PlayerProgress Instance { get; private set; }

        private const int TotalLevels = 10;

        // Improvement coins earned via score conversion
        public int ImprovementCoins { get; private set; }

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Load();
        }

        // --- Queries ---

        public bool IsLevelUnlocked(int levelIndex)
        {
            if (levelIndex == 0) return true; // level 1 always unlocked
            return PlayerPrefs.GetInt(UnlockedKey(levelIndex), 0) == 1;
        }

        public bool IsLevelCompleted(int levelIndex)
            => PlayerPrefs.GetInt(CompletedKey(levelIndex), 0) == 1;

        public int GetHighScore(int levelIndex)
            => PlayerPrefs.GetInt(ScoreKey(levelIndex), 0);

        // --- Mutations ---

        public void CompleteLevel(int levelIndex, int score)
        {
            // Mark completed
            PlayerPrefs.SetInt(CompletedKey(levelIndex), 1);

            // Update high score
            int prev = GetHighScore(levelIndex);
            if (score > prev)
                PlayerPrefs.SetInt(ScoreKey(levelIndex), score);

            // Unlock next level
            int next = levelIndex + 1;
            if (next < TotalLevels)
                PlayerPrefs.SetInt(UnlockedKey(next), 1);

            // Award improvement coins from score
            int coins = Mathf.FloorToInt(score * 0.1f);
            ImprovementCoins += coins;
            PlayerPrefs.SetInt("ImprovementCoins", ImprovementCoins);

            PlayerPrefs.Save();
        }

        public bool SpendImprovementCoins(int amount)
        {
            if (ImprovementCoins < amount) return false;
            ImprovementCoins -= amount;
            PlayerPrefs.SetInt("ImprovementCoins", ImprovementCoins);
            PlayerPrefs.Save();
            return true;
        }

        public void ResetAll()
        {
            PlayerPrefs.DeleteAll();
            ImprovementCoins = 0;
        }

        // --- Internal ---

        private void Load()
        {
            ImprovementCoins = PlayerPrefs.GetInt("ImprovementCoins", 0);
        }

        private static string UnlockedKey (int i) => $"Level_{i}_Unlocked";
        private static string CompletedKey(int i) => $"Level_{i}_Completed";
        private static string ScoreKey    (int i) => $"Level_{i}_HighScore";
    }
}
