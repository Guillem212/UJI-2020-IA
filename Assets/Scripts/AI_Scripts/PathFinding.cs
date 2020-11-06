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
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    private Grid grid;
    private PathNode[] pathNodeArray;
    private int2[] neighbourOffsetArray;

    public void Start()
    {
        grid = GetComponent<Grid>();
        

        neighbourOffsetArray = new int2[8];
        neighbourOffsetArray[0] = new int2(-1, 0);    //Left
        neighbourOffsetArray[1] = new int2(+1, 0);    //Right
        neighbourOffsetArray[2] = new int2(0, +1);    //Up
        neighbourOffsetArray[3] = new int2(0, -1);    //Down
        neighbourOffsetArray[4] = new int2(-1, -1);   //Left Down
        neighbourOffsetArray[5] = new int2(-1, +1);   //Left Up
        neighbourOffsetArray[6] = new int2(+1, -1);   //Right Down
        neighbourOffsetArray[7] = new int2(+1, +1);   //Right Up
    }

    public List<PathNode> CalculatePath(Vector3 start, Vector3 end)
    {
        pathNodeArray = grid.GetPathNodeArray();

        PathNode sN = grid.NodeFromWorld(start);
        PathNode eN = grid.NodeFromWorld(end);

        int2 startPosition = new int2(sN.x, sN.y);
        int2 endPosition = new int2(eN.x, eN.y);

        int endNodeIndex = CalculateIndex(endPosition.x, endPosition.y, grid.gridSizeX);

        //Set starting node with 0 G cost so the algorithim can run
        PathNode startNode = pathNodeArray[CalculateIndex(startPosition.x, startPosition.y, grid.gridSizeX)];
        startNode.gCost = 0;
        startNode.CalculateFCost();
        pathNodeArray[startNode.index] = startNode;

        List<int> openList = new List<int>();
        List<int> closedList = new List<int>();

        openList.Add(startNode.index);

        while (openList.Count > 0)
        {
            int currentNodeIndex = GetLowestCostFNodeIndex(openList, pathNodeArray);
            PathNode currentNode = pathNodeArray[currentNodeIndex];

            //Destination reached
            if (currentNodeIndex == endNodeIndex)
            {
                break;
            }

            //Remove current node from the openList
            for (int i = 0; i < openList.Count; i++)
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
                    neighbourNode.hCost = CalculateDistanceCost(neighbourPosition, endPosition);
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
        List<PathNode> path = new List<PathNode>();
        List<PathNode> pathSorted = new List<PathNode>();
        if (endNode.parentIndex == -1)
        { //Didn't find a path
            //Debug.Log("Path not found!");
        }
        else
        {   //Path found
            path = ReturnPath(pathNodeArray, endNode);
            for (int i = path.Count - 1; i >= 0; i--)
            {
                //Debug.Log(path[i].x + " " + path[i].y);
                pathSorted.Add(path[i]);
            }
        }
        //Debug.Log("Path found!");
        return pathSorted;
    }
    


    /// <summary>
    /// Caculates the path of a given goal(endNode) in the array passed.
    /// </summary>
    /// <returns>Returns an empty list if couldn't reach the path, and a list that starts from the endNode to the starting position if find the path.</returns>
    private List<PathNode> ReturnPath(PathNode[] pathNodeArray, PathNode endNode)
    {
        if (endNode.parentIndex == -1)
        { //path not found
            return null;    //This is and empty List, you can also return null
        }
        else
        {   //path found, goes backwards picking the parents nodes
            List<PathNode> path = new List<PathNode>();
            path.Add(endNode);
            PathNode currentNode = endNode;
            while (currentNode.parentIndex != -1)
            {
                PathNode parentNode = pathNodeArray[currentNode.parentIndex];
                Vector2 direction = new Vector2(parentNode.x, parentNode.y) - new Vector2(currentNode.x, currentNode.y);
                direction.Normalize();
                Vector2 auxDir = new Vector2();
                if(parentNode.parentIndex != -1){
                    auxDir = new Vector2(pathNodeArray[parentNode.parentIndex].x, pathNodeArray[parentNode.parentIndex].y) - new Vector2(parentNode.x, parentNode.y);
                    auxDir.Normalize();
                }

                if(direction != auxDir)
                    path.Add(parentNode);
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
    private int GetLowestCostFNodeIndex(List<int> openList, PathNode[] pathNodeArray)
    {
        PathNode lowestCostPathNode = pathNodeArray[openList[0]];
        for (int i = 1; i < openList.Count; i++)
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
