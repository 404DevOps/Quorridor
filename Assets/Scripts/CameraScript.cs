using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Transform lookAt;

    public Vector3 playerOnePos = new Vector3(0f, 0.6f, 0.2f);
    public Vector3 playerTwoPos = new Vector3(0f, 0.6f, -0.2f);

    public float rotationSpeed;
    public float scrollAmount = 0.05f;
    public bool isPlayerOneTurn;

    private Transform camBasePos;

    private Vector3 dragOrigin;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = playerOnePos;

    }

    // Update is called once per frame
    void Update()
    {
        this.transform.LookAt(lookAt);

        MoveCamera();

        //reset camera 
        if (Input.GetKeyDown(KeyCode.Space))
        {
            transform.position = playerOnePos;
            transform.LookAt(lookAt);
        }
    }

    private void MoveCamera()
    {
        //drag camera with right click
        if (Input.GetMouseButtonDown(1))
        {
            dragOrigin = Input.mousePosition;
            
        }
        if(!Input.GetMouseButton(1) || Input.GetMouseButton(0)) return;

        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        Vector3 move = new Vector3(pos.x, pos.y, 0);

        //transform.Translate(move, Space.World);

        transform.RotateAround(lookAt.position, Vector3.up, rotationSpeed * move.x);


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
