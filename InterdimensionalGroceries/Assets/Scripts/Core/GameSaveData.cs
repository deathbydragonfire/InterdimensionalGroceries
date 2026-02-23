using System;
using System.Collections.Generic;

namespace InterdimensionalGroceries.Core
{
    [Serializable]
    public class GameSaveData
    {
        public float currentMoney;
        public string currentScene;
        public string currentPhase;
        public List<UpgradeLevel> upgradeLevels = new List<UpgradeLevel>();
    }

    [Serializable]
    public class UpgradeLevel
    {
        public string upgradeName;
        public int level;

        public UpgradeLevel(string name, int lvl)
        {
            upgradeName = name;
            level = lvl;
        }
    }
}
