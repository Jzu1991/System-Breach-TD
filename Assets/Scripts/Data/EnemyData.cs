using UnityEngine;

namespace SystemBreach.Data
{
    public enum EnemyType { Normal, Fast, Tank, Boss }

    [CreateAssetMenu(fileName = "EnemyData", menuName = "SystemBreach/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        [Header("Identity")]
        public EnemyType enemyType;
        public string enemyName;
        [TextArea] public string loreDescription;
        public GameObject prefab;

        [Header("Base Stats")]
        public float baseHP       = 50f;
        public float moveSpeed    = 3f;
        public int   killReward   = 10;   // money awarded on death

        // Boss HP scales by level index (0-9); other stats unchanged
        [Header("Boss HP Scaling (Boss only)")]
        public float[] bossHPPerLevel = new float[10];

        public float GetHP(int levelIndex = 0, bool isBoss = false)
        {
            if (isBoss && bossHPPerLevel.Length > levelIndex)
                return bossHPPerLevel[levelIndex];
            return baseHP;
        }
    }
}
