using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : Draggable
{
    internal DropZone lastHitObject;

    public override void RayCastDropLocation()
    {
        RaycastHit hit;
        var screenPos = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        int layermask = 1 << 6;

        Debug.DrawRay(ray.origin, ray.direction, Color.red);
        if (Physics.Raycast(ray, out hit, 10f, layermask))
        {
            lastHitObject = hit.collider.gameObject.GetComponent<DropZone>(); ;
            lastHitObject.RayCastEnter(this);
            SnapPlayer();
        }
        else
        {
            if (lastHitObject != null)
            {
                lastHitObject.RayCastExit();
                UnsnapPlayer();
            }
        }
    }

    void AbortDrop()
    {
        TintObject(defaultColor.a, defaultColor);
        transform.position = currentPosition;

        if (lastHitObject != null)
        {
            SoundEngine.Instance.PlayBleep();
            lastHitObject.RayCastExit();
            lastHitObject = null;
        }
    }

    public override bool DropObject()
    {
        if (isSnapped)
        {
            if (lastHitObject != null)
            {
                //actually lock the snap
                var snapPos = GetSnapPosition(lastHitObject.xPos, lastHitObject.yPos);
                this.transform.localPosition = snapPos;

                currentX = lastHitObject.xPos;
                currentY = lastHitObject.yPos;
                currentPosition = snapPos;

                //reset color after drop
                TintObject(defaultColor.a, defaultColor);

                if ((name == "Player1" && currentY == 9) || (name == "Player2" && currentY == 1))
                {
                    GameManager.Instance.GameOver(this.gameObject.name);
                }
                lastHitObject = null;
                SoundEngine.Instance.PlayPlaced();
                return true;
            }
        }

        AbortDrop();
        return false;
    }

    private void UnsnapPlayer()
    {
        lastHitObject.RayCastExit();
        lastHitObject = null;
        TintObject(defaultColor.a, defaultColor);
        var pos = transform.position;
        pos.y = liftHeight;
        this.transform.position = pos;
        isSnapped = false;
    }

    private void SnapPlayer()
    {
        var drop = lastHitObject;
        bool invalidDrop = false;

        Vector3 snapPos = GetSnapPosition(drop.xPos, drop.yPos);
        transform.localPosition = snapPos;

        if (!lastHitObject.IsValidDropLocation(this))
        {
            invalidDrop = true;
        }
        if (drop.xPos == currentX && drop.yPos == currentY)
        {
            Debug.Log("Same position Drop not valid.");
            invalidDrop = true;
        }
        if (invalidDrop)
        {
            Debug.Log("Player drop invalid");
            TintObject(0.5f, Color.red);
            isSnapped = false;
        }
        else
        {
            TintObject(0.5f, Color.green);
            Debug.Log("Player drop is valid.");
            isSnapped = true;
        }
    }

    private Vector3 GetSnapPosition(int x, int y)
    {
        //only look for player dropzones
        var playerzonesParent = GameObject.Find("DropZonesPlayer");
        var dropZone = playerzonesParent.GetComponentsInChildren<DropZone>().FirstOrDefault(zone => zone.xPos == x && zone.yPos == y);

        Vector3 pos = dropZone.transform.position;
        pos.y = placedHeight;

        return pos;
    }

}
