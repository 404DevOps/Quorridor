using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Draggable : MonoBehaviour
{
    //lift and place
    public float liftHeight = 0.2f;
    public float placedHeight = 0.03f;

    public Vector3 offset;
    public Vector3 currentPosition;

    public int currentX;
    public int currentY;

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
        if (CompareTag("Wall"))
        {
            GetComponent<Wall>().Rotate();
        }
    }

    private void OnMouseUp()
    {
        if (Owner != GameManager.Instance.currentPlayer)
        {
            //TODO: play not allowed sound
            return;
        }

        if (DropObject())
            GameManager.Instance.NextPlayer();
        
    }
   
    private Vector3 GetMouseWorldPos()
    {
        var mousePos = Input.mousePosition;
        mousePos.z = Camera.main.WorldToScreenPoint(transform.position).z;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    #endregion

    #region Overrides
    public virtual bool CanObjectBeMoved()
    {
        if (!GameManager.Instance.IsGameRunning)
        {
            return false;
        }
        if (Owner != GameManager.Instance.currentPlayer)
        {
            return false;
        }

        return true;
    }
    public abstract void RayCastDropLocation();

    public abstract bool DropObject();

    #endregion
}
