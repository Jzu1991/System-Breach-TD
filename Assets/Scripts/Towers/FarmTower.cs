using System.Collections.Generic;
using UnityEngine;
using SystemBreach.Core;

namespace SystemBreach.Towers
{
    // Generates money at the start of each wave. Does not attack.
    public class FarmTower : Tower
    {
        private static readonly List<FarmTower> activeFarms = new();

        void OnEnable()  => activeFarms.Add(this);
        void OnDisable() => activeFarms.Remove(this);
        void OnDestroy() => activeFarms.Remove(this);

        // Called by WaveManager at the start of each wave
        public static void NotifyWaveStart()
        {
            foreach (var farm in activeFarms)
                farm.GenerateMoney();
        }

        private void GenerateMoney()
        {
            if (data == null || EconomyManager.Instance == null) return;
            if (data.moneyPerWave.Length == 0) return;

            int money = data.moneyPerWave[LevelIdx];
            EconomyManager.Instance.AddMoney(money);
        }

        public int MoneyPerWave => (data != null && data.moneyPerWave.Length > LevelIdx)
            ? data.moneyPerWave[LevelIdx]
            : 0;
    }
}
