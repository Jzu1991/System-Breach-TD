using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace SystemBreach.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private Button playButton;
        [SerializeField] private Button optionsButton;
        [SerializeField] private Button creditsButton;
        [SerializeField] private Button quitButton;

        [SerializeField] private GameObject optionsPanel;
        [SerializeField] private GameObject creditsPanel;

        void Start()
        {
            if (playButton    != null) playButton.onClick.AddListener(OnPlay);
            if (optionsButton != null) optionsButton.onClick.AddListener(() => Toggle(optionsPanel));
            if (creditsButton != null) creditsButton.onClick.AddListener(() => Toggle(creditsPanel));
            if (quitButton    != null) quitButton.onClick.AddListener(OnQuit);

            Core.AudioManager.Instance?.PlayMusic(Core.AudioManager.Instance.mainMenuMusic);
        }

        private void OnPlay()    => SceneManager.LoadScene("LevelSelect");
        private void OnQuit()    => Application.Quit();
        private void Toggle(GameObject panel)
        {
            if (panel != null) panel.SetActive(!panel.activeSelf);
        }
    }
}
