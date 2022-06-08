using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Transform lookAt;

    public Vector3 playerOnePos = new Vector3(0f, 0.6f, 0.2f);
    public Vector3 playerTwoPos = new Vector3(0f, 0.6f, -0.2f);

    public float rotationSpeed = 30f;
    public float scrollAmount = 0.05f;
    public bool isPlayerOneTurn;

    private Transform camBasePos; 

    // Start is called before the first frame update
    void Start()
    {
        transform.position = playerOnePos;
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.LookAt(lookAt);

        //drag camera with right click // only if no object is dragged
        if (Input.GetMouseButton(1) && !Input.GetMouseButton(0))
        {
            
            //see if mouse pos on screen is x+ or x-, rotate accordingly -> set rotation speed to mouse distance from screenmiddle
            
            var screenPos = Input.mousePosition.x - (Screen.width / 2);
            Debug.Log(screenPos);

            transform.RotateAround(lookAt.position, Vector3.up, rotationSpeed * screenPos * Time.deltaTime);
        }
        //set camera height with scroll wheel
        if (Input.mouseScrollDelta != new Vector2(0,0))
        {
            var currentPos = transform.position;

            currentPos.y += -Input.mouseScrollDelta.y * scrollAmount;

            //only set if within bounds
            if(currentPos.y < 0.8f && currentPos.y > 0.35)
                transform.position = currentPos;
        }

        //reset camera 
        if (Input.GetKeyDown(KeyCode.Space))
        {
            transform.position = playerOnePos;
            transform.LookAt(lookAt);

        }

        //if (isPlayerOneTurn)
        //{
        //    if (transform.position.z <= 0.2)
        //        RotateBoard();

        //}
        //else
        //{
        //    if (transform.position.y >= -0.2)
        //        RotateBoard();
        //}
    }

    public void RotateBoard()
    {
        transform.RotateAround(lookAt.position, Vector3.up, rotationSpeed * Time.deltaTime);
    }

    public void ChangePlayerPOV()
    {
        isPlayerOneTurn = !isPlayerOneTurn;
    }




}
