using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public struct PathNode {
    public int x;
    public int y;
    public int index;

    public float3 position;

    public int gCost;
    public int hCost;   //heuristic cost
    public int fCost;

    public bool isWalkable;

    public int parentIndex;

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    public void setIsWalkable(bool isWalkable)
    {
        this.isWalkable = isWalkable;
    }
}