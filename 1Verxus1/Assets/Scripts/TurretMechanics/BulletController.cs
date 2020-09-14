using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{

    public float speed;
    public Vector3 direction;
    public int belongingPlayer;

    private Rigidbody2D myRigidbody2D;

    public GameObject levelManagerPrefab;
    private LevelManager levelManagerScript;



    public void Init()
    {

        //transform.Rotate(0, 0, direction, Space.Self);

        myRigidbody2D = GetComponent<Rigidbody2D>();


        levelManagerScript = levelManagerPrefab.GetComponent<LevelManager>();

    }

    private void Update()
    {
        transform.position += direction * Time.deltaTime * speed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {


        if(belongingPlayer == 1)
        {
            if (other.tag == "Player2")
            {


                PlayerController player;
                player = other.GetComponent<PlayerController>();
                player.Die();
                Destroy(gameObject);
            }

            if (other.tag == "Player2Shield")
            {
                ShieldController playerShield;
                playerShield = other.GetComponent<ShieldController>();
                playerShield.Charged();
                Destroy(gameObject);
            }
        }

        if (belongingPlayer == 2)
        {
            if (other.tag == "Player1")
            {
                PlayerController player;
                player = other.GetComponent<PlayerController>();
                player.Die();
                Destroy(gameObject);
            }

            if (other.tag == "Player1Shield")
            {
                ShieldController playerShield;
                playerShield = other.GetComponent<ShieldController>();
                playerShield.Charged();
                Destroy(gameObject);
            }
        }

    }
}
