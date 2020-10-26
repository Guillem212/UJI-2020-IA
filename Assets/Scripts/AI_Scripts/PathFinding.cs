using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;

public class PathFinding : MonoBehaviour
{
    //contamos con que los grid son de tamaño 1x1 por lo que ir de uno a otro seria coste 1 (Straight cost)
    // y en diagonal seria raiz de 2 (Diagonal cost) que es aproximadamente 1.4, lo multiplicamos por 10 para quitarnos floats.
    // y diras, y esto que hace en español, esque era mucho mucho texto luego lo cambio que es tarde y tinc som
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    public Grid grid;
    private PathNode[] pathNodeArray;

    public Transform startPosition;

    public void Start()
    {
        //grid = GetComponent<Grid>();
        pathNodeArray = grid.GetPathNodeArray();
    }

    private void Update()
    {
        PathNode nodePos = grid.NodeFromWorld(startPosition.position);

        FindPath(new int2(nodePos.x, nodePos.y), new int2(grid.gridSizeX - 1, grid.gridSizeY - 1));
    }


    public void FindPath(int2 startPosition, int2 endPosition)
    {
        for (int i = 0; i < pathNodeArray.Length; i++)
        {
            PathNode nodeAux = pathNodeArray[i];
            nodeAux.hCost = CalculateDistanceCost(new int2(pathNodeArray[i].x, pathNodeArray[i].y), endPosition);
            pathNodeArray[i] = nodeAux;
        }

        NativeArray<int2> neighbourOffsetArray = new NativeArray<int2>(8, Allocator.Temp);
        neighbourOffsetArray[0] = new int2(-1, 0);    //Left
        neighbourOffsetArray[1] = new int2(+1, 0);    //Right
        neighbourOffsetArray[2] = new int2(0, +1);    //Up
        neighbourOffsetArray[3] = new int2(0, -1);    //Down
        neighbourOffsetArray[4] = new int2(-1, -1);   //Left Down
        neighbourOffsetArray[5] = new int2(-1, +1);   //Left Up
        neighbourOffsetArray[6] = new int2(+1, -1);   //Right Down
        neighbourOffsetArray[7] = new int2(+1, +1);   //Right Up

        int endNodeIndex = CalculateIndex(endPosition.x, endPosition.y, grid.gridSizeX);

        //Set starting node with 0 G cost so the algorithim can run
        PathNode startNode = pathNodeArray[CalculateIndex(startPosition.x, startPosition.y, grid.gridSizeX)];
        startNode.gCost = 0;
        startNode.CalculateFCost();
        pathNodeArray[startNode.index] = startNode;

        NativeList<int> openList = new NativeList<int>(Allocator.Temp);
        NativeList<int> closedList = new NativeList<int>(Allocator.Temp);

        openList.Add(startNode.index);

        while (openList.Length > 0)
        {
            int currentNodeIndex = GetLowestCostFNodeIndex(openList, pathNodeArray);
            PathNode currentNode = pathNodeArray[currentNodeIndex];

            //Destination reached
            if (currentNodeIndex == endNodeIndex)
            {
                break;
            }

            //Remove current node from the openList
            for (int i = 0; i < openList.Length; i++)
            {
                if (openList[i] == currentNodeIndex)
                {
                    openList.RemoveAtSwapBack(i);
                    break;
                }
            }

            closedList.Add(currentNodeIndex);

            for (int i = 0; i < neighbourOffsetArray.Length; i++)
            {
                int2 neighbourOffset = neighbourOffsetArray[i];
                int2 neighbourPosition = new int2(currentNode.x + neighbourOffset.x, currentNode.y + neighbourOffset.y);

                //Validates neighbour of current Node
                if (!IsPositionInsideGrid(neighbourPosition, new int2(grid.gridSizeX, grid.gridSizeY)))
                { //Cheks if is on grid
                    continue;
                }

                int neighbourNodeIndex = CalculateIndex(neighbourPosition.x, neighbourPosition.y, grid.gridSizeX);

                if (closedList.Contains(neighbourNodeIndex))
                { //Cheks if has already searched this node
                    continue;
                }

                PathNode neighbourNode = pathNodeArray[neighbourNodeIndex];
                if (!neighbourNode.isWalkable)
                { //Cheks if is Walkable
                    continue;
                }

                int2 currentNodePosition = new int2(currentNode.x, currentNode.y);

                //Algorithm that updates the cost and the path to follow.
                int tentativeGcost = currentNode.gCost + CalculateDistanceCost(currentNodePosition, neighbourPosition);
                if (tentativeGcost < neighbourNode.gCost)
                {
                    neighbourNode.parentIndex = currentNodeIndex;
                    neighbourNode.gCost = tentativeGcost;
                    neighbourNode.CalculateFCost();
                    pathNodeArray[neighbourNodeIndex] = neighbourNode;

                    if (!openList.Contains(neighbourNode.index))
                    {
                        openList.Add(neighbourNode.index);
                    }
                }

            }
        }


        PathNode endNode = pathNodeArray[endNodeIndex];
        if (endNode.parentIndex == -1)
        { //Didn't find a path
            Debug.Log("Path not found!");
        }
        else
        {   //Path found
            List<float3> path = CalculatePath(pathNodeArray, endNode);

             /*foreach (float3 pathPosition in path)
             { //Debug the path backwards
                 Debug.Log(pathPosition);  //Cant debug this with burst activated
             }*/
            Debug.Log("Path found!");
            grid.setPath(path);

        }

        neighbourOffsetArray.Dispose();
        openList.Dispose();
        closedList.Dispose();
    }
    


