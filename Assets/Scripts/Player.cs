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
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int layermask = 1 << 6;

        Debug.DrawRay(ray.origin, ray.direction, Color.red);
        if (Physics.Raycast(ray, out hit, 10f, layermask))
        {
            lastHitObject = hit.collider.gameObject.GetComponent<DropZone>(); ;
            lastHitObject.RayCastEnter(this);
        }
        else
        {
            if (lastHitObject != null)
            {
                lastHitObject.RayCastExit();
            }
        }
    }

    public override bool DropObject()
    {
        if (lastHitObject != null)
        {
            if (lastHitObject.IsValidDropLocation(this))
            {
                if (CompareTag("Player"))
                {
                    SnapPlayerToGrid(lastHitObject.xPos, lastHitObject.yPos);
                }
                if ((name == "Player1" && currentY == 9) || (name == "Player2" && currentY == 1))
                {
                    GameManager.Instance.GameOver(this.gameObject.name);
                }
                lastHitObject = null;
                return true;
            }
            else
            {
                //reset object if drop is not valid
                transform.position = currentPosition;
            }
        }

        //reset current object
        if (lastHitObject != null)
        {
            lastHitObject.RayCastExit();
            lastHitObject = null;
            transform.position = currentPosition;
        }
        else
        {
            transform.position = currentPosition;
        }

        return false;
    }

    private void SnapPlayerToGrid(int x, int y)
    {
        if (x == currentX && y == currentY)
        {
            return;
        }

        currentX = x;
        currentY = y;
        //only look for player dropzones
        var playerzonesParent = GameObject.Find("DropZonesPlayer");
        var dropZone = playerzonesParent.GetComponentsInChildren<DropZone>().FirstOrDefault(zone => zone.xPos == x && zone.yPos == y);

        Vector3 newPlayerPosition = dropZone.transform.position;
        newPlayerPosition.y = placedHeight;
        transform.position = newPlayerPosition;
        currentPosition = newPlayerPosition;
    }


}
