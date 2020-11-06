using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{

    //Position of start, in the future will be the position of the enemy.
    public Transform startPos;
    public LayerMask wallMask;
    public Vector2 gridWorldSize;
    public float nodeRadious;
    public float Distance;

    [Range(0, 1)]
    public float humanoidSize;

    Node[,] grid;
    public List<Node> FinalPath;

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start(){
        nodeDiameter = nodeRadious * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        CreateGrid();
    }

    private void CreateGrid(){
        grid = new Node[gridSizeX, gridSizeY];

        Vector3 bottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        for (int y = 0; y < gridSizeY; y++){
            for (int x = 0; x < gridSizeX; x++){
                Vector3 worldPoint = bottomLeft + Vector3.right * (x * nodeDiameter + nodeRadious) + Vector3.forward * (y * nodeDiameter + nodeRadious);
                bool wall = true;

                if (Physics.CheckSphere(worldPoint, nodeRadious + humanoidSize, wallMask)){
                    wall = false;
                }
                grid[x, y] = new Node(wall, worldPoint, x, y);
            }
        }
    }

    //Only for Debug, not visible in the game.
    private void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 0, gridWorldSize.y));

        if (grid != null){
            foreach (Node node in grid){
                if (node.isWall){
                    Gizmos.color = Color.white;
                }
                else {
                    Gizmos.color = Color.yellow;
                }

                if (FinalPath != null){
                    Gizmos.color = Color.red;
                }

                Gizmos.DrawWireCube(node.position, Vector3.one * (nodeDiameter - Distance));
            }
        }
    }
}
