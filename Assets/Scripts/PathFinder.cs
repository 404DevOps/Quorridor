using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinder
{
    private List<Coordinates> openList;
    private List<Coordinates> closedList;

    bool FindPath(Coordinates start, Coordinates end)
    {
        openList = new List<Coordinates>() { start };
        bool pathFound = false;

        Coordinates lastChecked;

        Coordinates nextCheck;
        Coordinates currentCheck = new Coordinates(start.xPos, start.yPos);
 

        while (!pathFound)
        {

        }

        return false;
    }

    public static bool IsPathBlocked(Coordinates from, Coordinates to)
    {
        return IsPathBlocked(from.xPos, from.yPos, to.xPos, to.yPos);
    }


    public static bool IsPathBlocked(int xFrom, int yFrom, int xTo, int yTo)
    {
        //check path blocked on way from left to right
        var block = GameManager.Instance.blockedPaths.FirstOrDefault(b => b.From.xPos == xFrom && b.From.yPos == yFrom && b.To.xPos == xTo && b.To.yPos == yTo);
        if (block != null)
            return true;
        //check  path blocked on way from right to left
        block = GameManager.Instance.blockedPaths.FirstOrDefault(b => b.From.xPos == xTo && b.From.yPos == yTo && b.To.xPos == xFrom && b.To.yPos == yFrom);
        if (block != null)
            return true;

        return false;
    }
}
