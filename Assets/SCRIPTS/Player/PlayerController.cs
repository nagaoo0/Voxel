using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 6;
    public float jumpSpeed = 8;
    public float gravity = 20;
    Vector3 moveDir = Vector3.zero;

    float yRotation = 0;
    float yRot;
    float xRot;
    public float sensitivity = 4;
    Camera cam;

    public bool canInteract = true;

    void Start()
    {
        cam = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        movePlayer();
        mouseLook();

        if (canInteract)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            //RAYCAST BLOCK INTERACTION HERE 

        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    void movePlayer()
    {
        CharacterController character = GetComponent<CharacterController>();

        //jump
        if (character.isGrounded)
        {
            moveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            moveDir = transform.TransformDirection(moveDir);
            moveDir *= moveSpeed;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                moveDir.y += jumpSpeed;
            }
        }

        moveDir.y -= gravity * Time.deltaTime;
        character.Move(moveDir * Time.deltaTime);
    }

    void mouseLook()
    {
        yRot = -Input.GetAxis("Mouse Y") * sensitivity;
        xRot = Input.GetAxis("Mouse X") * sensitivity;

        yRotation += yRot;
        yRotation = Mathf.Clamp(yRotation, -80, 80);

        if (xRot != 0)
        {
            transform.eulerAngles += new Vector3(0, xRot, 0);
        }
        if (xRot != 0)
        {
            cam.transform.eulerAngles = new Vector3(yRotation, transform.eulerAngles.y, 0);
        }

    }

}
