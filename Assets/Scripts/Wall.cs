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

    private Coordinates placedLeft;
    private Coordinates placedRight;

    private BlockedPath presaveBlockedLeft;
    private BlockedPath presaveBlockedRight;

    private Color defaultColor;
    private Renderer render;

    private bool snapped = false;

    private void Start()
    {
        render = GetComponent<Renderer>();
        defaultColor = render.material.color;
    }
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

            //place ghost wall
            SnapWall(true);
        }
        else if (lastHitRight != null && lastHitLeft != null)
        {
            lastHitRight.RayCastExit();
            lastHitRight = null;
            lastHitLeft.RayCastExit();
            lastHitLeft = null;

            //reset wall position
            UnsnapWall();
            Debug.Log("Not hitting Zones anymore");

        }
    }

    public void UnsnapWall()
    {
        TintWall(defaultColor.a, defaultColor);
        var pos = transform.position;
        pos.y = liftHeight;
        this.transform.position = pos;
        snapped = false;
    }

    public void SnapWall(bool isGhost)
    {
        bool invalidDrop = false;
        if (lastHitLeft.IsValidDropLocation(this) && lastHitRight.IsValidDropLocation(this))
        {
            //drop locations need to be adjacent
            var diffX = lastHitRight.xPos - lastHitLeft.xPos;
            var diffY = lastHitRight.yPos - lastHitLeft.yPos;
            if (diffX > 1 || diffX < -1 || diffY > 1 || diffY < -1)
            {
                Debug.Log("Dropzones not adjacent.");
                invalidDrop = true;
            }

            var position = GetSnapPosition(lastHitLeft.xPos, lastHitLeft.yPos, lastHitRight.xPos, lastHitRight.yPos);

            //snap to grid without actually placing it
            if (!SnapWallToGrid(position))
            {
                invalidDrop = true;
            }
            if (IsThereWall(lastHitLeft.xPos, lastHitLeft.yPos, lastHitRight.xPos, lastHitRight.yPos, position))
            {
                Debug.Log("There is already a Wall present.");
                invalidDrop = true;
            }
            //Tint snapped Wall accordingly
            if (invalidDrop)
            {
                Debug.Log("WallDrop is invalid.");
                TintWall(0.5f, Color.red);
                ResetRaycast();
            }
            else
            {
                TintWall(0.5f, Color.green);
                Debug.Log("WallDrop is valid.");
                snapped = true;
            }
        }
    }

    public void TintWall(float alpha, Color color)
    {
        var c = color;
        c.a = alpha;
        render.material.color = c;
    }

    /// <summary>
    /// Tries to drop the Object
    /// </summary>
    /// <returns>True if Object was successfully dropped.</returns>
    public override bool DropObject()
    {
        //only drop if snapped to a position
        if (snapped)
        {
            var snapPos = GetSnapPosition(lastHitLeft.xPos, lastHitLeft.yPos, lastHitRight.xPos, lastHitRight.yPos);
            if (SnapWallToGrid(snapPos))
            {
                //Reset Tint, set placed to true, set placement variables and add blocked paths
                TintWall(defaultColor.a, defaultColor);
                isPlaced = true;
                placedLeft = new Coordinates(lastHitLeft.xPos, lastHitLeft.yPos);
                placedRight = new Coordinates(lastHitRight.xPos, lastHitRight.yPos);

                GameManager.Instance.blockedPaths.Add(presaveBlockedLeft);
                GameManager.Instance.blockedPaths.Add(presaveBlockedRight);

                return true;
            }
        }

        AbortDrop();
        return false;
    }

    void AbortDrop()
    {
        TintWall(defaultColor.a, defaultColor);
        transform.position = currentPosition;
        presaveBlockedLeft = null;
        presaveBlockedRight = null;
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
        //isPlaced = true;
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
        Testing.ShowPath(p1Path, 1);

        var p2 = GameObject.Find("Player2").GetComponent<Player>();
        var p2EndZones = GetEndZonesForPlayer(2);
        var p2Path = GameManager.Instance.pathFinder.FindPath(new Coordinates(p2.currentX, p2.currentY), p1EndZones.ToArray());
        //if not already blocked, check if player 2 is blocked
        if(!hasBlockedPath) hasBlockedPath = p2Path != null ? false : true;
        Testing.ShowPath(p2Path, 2);

        //Remove anyways, only place in dropobject
        GameManager.Instance.blockedPaths.Remove(blockedRight);
        GameManager.Instance.blockedPaths.Remove(blockedLeft);

        if (hasBlockedPath)
        {
            Debug.Log("Wall would Block Path");
            //reset presaved position
            presaveBlockedLeft = null;
            presaveBlockedRight = null;
            return true;
        }
        else 
        {
            //presave position for later if Path is not blocked
            presaveBlockedLeft = blockedLeft;
            presaveBlockedRight = blockedRight;
            return false;
        }
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

    //bool IsThereWall(Vector3 position)
    //{
    //    //TODO, Check for Overlapping Walls.
    //    var walls = GameObject.FindGameObjectsWithTag("Wall");
    //    foreach (var wall in walls)
    //    {
    //        if (position == wall.transform.position)
    //        {
    //            Debug.Log("There is already a Wall on that Position");
    //            return true;
    //        }   
    //    }

    //    return false;
    //}
    bool IsThereWall(int xLeft, int yLeft, int xRight, int yRight, Vector3 snapPos)
    {
        var coordsLeft = new Coordinates(xLeft, yLeft);
        var coordsRight = new Coordinates(xRight, yRight);
        //TODO, Check for Overlapping Walls.
        var walls = GameObject.FindGameObjectsWithTag("Wall");
        foreach (var wall in walls)
        {
            if (snapPos == wall.transform.position && wall != this.gameObject)
                return true;
            var w = wall.GetComponent<Wall>();

            //check only for placed walls
            if (w.isPlaced)
            {
                //check if sides interfere with each other
                if (w.isHorizontal == isHorizontal)
                {
                    if (w.placedLeft.Equals(coordsLeft) || w.placedRight.Equals(coordsLeft))
                    {
                        Debug.Log("Left Edge interfered with placed Wall.");
                        return true;
                    }
                    if (w.placedLeft.Equals(coordsRight) || w.placedRight.Equals(coordsRight))
                    {
                        Debug.Log("Right Edge interfered with placed Wall.");
                        return true;
                    }
                }
                //check if this crosses a wall in diferent rotation
                else
                {
                    if (w.placedLeft.xPos == coordsLeft.xPos && w.placedLeft.yPos == (coordsLeft.yPos + 1) && w.placedRight.xPos == (coordsRight.xPos - 1) && w.placedRight.yPos == coordsRight.yPos)
                        return true;
                    if (w.placedLeft.xPos == coordsLeft.xPos && w.placedLeft.yPos == (coordsLeft.yPos - 1) && w.placedRight.xPos == (coordsRight.xPos + 1) && w.placedRight.yPos == coordsRight.yPos)
                        return true;
                }
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
