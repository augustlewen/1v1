using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public float ingameTime;
    [Space(20)]
    public int difficultyLevel;
    [Tooltip("Amount of seconds between difficulties")]
    public float difficultyRiseRate;
    private float difficultyRiseCountdown;

    [Space(10), Header("Turret Settings")]

    [Tooltip("The time between turrets charging up to fire at difficulty 1")]
    public float turretFireCooldown;
    private float turretFireCountdownTimer;

    [Tooltip("Chance to fire a beam instead of a bullet"), Range(0, 100)]
    public int beamFirePercentage;
    public float beamChargeupTime;
    public float bulletChargeupTime;
    public float bulletSpeed;

    [Space(10), Header("Difficulty Influence")]

    [Tooltip("Turret cooldown reduction percentage per difficulty"), Range(0, 100)]
    public float turretCooldownReduction;
    [Tooltip("The lowest amount of time between turrets charging up to fire")]
    public float turretMinFireCooldown;

    [Space(10), Tooltip("beam chargeup reduction precentage per difficulty"), Range(0, 100)]
    public float beamChargeupReduction;
    [Tooltip("The lowest amount of charge up time for the beam")]
    public float beamMinChargeupTime;

    [Space(10), Tooltip("bullet chargeup reduction precentage per difficulty"), Range(0, 100)]
    public float bulletChargeupReduction;
    [Tooltip("The lowest amount of charge up time for the bullet")]
    public float bulletMinChargeupTime;

    [Space(10), Tooltip("bullet speed increase precentage per difficulty"), Range(0, 100)]
    public float bulletSpeedIncrease;
    public float bulletMaxSpeed;

    public List<GameObject> turrets = new List<GameObject>();

    private void Start()
    {
        //Adds all children of LevelManager (Hazard Spawn Points) to List
        foreach (Transform child in transform)
        {
            turrets.Add(child.gameObject);
        }

        turretFireCountdownTimer = turretFireCooldown;
        difficultyRiseCountdown = difficultyRiseRate;
    }
    private void Update()
    {
        ingameTime += Time.deltaTime;

        //Difficulty Rises
        if (difficultyRiseCountdown > 0)
            difficultyRiseCountdown -= Time.deltaTime;
        else
            RiseInDifficulty();

        //Counting down to fire
        if (turretFireCountdownTimer > 0)
            turretFireCountdownTimer -= Time.deltaTime;
        else
        {
            while(turretFireCountdownTimer != turretFireCooldown)
            {
                //Pick a random turret to shoot from
                int listIndexNumber = Random.Range(0, turrets.Count);
                GameObject turretToFire = turrets[listIndexNumber];

                TurretController turretControllerScript = turretToFire.GetComponent<TurretController>();
                
                //Check if turret is already charging, if so pick another
                if(turretControllerScript.isCharging == false)
                {
                    turretControllerScript.ChargeUp();
                    turretFireCountdownTimer = turretFireCooldown;
                }
            }
        }
    }

    //Difficulty rises
    private void RiseInDifficulty()
    {
        difficultyLevel++;
        difficultyRiseCountdown = difficultyRiseRate;

        //Turret fire-cooldown reduction
        if (turretFireCooldown > turretMinFireCooldown)
            turretFireCooldown -= (turretFireCooldown * turretCooldownReduction) / 100;

        if (turretFireCooldown < turretMinFireCooldown)
            turretFireCooldown = turretMinFireCooldown;

        //Beam chargeup-time reduction
        if (beamChargeupTime > beamMinChargeupTime)
            beamChargeupTime -= (beamChargeupTime * beamChargeupReduction) / 100;

        if (beamChargeupTime < beamMinChargeupTime)
            beamChargeupTime = beamMinChargeupTime;

        //Bullet chargeup-time reduction
        if (bulletChargeupTime > bulletMinChargeupTime)
        bulletChargeupTime -= (bulletChargeupTime * bulletChargeupReduction) / 100;

        if (bulletChargeupTime < bulletMinChargeupTime)
            bulletChargeupTime = bulletMinChargeupTime;

        //Bullet speed increase
        if (bulletSpeed < bulletMaxSpeed)
            bulletSpeed += (bulletSpeed * bulletSpeedIncrease) / 100;

        if (bulletSpeed > bulletMaxSpeed)
            bulletSpeed = bulletMaxSpeed;

        return;
    }
}
