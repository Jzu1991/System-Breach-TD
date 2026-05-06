using System;
using UnityEngine;

namespace SystemBreach.Core
{
    public class EconomyManager : MonoBehaviour
    {
        public static EconomyManager Instance { get; private set; }

        public static event Action<int> OnMoneyChanged; // current money
        public static event Action<int> OnScoreChanged; // current score

        [SerializeField] private int currentMoney;
        [SerializeField] private int score;

        public int Money => currentMoney;
        public int Score => score;

        // How many improvement coins 1 score point converts to
        [SerializeField] private float scoreToCoinsRatio = 0.1f;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public void Initialize(int startingMoney)
        {
            currentMoney = startingMoney;
            score        = 0;
            OnMoneyChanged?.Invoke(currentMoney);
            OnScoreChanged?.Invoke(score);
        }

        public void AddMoney(int amount)
        {
            currentMoney += amount;
            OnMoneyChanged?.Invoke(currentMoney);
        }

        public bool SpendMoney(int amount)
        {
            if (currentMoney < amount) return false;
            currentMoney -= amount;
            OnMoneyChanged?.Invoke(currentMoney);
            return true;
        }

        public void AddScore(int amount)
        {
            score = Mathf.Max(0, score + amount);
            OnScoreChanged?.Invoke(score);
        }

        public bool CanAfford(int amount) => currentMoney >= amount;

        public int GetImprovementCoins() => Mathf.FloorToInt(score * scoreToCoinsRatio);
    }
}
