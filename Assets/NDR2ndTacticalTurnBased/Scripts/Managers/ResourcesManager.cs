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
            LoadDescriptions();
        }

        Dictionary<string, int> char_indexes = new Dictionary<string, int>();
        Dictionary<string, int> char_stats_indexes = new Dictionary<string, int>();

        public CharactersScriptableObject charactersScriptableObject;
        public DescriptionSceriptable descriptionSceriptable;

        void LoadCharacters()
        {
            charactersScriptableObject = Resources.Load
                ("NDR2ndTTB.CharactersScriptableObject") as CharactersScriptableObject;
            if (!charactersScriptableObject)
            {
                Debug.Log("NDR2ndTTB.CharactersScriptableObject can't be loaded.");
                return;
            }

            for (int i = 0; i < charactersScriptableObject.allCharacters.Count; i++)
            {
                if (char_indexes.ContainsKey(charactersScriptableObject.allCharacters[i].charID))
                {
                    Debug.Log(charactersScriptableObject.allCharacters[i].charID + " is a duplicate!");
                }
                else
                {
                    char_indexes.Add(charactersScriptableObject.allCharacters[i].charID, i);
                }
            }

            for (int i = 0; i < charactersScriptableObject.defaultStats.Count; i++)
            {
                if (char_indexes.ContainsKey(charactersScriptableObject.defaultStats[i].CharID))
                {
                    Debug.Log(charactersScriptableObject.defaultStats[i].CharID + " is a duplicate!");
                }
                else
                {
                    char_indexes.Add(charactersScriptableObject.defaultStats[i].CharID, i);
                }
            }
        }

        void LoadDescriptions()
        {
            descriptionSceriptable.keyValues.Clear();
            for (int i = 0; i < descriptionSceriptable.descriptions.Length; i++)
            {
                descriptionSceriptable.keyValues.Add(descriptionSceriptable.descriptions[i].id, i);
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
            if (!charactersScriptableObject)
            {
                Debug.Log("NDR2ndTTB.CharactersScriptableObject can't be loaded.");
                return null;
            }

            int index = GetIndexFromString(char_indexes, charID);
            if (index == -1)
                return null;

            return charactersScriptableObject.allCharacters[index];
        }

        public UnitStats GetCharacterStats(string charID)
        {
            if (!charactersScriptableObject)
            {
                Debug.Log("NDR2ndTTB.CharactersScriptableObject can't be loaded.");
                return null;
            }

            int index = GetIndexFromString(char_stats_indexes, charID);
            if (index == -1)
                return null;

            return charactersScriptableObject.defaultStats[index];
        }

        public DescriptionContainter GetDescription(string id)
        {
            int index = GetIndexFromString(descriptionSceriptable.keyValues, id);
            return descriptionSceriptable.descriptions[index];
        }
    }
}