using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Level;

namespace NDR2ndTTB
{
    public class LevelManager : MonoBehaviour
    {
        #region Singleton
        public static LevelManager instance;
        private void Awake()
        {
            if (!instance) instance = this;
        }
        #endregion

        public List<Obstacle> obstacles = new List<Obstacle>();

        public void LoadObstacles(GridBase gridBase, bool isInEdit = false)
        {
            Obstacle[] allObstacle = GameObject.FindObjectsOfType<Obstacle>();
            obstacles.AddRange(allObstacle);

            foreach (Obstacle obstacle in obstacles)
            {
                BoxCollider bx = null;
                if (obstacle.mainRenderer)
                    bx = obstacle.mainRenderer.gameObject.AddComponent<BoxCollider>();
                if (bx)
                {
                    float halfX = bx.size.x * 0.5f;
                    float halfY = bx.size.y * 0.5f;
                    float halfZ = bx.size.z * 0.5f;

                    Vector3 center = obstacle.mainRenderer.bounds.center;
                    Vector3 from = obstacle.mainRenderer.bounds.min;

                    from.y = 0;

                    Vector3 to = obstacle.mainRenderer.bounds.max;
                    to.y = 0;

                    int stepX = Mathf.RoundToInt(Mathf.Abs(from.x - to.x) / gridBase.scaleXZ);
                    int stepZ = Mathf.RoundToInt(Mathf.Abs(from.z - to.z) / gridBase.scaleXZ);

                    for (int x = 0; x < stepX; x++)
                    {
                        for (int z = 0; z < stepZ; z++)
                        {
                            Vector3 tp = from;
                            tp.x += gridBase.scaleXZ * x;
                            tp.z += gridBase.scaleXZ * z;
                            //tp.y = obstacle.mainRenderer.transform.position.y;

                            Vector3 p = obstacle.mainRenderer.transform.InverseTransformPoint(tp) - bx.center;
                            tp.y = 0;

                            if (p.x < halfX && p.y < halfY && p.z < halfZ && p.x > -halfX && p.y > -halfY && p.z > -halfZ)
                            {
                                Node n = gridBase.GetNodeFromWorldPosition(tp);
                                n.ChangeNodeStatus(false, gridBase);
                            }
                        }
                    }
                    

                }
                if (isInEdit)
                {
                    DestroyImmediate(bx);
                }
                else
                {
                    Destroy(bx);
                }
            }

        }


    }
}