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

    // Start is called before the first frame update
    private void OnMouseDown()
    {
        if (Owner != GameManager.Instance.currentPlayer)
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
        if (Owner != GameManager.Instance.currentPlayer)
        {
            //TODO: play not allowed sound
            return;
        }

        this.transform.position = GetMouseWorldPos() + offset;
        Debug.Log(this.name + "is being dragged to " + GetMouseWorldPos());

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
}
