using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[RequireComponent (typeof(Rigidbody2D),typeof(TouchingDirection),typeof(Damageable))]

public class EvilWizard : MonoBehaviour
{
    public float walkAcceleration = 3f;
    public float maxSpeed = 3f;
    [SerializeField]private float walkStopRate=0.05f;
    public DetectionZone attackZone;
    public DetectionZone cliffDetectionZone;
    Animator animator;
    Damageable damageable;

    Rigidbody2D rb;
    TouchingDirection touchingDirection;

    public enum WalkableDirection { Right,Left}

    public WalkableDirection _walkDirection;

    private Vector2 walkDirectionVector = Vector2.right;

    public WalkableDirection WalkDirection
    {
        get { return _walkDirection; }
        set
        {

            if (_walkDirection != value)
            {
                gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x * -1, gameObject.transform.localScale.y);
            }
            if (value == WalkableDirection.Right)
            {
                walkDirectionVector = Vector2.right;
            }else if(value==WalkableDirection.Left)
            {
                walkDirectionVector = Vector2.left;
            }
            


            _walkDirection = value;
        }
        }
    public bool _hasTarget = false;
    

    public bool HasTarget
    {
        get { return _hasTarget; }
        private set
        {
            _hasTarget = value;
            animator.SetBool(AnimationStrings.hasTarget, value);
        }
    }

    public bool CanMove
    {
        get
        {
            return animator.GetBool(AnimationStrings.canMove);
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        touchingDirection = GetComponent<TouchingDirection>();
        animator = GetComponent<Animator>();
        damageable = GetComponent<Damageable>();
    }

   
    void Update()
    {
        HasTarget=attackZone.detectedColliders.Count > 0;
    }

    private void FixedUpdate()
    {
        if(touchingDirection.IsGrounded && touchingDirection.IsOnWall )
        {
            FlipDirection();
        }
        if(!damageable.LockVelocity)
        {
            if (CanMove)
                rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x + (walkAcceleration * walkDirectionVector.x * Time.fixedDeltaTime), -maxSpeed, maxSpeed), rb.velocity.y);
            else
                rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0, walkStopRate), rb.velocity.y);

        }
        


    }

    private void FlipDirection()
    {
       if(WalkDirection== WalkableDirection.Right) {
        WalkDirection=WalkableDirection.Left;
        
        }else if(WalkDirection== WalkableDirection.Left)
        {
            WalkDirection = WalkableDirection.Right;
        }else
        {
            Debug.LogError("current walkable direction is not set to legal values or right or left");
        }
    }

    
   public void OnHit(int damage,Vector2 knockback)
    {
        
        rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);
  
    }

    public void OnCliffDetected()
    {
        if(touchingDirection.IsGrounded)
        {
            FlipDirection();
        }
    }

}
