using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class obstacleBehavior : MonoBehaviour
{
    Vector3 grabOffset;
    //bool isHeld = false;
    //[SerializeField] GameObject theHand;
    //[SerializeField] Transform testPos;
    public Vector3 spawnPoint;
    public Quaternion spawnRot;

    private Transform spawnT;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private float clippingCheckRadius = 0.1f;

    public Collider2D boxCollider;
    private Rigidbody2D rb;
    public bool isAwake;
    public bool wasStatic;
    public bool doesNotFall;
    public bool superStatic;

    void Start()
    {
        //handControl = theHand.GetComponent<handController>();

        rb = GetComponent<Rigidbody2D>();
        isAwake = true;
        wasStatic = (rb.bodyType == RigidbodyType2D.Static);
        spawnPoint = rb.transform.position;
        spawnRot = rb.transform.rotation;
        boxCollider = GetComponent<BoxCollider2D>();

        spawnT = transform;
    }

    public void SetGrabbed(Transform p, Transform l)
    {
        Physics.IgnoreLayerCollision(gameObject.layer, Physics.AllLayers, true);
        boxCollider.enabled = false;

        rb.isKinematic = true;
        transform.position = l.position;
        transform.SetParent(p);
    }

    public void SetReleased()
    {
        Physics.IgnoreLayerCollision(gameObject.layer, Physics.AllLayers, false);
        boxCollider.enabled = true;

        if (doesNotFall) rb.bodyType = RigidbodyType2D.Static;
        rb.isKinematic = false;

        transform.SetParent(spawnT.parent);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            // Push out of ground if clipping
                // Bottom
            if (IsGrounded(groundCheck, clippingCheckRadius, groundLayer))
            {
                rb.transform.position = new Vector3(rb.transform.position.x, rb.transform.position.y + 1f, rb.transform.position.z);
            }
                // Top
            else if (
                Physics2D.OverlapCircle(
                    new Vector2( groundCheck.position.x, transform.position.y + (transform.position.y - groundCheck.position.y) ), 
                    clippingCheckRadius, 
                    groundLayer)
                )
            {
                rb.transform.position = new Vector3(rb.transform.position.x, rb.transform.position.y - 1f, rb.transform.position.z);
            }
            // Will fall but won't move
            if (wasStatic)
            {
                rb.bodyType = RigidbodyType2D.Static;
            }
        }
    }

    public bool CheckOnGround()
    {
        return IsGrounded(groundCheck, groundCheckRadius, groundLayer);
    }

    private bool IsGrounded(Transform t, float r, LayerMask l)
    {
        return Physics2D.OverlapCircle(t.position, r, l);
    }
}
