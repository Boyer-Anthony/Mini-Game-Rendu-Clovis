using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private enum PlayerState
    {
        Idle,
        Walking,
        Sprint,
        JumpStart,
        JumpLoop,
        JumpEnd,
    }

    private PlayerState currentState;
    public Animator aN_Player;

    [Header("Initialisation")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float moveInput;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector2 vecGravity;

    [Header("Parametre Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float walkSpeed;    // Vitesse de marche
    [SerializeField] private float sprintSpeed; // Vitesse de sprint

    [Header("Parametre Jump")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float fallMultiplier;

    [Header("Parametre Stamina")]
    [SerializeField] private float currentStam;
    [SerializeField] private float stamMax;
    [SerializeField] private float stamDrainRate;    // combien on perd par seconde en sprint
    [SerializeField] private float stamRegenRate;  // combien on regagne par seconde au repos

    [Header("Booleen")]
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool wasInAir;

    [Header("Key Binding")]
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;



    #region Encapsulation
    private PlayerState CurrentState { get => currentState; set => currentState = value; }
    public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
    public float CurrentStam { get => currentStam; set => Mathf.Clamp(value, 0, stamMax); }
    public float StamMax { get => stamMax; set => Mathf.Max(0, stamMax); }
    public float WalkSpeed { get => walkSpeed; set => walkSpeed = value; }
    public float SprintSpeed { get => sprintSpeed; set => sprintSpeed = value; }
    public KeyCode SprintKey { get => sprintKey; set => sprintKey = value; }
    public Rigidbody2D Rb { get => rb; set => rb = value; }
    public float JumpForce { get => jumpForce; set => jumpForce = value; }
    public Transform GroundCheck { get => groundCheck; set => groundCheck = value; }
    public LayerMask GroundLayer { get => groundLayer; set => groundLayer = value; }
    public Animator AN_Player { get => aN_Player; set => aN_Player = value; }
    public float StamDrainRate { get => stamDrainRate; set => stamDrainRate = value; }
    public float StamRegenRate { get => stamRegenRate; set => stamRegenRate = value; }
    public float MoveInput { get => moveInput; set => moveInput = value; }
    public bool WasInAir { get => wasInAir; set => wasInAir = value; }


    #endregion


    void Start()
    {
        vecGravity = new Vector2(0, -Physics.gravity.y);
        Application.targetFrameRate = 144;
        currentStam = stamMax;
    }


    void Update()
    {
        PlayerMovement();
        HandleJump();
        //CancelJump();
        StateHandle();
        HandleStamina();
        StateSwitching();
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCapsule(GroundCheck.position, new Vector2(1, 0.5f), CapsuleDirection2D.Horizontal, 0, GroundLayer);

        ApplyGraviter();
    }

    public void PlayerMovement()
    {
        // Initialisation GetAxis
        MoveInput = (Input.GetAxis("Horizontal"));

        // Movement Player uniquement sur l'axe X Left/right
        Rb.velocity = new Vector2(MoveInput * MoveSpeed, Rb.velocity.y);

    }

    public void HandleJump() // Quand le joueur appuie sur la touche Jump
    {
        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            Rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            AN_Player.SetTrigger("JumpStart");
            

        }
    }

    public void CancelJump() // Quand le joueur relache la touche Jump ==> Subit la graviter
    {
        if (Input.GetKeyUp(jumpKey) && Rb.velocity.y > 0)
        {
            Rb.velocity = new Vector2(Rb.velocity.x, Rb.velocity.y * 0.1f);
        }
    }

    public void StateHandle()
    {
       
        float input = Input.GetAxis("Horizontal");

        if (!isGrounded)
        {
            if (!WasInAir)
            {
                currentState = PlayerState.JumpStart;
                WasInAir = true;
            }
            else if (Rb.velocity.y < 0)
            {
                currentState = PlayerState.JumpLoop;
            }
        }
        else
        {
            if (WasInAir)
            {
                currentState = PlayerState.JumpEnd;
                WasInAir = false;
            }
            else if (Input.GetKey(sprintKey) && CurrentStam > 0 && Mathf.Abs(input) > 0.1f)
            {
                currentState = PlayerState.Sprint;
                MoveSpeed = SprintSpeed;
            }
            else if (Mathf.Abs(input) > 0.1f)
            {
                currentState = PlayerState.Walking;
                MoveSpeed = WalkSpeed;
            }
            else
            {
                currentState = PlayerState.Idle;
            }
        }
        

        
    }

    private void HandleStamina()
    {


        if (currentState == PlayerState.Sprint && currentStam > 0)
        {
            currentStam -= stamDrainRate * Time.deltaTime;

            // Si on a plus de stam, stop Sprinting

            if (currentStam <= 0)
            {
                currentStam = 0;
                MoveSpeed = walkSpeed;
                currentState = PlayerState.Walking;
            }
        }

        else if (currentStam < stamMax)
        {
            currentStam += stamRegenRate * Time.deltaTime;
            if (currentStam > stamMax) currentStam = stamMax;
        }


    }

    public void AddStamina(float amount)
    {
        currentStam = Mathf.Clamp(currentStam + amount, 0, stamMax);
    }
    private void ApplyGraviter()
    {
        if (Rb.velocity.y < 0)
        {
            Rb.velocity -= vecGravity * fallMultiplier * Time.fixedDeltaTime;
        }
    }

    private void StateSwitching()
    {

        //ResetJumpBools();

        switch (CurrentState)
        {
            case PlayerState.Idle:

                AN_Player.SetFloat("Speed", 0);

                break;

            case PlayerState.Walking:

                AN_Player.SetFloat("Speed", 2);

                break;

            case PlayerState.Sprint:

                AN_Player.SetFloat("Speed", 6);

                break;

            case PlayerState.JumpStart:

               // AN_Player.SetTrigger("JumpStart");

                break;

            case PlayerState.JumpLoop:

                AN_Player.SetTrigger("JumpLoop");

                break;

            case PlayerState.JumpEnd:

                AN_Player.SetTrigger("JumpEnd");

                break;

        }
    }

    
}

    
