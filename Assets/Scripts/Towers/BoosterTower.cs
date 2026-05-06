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

        void Awake()
        {
            rangeCollider           = GetComponent<CircleCollider2D>();
            rangeCollider.isTrigger = true;
        }

        protected override void OnLevelChanged()
        {
            float effectiveRange = data.boostRange.Length > LevelIdx
                ? data.boostRange[LevelIdx]
                : 2f;

            if (rangeCollider != null)
                rangeCollider.radius = effectiveRange;

            // Recalculate bonuses for currently boosted towers
            float bonus = data.dexterityBonus.Length > LevelIdx
                ? data.dexterityBonus[LevelIdx]
                : 0f;

            foreach (var t in boostedTowers)
            {
                if (t != null) t.boosterDexterityBonus = bonus;
            }
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            var atk = other.GetComponent<AttackTower>();
            if (atk == null || boostedTowers.Contains(atk)) return;

            atk.boosterDexterityBonus += GetDexterityBonus();
            boostedTowers.Add(atk);
        }

        void OnTriggerExit2D(Collider2D other)
        {
            var atk = other.GetComponent<AttackTower>();
            if (atk == null) return;

            atk.boosterDexterityBonus -= GetDexterityBonus();
            boostedTowers.Remove(atk);
        }

        void OnDestroy()
        {
            float bonus = GetDexterityBonus();
            foreach (var t in boostedTowers)
                if (t != null) t.boosterDexterityBonus -= bonus;
        }

        private float GetDexterityBonus()
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
