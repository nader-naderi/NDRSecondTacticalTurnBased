using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NDRPathfinder;
using NDR2ndTTB.UI;
using UnityEngine.EventSystems;

namespace NDR2ndTTB
{
    public class GameManager : MonoBehaviour
    {
        #region Singleton
        public static GameManager instance;
        private void Awake()
        {
            instance = this;
        }

        #endregion

        bool hasPath;

        public List<UnitController> units = new List<UnitController>();
        Dictionary<string, int> unitIndex = new Dictionary<string, int>();

        public UnitController curUnit;
        public bool movingPlayer;
        Node curNode;
        Node preNode;


        List<PathInfo> redInfo;
        List<PathInfo> pathsInfo;

        public Material blue, red;

        LineRenderer pathRedVisual;

        LineRenderer pathBlueVisual;
        GridBase grid;
        UIManager uIManager;

        public void Init()
        {
            grid = GridBase.instance;
            uIManager = UIManager.instance;



            GameObject blue = new GameObject();
            blue.name = "lvl visual blue";
            pathBlueVisual = blue.AddComponent<LineRenderer>();
            pathBlueVisual.startWidth = 0.2f;
            pathBlueVisual.endWidth = 0.2f;
            pathBlueVisual.material = this.blue;

            GameObject red = new GameObject();
            red.name = "lvl visual red";
            pathRedVisual = red.AddComponent<LineRenderer>();
            pathRedVisual.startWidth = 0.2f;
            pathRedVisual.endWidth = 0.2f;
            pathRedVisual.material = this.red;
            InitUnit();
        }

        void InitUnit()
        {
            
            for (int i = 0; i < units.Count; i++)
            {
                units[i].Init();
                uIManager.AddSmallPortrait(units[i].Stats);

                unitIndex.Add(units[i].Stats.CharID, i);
            }

           // uIManager.UpdateCharacterPanel(curUnit.Stats);
            uIManager.CharacterPanel.SetActive(false);
        }

        private void Update()
        {
            if (!GridBase.instance.isInited)
                return;


            bool isOverUIElement = EventSystem.current.IsPointerOverGameObject();

            FindNode();



           
            if (Input.GetMouseButtonDown(0) && !isOverUIElement)
            {
                UnitController hasUnit = NodeHasUnit(curNode);

                if (curUnit)
                {
                    if (curUnit.IsMoving())
                        return;
                }

                if (!hasUnit && curUnit)
                {
                    if (hasPath && pathsInfo != null)
                    {
                        curUnit.ConfirmOrder();
                        curUnit.AddPath(pathsInfo);
                    }
                   
                }
                else
                {
                    curUnit = hasUnit;

                    if (curUnit)
                    {
                        uIManager.UpdateCharacterPanel(curUnit.Stats);
                    }
                }
            }


            if (!curUnit) return;
            if (curUnit.IsMoving())
                return;
            PathFinder();
        }

        private void PathFinder()
        {
            if (preNode != curNode)
            {
                PathfindMaster.instance.RequestPathfind(curUnit.GetNode, curNode, PathfinderCallback);
            }

            preNode = curNode;


            if (hasPath && pathsInfo != null)
            {
                if (pathsInfo.Count > 0)
                {
                    pathBlueVisual.positionCount = pathsInfo.Count;
                    for (int i = 0; i < pathsInfo.Count; i++)
                    {
                        //Node n = pathsInfo[i];

                        //Vector3 p = grid.GetWorldCoordinatesFromNode(n.X, n.Y, n.Z);
                        pathBlueVisual.SetPosition(i, pathsInfo[i].targetPositon);
                    }
                }

                if (redInfo != null)
                {

                    if (redInfo.Count > 1)
                    {
                        pathRedVisual.positionCount = redInfo.Count;
                        
                        pathRedVisual.gameObject.SetActive(true);

                        for (int i = 0; i < redInfo.Count; i++)
                        {
                            pathRedVisual.SetPosition(i, redInfo[i].targetPositon);
                        }
                    }
                    else
                    {
                        pathRedVisual.gameObject.SetActive(false);
                    }
                }
            }
        }

