using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class handController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] public float speed = 5f;
    private float horizontalMove = 0f, verticalMove = 0f;
    public bool canMove = true;

    private Rigidbody2D rb;
    private Vector3 grabOffset;

    [Header("Grab")]
    [SerializeField] private Transform grabPoint;
    [SerializeField] private const float grabRange = 3f;
    [SerializeField] private LayerMask targetLayer;
    private GameObject grabbedObject;
    private Collider2D myCollider, tempCollision;

    public obstacleBehavior obby;
    public Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<Collider2D>();
        targetLayer = LayerMask.GetMask("obstacles");
        gameObject.layer = LayerMask.NameToLayer("Default");
    }

    void Update()
    {
        if (canMove)
        {
            MoveCheck();
            GrabCheck();
        }        
    }

    private void MoveCheck()
    {
        //MOVEMENT INPUTS

        horizontalMove = Input.GetAxisRaw("Horizontal1") * speed;
        verticalMove = Input.GetAxisRaw("Vertical1") * speed;
        rb.velocity = new Vector2(horizontalMove, verticalMove);
    }

    private void GrabCheck()
    {
        // Pickup objects in ranged if our hand is empty

        if (Input.GetKey(KeyCode.Space) && grabbedObject == null)
        {
            // Raycast circle range 
            // Pickup the closest hit object
            // Eearly break for none found
            Collider2D[] hit = Physics2D.OverlapCircleAll(rb.position, grabRange, targetLayer);
            if (hit.Length > 0) { return; }

            float dis = 0f;
            tempCollision = null;

            foreach (Collider2D c in hit)
            {
                float cd = Vector2.Distance(rb.position, c.transform.position);
                if (cd < dis || tempCollision == null)
                {
                    tempCollision = c;
                    dis = cd;
                }
            }
            PickupFixed(tempCollision);
        }

        // Release held object
        else if (Input.GetKeyUp(KeyCode.Space) && grabbedObject != null)
        {
            ReleaseFixed();
        }
    }

    private void PickupFixed(Collider2D collision)
    {
        // Grabbed objects should not interrupt the physics of the hand
        anim.SetBool("Grabbing", true);
        grabbedObject = collision.gameObject;
        Physics2D.IgnoreCollision(collision, myCollider, true);
        grabbedObject.GetComponent<obstacleBehavior>().SetGrabbed(transform, grabPoint);
    }

    private void ReleaseFixed()
    {
        // Return to normal physics
        anim.SetBool("Grabbing", false);
        Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), myCollider, false);
        grabbedObject.GetComponent<obstacleBehavior>().SetReleased();
        grabbedObject = null;
    }
}