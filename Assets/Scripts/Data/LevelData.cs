using UnityEngine;

namespace SystemBreach.Data
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "SystemBreach/Level Data")]
    public class LevelData : ScriptableObject
    {
        [Header("Level Info")]
        public int    levelIndex;       // 0-9
        public string levelName;
        [TextArea] public string zoneName;  // e.g. "Motherboard", "RAM", "GPU"
        public Sprite thumbnail;

        [Header("Economy")]
        public int startingMoney = 150;
        public int earlyWaveBonus = 10;  // money for manually triggering wave early

        [Header("Waves")]
        public WaveData[] waves;

        [Header("Unlockable Towers")]
        public TowerData[] availableTowers; // towers the player can place in this level

        [Header("PC Health")]
        public float pcMaxHP = 1000f;
    }
}
