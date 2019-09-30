using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private void OnBecameInvisible()
    {
        Destroy(gameObject); // Destroy bullets when they go off-screen
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject); // Destroy bullets when the collide with other objects
    }

    void Start()
    {
        
    }
    
    void Update()
    {
        
    }
}
