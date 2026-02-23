using System;
using System.Collections.Generic;
using UnityEngine;

namespace InterdimensionalGroceries.Core
{
    [Serializable]
    public class GameSaveData
    {
        public float currentMoney;
        public string currentScene;
        public string currentPhase;
        public List<UpgradeLevel> upgradeLevels = new List<UpgradeLevel>();
        public List<WorldObjectData> worldObjects = new List<WorldObjectData>();
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

    [Serializable]
    public class WorldObjectData
    {
        public string prefabIdentifier;
        public SerializableVector3 position;
        public SerializableQuaternion rotation;
        public SerializableVector3 scale;
        public string objectType;

        public WorldObjectData(string identifier, Vector3 pos, Quaternion rot, Vector3 scl, SaveableObjectType type)
        {
            prefabIdentifier = identifier;
            position = new SerializableVector3(pos);
            rotation = new SerializableQuaternion(rot);
            scale = new SerializableVector3(scl);
            objectType = type.ToString();
        }
    }

    [Serializable]
    public class SerializableVector3
    {
        public float x;
        public float y;
        public float z;

        public SerializableVector3(Vector3 vector)
        {
            x = vector.x;
            y = vector.y;
            z = vector.z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }
    }

    [Serializable]
    public class SerializableQuaternion
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public SerializableQuaternion(Quaternion quaternion)
        {
            x = quaternion.x;
            y = quaternion.y;
            z = quaternion.z;
            w = quaternion.w;
        }

        public Quaternion ToQuaternion()
        {
            return new Quaternion(x, y, z, w);
        }
    }
}
