using System.Collections;
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

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")); // creates new vector3 to store movement direction
        characterController.SimpleMove(moveDirection * moveSpeed); // built-in method that automatically moves character in given direction but not allowing to move through obstacles
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
