using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMovement : MonoBehaviour
{
    private const string Horizontal = "Horizontal";
    private const string Vertical = "Vertical";

    private float horizontalInput, verticalInput;

    private float speed = 10, rotationSpeed = 300, dashCount = 2, remainingPropulsion = 500;

    private bool inWater = false,
        canGetHit = true,
        canShoot = true,
        isDashing = false,
        isRecharging = true,
        waitingForRecharge = false;

    private int health = 10;

    [SerializeField] private Transform tail, head;
    [SerializeField] private GameObject ink, bullet;
    [SerializeField] private Camera cam;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //print("Health: " + health);
        //print("horizontal: " + horizontalInput);
        //print("vertical: " + verticalInput);
        print("Remaining propulsion: " + remainingPropulsion);
        print(isRecharging);

        if (Input.GetMouseButtonDown(0) && canShoot)
        {
            StartCoroutine(Shoot());
        }

        //Main movement
        horizontalInput = Input.GetAxis(Horizontal);
        verticalInput = Input.GetAxis(Vertical);
        
        GetComponent<Rigidbody2D>().AddForce(Vector2.right * horizontalInput * speed);
        if (inWater) GetComponent<Rigidbody2D>().AddForce(Vector2.up * verticalInput * speed);

        if (Input.GetKeyDown(KeyCode.LeftShift) && dashCount > 0 && !isDashing)
        {
            StartCoroutine(Dash());
        }
        
        if (Input.GetKey(KeyCode.Space))
        {
            isRecharging = false;
            remainingPropulsion -= 3 * Time.time;
            if (remainingPropulsion > 1) GetComponent<Rigidbody2D>().AddForce(tail.up * 20);
        }
        else if (!waitingForRecharge && remainingPropulsion < 499)
        {
            StartCoroutine(Recharge());
        }

        if (isRecharging) remainingPropulsion += 100 * Time.deltaTime;
        if (remainingPropulsion < 0) remainingPropulsion = 0;
        if (remainingPropulsion > 500) remainingPropulsion = 500;
        
        //Change drag based on water/air
        inWater = transform.position.y < 0;
        GetComponent<Rigidbody2D>().gravityScale = inWater ? .3f : 1;

        //Rotate tail in movement direction
        Vector2 movementDirection = new Vector2(horizontalInput, verticalInput);
        movementDirection.Normalize();
        if (movementDirection != Vector2.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, movementDirection);
            tail.rotation =
                Quaternion.RotateTowards(tail.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        
        //Rotate head towards mouse
        Vector3 pos = cam.WorldToScreenPoint(head.position);
        Vector3 dir = Input.mousePosition - pos;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        head.rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);
        
        //Manage health
        if (health == 0)
        {
            Destroy(gameObject);
        }
    }
    
    IEnumerator Dash()
    {
        dashCount--;
        isDashing = true;
        GetComponent<Rigidbody2D>().AddForce(Vector2.right * horizontalInput * 200);
        GetComponent<Rigidbody2D>().AddForce(Vector2.up * verticalInput * 200);
        ink.SetActive(true);
        yield return new WaitForSeconds(.5f);
        isDashing = false;
        ink.SetActive(false);
        yield return new WaitForSeconds(3.5f);
        dashCount++;
    }
    
    IEnumerator Shoot()
    {
        canShoot = false;
        GameObject spawnedBullet = GameObject.Instantiate(bullet, head.position + head.up, head.rotation);
        spawnedBullet.GetComponent<Rigidbody2D>().AddForce(head.up * 500);
        yield return new WaitForSeconds(1);
        canShoot = true;
    }

    public void TakeDamage(int damage)
    {
        if (canGetHit)
        {
            health -= damage;
            StartCoroutine(Recover());
        }
    }

    IEnumerator Recover()
    {
        canGetHit = false;
        yield return new WaitForSeconds(3);
        canGetHit = true;
    }

    IEnumerator Recharge()
    {
        waitingForRecharge = true;
        yield return new WaitForSeconds(1);
        waitingForRecharge = false;
        isRecharging = true;
    }
}
