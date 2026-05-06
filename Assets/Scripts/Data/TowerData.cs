using UnityEngine;

namespace SystemBreach.Data
{
    public enum TowerType { Melee, Common, Metralleta, Franco, Farm, Boost }

    [CreateAssetMenu(fileName = "TowerData", menuName = "SystemBreach/Tower Data")]
    public class TowerData : ScriptableObject
    {
        [Header("Identity")]
        public TowerType towerType;
        public string towerName;
        [TextArea] public string description;
        public Sprite icon;
        public GameObject prefab;

        [Header("Economy")]
        public int baseCost;
        public float sellPercentage = 0.7f;
        public int[] upgradeCosts = new int[4]; // costs for levels 2-5

        [Header("Stats per Level (index 0 = level 1)")]
        // Attack towers
        public int[] damage     = new int[5];
        public float[] dexterity = new float[5]; // attacks per second
        public float[] critChance = new float[5]; // 0..1
        public float[] range    = new float[5];

        // Farm tower
        public int[] moneyPerWave = new int[5];

        // Boost tower
        public float[] dexterityBonus = new float[5];
        public float[] boostRange = new float[5];

        [Header("Crit Multiplier")]
        public float critMultiplier = 2f;

        public int GetUpgradeCost(int currentLevel)
        {
            if (currentLevel >= 5) return 0;
            return upgradeCosts[currentLevel - 1]; // currentLevel 1 → index 0
        }

        public int GetTotalInvested(int currentLevel)
        {
            int total = baseCost;
            for (int i = 0; i < currentLevel - 1; i++)
                total += upgradeCosts[i];
            return total;
        }
    }
}
