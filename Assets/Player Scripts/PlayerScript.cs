using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public bool moveAtStart = true;
    public float speed = 6.0F;
    public float jumpSpeed = 8.0F;
    public float gravity = 20.0F;
    private Vector3 moveDirection = Vector3.zero;
    private float turner;
    private float looker;
    public float sensitivity;
    public TerrainGenerator terrain;
    public bool freecam;
    public float freecamSpeed = 30f;
    public float vertSpeed = 30f;
    protected static bool go = false;
    public Vector2 StartingPos;
    public Camera camera;
    private static bool terrainInit = false;

    // Use this for initialization
    void Start()
    {
        go = false;
        if(moveAtStart)
        transform.position = new Vector3(StartingPos.x, HeightNoise.getHeight(new Vector3(StartingPos.x, 0, StartingPos.y))[0]+1, StartingPos.y);
        Cursor.lockState = CursorLockMode.Locked;
    }

    public static void SetHeight()
    {
        if (!terrainInit)
        {
            terrainInit = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (go && terrainInit)
        {
            if (freecam)
            {
                moveCam();
            }
            else
            {
                move();
            }

            
            turn();
        }
    }

    void moveCam()
    {
        CharacterController controller = GetComponent<CharacterController>();
        // is the controller on the ground?
            //Feed moveDirection with input.
        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        moveDirection = transform.TransformDirection(moveDirection);
        //Multiply it by speed.
        moveDirection *= freecamSpeed;
        //Jumping
        if (Input.GetButton("Jump"))
            moveDirection.y = vertSpeed;

        if (Input.GetKey(KeyCode.LeftControl))
            moveDirection.y = -vertSpeed;



        //Applying gravity to the controller
        //Making the character move
        controller.Move(moveDirection * Time.deltaTime);
    }

    void turn()
    {
        turner = Input.GetAxis("Mouse X") * sensitivity;
        looker = -Input.GetAxis("Mouse Y") * sensitivity;
        if (turner != 0)
        {
            //Code for action on mouse moving right
            transform.eulerAngles += new Vector3(0, turner, 0); 
        }
        if (looker != 0)
        {
            //Code for action on mouse moving right
            if(camera.transform.eulerAngles.x < 80 || camera.transform.eulerAngles.x > 100 || looker < 0)
                camera.transform.eulerAngles += new Vector3(looker, 0, 0);
        }
    }

    void move()
    {
        CharacterController controller = GetComponent<CharacterController>();
        // is the controller on the ground?
        if (controller.isGrounded)
        {
            //Feed moveDirection with input.
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection.y = 0;
            //Multiply it by speed.
            moveDirection *= speed;
            //Jumping
            if (Input.GetButton("Jump"))
                moveDirection.y = jumpSpeed;

        }

        
        //Applying gravity to the controller
        moveDirection.y -= gravity * Time.deltaTime;
        //Making the character move
        controller.Move(moveDirection * Time.deltaTime);
    }

    public WeaponType GetWeapon()
    {
        return WeaponType.Fist;
    }

    public void ToggleGo(bool on)
    {
        go = on;
    }
}
