using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldController : MonoBehaviour
{
    private CircleCollider2D myCollider;
    private PlayerController myPlayerController;
    private Animator myAnimator;

    private float shieldCounter;
    public float chargeTime;
    private float chargeCounter;
    public float shieldDuration = 3;
    public bool charged;
    public bool shieldEnded;

    // Start is called before the first frame update
    void Start()
    {
        myPlayerController = gameObject.GetComponentInParent<PlayerController>();
        myAnimator = GetComponent<Animator>();
        shieldCounter = shieldDuration;
    }

    // Update is called once per frame
    void Update()
    {
        shieldCounter -= Time.deltaTime;

        if (shieldCounter <= 0f)
        {
            shieldEnded = true;
        }


        if(chargeCounter > 0f)
        {
            chargeCounter -= Time.deltaTime;
        }

        if(chargeCounter <= 0.1f && charged)
        {
            UnCharge();
        }

        myAnimator.SetBool("charge", charged);
        myAnimator.SetBool("shieldEnded", shieldEnded);
    }

    private void OnEnable()
    {
        myCollider = GetComponent<CircleCollider2D>();
        shieldCounter = shieldDuration;
        myCollider.enabled = true;
        shieldEnded = false;

    }

    public void Charged()
    {
        chargeCounter = chargeTime;
        shieldCounter = chargeTime;
        myCollider.enabled = false;
        charged = true;
    }

    public void UnCharge()
    {
        gameObject.SetActive(false);
        myPlayerController.isShielded = false;
        charged = false;
    }

}