    /// <summary>
    /// Caculates the path of a given goal(endNode) in the array passed.
    /// </summary>
    /// <returns>Returns an empty list if couldn't reach the path, and a list that starts from the endNode to the starting position if find the path.</returns>
    private List<float3> CalculatePath(PathNode[] pathNodeArray, PathNode endNode)
    {
        if (endNode.parentIndex == -1)
        { //path not found
            return new List<float3>();    //This is and empty List, you can also return null
        }
        else
        {   //path found, goes backwards picking the parents nodes
            List<float3> path = new List<float3>();
            path.Add(endNode.position);
            PathNode currentNode = endNode;
            while (currentNode.parentIndex != -1)
            {
                PathNode parentNode = pathNodeArray[currentNode.parentIndex];
                path.Add(parentNode.position);
                currentNode = parentNode;
            }
            return path;
        }
    }
    /// <summary>
    /// Returns true if the given position is inside the grid
    /// </summary>
    /// <returns>Returns a bool based on the passed value.</returns>
    private bool IsPositionInsideGrid(int2 gridPosition, int2 gridSize)
    {
        return gridPosition.x >= 0 && gridPosition.y >= 0 && gridPosition.x < gridSize.x && gridPosition.y < gridSize.y;
    }

    /// <summary>
    /// Calculates the index of the node based in his position
    /// </summary>
    /// <returns>Returns an integer based on the passed value.</returns>
    private int CalculateIndex(int x, int y, int gridWidth)
    {
        return x + y * gridWidth;
    }

    /// <summary>
    /// Calculates the distance cost between 2 positions.
    /// </summary>
    /// <returns>Returns an integer based on the passed values.</returns>
    private int CalculateDistanceCost(int2 aPosition, int2 bPosition)
    {
        int xDistance = math.abs(aPosition.x - bPosition.x);
        int yDistance = math.abs(aPosition.y - bPosition.y);
        int remaining = math.abs(xDistance - yDistance);
        return MOVE_DIAGONAL_COST * math.min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    /// <summary>
    /// It compares all the nodes passed in the array and returns the one with the lowest f cost.
    /// </summary>
    /// <returns>Returns an integer, which is the index of the lowest f cost node.</returns>
    private int GetLowestCostFNodeIndex(NativeList<int> openList, PathNode[] pathNodeArray)
    {
        PathNode lowestCostPathNode = pathNodeArray[openList[0]];
        for (int i = 1; i < openList.Length; i++)
        {
            PathNode testPathNode = pathNodeArray[openList[i]];
            if (testPathNode.fCost < lowestCostPathNode.fCost)
            {
                lowestCostPathNode = testPathNode;
            }
        }
        return lowestCostPathNode.index;
    }
}
