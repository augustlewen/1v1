using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpriteController : MonoBehaviour
{
    public PlayerController myPlayerController;

    // Start is called before the first frame update
    void Start()
    {
        myPlayerController = gameObject.GetComponentInParent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DeathFinish()
    {
        myPlayerController.DeathExplosion();
        Destroy(gameObject);

    }
}
