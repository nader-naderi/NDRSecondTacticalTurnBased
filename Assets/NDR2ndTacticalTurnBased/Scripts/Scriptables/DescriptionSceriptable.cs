using System.Collections.Generic;
using UnityEngine;
namespace NDR2ndTTB
{
    [CreateAssetMenu(fileName = "DescriptionSceriptable", menuName = "DescriptionSceriptable", order = 0)]
    public class DescriptionSceriptable : ScriptableObject
    {
        public Dictionary<string, int> keyValues = new Dictionary<string, int>();
        public DescriptionContainter[] descriptions;
        public DescriptionContainter GetDescription(int index)
        {
            if (index < 0)
                index = 0;

            if (index > descriptions.Length - 1)
                index = 0;

            return descriptions[index];
        }
    }

    [System.Serializable]
    public class DescriptionContainter
    {
        public string id;
        public string description;
    }
   
}