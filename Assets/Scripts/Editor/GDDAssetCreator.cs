// Editor-only utility: creates all TowerData and EnemyData ScriptableObjects
// pre-filled with the exact stats from the GDD v0.5.
// Menu: SystemBreach > Create GDD Assets

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using SystemBreach.Data;

namespace SystemBreach.Editor
{
    public static class GDDAssetCreator
    {
        private const string TowerFolder = "Assets/ScriptableObjects/Towers";
        private const string EnemyFolder = "Assets/ScriptableObjects/Enemies";

        [MenuItem("SystemBreach/Create GDD Assets")]
        public static void CreateAll()
        {
            EnsureFolder("Assets/ScriptableObjects");
            EnsureFolder(TowerFolder);
            EnsureFolder(EnemyFolder);

            CreateTowers();
            CreateEnemies();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[SystemBreach] All GDD assets created successfully.");
        }

        // ── TOWERS ──────────────────────────────────────────────────────────

        private static void CreateTowers()
        {
            // Melee — Corp-a-corp
            var melee = Create<TowerData>(TowerFolder, "Tower_Melee");
            melee.towerType   = TowerType.Melee;
            melee.towerName   = "Melee";
            melee.description = "Torre de corto alcance que propina puñetazos rápidos. Alta frecuencia de golpe.";
            melee.baseCost    = 50;
            melee.sellPercentage = 0.7f;
            melee.upgradeCosts   = new[] { 75, 100, 150, 200 };
            melee.damage         = new[] { 5, 10, 20, 35, 50 };
            melee.dexterity      = new[] { 10f, 12f, 15f, 22f, 30f };
            melee.critChance     = new[] { 0f, 0.05f, 0.10f, 0.20f, 0.30f };
            melee.range          = new[] { 1f, 2f, 3f, 4f, 5f };
            melee.critMultiplier = 2f;
            EditorUtility.SetDirty(melee);

            // Common — Torre Estándar
            var common = Create<TowerData>(TowerFolder, "Tower_Common");
            common.towerType   = TowerType.Common;
            common.towerName   = "Common";
            common.description = "Torre de alcance medio que dispara proyectiles. Stats equilibrados.";
            common.baseCost    = 75;
            common.sellPercentage = 0.7f;
            common.upgradeCosts   = new[] { 100, 150, 200, 300 };
            common.damage         = new[] { 10, 15, 22, 30, 50 };
            common.dexterity      = new[] { 3f, 5f, 8f, 12f, 15f };
            common.critChance     = new[] { 0f, 0.05f, 0.10f, 0.20f, 0.30f };
            common.range          = new[] { 5f, 7f, 10f, 14f, 20f };
            common.critMultiplier = 2f;
            EditorUtility.SetDirty(common);

            // Metralleta — Alta Cadencia
            var metra = Create<TowerData>(TowerFolder, "Tower_Metralleta");
            metra.towerType   = TowerType.Metralleta;
            metra.towerName   = "Metralleta";
            metra.description = "Alta cadencia, daño medio-bajo. Ideal contra grupos de enemigos rápidos.";
            metra.baseCost    = 100;
            metra.sellPercentage = 0.7f;
            metra.upgradeCosts   = new[] { 150, 200, 300, 500 };
            metra.damage         = new[] { 3, 5, 8, 12, 15 };
            metra.dexterity      = new[] { 30f, 45f, 60f, 75f, 100f };
            metra.critChance     = new[] { 0f, 0.05f, 0.10f, 0.20f, 0.30f };
            metra.range          = new[] { 4f, 6f, 8f, 10f, 14f };   // WIP in GDD
            metra.critMultiplier = 2f;
            EditorUtility.SetDirty(metra);

            // Franco — Francotirador
            var franco = Create<TowerData>(TowerFolder, "Tower_Franco");
            franco.towerType   = TowerType.Franco;
            franco.towerName   = "Franco";
            franco.description = "Alcance extremo, altísimo daño, cadencia muy baja. Especialista en objetivos de alta vida.";
            franco.baseCost    = 150;
            franco.sellPercentage = 0.7f;
            franco.upgradeCosts   = new[] { 200, 300, 500, 750 };
            franco.damage         = new[] { 25, 45, 60, 75, 100 };
            franco.dexterity      = new[] { 1f, 2f, 3f, 5f, 8f };
            franco.critChance     = new[] { 0f, 0.05f, 0.10f, 0.20f, 0.30f };
            franco.range          = new[] { 50f, 100f, 150f, 200f, 500f };
            franco.critMultiplier = 2f;
            EditorUtility.SetDirty(franco);

            // Farm — Generadora de Dinero
            var farm = Create<TowerData>(TowerFolder, "Tower_Farm");
            farm.towerType   = TowerType.Farm;
            farm.towerName   = "Farm";
            farm.description = "Genera dinero pasivo al inicio de cada oleada. No ataca enemigos.";
            farm.baseCost    = 100;
            farm.sellPercentage  = 0.7f;
            farm.upgradeCosts    = new[] { 150, 200, 300, 500 };
            farm.moneyPerWave    = new[] { 20, 35, 55, 80, 120 };     // WIP in GDD
            EditorUtility.SetDirty(farm);

            // Boost — Torre de Apoyo
            var boost = Create<TowerData>(TowerFolder, "Tower_Boost");
            boost.towerType   = TowerType.Boost;
            boost.towerName   = "Boost";
            boost.description = "Potencia la destreza de torres adyacentes en su rango. No ataca.";
            boost.baseCost    = 100;
            boost.sellPercentage  = 0.7f;
            boost.upgradeCosts    = new[] { 150, 200, 300, 500 };
            boost.dexterityBonus  = new[] { 2f, 4f, 7f, 12f, 20f };  // WIP in GDD
            boost.boostRange      = new[] { 2f, 3f, 4f, 5f, 7f };    // WIP in GDD
            EditorUtility.SetDirty(boost);
        }

