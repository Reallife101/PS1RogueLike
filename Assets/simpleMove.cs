using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class simpleMove : MonoBehaviour
{
    CharacterController characterController;

    public float walkSpeed = 3f;
    public float gravity = -9.81f;
    public float jumpHeight = 3.0f;

    //Check ground variables
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    //Flashlight
    public GameObject flashlight;


    Vector3 velocity;
    float speed;
    bool isGrounded;
    bool flashlightOn;
    bool crouching;
    bool isSliding;
    Vector3 lastMove;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        flashlightOn = false;
        flashlight.SetActive(false);
        crouching = false;
        isSliding = false;
    }

    void Update()
    {
        //Grounded
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Flashlight
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (flashlightOn)
            {
                flashlightOn = false;
                flashlight.SetActive(false);
            }
            else
            {
                flashlightOn = true;
                flashlight.SetActive(true);
            }
        }

        // Sprint
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = walkSpeed * 2;
        }
        else
        {
            speed = walkSpeed;
        }

        //Crouching
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            crouching = true;
        } else
        {
            crouching = false;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;


        if (crouching)
        {
            characterController.height = 1.0f;
            transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
            
            //Add Slide
            if (!isSliding && speed > walkSpeed)
            {
                isSliding = true;
                lastMove = move;
            }
            else if (lastMove == Vector3.zero)
            {
                speed = walkSpeed * 0.75f;
                characterController.Move(move * speed * Time.deltaTime);

            } else
            {
                speed = walkSpeed * 3f;
                characterController.Move(lastMove * speed * Time.deltaTime);
            }
        }
        else
        {
            isSliding = false;
            characterController.height = 2.0f;
            transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
            characterController.Move(move * speed * Time.deltaTime);
        }

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        characterController.Move(velocity * Time.deltaTime);

    }
}
