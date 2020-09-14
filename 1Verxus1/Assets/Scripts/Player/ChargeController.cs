using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeController : MonoBehaviour
{

    // NONSERIALIZE ALL
    public GameObject playerToFollow;
    public PlayerController myPlayerController;
    public GameObject myCharge;
    public GameObject myImage;
    public Animator myAnim;

    private float lerpSpeed;
    public float lagSpeed;
    public float circleRadius;
    public float minLerpSpeed;
    public float maxLerpSpeed;
    public float minDestinationSwitchTime;
    public float maxDestinationSwitchTime;
    private float destinationTime;
    private Vector3 nextPosition;


    // Start is called before the first frame update
    void Start()
    {
        myImage.transform.SetParent(null);
        myAnim = myImage.GetComponent<Animator>();
        myPlayerController = playerToFollow.GetComponent<PlayerController>();

    }

    // Update is called once per frame
    void Update()
    {
        // make parent object follow player
        gameObject.transform.position = playerToFollow.transform.position;

        //Constantly tic down destination time
        destinationTime -= Time.deltaTime;


        //Randomise values for next path
        if (destinationTime <= 0f)
        {

            Vector2 posOffset = Random.insideUnitCircle * circleRadius;

            nextPosition = new Vector3(posOffset.x, posOffset.y, 0f);
            destinationTime = Random.Range(minDestinationSwitchTime, maxDestinationSwitchTime);
            lerpSpeed = Random.Range(minLerpSpeed, maxLerpSpeed);
        } 


        //Constantly lerp orb toward next location
        myCharge.transform.position = Vector3.Lerp(myCharge.transform.position, nextPosition + playerToFollow.transform.position, lerpSpeed * Time.deltaTime);


        //Destroy self if player dies
        if(myPlayerController.isDead)
        {
            Destroy(myImage);
            Destroy(gameObject);
        }

    }

    void LateUpdate()
    {
        // Smoothens out the lerp
        myImage.transform.position = Vector3.Lerp(myImage.transform.position, myCharge.transform.position, lagSpeed * Time.deltaTime);
    }

    public void DestroySelfDelayed()
    {
        //Controls animation
        myAnim.SetBool("isDestroyed", true);
    }
}
