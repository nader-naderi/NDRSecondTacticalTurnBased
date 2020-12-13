using UnityEngine;
using System.Collections.Generic;
using NDR2ndTTB;

namespace NDRPathfinder
{
    /// <summary>
    /// Responsible for use A* Algorithm.
    /// </summary>
    public class Pathfinder
    {
        GridBase gridBase;
        public Node startPosition;
        public Node endPosition;

        public volatile bool jobDone = false;
        
        List<Node> foundPath;
        PathfindMaster.PathfindingJobComplete completeCallback;

        public Pathfinder(Node start, Node target, PathfindMaster.PathfindingJobComplete callback)
        {
            startPosition = start;
            endPosition = target;
            completeCallback = callback;
            gridBase = GridBase.instance;
        }

        /// <summary>
        /// method calls on requesting a new path.
        /// </summary>
        /// <returns></returns>
        public void FindPath()
        {
            foundPath = FindPathActual(startPosition, endPosition);

            jobDone = true;
        }

        public void NotifyComplete()
        {
            if(completeCallback != null)
            {
                completeCallback(foundPath);
            }
        }

        private List<Node> FindPathActual(Node startingNode, Node targetNode)
        {
            // A* Algorithm begins here ...
            List<Node> foundPath = new List<Node>();

            // nodes we need to check.
            List<Node> openSet = new List<Node>();

            // nodes we've already checking.
            HashSet<Node> closedSet = new HashSet<Node>();

            // add starting Node to open set.
            openSet.Add(startingNode);
            while (openSet.Count > 0)
            {
                Node currentNode = openSet[0];

                for (int i = 0; i < openSet.Count; i++)
                {
                    // we check the cost for the current node.
                    // we can have more options here, but right now, it's not necessary.
                    if(openSet[i].FCost < currentNode.FCost || 
                        (openSet[i].FCost.Equals(currentNode.FCost) && openSet[i].HCost < currentNode.HCost))
                    {
                        if(!currentNode.Equals(openSet[i]))
                        {
                            currentNode = openSet[i];
                        }
                    }

                }

                // we remove the current node form the open set and add to the closed set.
                openSet.Remove(currentNode);
                closedSet.Add(currentNode);
            
                //if the current node is the target node ...
                if(currentNode.Equals(targetNode))
                {
                    // we reached our destination. retrace it ...
                    foundPath = RetracePath(startPosition, currentNode);
                    break;
                }

                // but if we have not reached our target, then we need to start looking the neighbours.
                foreach (Node neighbour in GetNeighbours(currentNode, true))
                {
                    if(!closedSet.Contains(neighbour))
                    {
                        // create  a new movement cost for our neighbours.
                        float newMovementCostToNeighbour = currentNode.GCost + GetDistance(currentNode, neighbour);

                        // if it's lowerr than the neighbour cost ... 
                        if(newMovementCostToNeighbour < neighbour.GCost || !openSet.Contains(neighbour))
                        {
                            // we calculate the new costs.
                            neighbour.GCost = newMovementCostToNeighbour;
                            neighbour.HCost = GetDistance(neighbour, targetNode);

                            //Assign the parent node.
                            neighbour.parentNode = currentNode;

                            if(!openSet.Contains(neighbour))
                            {
                                openSet.Add(neighbour);
                            }
                        }
                    }
                }
            }

            return foundPath;

        }
        /// <summary>
        /// find the distance between two node.
        /// </summary>
        /// <param name="startingNode"></param>
        /// <param name="targetNode"></param>
        /// <returns> distance between two nodes </returns>
        private int GetDistance(Node startingNode, Node targetNode)
        {
            int distanceX = Mathf.Abs(startingNode.X - targetNode.X);
            int distanceY = Mathf.Abs(startingNode.Y - targetNode.Y);
            int distanceZ = Mathf.Abs(startingNode.Z - targetNode.Z);

            if(distanceX > distanceZ)
            {
                return 14 * distanceZ + 10 * (distanceX - distanceZ) + 10 * distanceY;
            }

            return 14 * distanceX + 10 * (distanceZ - distanceX) + 10 * distanceY;

        }


