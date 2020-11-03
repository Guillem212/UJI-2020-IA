using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;

public class Grid : MonoBehaviour
{
    public LayerMask wallMask;
    public Vector2 gridWorldSize;
    public float nodeRadious;

    [Range(0, 1)]
    public float humanoidSize;

    private PathNode[] pathNodeArray;

    float nodeDiameter;
    public int gridSizeX, gridSizeY;


    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Awake(){
       
        nodeDiameter = nodeRadious * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        float3 bottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
        pathNodeArray = new PathNode[gridSizeX * gridSizeY];
        
        CreateGrid(new int2(gridSizeX, gridSizeY), ref pathNodeArray);
    }
    public PathNode[] GetPathNodeArray()
    {
        return pathNodeArray;
    }

    /// <summary>
    /// Creates the grid and populates it with nodes.
    /// </summary>
    /// <returns>Returns an array with the nodes.</returns>
    private void CreateGrid(int2 gridSize, ref PathNode[] pathNodeArray){
        float3 bottomLeft = (float3)transform.position - new float3(1, 0, 0) * gridWorldSize.x / 2 - new float3(0, 0, 1) * gridWorldSize.y / 2;

        for (int y = 0; y < gridSizeY; y++){
            for (int x = 0; x < gridSizeX; x++){
                float3 worldPoint = bottomLeft + new float3(1, 0, 0) * (x * nodeDiameter + nodeRadious) + new float3(0, 0, 1) * (y * nodeDiameter + nodeRadious);
                bool wall = true;

                if (Physics.CheckSphere(worldPoint, nodeRadious + humanoidSize, wallMask)){
                    wall = false;
                }
                PathNode pathNode = new PathNode();
                pathNode.x = x;
                pathNode.y = y;
                pathNode.index = CalculateIndex(x, y, gridSize.x);

                pathNode.gCost = int.MaxValue;
                pathNode.CalculateFCost();

                pathNode.isWalkable = wall;
                pathNode.parentIndex = -1;

                pathNode.position = worldPoint;

                pathNodeArray[pathNode.index] = pathNode;
            }
        }
    }

    /// <summary>
    /// Calculates the index of the node based in his position
    /// </summary>
    /// <returns>Returns an integer based on the passed value.</returns>
    private int CalculateIndex(int x, int y, int gridWidth)
    {
        return x + y * gridWidth;
    }

    public PathNode NodeFromWorld(Vector3 worldPosition){
        float percentX = (worldPosition.x + gridWorldSize.x/2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y/2) / gridWorldSize.y;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX-1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY-1) * percentY);
        return pathNodeArray[CalculateIndex(x, y, gridSizeX)];
    }

    /// <summary>
    /// Callback to draw gizmos that are pickable and always drawn.
    /// </summary>
    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 0, gridWorldSize.y));
        if(pathNodeArray != null){
            foreach (PathNode node in pathNodeArray)
            {
                if(node.isWalkable){
                    Gizmos.color = Color.white;
                }
                else{
                    Gizmos.color = Color.yellow;
                }
                Gizmos.DrawWireCube(node.position, new Vector3(nodeDiameter, 0, nodeDiameter));
            }
        }
    }
}
