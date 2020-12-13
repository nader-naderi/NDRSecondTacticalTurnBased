using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NDR2ndTTB
{
    public class Node
    {
        //Node's position in grid.
        private int x;
        private int y;
        private int z;

        //Node's cost for pathfinding process.
        private float hCost;
        private float gCost;
       
        public float GCost { get => gCost; set => gCost = value; }
        public float HCost { get => hCost; set => hCost = value; }
        public float FCost { get => gCost + hCost; }

        public int X { get => x; set => x = value; }
        public int Y { get => y; set => y = value; }
        public int Z { get => z; set => z = value; }

        public Node parentNode;
        public bool IsWalkable { get; set; } = true;

        public void ChangeNodeStatus(bool value, GridBase gridBase)
        {
            if (worldGameObjet != null)
            {
                worldGameObjet.GetComponentInChildren<MeshRenderer>().material
                    = (value) ? gridBase.debugMat : gridBase.unwalkableMaterial;
            }

            IsWalkable = value;
        }

        //Refrence to a gameObject so we can see its world position in game world.
        public GameObject worldGameObjet;

        public ENodeType nodeType;

    }
}