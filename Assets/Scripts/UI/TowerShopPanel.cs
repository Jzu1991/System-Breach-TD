using System.Collections.Generic;
using UnityEngine;
using SystemBreach.Core;
using SystemBreach.Data;

namespace SystemBreach.UI
{
    // Displays available tower buttons for the current level.
    public class TowerShopPanel : MonoBehaviour
    {
        [SerializeField] private Transform  buttonContainer;
        [SerializeField] private GameObject towerButtonPrefab;

        private readonly List<TowerShopButton> buttons = new();

        void Start()
        {
            if (Core.GameManager.Instance?.currentLevel != null)
                Populate(Core.GameManager.Instance.currentLevel.availableTowers);
        }

        public void Populate(TowerData[] towers)
        {
            foreach (var b in buttons) if (b != null) Destroy(b.gameObject);
            buttons.Clear();

            foreach (var data in towers)
            {
                var go  = Instantiate(towerButtonPrefab, buttonContainer);
                var btn = go.GetComponent<TowerShopButton>();
                if (btn != null)
                {
                    btn.Setup(data);
                    buttons.Add(btn);
                }
            }
        }

        void OnEnable()  => EconomyManager.OnMoneyChanged += RefreshAffordability;
        void OnDisable() => EconomyManager.OnMoneyChanged -= RefreshAffordability;

        private void RefreshAffordability(int _)
        {
            foreach (var b in buttons) b?.RefreshAffordability();
        }
    }
}
