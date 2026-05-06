using System;
using UnityEngine;

namespace SystemBreach.Data
{
    [Serializable]
    public class SpawnGroup
    {
        public EnemyData enemyData;
        public int       count;
        public float     spawnInterval = 0.8f; // seconds between each enemy in the group
        public float     delayBefore   = 0f;   // wait before this group starts
    }

    [CreateAssetMenu(fileName = "WaveData", menuName = "SystemBreach/Wave Data")]
    public class WaveData : ScriptableObject
    {
        public SpawnGroup[] spawnGroups;
    }
}
