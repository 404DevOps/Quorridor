using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Wall : Draggable
{
    public bool isPlaced = false;
    public bool isHorizontal = true;

    internal DropZone lastHitLeft;
    internal DropZone lastHitRight;

    internal void Rotate()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (isHorizontal)
            {
                this.transform.Rotate(0, 0, 90);
            }
            else 
            {
                this.transform.Rotate(0, 0, -90);
            }

            isHorizontal = !isHorizontal;
            ResetRaycast();
        } 
    }

    public override void RayCastDropLocation()
    {
        //fix this mofucking cast, maybe also 2 raycast going trough the two bottom edges of the dragged wall
        var rightEdge = transform.position;
        rightEdge.y = transform.position.y - (transform.localScale.y / 2);
        var leftEdge = rightEdge;
        //adjust ray location depending on rotation
        if (isHorizontal)
        {
            rightEdge.x = rightEdge.x - (transform.localScale.x / 2);
            leftEdge.x = leftEdge.x + (transform.localScale.x / 2);
        }
        else
        {
            rightEdge.z = rightEdge.z + (transform.localScale.x / 2);
            leftEdge.z = leftEdge.z - (transform.localScale.x / 2);
        }

        var rightRay = Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(rightEdge));
        var leftRay = Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(leftEdge));

        //look only for certain rotation depending on wallrotation
        var layer = isHorizontal ? 7 : 8;
        int layermask = 1 << layer;


        Debug.DrawRay(rightRay.origin, rightRay.direction, Color.blue);
        Debug.DrawRay(leftRay.origin, leftRay.direction, Color.green);

        RaycastHit hitright;
        RaycastHit hitleft;

        bool didrightHit = Physics.Raycast(rightRay, out hitright, 10, layermask);
        bool didleftHit = Physics.Raycast(leftRay, out hitleft, 10, layermask);

        if (didrightHit && didleftHit)
        {
            lastHitRight = hitright.collider.gameObject.GetComponent<DropZone>();
            lastHitRight.RayCastEnter(this);
            lastHitLeft = hitleft.collider.gameObject.GetComponent<DropZone>();
            lastHitLeft.RayCastEnter(this);
        }
        else if (lastHitRight != null && lastHitLeft != null)
        {
            lastHitRight.RayCastExit();
            lastHitRight = null;
            lastHitLeft.RayCastExit();
            lastHitLeft = null;
        }


        //Debug.DrawRay(rightRay.origin, rightRay.direction, Color.green);


    }

    /// <summary>
    /// Tries to drop the Object
    /// </summary>
    /// <returns>True if Object was successfully dropped.</returns>
    public override bool DropObject()
    {
        if (lastHitLeft != null && lastHitRight != null)
        {
            if (lastHitLeft.IsValidDropLocation(this) && lastHitRight.IsValidDropLocation(this))
            {
                //drop locations need to be adjacent
                var diffX = lastHitRight.xPos - lastHitLeft.xPos;
                var diffY = lastHitRight.yPos - lastHitLeft.yPos;
                if (diffX > 1 || diffX < -1 || diffY > 1 || diffY < -1)
                {
                    AbortDrop();
                    return false;
                }

                var position = GetSnapPosition(lastHitLeft.xPos, lastHitLeft.yPos, lastHitRight.xPos, lastHitRight.yPos);

                if (IsThereWall(position))
                {
                    AbortDrop();
                    return false;
                }
                else 
                {
                    if (!SnapWallToGrid(position))
                        return false;
                    ResetRaycast();
                    Debug.Log("Walldrop is Valid");
 
                    return true;
                }

            }
        }

        AbortDrop();
        return false;
    }

    void AbortDrop()
    {
        transform.position = currentPosition;
        ResetRaycast();
        ResetWallRotation();
    }

    void ResetRaycast()
    {
        if (lastHitLeft != null)
        {
            lastHitLeft.RayCastExit();
            lastHitLeft = null;
        }
        if (lastHitRight != null)
        {
            lastHitRight.RayCastExit();
            lastHitRight = null;
        }
    }

    private Vector3 GetSnapPosition(int xLeft, int yLeft, int xRight, int yRight)
    {
        DropZone leftDrop = lastHitLeft;
        DropZone rightDrop = lastHitRight;

        if (isHorizontal)
        {
            var snapPosition = new Vector3();
            snapPosition.x = (rightDrop.transform.position.x + leftDrop.transform.position.x) / 2;
            snapPosition.z = leftDrop.transform.position.z;
            snapPosition.y = placedHeight;
            return snapPosition;
        }
        else
        {
            var snapPosition = new Vector3();
            snapPosition.z = (rightDrop.transform.position.z + leftDrop.transform.position.z) / 2;
            snapPosition.x = leftDrop.transform.position.x;
            snapPosition.y = placedHeight;
            return snapPosition;
        }
    }

    private bool SnapWallToGrid(Vector3 snapPosition)
    {
        DropZone leftDrop = lastHitLeft;
        DropZone rightDrop = lastHitRight;
        if (isHorizontal)
        {
            transform.position = snapPosition;

            var blockedLeft = new BlockedPath(new Coordinates(leftDrop.xPos, leftDrop.yPos), new Coordinates(leftDrop.xPos, leftDrop.yPos + 1));
            var blockedRight = new BlockedPath(new Coordinates(rightDrop.xPos, rightDrop.yPos), new Coordinates(rightDrop.xPos, rightDrop.yPos + 1));

            if (IsPlayerPathBlocked(blockedLeft, blockedRight))
            {
                return false;
            }
        }
        else 
        {
            transform.position = snapPosition;
            var blockedLeft = new BlockedPath(new Coordinates(leftDrop.xPos, leftDrop.yPos), new Coordinates(leftDrop.xPos + 1, leftDrop.yPos));
            var blockedRight = new BlockedPath(new Coordinates(rightDrop.xPos, rightDrop.yPos), new Coordinates(rightDrop.xPos + 1, rightDrop.yPos));

            if (IsPlayerPathBlocked(blockedLeft, blockedRight))
            {
                return false;
            }
        }
        isPlaced = true;
        return true;
    }

    private bool IsPlayerPathBlocked(BlockedPath blockedLeft, BlockedPath blockedRight)
    {
        Testing.ResetAllNodes();

        GameManager.Instance.blockedPaths.Add(blockedRight);
        GameManager.Instance.blockedPaths.Add(blockedLeft);

        bool hasBlockedPath = false;
        //check player 1
        var p1 = GameObject.Find("Player1").GetComponent<Player>();
        var p1EndZones = GetEndZonesForPlayer(1);
        var p1Path = GameManager.Instance.pathFinder.FindPath(new Coordinates(p1.currentX, p1.currentY), p1EndZones.ToArray());
        hasBlockedPath = p1Path != null ? false : true;
        //Testing.ShowPath(p1Path, 1);

        var p2 = GameObject.Find("Player2").GetComponent<Player>();
        var p2EndZones = GetEndZonesForPlayer(2);
        var p2Path = GameManager.Instance.pathFinder.FindPath(new Coordinates(p2.currentX, p2.currentY), p1EndZones.ToArray());
        //if not already blocked, check if player 2 is blocked
        if(!hasBlockedPath) hasBlockedPath = p2Path != null ? false : true;
        //Testing.ShowPath(p2Path, 2);

        if (hasBlockedPath)
        {
            GameManager.Instance.blockedPaths.Remove(blockedRight);
            GameManager.Instance.blockedPaths.Remove(blockedLeft);

            Debug.Log("Wall would Block Path");
            AbortDrop();
            return true;
        }

        return false;
    }

    private List<Coordinates> GetEndZonesForPlayer(int player)
    {
        int y;
        List<Coordinates> endZones = new List<Coordinates>();

        y = player == 1 ? 9 : 1;

        for (int x = 1; x <= 9; x++)
        {
            endZones.Add(new Coordinates(x, y));
        }

        return endZones;
    }

    bool IsThereWall(Vector3 position)
    {
        var walls = GameObject.FindGameObjectsWithTag("Wall");
        foreach (var wall in walls)
        {
            if (position == wall.transform.position)
            {
                Debug.Log("There is already a Wall on that Position");
                return true;
            }   
        }

        return false;
    }

    void ResetWallRotation()
    {
        if (CompareTag("Wall") && isHorizontal)
        {
            transform.Rotate(0, 0, -90);
            isHorizontal = false;
        }
    }

    public override bool CanObjectBeMoved()
    {
        if (isPlaced)
           return false;
        
        return base.CanObjectBeMoved();
    }
}
