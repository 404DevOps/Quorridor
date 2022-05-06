using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    //lift and place
    public float liftHeight = 0.2f;
    public float placedHeight = 0.03f;

    public Vector3 offset;
    public Vector3 currentPosition;

    public int currentX;
    public int currentY;

    DropZone lastHitObject;

    //Which Player can move this Object
    public int Owner;

    #region Drag & Drop
    private void OnMouseDown()
    {
        if (!CanObjectBeMoved())
        {
            //TODO: play not allowed sound
            return;
        }

        GameManager.Instance.selectedObject = this.gameObject;
        currentPosition = transform.position;
        offset = transform.position - GetMouseWorldPos();

        //lift on mouse down
        this.transform.position = new Vector3(transform.position.x, liftHeight, transform.position.z);
    }

    private void OnMouseDrag()
    {
        if (!CanObjectBeMoved())
        {
            //TODO: play not allowed sound
            return;
        }

        //raycast to check for droplocation
        RayCastDropLocation();

        //follow mouse
        this.transform.position = GetMouseWorldPos() + offset;

        //rotate only if its a wall
        if (CompareTag("Wall"))
        {
            if (Input.GetMouseButtonDown(1))
            {
                this.transform.Rotate(0, 0, 90);
            }
        }
    }

    void RayCastDropLocation()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int layermask = 1 << 3;
        layermask = ~layermask;

        //Debug.DrawRay(ray,Color.red);
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

    private void OnMouseUp()
    {
        if (Owner != GameManager.Instance.currentPlayer)
        {
            //TODO: play not allowed sound
            return;
        }

        if (lastHitObject != null)
        {
            if (lastHitObject.IsValidDropLocation(this))
            {
                Debug.Log("Drop is Valid");
                //if successfully placed, next players turn
                if (CompareTag("Wall")) 
                {
                    gameObject.GetComponent<Wall>().isPlaced = true;
                }
                if (CompareTag("Player"))
                {
                    SnapPlayerToGrid(lastHitObject.xPos, lastHitObject.yPos);
                }
                if ((name == "Player1" && currentY == 9) || (name == "Player2" && currentY == 1))
                {
                    GameManager.Instance.GameOver(this.gameObject.name);
                }
                lastHitObject = null;
                //change turn after each move
                GameManager.Instance.NextPlayer();
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
    }

    void SnapPlayerToGrid(int x, int y)
    {
        if (x == currentX && y == currentY)
        {
            return;
        }

        currentX = x;
        currentY = y;
        var dropZone = FindObjectsOfType<DropZone>().FirstOrDefault(zone => zone.xPos == x && zone.yPos == y);
        Vector3 newPlayerPosition =  dropZone.transform.position;
        newPlayerPosition.y = placedHeight;
        transform.position = newPlayerPosition;
        currentPosition = newPlayerPosition;
    }
    private Vector3 GetMouseWorldPos()
    {
        var mousePos = Input.mousePosition;
        mousePos.z = Camera.main.WorldToScreenPoint(transform.position).z;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    #endregion

    #region Rules
    private bool CanObjectBeMoved()
    {
        if (!GameManager.Instance.IsGameRunning)
        {
            return false;
        }
        if (Owner != GameManager.Instance.currentPlayer)
        {
            return false;
        }
        if (CompareTag("Wall"))
        {
            if (GetComponent<Wall>().isPlaced)
                return false;
        }
        return true;
    }

    #endregion
}
