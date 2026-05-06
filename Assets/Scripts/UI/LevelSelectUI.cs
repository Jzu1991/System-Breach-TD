using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SystemBreach.Data;
using SystemBreach.Progression;

namespace SystemBreach.UI
{
    // Grid of 10 level buttons. Locks/unlocks based on player progress.
    public class LevelSelectUI : MonoBehaviour
    {
        [SerializeField] private Transform  buttonContainer;
        [SerializeField] private GameObject levelButtonPrefab;
        [SerializeField] private LevelData[] levels; // assign all 10 LevelData assets
        [SerializeField] private Button     backButton;

        void Start()
        {
            if (backButton != null)
                backButton.onClick.AddListener(() => SceneManager.LoadScene("MainMenu"));

            PopulateLevels();
        }

        private void PopulateLevels()
        {
            if (levels == null) return;

            for (int i = 0; i < levels.Length; i++)
            {
                var go  = Instantiate(levelButtonPrefab, buttonContainer);
                var btn = go.GetComponent<LevelButton>();
                if (btn == null) continue;

                bool unlocked  = PlayerProgress.Instance != null &&
                                  PlayerProgress.Instance.IsLevelUnlocked(i);
                int  highScore = PlayerProgress.Instance?.GetHighScore(i) ?? 0;
                bool completed = PlayerProgress.Instance?.IsLevelCompleted(i) ?? false;

                btn.Setup(levels[i], unlocked, highScore, completed);
            }
        }
    }
}
