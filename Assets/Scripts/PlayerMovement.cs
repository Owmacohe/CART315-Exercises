using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float movementSpeed = 10;
    public float mouseRotationSpeed = 2;
    public float joystickRotationSpeed = 0.7f;
    public float jumpHeight = 5;

    private enum inputTypes { None, Keyboard, Controller }
    private inputTypes inputType = inputTypes.None;
    private inputTypes lastInputType = inputTypes.None;
    private enum controllerTypes { None, Xbox, PS }
    private controllerTypes controllerType = controllerTypes.None;
    private int controllerCount, lastControllerCount;

    private Rigidbody rb;
    private GroundChecker gc;
    private bool isOnGround, lastIsOnGround, isOnWall, isJumping;
    [HideInInspector]
    public bool isMovementPaused;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        gc = GetComponentInChildren<GroundChecker>();

        checkInputType();
        checkControllerType();
    }

    private void Update()
    {
        if (rb.velocity.y >= 5)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        }

        Vector2 mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector2 controllerInputRight = new Vector2(Input.GetAxis("VerticalRight"), Input.GetAxis("HorizontalRight"));

        if (!mouseInput.Equals(Vector2.zero))
        {
            inputType = inputTypes.Keyboard;

            if (!isMovementPaused)
            {
                transform.Rotate(0, mouseInput.x * mouseRotationSpeed, 0, Space.World);
            }
        }

        if (!controllerInputRight.Equals(Vector2.zero))
        {
            inputType = inputTypes.Controller;

            if (!isMovementPaused)
            {
                transform.Rotate(0, controllerInputRight.x * joystickRotationSpeed, 0, Space.World);
            }
        }
    }

    private void FixedUpdate()
    {
        checkControllerCount();

        if (lastControllerCount != controllerCount)
        {
            if (controllerCount < lastControllerCount)
            {
                inputType = inputTypes.Keyboard;
            }
            else
            {
                inputType = inputTypes.Controller;
            }

            lastControllerCount = controllerCount;
        }

        isOnGround = gc.isOnGround;

        if (isOnGround)
        {
            isOnWall = false;
            rb.drag = 3;
        }

        if (lastIsOnGround != isOnGround)
        {
            if (isOnGround)
            {
                isJumping = false;
            }

            lastIsOnGround = isOnGround;
        }

        Vector2 controllerInputLeft = new Vector2(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            movePlayer(inputTypes.Keyboard, transform.forward);
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            movePlayer(inputTypes.Keyboard, -transform.forward);
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            movePlayer(inputTypes.Keyboard, transform.right);
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            movePlayer(inputTypes.Keyboard, -transform.right);
        }

        if (Input.GetKey(KeyCode.Space) && isOnGround && !isJumping)
        {
            inputType = inputTypes.Keyboard;

            isJumping = true;

            if (!isMovementPaused)
            {
                Invoke("limitDrag", jumpHeight / 30);

                rb.velocity += Vector3.up * jumpHeight;
            }
        }

        if (controllerInputLeft.x > 0)
        {
            movePlayer(inputTypes.Controller, transform.forward);
        }

        if (controllerInputLeft.x < 0)
        {
            movePlayer(inputTypes.Controller, -transform.forward);
        }

        if (controllerInputLeft.y > 0)
        {
            movePlayer(inputTypes.Controller, transform.right);
        }

        if (controllerInputLeft.y < 0)
        {
            movePlayer(inputTypes.Controller, -transform.right);
        }

        if (Input.GetButton("Jump") && isOnGround && !isJumping)
        {
            inputType = inputTypes.Controller;

            isJumping = true;

            if (!isMovementPaused)
            {
                Invoke("limitDrag", jumpHeight / 30);

                rb.velocity += Vector3.up * jumpHeight;
            }
        }

        //print(Time.time + " " + isOnGround);
        //print("<b>[INPUT TYPE]:</b> " + inputType);
    }

    private void checkInputType()
    {
        string[] controllerNames = Input.GetJoystickNames();

        foreach (string i in controllerNames)
        {
            if (i.Length > 0)
            {
                inputType = inputTypes.Controller;
            }
        }

        if (!inputType.Equals(inputTypes.Controller))
        {
            inputType = inputTypes.Keyboard;
        }
    }

    private void checkControllerType()
    {
        if (inputType.Equals(inputTypes.Controller))
        {
            string[] controllerNames = Input.GetJoystickNames();

            foreach (string i in controllerNames)
            {
                if (i.ToUpper().Contains("XBOX"))
                {
                    controllerType = controllerTypes.Xbox;
                }
            }

            if (!controllerType.Equals(controllerTypes.Xbox))
            {
                controllerType = controllerTypes.PS;
            }
        }
        else
        {
            controllerType = controllerTypes.None;
        }
    }

    private void checkControllerCount()
    {
        string[] controllerNames = Input.GetJoystickNames();
        controllerCount = 0;

        foreach (string i in controllerNames)
        {
            if (!i.Equals(""))
            {
                controllerCount++;
            }
        }
    }

    private void limitDrag() { rb.drag = -3; }

    public void pauseMovement(bool value) { isMovementPaused = value; }

    private void movePlayer(inputTypes input, Vector3 dir)
    {
        inputType = input;

        if (!isMovementPaused && (isOnGround || (!isOnGround && !isOnWall)))
        {
            rb.position += dir * movementSpeed * 0.01f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isOnGround && !other.gameObject.Equals(gc.gameObject) && ((gc.groundObject != null && !other.gameObject.Equals(gc.groundObject)) || gc.groundObject == null))
        {
            isOnWall = true;
            limitDrag();
        }
    }
}
