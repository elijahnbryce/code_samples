using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class miceController : MonoBehaviour
{
    [SerializeField] private Transform verticalBound;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask obbyLayer;
    [SerializeField] private float checkRadius = 0.2f;
    [SerializeField] public float speed = 5f;
    [SerializeField] public float jumpHeight = 5f;
    bool facingRight = true;
    public bool canMove = true;
    //bool isGrounded = true;
    float horizontalMove = 0f;

    private Rigidbody2D rb;

    [SerializeField] private Animator anim;
    public AudioSource walkSound;
    public AudioSource jumpSound;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (canMove)
        {
            MoveCheck();
        }
    }

    private void MoveCheck()
    {
        //MOVE
        horizontalMove = Input.GetAxisRaw("Horizontal2");
        rb.velocity = new Vector2(horizontalMove * speed, rb.velocity.y);

        if (horizontalMove != 0)
        {
            anim.SetBool("Moving", true);
            if (walkSound.GetComponent<AudioSource>().isPlaying == false)
            {
                //walkSound.GetComponent<AudioSource>().Play();
            }
}
        else
        {
            anim.SetBool("Moving", false);
            //walkSound.GetComponent<AudioSource>().Stop();
        }
        //LOOK DIRECTION
        FlipSprite();

        //JUMP
        if (Input.GetButtonDown("Jump") && (IsGrounded() || OnObstacle()))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
            anim.SetTrigger("Jump");
            if (walkSound.GetComponent<AudioSource>().isPlaying == false)
            {
                //jumpSound.GetComponent<AudioSource>().Play();
            }
        }

        anim.SetBool("OnGround", IsGrounded());
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
    }
    private bool OnObstacle()
    {
        return Physics2D.OverlapCircle(groundCheck.position, checkRadius, obbyLayer);
    }

    private void FlipSprite()
    {
        if (facingRight && horizontalMove < 0f || !facingRight && horizontalMove > 0f)
        {
            facingRight = !facingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private void checkVert()
    {
        if (transform.position.y < verticalBound.position.y)
        {
            Debug.Log("Fell in the void");
            Time.timeScale = 0;
            //TODO: end game, freeze time, call game over screen, destroy game object
        }
    }
}