        // ── ENEMIES ─────────────────────────────────────────────────────────

        private static void CreateEnemies()
        {
            // Normal
            var normal = Create<EnemyData>(EnemyFolder, "Enemy_Normal");
            normal.enemyType    = EnemyType.Normal;
            normal.enemyName    = "Virus";
            normal.loreDescription = "Enemigo base con stats equilibrados. Referencia de balance.";
            normal.baseHP       = 50f;
            normal.moveSpeed    = 3f;
            normal.killReward   = 10;
            EditorUtility.SetDirty(normal);

            // Fast
            var fast = Create<EnemyData>(EnemyFolder, "Enemy_Fast");
            fast.enemyType    = EnemyType.Fast;
            fast.enemyName    = "Worm";
            fast.loreDescription = "Poca vida, gran velocidad. Prioritario para Metralleta.";
            fast.baseHP       = 20f;
            fast.moveSpeed    = 7f;
            fast.killReward   = 15;
            EditorUtility.SetDirty(fast);

            // Tank
            var tank = Create<EnemyData>(EnemyFolder, "Enemy_Tank");
            tank.enemyType    = EnemyType.Tank;
            tank.enemyName    = "Trojan";
            tank.loreDescription = "Mucha vida, velocidad muy baja. Prioritario para Franco y Melee.";
            tank.baseHP       = 200f;
            tank.moveSpeed    = 1f;
            tank.killReward   = 25;
            EditorUtility.SetDirty(tank);

            // Boss — HP scales per level
            var boss = Create<EnemyData>(EnemyFolder, "Enemy_Boss");
            boss.enemyType    = EnemyType.Boss;
            boss.enemyName    = "Ransomware";
            boss.loreDescription = "Un Boss por nivel. Solo escala la vida (HP). Recompensa elevada.";
            boss.baseHP       = 1000f;
            boss.moveSpeed    = 1.5f;
            boss.killReward   = 100;
            boss.bossHPPerLevel = new float[]
            {
                500f, 1000f, 1800f, 3000f, 5000f,
                8000f, 13000f, 20000f, 32000f, 50000f
            };
            EditorUtility.SetDirty(boss);
        }

        // ── Helpers ─────────────────────────────────────────────────────────

        private static T Create<T>(string folder, string assetName) where T : ScriptableObject
        {
            string path = $"{folder}/{assetName}.asset";
            var existing = AssetDatabase.LoadAssetAtPath<T>(path);
            if (existing != null) return existing;

            var asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }

        private static void EnsureFolder(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                int lastSlash = path.LastIndexOf('/');
                string parent = path[..lastSlash];
                string folder = path[(lastSlash + 1)..];
                AssetDatabase.CreateFolder(parent, folder);
            }
        }
    }
}
#endif
