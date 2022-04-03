using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GridObjectType
{
    Empty,
    Obstacle,
    Track
}

public struct Coordinate
{
    public int x;
    public int y;
}

public class GridObjects : MonoBehaviour
{
    public GridObjectType goType;
    public float xOffset;
    public float yOffset;
    private Coordinate rootLocationOnGrid;
    public Vector2[] tilesOccupied;//a collection of coordinates, representing which tiles are occupied,
    //with each the coordinates showing the offset from the root.The root should always be the first in the array, with the value of (0,0)
    public int GetGridObjectType()
    {
        return (int)goType;
    }

    public void SetRootLocation(int x, int y)
    {
        rootLocationOnGrid.x = x;
        rootLocationOnGrid.y = y;
    }

    public int GetRootX()
    {
        return rootLocationOnGrid.x;
    }

    public int GetRootY()
    {
        return rootLocationOnGrid.y;
    }

}
