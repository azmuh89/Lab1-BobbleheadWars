﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 50.0f;
    private CharacterController characterController;
    public Rigidbody head;
    public LayerMask layerMask; // indicates what layers the ray should hit
    private Vector3 currentLookTarget = Vector3.zero; // where marine should stare
    public Animator bodyAnimator;
    public float[] hitForce;
    public float timeBetweenHits = 2.5f;
    private bool isHit = false;
    private float timeSinceHit = 0;
    private int hitNumber = -1;    public Rigidbody marineBody;
    private bool isDead = false;    private DeathParticles deathParticles;
    void OnTriggerEnter(Collider other)
    {
        Alien alien = other.gameObject.GetComponent<Alien>();
        if (alien != null)
        { // check if colliding object has Alien script attached to it
            if (!isHit)
            {
                hitNumber += 1;
                CameraShake cameraShake = Camera.main.GetComponent<CameraShake>();
                if (hitNumber < hitForce.Length)
                {
                    cameraShake.intensity = hitForce[hitNumber];
                    cameraShake.Shake();
                }
                else
                {
                    Die();
                }
                isHit = true; // This sets isHit to true, plays grunt sound and kills alien
                SoundManager.Instance.PlayOneShot(SoundManager.Instance.hurt);
            }
            alien.Die();
        }
    }

    public void Die()
    {
        bodyAnimator.SetBool("IsMoving", false);
        marineBody.transform.parent = null;
        marineBody.isKinematic = false;
        marineBody.useGravity = true;
        marineBody.gameObject.GetComponent<CapsuleCollider>().enabled = true;
        marineBody.gameObject.GetComponent<Gun>().enabled = false;

        Destroy(head.gameObject.GetComponent<HingeJoint>());
        head.transform.parent = null;
        head.useGravity = true;
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.marineDeath);
        deathParticles.Activate();
        Destroy(gameObject);
    }

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        deathParticles = gameObject.GetComponentInChildren<DeathParticles>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")); // creates new vector3 to store movement direction
        characterController.SimpleMove(moveDirection * moveSpeed); // built-in method that automatically moves character in given direction but not allowing to move through obstacles

        if (isHit)
        {
            timeSinceHit += Time.deltaTime;
            if (timeSinceHit > timeBetweenHits)
            {
                isHit = false;
                timeSinceHit = 0;
            }
        }
    }

    void FixedUpdate()
    {
        Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        if (moveDirection == Vector3.zero)
        {
            bodyAnimator.SetBool("IsMoving", false);
        }
        else
        {
            head.AddForce(transform.right * 150, ForceMode.Acceleration); // moves the head when the marine moves
            bodyAnimator.SetBool("IsMoving", true);
        }

        RaycastHit hit; // empty RaycastHit for when you get a hit, it'll be populated with an object
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // cast the ray from main camera to mouse position

        Debug.DrawRay(ray.origin, ray.direction * 1000, Color.green); // this will draw a ray in the scene view while playing the game

        if (Physics.Raycast(ray, out hit, 1000, layerMask, QueryTriggerInteraction.Ignore))
        {
            currentLookTarget = hit.point; // coordinates of the raycast hit, the point where hero should look
        }

        //1
        Vector3 targetPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z);
        //2
        Quaternion rotation = Quaternion.LookRotation(targetPosition - transform.position);
        //3
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 10.0f);
    }
}
