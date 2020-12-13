using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NDR2ndTTB
{
    public class GridBase : MonoBehaviour
    {
        public static GridBase instance;
        private void Awake()
        {
            if (!instance) instance = this;
        }

        public bool isInited;
        public int sizeX = 32;
        public int sizeY = 32;
        public int sizeZ = 32;
        public float scaleXZ = 1;
        public float scaleY = 2.3f;



        public Node[,,] grid;
        public List<YLevels> yLevels = new List<YLevels>();

        public string levelName;
        public bool saveLevel;
        public bool loadLevel;


        public bool debugNode = false;
        public Material debugMat;
        public Material unwalkableMaterial;

        GameObject debugNodeObject;

        SaveLevelFile savedLevel;

        private void Start()
        {
            InitPhase();
        }

      

        public void InitPhase()
        {
            if (debugNode)
                debugNodeObject = WorldNode();

            bool hasSaveLevel = (loadLevel)? CheckForSavedLevel() : false;

            if(hasSaveLevel)
            {
                sizeX = savedLevel.sizeX;
                sizeY = savedLevel.sizeY;
                sizeZ = savedLevel.sizeZ;

                scaleXZ = savedLevel.scaleXZ;
                scaleY = savedLevel.scaleY;


            }

            Checks();
            CreateGrid();

            GameManager.instance.Init();
            if (! hasSaveLevel)
                LevelManager.instance.LoadObstacles(this);
            else
                LoadLevel();

            CameraManager.instance.Init();

            isInited = true;
        }

        public void InitForEditor(LevelManager levelManager)
        {
            if (debugNode)
                debugNodeObject = WorldNode(true);


            Checks();
            CreateGrid();

            levelManager.LoadObstacles(this, true);
        }


        void LoadLevel(SaveLevelFile sf = null)
        {
            SaveLevelFile targetSaveFile = sf;
            if (targetSaveFile == null)
                targetSaveFile = savedLevel;

            List<SaveableNode> s = targetSaveFile.savedNodes;
            for (int i = 0; i < s.Count; i++)
            {
                grid[s[i].x, s[i].y, s[i].z].ChangeNodeStatus(s[i].isWalkable, this);

            }
        }

        public bool LoadForEditor(LevelManager levelManager)
        {
            SaveLevelFile save = Serialization.LoadLevel(levelName);

            if (save == null)
                return false;

            if (debugNode)
                debugNodeObject = WorldNode(true);

            sizeX = save.sizeX;
            sizeY = save.sizeY;
            sizeZ = save.sizeZ;

            scaleXZ = save.scaleXZ;
            scaleY = save.scaleY;

            Checks();
            CreateGrid();
            LoadLevel(save);

            return true;
        }

        bool CheckForSavedLevel()
        {
            SaveLevelFile save = Serialization.LoadLevel(levelName);

            if (save == null)
                return false;

            savedLevel = save;

            return true;
        }

        void Checks()
        {
            if (sizeX == 0)
            {
                Debug.Log("Size X is 0, assinging min.");
                sizeX = 16;
            }

            if (sizeY == 0)
            {
                Debug.Log("Size Y is 0, assinging min.");
                sizeX = 1;
            }

            if (sizeZ == 0)
            {
                Debug.Log("Size z is 0, assinging min.");
                sizeZ = 1;
            }

            if (scaleXZ == 0)
            {
                Debug.Log("scale x z is 0, assinging min.");
                sizeZ = 1;
            }
            if (scaleY == 0)
            {
                Debug.Log("scale y is 0, assinging min.");
                sizeZ = 2;
            }
        }

        void CreateGrid()
        {
            grid = new Node[sizeX, sizeY, sizeZ];

            for (int y = 0; y < sizeY; y++)
            {
                YLevels ylvl = new YLevels();
                ylvl.nodeParent = new GameObject();
                ylvl.nodeParent.name = "level " + y.ToString();
                ylvl.y = y;
                yLevels.Add(ylvl);

                CreateCollision(y);

                for (int x = 0; x < sizeX; x++)
                {
                    for (int z = 0; z < sizeZ; z++)
                    {
                        Node node = new Node();
                        node.X = x;
                        node.Y = y;
                        node.Z = z;
                        node.ChangeNodeStatus(true, this);

                        if (debugNode)
                        {
                            Vector3 targetPos = GetWorldCoordinatesFromNode(x, y, z);

                            GameObject go = Instantiate(debugNodeObject, targetPos, Quaternion.identity) as GameObject;

                            go.transform.parent = ylvl.nodeParent.transform;
                            go.SetActive(true);
                            node.worldGameObjet = go;
                        }

                        grid[x, y, z] = node;

                    }
                }
            }
        }

        void CreateCollision(int y)
        {
            YLevels lvl = yLevels[y];
            GameObject go = new GameObject();

            BoxCollider box = go.AddComponent<BoxCollider>();

            box.size = new Vector3(sizeX * scaleXZ + (scaleXZ * 2), 0.2f, sizeZ * scaleXZ + (scaleXZ * 2));

            box.transform.position = new Vector3((sizeX * scaleXZ) * 0.5f - (scaleXZ * .5f),
                y * scaleY,
                (sizeZ * scaleXZ) * .5f - (scaleXZ * .5f));

            lvl.collisionObj = go;
            lvl.collisionObj.name = " lvl " + y + " collision";
        }

        public Vector3 GetWorldCoordinatesFromNode(int x, int y, int z)
        {
            Vector3 r = Vector3.zero;
            r.x = x * scaleXZ;
            r.y = y * scaleY;
            r.z = z * scaleXZ;
            return r;
        }

        GameObject WorldNode(bool isInEditMode = false)
        {
            GameObject go = new GameObject();
            GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);

            if (!isInEditMode)
                Destroy(quad.GetComponent<Collider>());
            else
                DestroyImmediate(quad.GetComponent<Collider>());

            quad.transform.parent = go.transform;
            quad.transform.localPosition = Vector3.zero;
            quad.transform.localEulerAngles = new Vector3(90, 0, 0);
            quad.transform.localScale = Vector3.one * 0.95f;
            quad.GetComponentInChildren<MeshRenderer>().material = debugMat;
            go.SetActive(false);
            return go;

        }

        public Node GetNode(int x, int y, int z, bool dontClamp = false)
        {
            if (!dontClamp)
            {
                x = Mathf.Clamp(x, 0, sizeX - 1);
                y = Mathf.Clamp(y, 0, sizeY - 1);
                z = Mathf.Clamp(z, 0, sizeZ - 1);
            }
            else
            {
                if (x > sizeX || x < 0 || y < 0 || y > sizeY || z < 0 || z > sizeZ)
                    return null;
            }
            return grid[x, y, z];

        }

        public Node GetNodeFromWorldPosition(Vector3 wp, bool dontClamp = false)
        {
            int x = Mathf.RoundToInt(wp.x / scaleXZ);
            int y = Mathf.RoundToInt(wp.y / scaleY);
            int z = Mathf.RoundToInt(wp.z / scaleXZ);

            return GetNode(x, y, z, dontClamp);
        }

        public void ClearLevel(bool inEdit = false)
        {
            for (int i = 0; i < yLevels.Count; i++)
            {
                if (inEdit)
                {
                    DestroyImmediate(yLevels[i].nodeParent);
                    DestroyImmediate(yLevels[i].collisionObj);
                }
                else
                {
                    Destroy(yLevels[i].nodeParent);
                    Destroy(yLevels[i].collisionObj);
                }
            }

            if (debugNodeObject)
            {

                if (inEdit)
                {
                    DestroyImmediate(debugNodeObject);
                }
                else
                {
                    Destroy(debugNodeObject);
                }
            }
            yLevels.Clear();
        }

    }

    [System.Serializable]
    public class YLevels
    {
        public int y;
        public GameObject nodeParent;
        public GameObject collisionObj;
    }
}