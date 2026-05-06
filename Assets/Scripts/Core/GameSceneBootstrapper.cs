using UnityEngine;
using SystemBreach.Data;

namespace SystemBreach.Core
{
    // Placed in the GameScene. Reads the selected level from PlayerPrefs,
    // then initializes all managers in the correct order.
    public class GameSceneBootstrapper : MonoBehaviour
    {
        [Header("All 10 level assets (index 0-9)")]
        [SerializeField] private LevelData[] allLevels;

        [Header("Manager References")]
        [SerializeField] private GameManager     gameManager;
        [SerializeField] private WaveManager     waveManager;
        [SerializeField] private EconomyManager  economyManager;

        void Awake()
        {
            int selectedIndex = PlayerPrefs.GetInt("SelectedLevel", 0);
            selectedIndex = Mathf.Clamp(selectedIndex, 0, allLevels.Length - 1);

            LevelData level = allLevels[selectedIndex];

            economyManager.Initialize(level.startingMoney);
            gameManager.InitLevel(level);
            waveManager.Initialize(level.waves);

            AudioManager.Instance?.PlayMusic(AudioManager.Instance.gameplayMusic);
        }
    }
}
