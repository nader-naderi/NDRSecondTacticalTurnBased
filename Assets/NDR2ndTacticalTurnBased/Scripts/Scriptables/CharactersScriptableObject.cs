using System.Collections.Generic;
using UnityEngine;

namespace NDR2ndTTB
{
    public class CharactersScriptableObject : ScriptableObject
    {
        public List<Character> allCharacters = new List<Character>();
        public List<UnitStats> defaultStats = new List<UnitStats>();
    }
}