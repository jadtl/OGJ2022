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
    private bool isDashing = false;

    [SerializeField] private Transform tail, head;
    [SerializeField] private Camera cam;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        print("horizontal: " + horizontalInput);
        //print("vertical: " + verticalInput);
        horizontalInput = Input.GetAxis(Horizontal);
        verticalInput = Input.GetAxis(Vertical);
        
        GetComponent<Rigidbody2D>().AddForce(Vector2.right * horizontalInput * speed);
        GetComponent<Rigidbody2D>().AddForce(Vector2.up * verticalInput * speed);

        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing)
        {
            StartCoroutine(Dash());
        }

        Vector2 movementDirection = new Vector2(horizontalInput, verticalInput);
        movementDirection.Normalize();
        if (movementDirection != Vector2.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, movementDirection);
            tail.rotation =
                Quaternion.RotateTowards(tail.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        
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
        GetComponent<Rigidbody2D>().AddForce(Vector2.right * horizontalInput * 100);
        GetComponent<Rigidbody2D>().AddForce(Vector2.up * verticalInput * 100);
        yield return new WaitForSeconds(3);
        isDashing = false;
    }
}
