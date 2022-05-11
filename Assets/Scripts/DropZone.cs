using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DropZone : MonoBehaviour
{
    public string dropZoneType;
    public int xPos;
    public int yPos;

    public void RayCastEnter(Draggable drag)
    {
        var renderer = GetComponent<Renderer>();
        if (IsValidDropLocation(drag))
        {
            renderer.material.SetColor("_Color", Color.green);
        }
        else 
        {
            renderer.material.SetColor("_Color", Color.red);
        }
    }

    public void RayCastExit()
    {
        var renderer = GetComponent<Renderer>();
        renderer.material.SetColor("_Color", Color.white);
    }

    public bool IsValidDropLocation(Draggable drag)
    {
        //wrong dropzone
        if (dropZoneType != drag.gameObject.tag)
        {
            Debug.Log("Wrong Dropzone Type");
            return false;
        }

        //Check Player specific Rules
        if (drag.gameObject.CompareTag("Player"))
        {
            //same position as before
            if (drag.currentX == xPos && drag.currentY == yPos)
            {
                Debug.Log("Same Location drop not valid");
                return false;
            }
            //only move one field from current position
            var diffX = drag.currentX - xPos;
            if (diffX > 1 || diffX < -1)
            {
                Debug.Log("Cannot move farther than one Unit X");
                return false;
            }
            var diffY = drag.currentY - yPos;
            if (diffY > 1 || diffY < -1)
            {
                Debug.Log("Cannot move farther than one Unit Y");
                return false;
            }
            //cannot move diagonal
            if (diffY != 0 && diffX != 0)
            {
                Debug.Log("Cannot Move Diagonal");
                return false;
            }

            if (PathFinder.IsPathBlocked(drag.currentX, drag.currentY, xPos, yPos))
            {
                Debug.Log("Path is blocked");
                return false;
            }
        }

        return true;
    }

    public void VisualizePathFinding(Color color)
    {
        var renderer = GetComponent<Renderer>();
        renderer.material.SetColor("_Color", color);
    }

    public void ResetColor()
    {
        var renderer = GetComponent<Renderer>();
        renderer.material.SetColor("_Color", Color.white);
    }
}