        private List<Node> GetNeighbours(Node node, bool getVerticalNeighbours = false)
        {
            List<Node> desiredNodesList = new List<Node>();

            for (int x = -1; x <= 1; x++)
            {
                for (int yIndex = -1; yIndex <= 1; yIndex++)
                {
                    for (int z = -1; z <= 1; z++)
                    {
                        int y = yIndex;

                        // if we don't want a 3D A*, then we don't search the y.
                        if(!getVerticalNeighbours)
                        {
                            y = 0;
                        }

                        if(x == 0 && y == 0 && z  == 0)
                        {
                            // 000 is the current node.
                        }
                        else
                        {
                            Node searchPos = new Node();

                            searchPos.X = node.X + x;
                            searchPos.Y = node.Y + y;
                            searchPos.Z = node.Z + z;

                            Node newNode = GetNeighbourNode(searchPos, true, node);
                            if(newNode != null)
                            {
                                desiredNodesList.Add(newNode);
                            }

                        }
                    }
                }
            }
            return desiredNodesList;
        }

        private Node GetNeighbourNode(Node adjacentPos, bool searchTopDown, Node currentNodePos)
        {
            // return value.
            Node desiredNode = null;

            // take the node from adjacent position that we passed.
            Node node = GetNode(adjacentPos.X, adjacentPos.Y, adjacentPos.Z);

            //if it's not null, it means we can walk on it.
            if(node != null && node.IsWalkable)
            {
                //we use that node.
                desiredNode = node;
            }//if not ...
            else if(searchTopDown) // we want 3D A*, right?
            {
                // look for adjacent have under it.
                adjacentPos.Y -= 1;
                Node bottomBlock = GetNode(adjacentPos.X, adjacentPos.Y, adjacentPos.Z);
                
                // if there is a bottom block that we acn walk on it ...
                if(bottomBlock != null && bottomBlock.IsWalkable)
                    desiredNode = bottomBlock;
                else // otherwise, we look what it has on top of it.
                {
                    adjacentPos.Y += 2;

                    Node topBlock = GetNode(adjacentPos.X, adjacentPos.Y, adjacentPos.Z);
                    if(topBlock != null && topBlock.IsWalkable)
                        desiredNode = topBlock;
                }
            }

            // if the ndoe is diagonal to the current node then check the neighbouring nodes.
            // so to move diagonally, we need to have 4 node walkable.
            int originalX = adjacentPos.X - currentNodePos.X;
            int originalZ = adjacentPos.Z - currentNodePos.Z;

            if(Mathf.Abs(originalX) == 1 && Mathf.Abs(originalZ) == 1)
            {
                Node neighbour1 = GetNode(currentNodePos.X + originalX, currentNodePos.Y, currentNodePos.Z);
                
                if (neighbour1 == null || !neighbour1.IsWalkable)
                    desiredNode = null;

                Node neighbour2 = GetNode(currentNodePos.X, currentNodePos.Y, currentNodePos.Z + originalZ);
                if (neighbour2 == null || !neighbour2.IsWalkable)
                    desiredNode = null;

            }

            // more additional checks.
            if(desiredNode != null)
            {
                // EXP : do not approach a node from left.(like a wall or car ... obstacle)
                /*if(node.X > currentNodePos.X)
                {
                    node = null;
                }*/
            }

            return desiredNode;
        }

        /// <summary>
        /// Going from start node to end node.
        /// </summary>
        /// <param name="startNode"></param>
        /// <param name="endNode"></param>
        /// <returns></returns>
        private List<Node> RetracePath(Node startNode, Node endNode)
        {
            List<Node> path = new List<Node>();
            Node currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);

                // by taking the parent node we assigned.
                currentNode = currentNode.parentNode;
            }

            // then simply reverse the list.
            path.Reverse();

            return path;
        }

        private Node GetNode(int x, int y, int z)
        {
            Node n = null;

            // this ensures that one thread is executing a piece of code at one time.
            // keep in mind, 
            // one thread does not enter a critical section of code while another thread is in that critical section.
            lock (gridBase)
            {
                n = gridBase.GetNode(x, y, z);
            }

            return n;
        }
    }
}