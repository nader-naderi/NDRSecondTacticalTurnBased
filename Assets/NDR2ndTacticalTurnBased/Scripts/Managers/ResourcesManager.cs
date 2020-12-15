using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NDR2ndTTB
{
    public class ResourcesManager : MonoBehaviour
    {
        public static ResourcesManager instance;
        private void Awake()
        {
            instance = this;
            LoadCharacters();
        }

        Dictionary<string, int> char_indexes = new Dictionary<string, int>();
        Dictionary<string, int> char_stats_indexes = new Dictionary<string, int>();

        void LoadCharacters()
        {
            CharactersScriptableObject obj = Resources.Load("NDR2ndTTB.CharactersScriptableObject") as CharactersScriptableObject;
            if (!obj)
            {
                Debug.Log("NDR2ndTTB.CharactersScriptableObject can't be loaded.");
                return;
            }

            for (int i = 0; i < obj.allCharacters.Count; i++)
            {
                if (char_indexes.ContainsKey(obj.allCharacters[i].charID))
                {
                    Debug.Log(obj.allCharacters[i].charID + " is a duplicate!");
                }
                else
                {
                    char_indexes.Add(obj.allCharacters[i].charID, i);
                }
            }

            for (int i = 0; i < obj.defaultStats.Count; i++)
            {
                if (char_indexes.ContainsKey(obj.defaultStats[i].CharID))
                {
                    Debug.Log(obj.defaultStats[i].CharID + " is a duplicate!");
                }
                else
                {
                    char_indexes.Add(obj.defaultStats[i].CharID, i);
                }
            }
        }

        public int GetIndexFromString(Dictionary<string, int> d, string id)
        {
            int index = -1;
            d.TryGetValue(id, out index);
            return index;
        }

        public Character GetCharacter(string charID)
        {
            CharactersScriptableObject obj = Resources.Load("NDR2ndTTB.CharactersScriptableObject") as CharactersScriptableObject;
            if (!obj)
            {
                Debug.Log("NDR2ndTTB.CharactersScriptableObject can't be loaded.");
                return null;
            }

            int index = GetIndexFromString(char_indexes, charID);
            if (index == -1)
                return null;

            return obj.allCharacters[index];
        }

        public UnitStats GetCharacterStats(string charID)
        {
            CharactersScriptableObject obj = Resources.Load("NDR2ndTTB.CharactersScriptableObject") as CharactersScriptableObject;
            if (!obj)
            {
                Debug.Log("NDR2ndTTB.CharactersScriptableObject can't be loaded.");
                return null;
            }

            int index = GetIndexFromString(char_stats_indexes, charID);
            if (index == -1)
                return null;

            return obj.defaultStats[index];
        }

    }
}