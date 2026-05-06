using UnityEngine;
using SystemBreach.Core;

namespace SystemBreach.Enemies
{
    // Boss variant — plays boss music on spawn and fires boss arrival SFX.
    [RequireComponent(typeof(Enemy))]
    public class BossEnemy : MonoBehaviour
    {
        void Start()
        {
            AudioManager.Instance?.PlaySFX(AudioManager.Instance.bossArrival);
            AudioManager.Instance?.PlayMusic(AudioManager.Instance.bossMusic);
        }

        void OnDestroy()
        {
            // Return to gameplay music when boss dies
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayMusic(AudioManager.Instance.gameplayMusic);
        }
    }
}
