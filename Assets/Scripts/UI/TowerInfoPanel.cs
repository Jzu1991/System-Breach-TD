using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SystemBreach.Core;
using SystemBreach.Towers;
using SystemBreach.Map;

namespace SystemBreach.UI
{
    // Shows stats, upgrade, and sell options for the selected tower.
    public class TowerInfoPanel : MonoBehaviour
    {
        [Header("Info")]
        [SerializeField] private TMP_Text towerNameText;
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private TMP_Text damageText;
        [SerializeField] private TMP_Text dexterityText;
        [SerializeField] private TMP_Text critText;
        [SerializeField] private TMP_Text rangeText;
        [SerializeField] private TMP_Text focusDropdownLabel;

        [Header("Buttons")]
        [SerializeField] private Button   upgradeButton;
        [SerializeField] private TMP_Text upgradeCostText;
        [SerializeField] private Button   sellButton;
        [SerializeField] private TMP_Text sellValueText;
        [SerializeField] private Button   closeButton;

        [Header("Focus Dropdown")]
        [SerializeField] private TMPro.TMP_Dropdown focusDropdown;

        void OnEnable()
        {
            GridManager.OnTowerSelected   += Refresh;
            GridManager.OnTowerDeselected += Clear;
            EconomyManager.OnMoneyChanged += _ => RefreshButtons();
        }

        void OnDisable()
        {
            GridManager.OnTowerSelected   -= Refresh;
            GridManager.OnTowerDeselected -= Clear;
            EconomyManager.OnMoneyChanged -= _ => RefreshButtons();
        }

        void Start()
        {
            if (upgradeButton != null) upgradeButton.onClick.AddListener(OnUpgrade);
            if (sellButton    != null) sellButton.onClick.AddListener(OnSell);
            if (closeButton   != null) closeButton.onClick.AddListener(OnClose);
            if (focusDropdown != null) focusDropdown.onValueChanged.AddListener(OnFocusChanged);
        }

        private void Refresh(Tower tower)
        {
            if (tower == null) { Clear(); return; }

            var data = tower.data;
            int idx  = tower.CurrentLevel - 1;

            if (towerNameText != null) towerNameText.text = data.towerName;
            if (levelText     != null) levelText.text     = $"Level {tower.CurrentLevel} / 5";
            if (damageText    != null && data.damage.Length > idx)
                damageText.text = $"Damage: {data.damage[idx]}";
            if (dexterityText != null && data.dexterity.Length > idx)
                dexterityText.text = $"ATK Speed: {data.dexterity[idx]:F1}/s";
            if (critText      != null && data.critChance.Length > idx)
                critText.text  = $"Crit: {data.critChance[idx] * 100f:F0}%";
            if (rangeText     != null && data.range.Length > idx)
                rangeText.text = $"Range: {data.range[idx]}";

            // Focus dropdown — only for attack towers
            if (focusDropdown != null)
            {
                var atk = tower as AttackTower;
                focusDropdown.gameObject.SetActive(atk != null);
                if (atk != null) focusDropdown.value = (int)atk.Focus;
            }

            RefreshButtons();
        }

        private void RefreshButtons()
        {
            var tower = GridManager.Instance?.SelectedTower;
            if (tower == null) return;

            if (upgradeButton != null)
            {
                bool canUpgrade = tower.CanUpgrade();
                upgradeButton.interactable = canUpgrade;
                int upgCost = tower.data.GetUpgradeCost(tower.CurrentLevel);
                if (upgradeCostText != null)
                    upgradeCostText.text = canUpgrade ? $"Upgrade ${upgCost}" : "MAX";
            }

            if (sellButton != null && sellValueText != null)
                sellValueText.text = $"Sell ${tower.GetSellValue()}";
        }

        private void Clear()
        {
            if (towerNameText  != null) towerNameText.text  = string.Empty;
            if (levelText      != null) levelText.text      = string.Empty;
            if (damageText     != null) damageText.text     = string.Empty;
            if (dexterityText  != null) dexterityText.text  = string.Empty;
            if (critText       != null) critText.text       = string.Empty;
            if (rangeText      != null) rangeText.text      = string.Empty;
        }

        private void OnUpgrade()
        {
            var tower = GridManager.Instance?.SelectedTower;
            if (tower?.Upgrade() == true)
                Refresh(tower);
        }

        private void OnSell() => GridManager.Instance?.SellSelectedTower();

        private void OnClose() => GridManager.Instance?.DeselectTower();

        private void OnFocusChanged(int value)
        {
            var atk = GridManager.Instance?.SelectedTower as AttackTower;
            if (atk != null) atk.Focus = (FocusMode)value;
        }
    }
}
