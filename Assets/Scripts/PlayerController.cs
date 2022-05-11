using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Player Controller Settings")]
    [SerializeField] private float walkSpeed = 8f;
    [SerializeField] private float runSpeed = 12f;
    [SerializeField] private float jumpSpeed = 0.25f;
    [SerializeField] private float gravityModifer = 0.95f;
    [SerializeField] private InputAction newMovementInput;
    [Header("Mouse Control Options")]
    [SerializeField] float mouseSensivity = 5f;
    [SerializeField] float maxViewAngle = 60f;
    [SerializeField] bool invertX = false;
    [SerializeField] bool invertY = false;

    private CharacterController characterController;

    private float currentSpeed = 8f;
    private float horizontalInput;
    private float verticalInput;
    private bool jump = false;

    private Vector3 heightMovement;

    private Transform mainCamera;
    private Animator anim;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();

        if (Camera.main.GetComponent<CameraController>() == null)//E�er olurda kameram�zda CameraController Scripti yoksa g�venlik i�in �art koyuyorum.
        {
            Camera.main.gameObject.AddComponent<CameraController>();
        }
        mainCamera = GameObject.FindWithTag("CameraPoint").transform;
    }
    private void OnEnable()
    {
        newMovementInput.Enable();
    }
    private void OnDisable()
    {
        newMovementInput.Disable();   
    }

    void Update()
    {
        KeyboardInput();
        AnimationChanger();
    }

    

    private void FixedUpdate()
    {
        Move();
        Rotate();
    }

    private void Rotate()
    {
        //Bunda peki�tirme ara�t�rma yap tam anla��lmad� !!!!!!! Quaternionlar� tamamen ��ren !
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + MouseInput().x,
            transform.eulerAngles.z);
        if (mainCamera != null)
        {
            

            if (mainCamera.eulerAngles.x > maxViewAngle && mainCamera.eulerAngles.x < 180f)
            {
                mainCamera.rotation = Quaternion.Euler(maxViewAngle, mainCamera.eulerAngles.y, mainCamera.eulerAngles.z);
            }
            else if (mainCamera.eulerAngles.x > 180f && mainCamera.eulerAngles.x < 360f - maxViewAngle)
            {
                mainCamera.rotation = Quaternion.Euler(360f - maxViewAngle, mainCamera.eulerAngles.y, mainCamera.eulerAngles.z);

            }
            else
            {
                mainCamera.rotation = Quaternion.Euler(mainCamera.rotation.eulerAngles + new Vector3(-MouseInput().y, 0f, 0f));
            }
        }
    }

    private void Move()
    {
        if (jump)//Z�plama
        {
            heightMovement.y = jumpSpeed;
            jump = false;
        }

        heightMovement.y -= gravityModifer * Time.deltaTime; //Yer �ekimi
        //Bu Local i�lemi Eksen nereye bakarsa ona g�re gidicek Z eksenini 90 derece d�nd�rd���m�z� varsayal�m Z nereye bak�yorsa W tu�una bas�ld���nda karakter o y�ne gidicek
        //Yani Bakt���m�z y�ne hareket edicektir.
        Vector3 localVerticalVektor = transform.forward * verticalInput;
        Vector3 localHorizontalVektor = transform.right * horizontalInput;

        Vector3 movementVector = localVerticalVektor + localHorizontalVektor;

        movementVector.Normalize();//�apraz giderken karakterin h�zl� gitmesini engeller ve X Y eksenlerinde gitti�i h�zla ayn� h�zda gider.
        movementVector *= currentSpeed * Time.deltaTime;//Normalize den sonra okunmal� �nce okunursa bir anlam� kalmaz ve 1 e e�itlenir
        characterController.Move(movementVector + heightMovement);

        if (characterController.isGrounded)
        {
            heightMovement.y = 0f;
        }
    }
    private void AnimationChanger()
    {
        if (Keyboard.current.leftShiftKey.isPressed && characterController.isGrounded)
        {
            anim.SetBool("Run", true);
        }
        else
        {
            anim.SetBool("Run", false);
        }
        if (newMovementInput.ReadValue<Vector2>().magnitude > 0f && characterController.isGrounded)
        {
            anim.SetBool("Walk", true);
        }
        
        else
        {
            anim.SetBool("Walk",false);
        }
    }
    //Hareket etmemizi sa�layacak olan Inputlar
    private void KeyboardInput()
    {
        horizontalInput = newMovementInput.ReadValue<Vector2>().x;
        verticalInput = newMovementInput.ReadValue<Vector2>().y;

        if (Keyboard.current.spaceKey.wasPressedThisFrame && characterController.isGrounded)
        {
            jump = true;
        }
        if (Keyboard.current.leftShiftKey.isPressed)
        {
            currentSpeed = runSpeed;
        }
        else
        {
            currentSpeed = walkSpeed;
        }
    }
    
    private Vector2 MouseInput()
    {
        
        return new Vector2(invertX ? -Input.GetAxisRaw("Mouse X") : Input.GetAxisRaw("Mouse X"),invertY ? -Input.GetAxisRaw("Mouse Y") : Input.GetAxisRaw("Mouse Y")) * mouseSensivity;
    }
}
