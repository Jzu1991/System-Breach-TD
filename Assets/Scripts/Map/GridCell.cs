using UnityEngine;
using SystemBreach.Towers;

namespace SystemBreach.Map
{
    public enum CellState { Available, Occupied, OnPath, Blocked }

    public class GridCell : MonoBehaviour
    {
        [SerializeField] private CellState state = CellState.Available;
        [SerializeField] private Tower     placedTower;

        [Header("Visuals")]
        [SerializeField] private GameObject availableHighlight;
        [SerializeField] private GameObject occupiedHighlight;
        [SerializeField] private GameObject invalidHighlight;

        public CellState State       => state;
        public Tower     PlacedTower => placedTower;
        public bool      IsAvailable => state == CellState.Available;

        void Start() => RefreshVisuals();

        public void SetOnPath()
        {
            state = CellState.OnPath;
            RefreshVisuals();
        }

        public void SetBlocked()
        {
            state = CellState.Blocked;
            RefreshVisuals();
        }

        public bool PlaceTower(Tower tower)
        {
            if (!IsAvailable) return false;
            placedTower = tower;
            state       = CellState.Occupied;
            tower.transform.position = transform.position;
            tower.transform.SetParent(transform);
            RefreshVisuals();
            return true;
        }

        public void RemoveTower()
        {
            placedTower = null;
            state       = CellState.Available;
            RefreshVisuals();
        }

        // Called by GridManager during placement preview
        public void ShowPlacementPreview(bool valid)
        {
            if (availableHighlight) availableHighlight.SetActive(false);
            if (occupiedHighlight)  occupiedHighlight.SetActive(false);
            if (invalidHighlight)   invalidHighlight.SetActive(false);

            if (!IsAvailable) return;
            if (valid && availableHighlight) availableHighlight.SetActive(true);
            else if (!valid && invalidHighlight) invalidHighlight.SetActive(true);
        }

        public void HidePlacementPreview() => RefreshVisuals();

        private void RefreshVisuals()
        {
            if (availableHighlight) availableHighlight.SetActive(state == CellState.Available);
            if (occupiedHighlight)  occupiedHighlight.SetActive(state == CellState.Occupied);
            if (invalidHighlight)   invalidHighlight.SetActive(false);
        }

        void OnMouseEnter()
        {
            if (GridManager.Instance != null && GridManager.Instance.IsPlacing)
                ShowPlacementPreview(IsAvailable);
        }

        void OnMouseExit() => HidePlacementPreview();

        void OnMouseDown()
        {
            if (GridManager.Instance == null) return;

            if (GridManager.Instance.IsPlacing && IsAvailable)
                GridManager.Instance.ConfirmPlacement(this);
            else if (state == CellState.Occupied && placedTower != null)
                GridManager.Instance.SelectTower(placedTower);
        }
    }
}
