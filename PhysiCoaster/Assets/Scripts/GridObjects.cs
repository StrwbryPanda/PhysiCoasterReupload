using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GridObjectType
{
    Empty,
    Obstacle,
    Track
}
public class GridObjects: MonoBehaviour
{
    public GridObjectType goType;
    public float xOffset;
    public float yOffset;

    public int GetGridObjectType()
    {
        return (int)goType;
    }
}
