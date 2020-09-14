using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Assertions;

public class TurretController : MonoBehaviour
{
    private float fireCountdownTimer;

    public int directionRangeA;
    public int directionRangeB;
    private int fireDirection;
    private Vector3 myposition;

    public float beamFireTime;

    public int playerTurret;

    public Sprite turret1Sprite;
    public Sprite turret2Sprite;

    public GameObject bullet1Prefab;
    public GameObject bullet2Prefab;
    private GameObject bullet;

    private SpriteRenderer mySpriteRenderer;

    public LineRenderer beam1;
    public LineRenderer beam2;
    private LineRenderer beamLine;
    public LineRenderer beamIndicatorLine;

    private bool projectileIsBullet;

    [System.NonSerialized] public Animator myAnimator;

    public bool isCharging;

    public LayerMask whatIsPlayer;

    private LevelManager levelManager;
    private GameObject beam;

    private void Awake()
    {
        directionRangeA = -directionRangeA;
        directionRangeB = -directionRangeB;

        levelManager = gameObject.GetComponentInParent(typeof(LevelManager)) as LevelManager;

        mySpriteRenderer = GetComponent<SpriteRenderer>();
        myAnimator = GetComponent<Animator>();
        myposition = transform.position;

        beam = gameObject.transform.Find("Beam").gameObject;

        gameObject.transform.Rotate(0f, 0f, directionRangeA, Space.Self);
        Debug.DrawLine(myposition, myposition + (transform.up * 2), Color.green, 4);
        gameObject.transform.Rotate(0f, 0f, directionRangeB - directionRangeA, Space.Self);
        Debug.DrawLine(myposition, myposition + (transform.up * 2), Color.green, 4);
        gameObject.transform.Rotate(0f, 0f, -directionRangeB, Space.Self);

        Player_Turret();
    }

    private void Player_Turret()
    {
        if (playerTurret == 1)
        {
            bullet = bullet1Prefab;
            beamLine = beam1;
        }
        else
        {
            bullet = bullet1Prefab;
            beamLine = beam2;
        }
    }
    
    public void ChargeUp()
    {
        isCharging = true;

        //Randomize Fire type
        var firePercentageNumber = Random.Range(1, 100);

        //Set Direction
        fireDirection = Random.Range(directionRangeA, directionRangeB);

        //Check what type to fire
        if (firePercentageNumber > levelManager.beamFirePercentage)
        {
            //Start charging up to fire bullet
            fireCountdownTimer = levelManager.bulletChargeupTime;
            projectileIsBullet = true;
        }
        else
        {
            //Start charging up to fire beam
            fireCountdownTimer = levelManager.beamChargeupTime;
            projectileIsBullet = false;

            //Rotate object, to be able to accurateley aim an indicator in the beams direction
            beam.transform.Rotate(0f, 0f, fireDirection, Space.Self);

            //Create Beam Indicator with Raycast
            RaycastHit2D hit = Physics2D.Raycast(myposition, beam.transform.up);

            //Draw the Beam indicator with a Line
            if (hit && !hit.collider.gameObject.CompareTag("Bullet"))
            {
                beamIndicatorLine.SetPosition(0, myposition);
                beamIndicatorLine.SetPosition(1, hit.point);
            }
            else
            {
                beamIndicatorLine.SetPosition(0, myposition);
                beamIndicatorLine.SetPosition(1, myposition + beam.transform.up * 100);
            }
            //Enable Beam Indicator Line
            beamIndicatorLine.enabled = true;
        }
    }

    private void Update()
    {
        if (playerTurret == 1)
        {
            myAnimator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Turret/P1/P1TurretAnimator");
            
        }
        else if (playerTurret == 2)
        {
            myAnimator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Turret/P2/P2TurretAnimator");
            
        }
        //Updates Animation
        myAnimator.SetBool("isCharging", isCharging);
    }




    private void Shoot()
    {
        isCharging = false;

        if (projectileIsBullet)
            Fire_Bullet();
        else if (!projectileIsBullet)
            StartCoroutine(Fire_Beam());
    }
    

    //Fire either bullet or beam from Turret
    private void Fire_Bullet()
    {
        //Create Bullet object
        GameObject bulletObject = Instantiate(bullet, myposition, Quaternion.identity);     

        BulletController bulletControllerScript = bulletObject.GetComponent<BulletController>();
        Assert.IsNotNull(bulletControllerScript, "There is no bullet component on Hazard object");

        //Set Direction for bullet object
        //bulletControllerScript.direction = fireDirection;

        //Set Speed for bullet object
        bulletControllerScript.speed = levelManager.bulletSpeed;
        bulletControllerScript.Init();
    }

    IEnumerator Fire_Beam()
    {
        //Disable Indicator
        beamIndicatorLine.enabled = false;

        //Create Beam with Raycast
        RaycastHit2D hit = Physics2D.Raycast(myposition, beam.transform.up, 100, whatIsPlayer);

        //Check if beam hits a player
        if (hit)
        {
            //Player Get Hit
            PlayerController playerScript = hit.transform.GetComponent<PlayerController>();
            if (playerScript != null)
            {
                //Player take Damage
            }
            
            beamLine.SetPosition(0, myposition);
            beamLine.SetPosition(1, hit.point);
        }
        else
        {
            //Beam Keep going until it hits the player
            beamLine.SetPosition(0, myposition);
            beamLine.SetPosition(1, myposition + beam.transform.up * 100);
        }
        //Time for which the beam will exist after fired
        beamLine.enabled = true;
        yield return new WaitForSeconds(beamFireTime);
        beamLine.enabled = false;

        beam.transform.Rotate(0f, 0f, -fireDirection, Space.Self);
    }
}