        UnitController NodeHasUnit(Node n)
        {
            for (int i = 0; i < units.Count; i++)
            {
                Node un = units[i].GetNode;

                if (un.X == n.X && un.Y == n.Y && un.Z == n.Z)
                    return units[i];
            }

            return null;
        }

        void PathfinderCallback(List<Node> p)
        {
            int curAP = curUnit.GetActionPoint();
            int neededAP = 0;

            List<PathInfo> tp = new List<PathInfo>();
            PathInfo p1 = new PathInfo();
            p1.ap = 0;
            p1.targetPositon = curUnit.transform.position;
            tp.Add(p1);

            List<PathInfo> red = new List<PathInfo>();
            int baseAP = 2;
            int diag = 3; // Mathf.FloorToInt( baseAP / 2)

            if (curUnit.crouch)
            {
                baseAP = 4;
                diag = 6;
            }
            if (curUnit.prone)
            {
                baseAP = 6;
                diag = 7;
            }


            for (int i = 0; i < p.Count; i++)
            {
                Node n = p[i];
                Vector3 wp = grid.GetWorldCoordinatesFromNode(n.X, n.Y, n.Z);
                Vector3 dir = Vector3.zero;

                if (i == 0)
                    dir = GetPathDir(curUnit.GetNode, n);
                else
                    dir = GetPathDir(p[i - 1], p[i]);

                if (dir.x != 0 && dir.z != 0)
                    baseAP = diag;

                neededAP += baseAP;

                PathInfo pi = new PathInfo();
                pi.ap = baseAP;
                pi.targetPositon = wp;

                if(neededAP > curAP)
                {
                    if(red.Count == 0)
                    {
                        red.Add(tp[i]);
                    }

                    red.Add(pi);
                }
                else
                {
                    tp.Add(pi);
                }
            }

            UIManager.instance.UpdateActionPointsIndicator(neededAP);

            pathsInfo = tp;
            redInfo = red;
            hasPath = true;
        }

        Vector3 GetPathDir(Node n1, Node n2)
        {
            Vector3 dir = Vector3.zero;
            dir.x = n2.X - n1.X;
            dir.y = n2.Y - n1.Y;
            dir.z = n1.Z - n1.Z;

            return dir;
        }

        void FindNode()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                curNode = GridBase.instance.GetNodeFromWorldPosition(hit.point);
                
            }
        }

        public void EndTurn()
        {
            for (int i = 0; i < units.Count; i++)
            {
                units[i].EndTurn();
            }
        }

        public void ChangeStance(EStance targetStance)
        {
            if (!curUnit) return;

            EStance curStance = curUnit.GetStance;
            int targetPoints = 0;

            switch (curStance)
            {
                case EStance.Normal:
                    targetPoints = StaticFunctions.S_NormalToTarget(targetStance);
                    break;
                case EStance.Run:
                    targetPoints = StaticFunctions.S_NormalToTarget(targetStance);
                    break;
                case EStance.Crouch:
                    targetPoints = StaticFunctions.S_CrouchToTarget(targetStance);
                    break;
                case EStance.Prone:
                    targetPoints = StaticFunctions.S_ProneToTarget(targetStance);
                    break;
                default:
                    break;
            }

            if (curUnit.Stats.CurrentActionPoints < targetPoints)
                return;

            curUnit.Stats.CurrentActionPoints -= targetPoints;
            UIManager.instance.UpdateAponCharacterPanel(curUnit.Stats.CurrentActionPoints);
            curUnit.ChangeStance(targetStance);
            preNode = null;
        }

        public void ActivateUnitWithID(string id)
        {
            int index = ResourcesManager.instance.GetIndexFromString(unitIndex, id);
            if (index == -1)
                return;

            UnitController controller = units[index];
            uIManager.UpdateCharacterPanel(controller.Stats);
            curUnit = controller;
        }

        public bool IsCurrentUnit(UnitController con)
        {
            if (!curUnit) return false;

            if (curUnit.Equals(con))
                return true;

            return false;
        }


        //UnitController GetCharacter(string id)
        //{
        //    for (int i = 0; i < units.Count; i++)
        //    {

        //    }
        //}

    }
    public class PathInfo
    {
        public int ap;
        public Vector3 targetPositon;
    }
}