using System.Collections.Generic;
using UnityEngine;

namespace SystemBreach.Towers
{
    // Boosts the dexterity of attack towers within its range.
    [RequireComponent(typeof(CircleCollider2D))]
    public class BoosterTower : Tower
    {
        private CircleCollider2D rangeCollider;
        private readonly List<AttackTower> boostedTowers = new();

        // Track exactly how much bonus this booster is currently applying,
        // so upgrades only apply the delta rather than overwriting other boosters.
        private float appliedBonus = 0f;

        void Awake()
        {
            rangeCollider           = GetComponent<CircleCollider2D>();
            rangeCollider.isTrigger = true;
        }

        protected override void OnLevelChanged()
        {
            float newBonus = CurrentBonus();
            float delta    = newBonus - appliedBonus;
            appliedBonus   = newBonus;

            float effectiveRange = data.boostRange.Length > LevelIdx ? data.boostRange[LevelIdx] : 2f;
            if (rangeCollider != null)
                rangeCollider.radius = effectiveRange;

            foreach (var t in boostedTowers)
                if (t != null) t.boosterDexterityBonus += delta;
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            var atk = other.GetComponent<AttackTower>();
            if (atk == null || boostedTowers.Contains(atk)) return;

            atk.boosterDexterityBonus += appliedBonus;
            boostedTowers.Add(atk);
        }

        void OnTriggerExit2D(Collider2D other)
        {
            var atk = other.GetComponent<AttackTower>();
            if (atk == null) return;

            atk.boosterDexterityBonus -= appliedBonus;
            boostedTowers.Remove(atk);
        }

        void OnDestroy()
        {
            foreach (var t in boostedTowers)
                if (t != null) t.boosterDexterityBonus -= appliedBonus;
        }

        private float CurrentBonus()
        {
            if (data == null || data.dexterityBonus.Length == 0) return 0f;
            return data.dexterityBonus[LevelIdx];
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
            float r = (data != null && data.boostRange.Length > LevelIdx) ? data.boostRange[LevelIdx] : 2f;
            Gizmos.DrawWireSphere(transform.position, r);
        }
    }
}
