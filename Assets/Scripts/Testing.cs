using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Testing
{
    public static void ShowPath(List<PathNode> path, int player)
    {
        if (path == null || path.Count == 0)
        {
            //no path to draw
            return;
        }

        Color color = player == 1 ? Color.yellow : Color.blue;
        var pZones = GameObject.FindGameObjectsWithTag("PlayerDropZone");
        var dropzones = new List<DropZone>();
        foreach (var zone in pZones)
        {
            dropzones.Add(zone.GetComponent<DropZone>());
        }
        foreach (var node in path)
        {
            dropzones.FirstOrDefault(d => d.xPos == node.x && d.yPos == node.y).VisualizePathFinding(color);
        }
    }

    public static void ResetAllNodes()
    {
        foreach (var node in GameObject.FindGameObjectsWithTag("PlayerDropZone"))
        {
            node.GetComponent<DropZone>().ResetColor();
        }
    }
}
