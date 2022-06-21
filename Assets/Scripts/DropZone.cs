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
            //same position as before or other player on that field
            if (IsPlayerOnField(xPos, yPos))
            {
                Debug.Log("Only one Player is allowed on each Field.");
                return false;
            }

            //calc distance from current position
            var diffX = drag.currentX - xPos;
            var diffY = drag.currentY - yPos;

            if (diffX > 1 || diffX < -1)
            {
                //if path is farther than one field, player needs to be in between
                if (!IsStraightJumpValid(drag, diffX, diffY))
                {
                    Debug.Log("Cannot move farther than one Unit X");
                    return false;
                }
            }

            if (diffY > 1 || diffY < -1)
            {
                if (!IsStraightJumpValid(drag, diffX, diffY))
                {
                    Debug.Log("Cannot move farther than one Unit Y");
                    return false;
                }
            }
            //cannot move diagonal
            if (diffY != 0 && diffX != 0)
            {
                //only valid if wall is behind opponent player
                if (!IsDiagonalJumpValid(drag, diffX, diffY))
                {
                        Debug.Log("Cannot Move Diagonal");
                        return false;
                }
            }

            if (PathFinder.IsPathBlocked(drag.currentX, drag.currentY, xPos, yPos))
            {
                Debug.Log("Path is blocked");
                return false;
            }
        }

        return true;
    }

    private bool IsDiagonalJumpValid(Draggable drag, int diffX, int diffY)
    {
        var playerSelf = drag.gameObject.GetComponent<Player>();
        var playerOther = FindObjectsOfType<Player>().Where(p => p.name != playerSelf.name).FirstOrDefault();

        var directionX = playerOther.currentX < playerSelf.currentX ? -1 : 1;
        directionX = playerOther.currentX == playerSelf.currentX ? 0 : directionX;

        if (directionX != 0)
        {
            if (directionX == diffX)
            {
                Debug.Log("Diagonal Jump in wrong Direction");
                return false;
            }
            if (!PathFinder.IsPathBlocked(playerOther.currentX, playerOther.currentY, playerOther.currentX + directionX, playerOther.currentY))
            {
                //diagonal jump only valid if theres a wall behind the other Player
                Debug.Log("Jump not valid, no wall behind other Player");
                return false;
            }
        }
        var directionY = playerOther.currentY < playerSelf.currentY ? -1 : 1;
        directionY = playerOther.currentY == playerSelf.currentY ? 0 : directionY;
        if (directionY != 0)
        {
            if (directionY == diffY)
            {
                Debug.Log("Diagonal Jump in wrong Direction");
                return false;
            }
            if (!PathFinder.IsPathBlocked(playerOther.currentX, playerOther.currentY, playerOther.currentX, playerOther.currentY + directionY))
            {
                Debug.Log("Diagonal Jump not valid, no wall behind other Player");
                return false;
            }
        }

        if (PathFinder.IsPathBlocked(playerOther.currentX,playerOther.currentY, xPos, yPos))
        {
            Debug.Log("Wall inbetween");
            return false;
        }
        return true;
    }

    private bool IsStraightJumpValid(Draggable drag, int diffX, int diffY)
    {

        var endZone = new Coordinates[1];
        endZone[0] = new Coordinates(xPos, yPos);

        var path = GameManager.Instance.pathFinder.FindPath(new Coordinates(drag.currentX, drag.currentY), endZone);
        //check if path is max 3 steps
        if (path.Count != 3)
        {
            Debug.Log("Jump not valid, Path is too long.");
            return false;
        }
        //check if player is between current pos and target pos
        if (!IsPlayerOnField(path[1].x, path[1].y))
        {
            Debug.Log("Jump not valid, no Player in between.");
            return false;
        }

        return true;
    }

    private bool IsPlayerOnField(int xPos, int yPos)
    {
        var pArray = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in pArray)
        {
            var p = player.GetComponent<Player>();
            if (p.currentX == xPos && p.currentY == yPos)
            {
                return true;
            }
        }

        return false;
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
