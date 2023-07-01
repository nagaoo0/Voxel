using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitCamera : MonoBehaviour
{
    float mvtSpeed;
    public float mvtSpeedFast;
    public float mvtSpeedNormal;
    public float mvtTime;
    public float zoomSpeed;
    private float zoom;

    public Vector3 newposition;
    public Vector3 newCamZoom;
    public Quaternion newrotation;
    private float minXRotAngle = -80; //min angle around x axis
    private float maxXRotAngle = 80; // max angle around x axis

    //Mouse rotation related
    private float rotX  = 45; // around x
    private float rotY = 45; // around y
    public Camera cam;

    Vector3 mouseDragStart;
    Vector3 mouseDragEnd;

    // Start is called before the first frame update
    void Start()
    {
        newposition = transform.position;
        zoom = cam.transform.localPosition.z;
        newCamZoom = cam.transform.localPosition;
        newrotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMouseInput();
        HandleInput();
    }

    void HandleMouseInput()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetMouseButtonDown(2))
            {
                Plane plane = new Plane(Vector3.up, Vector3.zero);

                Ray ray = cam.ScreenPointToRay(Input.mousePosition);

                float entry;
                if (plane.Raycast(ray, out entry))
                {
                    mouseDragStart = ray.GetPoint(entry);
                }
            }
            if (Input.GetMouseButton(2))
            {
                Plane plane = new Plane(Vector3.up, Vector3.zero);

                Ray ray = cam.ScreenPointToRay(Input.mousePosition);

                float entry;
                if (plane.Raycast(ray, out entry))
                {
                    mouseDragEnd = ray.GetPoint(entry);

                    newposition = transform.position + (mouseDragStart - mouseDragEnd);
                }
            }
        }
        else
        {
            if (Input.GetMouseButton(2))
            {
                rotX += -Input.GetAxis("Mouse Y") * mvtSpeed; // around X
                rotY += Input.GetAxis("Mouse X") * mvtSpeed;
            }

            if (rotX < minXRotAngle)
            {
                rotX = minXRotAngle;
            }
            else if (rotX > maxXRotAngle)
            {
                rotX = maxXRotAngle;
            }
        }

        newrotation = Quaternion.Euler(rotX, rotY, 0);
        //newposition += transform.forward * Input.mouseScrollDelta.y * zoomSpeed * mvtSpeed;
        newCamZoom += new Vector3(0,0,Input.mouseScrollDelta.y * zoomSpeed * mvtSpeed );
    }

    void HandleInput()
    {
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            mvtSpeed = mvtSpeedFast;
        }
        else
            mvtSpeed = mvtSpeedNormal;

        if (Input.GetKey(KeyCode.W))
        {
            newposition += new Vector3(transform.forward.x, 0, transform.forward.z) * mvtSpeed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            newposition += new Vector3(transform.forward.x, 0, transform.forward.z) * -mvtSpeed;
        }
        if (Input.GetKey(KeyCode.A))
        {
            newposition += transform.right * -mvtSpeed;
        }
        if (Input.GetKey(KeyCode.D))
        {
            newposition += transform.right * mvtSpeed;
        }
        if (Input.GetKey(KeyCode.F))
        {
            newposition = Vector3.zero;
        }

        transform.position = Vector3.Lerp(
            transform.position,
            newposition,
            Time.deltaTime * mvtTime
        );

        
        cam.transform.localPosition = Vector3.Lerp(
            cam.transform.localPosition,
            newCamZoom,
            Time.deltaTime * mvtTime
        );


        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            newrotation,
            Time.deltaTime * mvtTime
        );

        cam.transform.LookAt(transform);
    }
}
