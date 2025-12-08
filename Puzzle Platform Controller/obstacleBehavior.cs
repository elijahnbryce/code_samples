using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class obstacleBehavior : MonoBehaviour
{
    public Vector3 spawnPoint;
    public Quaternion spawnRot;

    private Transform spawnTransform;
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
        rb = GetComponent<Rigidbody2D>();
        isAwake = true;
        wasStatic = (rb.bodyType == RigidbodyType2D.Static);
        spawnPoint = rb.transform.position;
        spawnRot = rb.transform.rotation;
        boxCollider = GetComponent<BoxCollider2D>();
        spawnTransform = transform;
    }

    public void SetGrabbed(Transform hand, Transform setPoint)
    {
        // We want to avoid clipping while the hand is moving the object
        // Set position and ignore collisions 

        Physics2D.IgnoreLayerCollision(gameObject.layer, Physics.AllLayers, true);
        boxCollider.enabled = false;

        rb.isKinematic = true;
        transform.position = setPoint.position;
        transform.SetParent(hand);
    }

    public void SetReleased()
    {
        // Return to normal physics
    
        Physics2D.IgnoreLayerCollision(gameObject.layer, Physics.AllLayers, false);
        boxCollider.enabled = true;

        if (doesNotFall) rb.bodyType = RigidbodyType2D.Static;
        rb.isKinematic = false;

        transform.SetParent(spawnTransform.parent);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == groundLayer)
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

    public bool IsGrounded()
    {
        return IsGrounded(groundCheck, groundCheckRadius, groundLayer);
    }

    private bool IsGrounded(Transform checkPosition, float range, LayerMask layer)
    {
        return Physics2D.OverlapCircle(checkPosition.position, range, layer);
    }
}
