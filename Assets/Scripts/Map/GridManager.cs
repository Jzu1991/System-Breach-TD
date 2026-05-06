using System;
using System.Collections.Generic;
using UnityEngine;
using SystemBreach.Data;
using SystemBreach.Towers;
using SystemBreach.Core;

namespace SystemBreach.Map
{
    public class GridManager : MonoBehaviour
    {
        public static GridManager Instance { get; private set; }

        public static event Action<Tower> OnTowerSelected;
        public static event Action        OnTowerDeselected;
        public static event Action<Tower> OnTowerPlaced;

        [SerializeField] private List<GridCell> cells = new();

        private TowerData pendingTowerData;
        private Tower     selectedTower;

        public bool IsPlacing => pendingTowerData != null;
        public Tower SelectedTower => selectedTower;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;

            // Auto-discover cells if not assigned manually
            if (cells.Count == 0)
                cells.AddRange(GetComponentsInChildren<GridCell>());
        }

        void Update()
        {
            // Cancel placement on right-click or Escape
            if (IsPlacing && (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape)))
                CancelPlacement();
        }

        // Begin placement mode for a tower type
        public void BeginPlacement(TowerData data)
        {
            if (!EconomyManager.Instance.CanAfford(data.baseCost)) return;
            pendingTowerData = data;
            DeselectTower();
        }

        public void CancelPlacement()
        {
            pendingTowerData = null;
            foreach (var cell in cells) cell.HidePlacementPreview();
        }

        public void ConfirmPlacement(GridCell cell)
        {
            if (pendingTowerData == null) return;
            if (!EconomyManager.Instance.SpendMoney(pendingTowerData.baseCost)) return;
            if (pendingTowerData.prefab == null) return;

            var go     = Instantiate(pendingTowerData.prefab);
            var tower  = go.GetComponent<Tower>();
            if (tower == null) { Destroy(go); return; }

            tower.Initialize(pendingTowerData);
            cell.PlaceTower(tower);
            OnTowerPlaced?.Invoke(tower);
            AudioManager.Instance?.PlaySFX(AudioManager.Instance.towerPlace);
            EconomyManager.Instance.AddScore(10); // small score bonus for placing

            pendingTowerData = null;
            SelectTower(tower);
        }

        public void SelectTower(Tower tower)
        {
            selectedTower = tower;
            OnTowerSelected?.Invoke(tower);
        }

        public void DeselectTower()
        {
            if (selectedTower == null) return;
            selectedTower = null;
            OnTowerDeselected?.Invoke();
        }

        public void SellSelectedTower()
        {
            if (selectedTower == null) return;

            var cell = selectedTower.GetComponentInParent<GridCell>();
            int refund = selectedTower.GetSellValue();
            EconomyManager.Instance?.AddMoney(refund);
            AudioManager.Instance?.PlaySFX(AudioManager.Instance.towerSell);

            cell?.RemoveTower();
            Destroy(selectedTower.gameObject);
            selectedTower = null;
            OnTowerDeselected?.Invoke();
        }
    }
}
