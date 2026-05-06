using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SystemBreach.Data;
using SystemBreach.Map;
using SystemBreach.Core;

namespace SystemBreach.UI
{
    // Single button in the tower shop. Initiates placement on click.
    public class TowerShopButton : MonoBehaviour
    {
        [SerializeField] private Image    iconImage;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text costText;
        [SerializeField] private Button   button;

        private TowerData towerData;

        public void Setup(TowerData data)
        {
            towerData = data;
            if (iconImage != null && data.icon != null) iconImage.sprite = data.icon;
            if (nameText  != null) nameText.text = data.towerName;
            if (costText  != null) costText.text = $"${data.baseCost}";
            if (button    != null) button.onClick.AddListener(OnClicked);

            RefreshAffordability();
        }

        public void RefreshAffordability()
        {
            if (button == null || towerData == null || EconomyManager.Instance == null) return;
            button.interactable = EconomyManager.Instance.CanAfford(towerData.baseCost);
        }

        private void OnClicked()
        {
            if (towerData == null || GridManager.Instance == null) return;
            GridManager.Instance.BeginPlacement(towerData);
        }
    }
}
