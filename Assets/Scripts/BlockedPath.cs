using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockedPath 
{
    public BlockedPath(Coordinates from, Coordinates to)
    {
        From = from;
        To = to;
    }
    public Coordinates From;
    public Coordinates To;
}

public class Coordinates 
{
    public Coordinates(int x, int y)
    {
        xPos = x;
        yPos = y;
    }
    public int xPos;
    public int yPos;
}