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
        Air,
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
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;

    [Header("Parametre Jump")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float fallMultiplier;

    [Header("Parametre Stamina")]
    [SerializeField] private float currentStam;
    [SerializeField] private float stamMax;

    [Header("Booleen")]
    [SerializeField] private bool isGrounded;

    [Header("Key Binding")]
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;



    #region Encapsulation
    private PlayerState CurrentState { get => currentState; set => currentState = value; }
    public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
    public float CurrentStam { get => CurrentStam; set => Mathf.Clamp(currentStam, 0, stamMax); }
    public float StamMax { get => stamMax; set => Mathf.Max(0, stamMax); }
    public float WalkSpeed { get => walkSpeed; set => walkSpeed = value; }
    public float SprintSpeed { get => sprintSpeed; set => sprintSpeed = value; }
    public KeyCode SprintKey { get => sprintKey; set => sprintKey = value; }
    public Rigidbody2D Rb { get => rb; set => rb = value; }
    public float JumpForce { get => jumpForce; set => jumpForce = value; }
    public Transform GroundCheck { get => groundCheck; set => groundCheck = value; }
    public LayerMask GroundLayer { get => groundLayer; set => groundLayer = value; }
    public Animator AN_Player { get => aN_Player; set => aN_Player = value; }


    #endregion


    void Start()
    {
        vecGravity = new Vector2(0, -Physics.gravity.y);
        Application.targetFrameRate = 144;
    }

    
    void Update()
    {
        PlayerMovement();
        HandleJump();
        CancelJump();
        StateHandle();
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
        moveInput = Input.GetAxis("Horizontal") * MoveSpeed;

        // Movement Player uniquement sur l'axe X Left/right
        Rb.velocity = new Vector2(moveInput, Rb.velocity.y);

    }

    public void HandleJump() // Quand le joueur appuie sur la touche Jump
    {
        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            Rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    public void CancelJump() // Quand le joueur relache la touche Jump ==> Subit la graviter
    {
        if(Input.GetKeyUp(jumpKey) && Rb.velocity.y > 0)
        {
            Rb.velocity = new Vector2(Rb.velocity.x, Rb.velocity.y * 0.1f);
        }
    }

    public void StateHandle()
    {
        // Mode - sprint
        if(isGrounded && Input.GetKey(sprintKey) && isGrounded)
        {
            Debug.Log("Sprint");
            currentState = PlayerState.Sprint;
            MoveSpeed = SprintSpeed;
        }

        // Mode - walking
        else if (isGrounded)
        {
            currentState = PlayerState.Walking;
            MoveSpeed = WalkSpeed;
        }

        // Mode - Air
        else
        {
            currentState = PlayerState.Air;
        }

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
        switch (CurrentState)
        {
            case PlayerState.Walking : 

                AN_Player.SetBool("Sprint", false); // Arrete l'animation Sprint

                break;

            case PlayerState.Sprint :

                AN_Player.SetBool("Sprint", true);

                break;


            case PlayerState.Air :

                break;

        }
    }
}
