using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Transform lookAt;

    public Vector3 playerOnePos = new Vector3(0f, 0.6f, 0.2f);
    public Vector3 playerTwoPos = new Vector3(0f, 0.6f, -0.2f);

    public float rotationSpeed;
    public bool isPlayerOneTurn;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = playerOnePos;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.LookAt(lookAt);



        if (isPlayerOneTurn)
        {
            if (transform.position.z <= 0.2)
                RotateBoard();

        }
        else
        {
            if (transform.position.y >= -0.2)
                RotateBoard();
        }
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
