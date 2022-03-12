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

    private float speed = 10, rotationSpeed = 300;
    private bool isDashing = false, inWater = false, canGetHit = true;

    private int health = 10;

    [SerializeField] private Transform tail, head;
    [SerializeField] private GameObject ink;
    [SerializeField] private Camera cam;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        print("Health: " + health);
        //print("horizontal: " + horizontalInput);
        //print("vertical: " + verticalInput);
        
        //Change drag based on water/air
        inWater = transform.position.y < 0;
        GetComponent<Rigidbody2D>().gravityScale = inWater ? .3f : 1;
        
        //Main movement
        horizontalInput = Input.GetAxis(Horizontal);
        verticalInput = Input.GetAxis(Vertical);
        
        GetComponent<Rigidbody2D>().AddForce(Vector2.right * horizontalInput * speed);
        if (inWater) GetComponent<Rigidbody2D>().AddForce(Vector2.up * verticalInput * speed);

        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing)
        {
            StartCoroutine(Dash());
        }

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
        Vector2 positionOnScreen = cam.WorldToViewportPoint(head.position);
        Vector2 mouseOnScreen = (Vector2)cam.ScreenToViewportPoint(Input.mousePosition);
        float angle = AngleBetweenTwoPoints(positionOnScreen, mouseOnScreen);
        head.rotation =  Quaternion.Euler(new Vector3(0f,0f,angle + 90));
    }
    
    float AngleBetweenTwoPoints(Vector3 a, Vector3 b) {
        return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }
    
    IEnumerator Dash()
    {
        isDashing = true;
        GetComponent<Rigidbody2D>().AddForce(Vector2.right * horizontalInput * 200);
        GetComponent<Rigidbody2D>().AddForce(Vector2.up * verticalInput * 200);
        ink.SetActive(true);
        yield return new WaitForSeconds(1);
        ink.SetActive(false);
        yield return new WaitForSeconds(2);
        isDashing = false;
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
}
