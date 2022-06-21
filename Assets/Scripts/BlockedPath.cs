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

    public override string ToString()
    {
        return $"X = {xPos}, Y= {yPos}";
    }
    public override bool Equals(object obj)
    {
        if (obj is Coordinates)
        {
            var compCoords = (Coordinates)obj;
            if (this.xPos == compCoords.xPos && this.yPos == compCoords.yPos)
                return true;
            else
                return false;
        }
        return base.Equals(obj);
    }

}