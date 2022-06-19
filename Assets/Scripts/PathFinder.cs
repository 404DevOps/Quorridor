using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinder
{
    private const int MOVE_COST = 10;
    private List<PathNode> grid;
    private List<PathNode> openList;
    private List<PathNode> closedList;

    public PathFinder()
    {
    }

    private void FillGrid()
    {
        grid = new List<PathNode>();
        var list = GameObject.FindGameObjectsWithTag("PlayerDropZone").ToList();
        foreach (var zone in list)
        {
            var dz = zone.GetComponent<DropZone>();
            grid.Add(new PathNode(dz.xPos, dz.yPos));
        }
    }

    public List<PathNode> FindPath(Coordinates start, Coordinates[] end)
    {
        if (grid == null)
            FillGrid();

        var endNodes = GetPathNodesFromCoordinates(end);
        var startNode = GetNode(start.xPos, start.yPos);

        //calculate endNodes
        foreach (PathNode node in endNodes)
        {
            node.gCost = 0;
            node.hCost = CalculateDistanceCost(startNode, node);
            node.CalculateFCost();
        }

        openList = new List<PathNode> { startNode };
        closedList = new List<PathNode>();

        for (int x = 1; x < 10; x++)
        {
            for (int y = 1; y < 10; y++)
            {
                PathNode pathNode = GetNode(x, y);
                pathNode.gCost = int.MaxValue;
                pathNode.CalculateFCost();
                pathNode.cameFromNode = null;
            }
        }

        var closestEndNode = GetLowestFCostNode(endNodes);

        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, closestEndNode);
        startNode.CalculateFCost();

        while (openList.Count > 0)
        {
            //something with node iteration is not quite right yet
            //Debug.LogError("Fix this");
            //return null;
            PathNode currentNode = GetLowestFCostNode(openList);
            
            if (currentNode.x == closestEndNode.x && currentNode.y == closestEndNode.y)
            {
                //reached final node
                return CalculatePath(currentNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (var neighbourNode in GetNeighbourList(currentNode))
            {
                if (closedList.Contains(neighbourNode)) continue;

                int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);
                if (tentativeGCost < neighbourNode.gCost) 
                {
                    neighbourNode.cameFromNode = currentNode;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.hCost = CalculateDistanceCost(neighbourNode, closestEndNode);
                    neighbourNode.CalculateFCost();

                    if (!openList.Contains(neighbourNode))
                        openList.Add(neighbourNode);
                }
            }
        }
        // out of nodes on openlist
        return null;
    }

    private List<PathNode> GetNeighbourList(PathNode currentNode)
    {
        List<PathNode> neighbourList = new List<PathNode>();

        //Left, only add if exists and not blocked
        if (currentNode.x - 1 >= 1 && !IsPathBlocked(currentNode.x -1, currentNode.y, currentNode.x,currentNode.y)) 
            neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y));
        //Right
        if (currentNode.x + 1 <= 9 && !IsPathBlocked(currentNode.x + 1, currentNode.y, currentNode.x, currentNode.y)) 
            neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y));
        //Down
        if (currentNode.y - 1 >= 1 && !IsPathBlocked(currentNode.x, currentNode.y -1, currentNode.x, currentNode.y)) 
            neighbourList.Add(GetNode(currentNode.x, currentNode.y -1));
        //Up
        if (currentNode.y + 1 <= 9 && !IsPathBlocked(currentNode.x, currentNode.y + 1, currentNode.x, currentNode.y)) 
            neighbourList.Add(GetNode(currentNode.x, currentNode.y + 1));

        return neighbourList;
    }

    PathNode GetNode(int x, int y)
    {
        return grid.FirstOrDefault(n => n.x == x && n.y == y);
    }

    private List<PathNode> CalculatePath(PathNode endNode)
    {
        List<PathNode> path = new List<PathNode>();
        path.Add(endNode);
        PathNode currentNode = endNode;
        while (currentNode.cameFromNode != null)
        {
            path.Add(currentNode.cameFromNode);
            currentNode = currentNode.cameFromNode;
        }

        path.Reverse();
        return path;
    }

    private List<PathNode> GetPathNodesFromCoordinates(Coordinates[] end)
    {
        var list = new List<PathNode>();
        foreach (var node in end)
        {
            list.Add(new PathNode(node.xPos, node.yPos));
        }
        return list;
    }

    private int CalculateDistanceCost(PathNode a, PathNode b)
    {
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);

        //node has lower cost if it moves trough player

        return MOVE_COST * (xDistance + yDistance);
    }

    private PathNode GetLowestFCostNode(List<PathNode> pathNodeList)
    {
        PathNode lowestFCostNode = pathNodeList[0];
        for (int i = 1; i < pathNodeList.Count; i++)
        {
            if (pathNodeList[i].fCost < lowestFCostNode.fCost)
            {
                lowestFCostNode = pathNodeList[i];
            }
        }

        return lowestFCostNode;
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
