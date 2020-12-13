using UnityEngine;
using System.Collections;
using NDR2ndTTB;

namespace NDR.Utilities
{
    [ExecuteInEditMode]
    public class LevelEditor : MonoBehaviour
    {
        public static LevelEditor instance;
        [SerializeField]
        GridBase targetGrid;
        [SerializeField]
        LevelManager levelManager;
        
        public HotKeys hotKeys;

        [Header("States")]
        public string levelName;
        public bool levelHasInited;

        [SerializeField] int sizeX = 64;
        [SerializeField] int sizeY = 1;
        [SerializeField] int sizeZ = 64;
        [SerializeField] int scaleXZ = 1;
        [SerializeField] int scaleY = 2;

        [Header("Modes")]

        public bool editMode;
        [Header("Prompts")]
        public bool initLevel;
        [Space(10)]
        public bool clearLevel;
        [Space(10)]
        public bool saveLevel;
        [Space]
        public bool loadLevel;
        

        private void Update()
        {
            if (!instance)
                instance = this;

            if (!levelHasInited)
                editMode = false;

            if (!targetGrid || !levelManager)
                editMode = false;

            UpdateStatesLogic();
        }

        private void UpdateStatesLogic()
        {
            if (initLevel)
            {
                initLevel = false;
                if (levelHasInited)
                {
                    ClearLevel();
                }
                InitializeLevel();
            }

            if (clearLevel)
            {
                clearLevel = false;
                if (levelHasInited)
                    ClearLevel();
            }

            if (saveLevel)
            {
                saveLevel = false;
                SaveLevel();
            }

            if(loadLevel)
            {
                loadLevel = false;
                LoadLevel();
            }
        }

        public void InitializeLevel()
        {
            if(!targetGrid)
            {
                Debug.LogError("To init level you need t assing the target gridbase first.");
                return;
            }
            if (!levelManager)
            {
                Debug.LogError("To init level you need t assing the target level maanagewr first.");
                return;
            }

            targetGrid.sizeX = sizeX;
            targetGrid.sizeY = sizeY;
            targetGrid.sizeZ = sizeZ;
            targetGrid.scaleXZ = scaleXZ;
            targetGrid.scaleY = scaleY;

            targetGrid.levelName = levelName;
            targetGrid.InitForEditor(levelManager);
            levelHasInited = true;
            editMode = true;
        }

        public void ClearLevel()
        {
            if (!targetGrid)
            {
                Debug.LogError("To init level you need t assing the target gridbase first.");
                return;
            }
            if (!levelManager)
            {
                Debug.LogError("To init level you need t assing the target level maanagewr first.");
                return;
            }
            targetGrid.ClearLevel(true);
            levelHasInited = false;
            editMode = false;
        }

        public void ChangeNodeStatusOnPosition(Vector3 targetPos, bool status)
        {
            Node n = targetGrid.GetNodeFromWorldPosition(targetPos, true);
            if (n == null)
                return;

            n.ChangeNodeStatus(status, targetGrid);
        }
    
        public void SaveLevel()
        {
            if (!targetGrid)
            {
                Debug.LogError("To init level you need t assing the target gridbase first.");
                return;
            }
            if (!levelManager)
            {
                Debug.LogError("To init level you need t assing the target level maanagewr first.");
                return;
            }

            Serialization.SaveLevel(levelName, targetGrid);
        }

        public void LoadLevel()
        {
            ClearLevel();
            if (!targetGrid)
            {
                Debug.LogError("To init level you need t assing the target gridbase first.");
                return;
            }
            if (!levelManager)
            {
                Debug.LogError("To init level you need t assing the target level maanagewr first.");
                return;
            }

            targetGrid.levelName = levelName;
            bool canLoad = targetGrid.LoadForEditor(levelManager);
            if(!canLoad)
            {
                Debug.LogError("cant find level " + levelName);
                return;
            }
            levelHasInited = true;
            editMode = true;
        }
    }

    [System.Serializable]
    public class HotKeys
    {
        public KeyCode editMode = KeyCode.A;
        public KeyCode initLevel = KeyCode.S;
        public KeyCode saveLevel = KeyCode.D;
        public KeyCode clearLevel = KeyCode.Alpha5;
        public KeyCode canWalk = KeyCode.G;
        public KeyCode dontWalk = KeyCode.B;
    }
}