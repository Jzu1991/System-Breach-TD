using UnityEngine;
using SystemBreach.Data;
using SystemBreach.Core;

namespace SystemBreach.Towers
{
    // Base class shared by all tower types
    public abstract class Tower : MonoBehaviour
    {
        [Header("Data")]
        public TowerData data;

        [Header("Runtime")]
        [SerializeField] protected int currentLevel = 1;

        public int   CurrentLevel => currentLevel;
        public bool  IsMaxLevel   => currentLevel >= 5;

        // Booster bonus applied from a nearby BoosterTower (additive dexterity)
        [HideInInspector] public float boosterDexterityBonus = 0f;

        public virtual void Initialize(TowerData towerData)
        {
            data         = towerData;
            currentLevel = 1;
            OnLevelChanged();
        }

        public bool CanUpgrade() => !IsMaxLevel &&
            EconomyManager.Instance != null &&
            EconomyManager.Instance.CanAfford(data.GetUpgradeCost(currentLevel));

        public bool Upgrade()
        {
            if (!CanUpgrade()) return false;
            int cost = data.GetUpgradeCost(currentLevel);
            if (!EconomyManager.Instance.SpendMoney(cost)) return false;

            currentLevel++;
            AudioManager.Instance?.PlaySFX(AudioManager.Instance.towerUpgrade);
            EconomyManager.Instance.AddScore(20);
            OnLevelChanged();
            return true;
        }

        public int GetSellValue()
        {
            int total = data.GetTotalInvested(currentLevel);
            return Mathf.FloorToInt(total * data.sellPercentage);
        }

        // Override to react to level changes
        protected virtual void OnLevelChanged() { }

        // Stat accessors — read from data arrays by currentLevel index
        protected int   Damage       => data.damage    [LevelIdx];
        protected float Dexterity    => data.dexterity [LevelIdx] + boosterDexterityBonus;
        protected float CritChance   => data.critChance[LevelIdx];
        protected float Range        => data.range     [LevelIdx];

        protected int LevelIdx => Mathf.Clamp(currentLevel - 1, 0, 4);

        protected bool RollCrit() => Random.value < CritChance;

        protected int CalculateDamage()
        {
            int base_ = Damage;
            return RollCrit() ? Mathf.RoundToInt(base_ * data.critMultiplier) : base_;
        }
    }
}
