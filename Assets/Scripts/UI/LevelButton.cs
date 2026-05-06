using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using SystemBreach.Data;

namespace SystemBreach.UI
{
    public class LevelButton : MonoBehaviour
    {
        [SerializeField] private Button   button;
        [SerializeField] private TMP_Text levelNameText;
        [SerializeField] private TMP_Text highScoreText;
        [SerializeField] private Image    completedIcon;
        [SerializeField] private Image    lockedOverlay;

        private LevelData levelData;

        public void Setup(LevelData data, bool unlocked, int highScore, bool completed)
        {
            levelData = data;

            if (levelNameText  != null) levelNameText.text  = data.levelName;
            if (highScoreText  != null) highScoreText.text  = unlocked ? $"{highScore} pts" : "---";
            if (completedIcon  != null) completedIcon.gameObject.SetActive(completed);
            if (lockedOverlay  != null) lockedOverlay.gameObject.SetActive(!unlocked);
            if (button         != null)
            {
                button.interactable = unlocked;
                button.onClick.AddListener(OnClick);
            }
        }

        private void OnClick()
        {
            if (levelData == null) return;
            // Store which level to load, then load the game scene
            PlayerPrefs.SetInt("SelectedLevel", levelData.levelIndex);
            PlayerPrefs.Save();
            SceneManager.LoadScene("GameScene");
        }
    }
}
