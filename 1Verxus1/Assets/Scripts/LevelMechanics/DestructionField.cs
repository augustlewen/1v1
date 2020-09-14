using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructionField : MonoBehaviour
{
    
    //Destroys Hazards that goes beyond the screen
    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.gameObject.CompareTag("Bullet"))
            Destroy(other.gameObject);
    }
}
