#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace NDR2ndTTB
{
    public class ScriptableObejctManager
    {
        public static void CreateAsste<T>() where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();

            if(Resources.Load(typeof(T).ToString()) == null)
            {
                string assetPath = AssetDatabase.GenerateUniqueAssetPath("Assets/Resources/" + typeof(T).ToString() + ".asset");
                AssetDatabase.CreateAsset(asset, assetPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = asset;
            }
            else
            {
                Debug.Log(typeof(T).ToString() + " already created!"); 
            }
        }

        [MenuItem("Turn Based/Databases/Create Characters List ... ")]
        public static void CreateCharctersObject()
        {
            ScriptableObejctManager.CreateAsste<NDR2ndTTB.CharactersScriptableObject>();
        }

        [MenuItem("Turn Based/Databases/Create Description List ... ")]
        public static void CreateDescriptionBase()
        {
            ScriptableObejctManager.CreateAsste<NDR2ndTTB.DescriptionSceriptable>();
        }
    }
}

#endif