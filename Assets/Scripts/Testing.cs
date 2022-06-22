using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Testing
{
    //private PathFinder pathfinder;
    // Start is called before the first frame update
    //void Start()
    //{
    //    pathfinder = new PathFinder();
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    if(Input.GetKeyDown(KeyCode.T))
    //    {
           
    //        Debug.Log("Finding path for Player One");

    //        var p = GameObject.Find("Player1").GetComponent<Player>();
    //        List<Coordinates> possibleEndCoordinates = new List<Coordinates>();
    //        for (int i = 1; i <= 9; i++)
    //        {
    //            possibleEndCoordinates.Add(new Coordinates(i, 9));
    //        }

    //        var path = pathfinder.FindPath(new Coordinates(p.currentX, p.currentY), possibleEndCoordinates.ToArray());

    //        if (path == null)
    //        {
    //            Debug.Log("No Path found");
    //            return;
    //        }

    //        ShowPath(path, 1);            
    //    }
    //}

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
