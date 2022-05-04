using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    public float liftHeight = 0.2f;
    public Vector3 offset;
    Vector3 startPosition;

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
       

        startPosition = transform.position;
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

    private void OnMouseUp()
    {
        if (Owner != GameManager.Instance.currentPlayer)
        {
            //TODO: play not allowed sound
            return;
        }
        //Detect What Field we're on
        //check if dragged type is allowed to be placed
        //place or return to default position

        //if successfully placed, next players turn
        if (CompareTag("Wall"))
            gameObject.GetComponent<Wall>().isPlaced = true;

        GameManager.Instance.NextPlayer();

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
