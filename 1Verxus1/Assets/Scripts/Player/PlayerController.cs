using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float maxMoveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpTime;
    [SerializeField] private float wallSlideSpeed;
    [SerializeField] private float wallJumpForce;
    [SerializeField] private int maxOrbs;
    [SerializeField] private int shootSpeed;


    [Header("References")]
    private Rigidbody2D myRigidbody;
    public Transform groundCheck;
    public Transform wallCheck;
    private GameObject myDeathExplosion;
    private Animator myAnimator;
    private GameObject myShield;
    private ShieldController myShieldController;
    public GameObject chargeOrb;
    public LayerMask whatIsGround;
    public LayerMask whatAreWalls;

    [Header("State-Debugging")]
    public bool isGrounded;
    public bool isWalled;
    public bool isWallSliding;
    public bool isFalling;
    public bool isDead;
    public bool isShielded = false;

    [Header("Data")]
    private bool stoppedJumping = true;
    private float jumpTimeCounter;
    public Vector2 groundCheckSize;
    public float wallCheckRadius;
    public int shieldCharges = 3;
    private float wallJumpDirection = -1;
    public Vector2 wallJumpAngle;
    [NonSerialized] public List<GameObject> orbs;

    [Header("Player Setup")]
    public int player;
    public string inputHorizontalAxisName;
    public string inputJumpName;
    public string inputFireName;
    public GameObject myBullet;
    public GameObject enemyPlayer;



    void Start()
    {
        GetAllReferences();
        ResetData();

    }

    void Update()
    {
        Animations();
    }

    private void FixedUpdate()
    {
        if (!isDead)
        {
            Movement();
            Jump();
            WallSlide();
            Shield();
            Shoot();
        }
    }




    ////////////FUNCTIONS/////////
    private void GetAllReferences()
    {
        // Finds references
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = transform.GetComponentInChildren<Animator>();
        myShield = gameObject.transform.Find("Shield").gameObject;
        myShieldController = myShield.GetComponent<ShieldController>();
        myDeathExplosion = gameObject.transform.Find("DeathExplosion").gameObject;
    }

    private void ResetData()
    {
        //Resets Data
        jumpTimeCounter = jumpTime;
        wallJumpAngle.Normalize();
        orbs = new List<GameObject>(maxOrbs);
    }

    private void Animations()
    {
        //Animations
        if (myAnimator != null)
        {
            myAnimator.SetFloat("Speed", Mathf.Abs(myRigidbody.velocity.x));
            myAnimator.SetBool("Grounded", isGrounded);
            myAnimator.SetBool("isFalling", isFalling);
            myAnimator.SetBool("isWallSliding", isWallSliding);
            myAnimator.SetBool("isDead", isDead);
        }
    }
    private void Movement()
    {

        if (Input.GetAxisRaw(inputHorizontalAxisName) > 0f)
        {
            myRigidbody.AddForce(new Vector2(moveSpeed, 0f), ForceMode2D.Force);
            if(myRigidbody.velocity.x > maxMoveSpeed)
            {
                myRigidbody.velocity = new Vector3(maxMoveSpeed, myRigidbody.velocity.y, 0f);
            }
            transform.localScale = new Vector3(1f, 1f, 1f);
            wallJumpDirection = -1;
        }
        else if (Input.GetAxisRaw(inputHorizontalAxisName) < 0f)
        {
            myRigidbody.AddForce(new Vector2(-moveSpeed, 0f), ForceMode2D.Force);
            if (myRigidbody.velocity.x < -maxMoveSpeed)
            {
                myRigidbody.velocity = new Vector3(-maxMoveSpeed, myRigidbody.velocity.y, 0f); ;
            }
            transform.localScale = new Vector3(-1f, 1f, 1f);
            wallJumpDirection = 1;
        }
        else
        {
            if (isGrounded)
            {
                myRigidbody.velocity = new Vector3(0f, myRigidbody.velocity.y, 0f);
            }
        }
    }
    private void Jump()
    {
        //Checks Ground
        isGrounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, whatIsGround);


        //Initial Jump
        if (Input.GetButton(inputJumpName) && isGrounded && stoppedJumping && jumpTimeCounter > 0)
        {
            myRigidbody.velocity = new Vector3(myRigidbody.velocity.x, jumpForce, 0f);
            stoppedJumping = false;
        }

        //Super Jump
        if (Input.GetButton(inputJumpName) && !stoppedJumping && jumpTimeCounter > 0f)
        {
            myRigidbody.velocity = new Vector3(myRigidbody.velocity.x, jumpForce, 0f);
            jumpTimeCounter -= Time.deltaTime;
        }

        //Reset jump
        if (!Input.GetButton(inputJumpName))
        {
            jumpTimeCounter = 0;
            stoppedJumping = true;
        }

        //Controls falling
        if (myRigidbody.velocity.y < 0)
        {
            isFalling = true;
        }
        else
        {
            isFalling = false;
        }

        // Always updates ground bool
        if (isGrounded && stoppedJumping)
        {
            jumpTimeCounter = jumpTime;
        }
    }

    private void WallSlide()
    {

        isWalled = Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, whatAreWalls);

        if (isWalled && !isGrounded && myRigidbody.velocity.y < 0f)
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }

        if(isWallSliding)
        {
            myRigidbody.velocity = new Vector3(myRigidbody.velocity.x, wallSlideSpeed, 0f);

            if (Input.GetButton(inputJumpName))
            {
                myRigidbody.AddForce(new Vector2(wallJumpForce * wallJumpDirection * wallJumpAngle.x, wallJumpForce * wallJumpAngle.y), ForceMode2D.Impulse);

                if (wallJumpDirection > 0f)
                {
                    transform.localScale = new Vector3(1f, 1f, 1f);
                }
                else
                {
                    transform.localScale = new Vector3(-1f, 1f, 1f);
                }

                wallJumpDirection = wallJumpDirection * -1f;
            }


        }


    }

    private void Shield()
    {
        //Shield mechanic
        if (Input.GetButton(inputFireName) && !isShielded && shieldCharges > 0)
        {
            isShielded = true;
            shieldCharges -= 1;

            orbs[0].GetComponent<ChargeController>().DestroySelfDelayed();
            orbs.RemoveAt(0);

            myShield.SetActive(true);
        }

    }

    public void AddOrb()
    {
        if (orbs.Count == maxOrbs) return;

        shieldCharges++;
        var orbCharge = Instantiate(chargeOrb, gameObject.transform.position,Quaternion.identity);
        orbCharge.GetComponent<ChargeController>().playerToFollow = gameObject;
        orbs.Add(orbCharge);
    }

    public void Die()
    {
        isDead = true;
        myRigidbody.velocity = new Vector3(0f, 0f, 0f);
        myRigidbody.gravityScale = 0f;
        Destroy(myShield);

        if (player == 1)
        {
            myAnimator.Play("PlayerDeath");
        }
        else
        {
            myAnimator.Play("Player2Death");
        }
        


        Time.timeScale = 0.1f;
        Time.fixedDeltaTime = 0.004F * Time.timeScale;
    }

    public void Shoot()
    {
        if(myShieldController.charged && Input.GetButton(inputFireName))
        {

            //Create Bullet object
            GameObject bullet = Instantiate(myBullet, transform.position, Quaternion.identity);

            BulletController bulletControllerScript = bullet.GetComponent<BulletController>();

            //Set Direction for bullet object
            var direction = (enemyPlayer.transform.position - transform.position).normalized;

            //bullet.transform.rotation = Quaternion.LookRotation(direction);


            bulletControllerScript.direction = direction;
            //Set Speed for bullet object
            bulletControllerScript.speed = shootSpeed;
            bulletControllerScript.Init();

            myShieldController.UnCharge();
        }
    }

    public void DeathExplosion()
    {
        myDeathExplosion.SetActive(true);
    }
     
    public void OnDrawGizmos()
    {

        if (Application.isPlaying)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(groundCheck.position, groundCheckSize);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(wallCheck.position, wallCheckRadius);
        }
    }

}
