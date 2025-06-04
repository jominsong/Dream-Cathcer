using System;
using JetBrains.Annotations;
using UnityEngine;

public class Playermove : MonoBehaviour
{
    Rigidbody2D rigid;
    public float maxSpeed;
    public float antiGravity;
    public float wallJumpPower;
    // jump
    public float jumpPower;
    public int jumpCount;
    public int maxJumpCount;
    public float afterWallJumpStiff;
    //dash & leap
    public float currentTime;
    public float lastTapTime;
    public float doubleTapTimeLimit;
    public bool firstTapDetected;
    public float dashCount;
    public float dashTime;
    public float dashCoolDown;
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        RaycastHit2D rayRight = Physics2D.Raycast(rigid.position, Vector3.right, 0.25f, LayerMask.GetMask("Platform"));
        RaycastHit2D rayLeft = Physics2D.Raycast(rigid.position, Vector3.left, 0.25f, LayerMask.GetMask("Platform"));
        RaycastHit2D rayDown = Physics2D.Raycast(rigid.position, Vector3.down, 0.25f, LayerMask.GetMask("Platform"));
        //jump
        if (Input.GetButtonDown("Jump") && jumpCount < maxJumpCount)
        {
            rigid.linearVelocityY = 0;
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            jumpCount++;
            //walljump
            if (rayLeft.collider != null)
            {
                rigid.AddForce(Vector2.right * wallJumpPower, ForceMode2D.Impulse);
                afterWallJumpStiff = 10;
            }
            if (rayRight.collider != null)
            {
                rigid.AddForce(Vector2.left * wallJumpPower, ForceMode2D.Impulse);
                afterWallJumpStiff = 10;
            }
        }
        //dash & leap
        currentTime = Time.time;
        if (Input.GetButtonDown("Horizontal"))
        {
            if (currentTime - lastTapTime <= doubleTapTimeLimit && firstTapDetected)
            {
                firstTapDetected = false;
                if (dashCount < 1 && dashCoolDown == 0)
                {
                    if (rayDown.collider == null)
                        rigid.linearVelocity = new Vector2(h * 10, 5);
                    else
                        rigid.linearVelocityX = h * 10;
                    dashCount++;
                    dashTime = 20;
                    dashCoolDown = 30;
                }
            }
            else
            {
                firstTapDetected = true;
                lastTapTime = currentTime;
            }
        }
        if (currentTime - lastTapTime > doubleTapTimeLimit)
        {
            lastTapTime = -1f;
            firstTapDetected = false;
        }
    }
    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");

        Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));
        Debug.DrawRay(rigid.position, Vector3.right, new Color(0, 1, 0));
        Debug.DrawRay(rigid.position, Vector3.left, new Color(0, 1, 0));
        RaycastHit2D rayDown = Physics2D.Raycast(rigid.position, Vector3.down, 0.25f, LayerMask.GetMask("Platform"));
        RaycastHit2D rayRight = Physics2D.Raycast(rigid.position, Vector3.right, 0.25f, LayerMask.GetMask("Platform"));
        RaycastHit2D rayLeft = Physics2D.Raycast(rigid.position, Vector3.left, 0.25f, LayerMask.GetMask("Platform"));
        //playermove
        if (afterWallJumpStiff == 0 && dashTime == 0)
        {
            if (rayRight.collider == null && rayLeft.collider == null)
                rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);
            else
            {
                if (rigid.linearVelocityY < 0)
                    rigid.AddForce(Vector2.up * antiGravity, ForceMode2D.Impulse);
                if (rayRight.collider != null && Input.GetKey(KeyCode.LeftArrow))
                    rigid.AddForce(Vector2.left, ForceMode2D.Impulse);
                if (rayLeft.collider != null && Input.GetKey(KeyCode.RightArrow))
                    rigid.AddForce(Vector2.right, ForceMode2D.Impulse);
            }
        }
        //speedlimit
        if (dashTime == 0)
        {
            if (rigid.linearVelocityX > maxSpeed)
                rigid.linearVelocity = new Vector2(maxSpeed, rigid.linearVelocityY);
            else if (rigid.linearVelocityX < maxSpeed * (-1))
                rigid.linearVelocity = new Vector2(maxSpeed * (-1), rigid.linearVelocityY);
        }

        if (rigid.linearVelocityY < 0)
        {
            if (rayDown.collider != null || rayRight.collider != null || rayLeft.collider != null)
            {
                jumpCount = 0;
                dashCount = 0;
            }
        }
        if (dashTime > 0)
            dashTime--;
        if (dashCoolDown > 0)
            dashCoolDown--;
        if (afterWallJumpStiff > 0)
            afterWallJumpStiff--;
    }
}