using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishEnemyMovement : Enemy
{
    [SerializeField] private Transform player;
    [SerializeField] private Camera cam;

    private bool canAttack = true;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 playerPosition = cam.WorldToViewportPoint(player.position);
        Vector2 transformPosition = (Vector2)cam.WorldToViewportPoint(transform.position);
        float angle = AngleBetweenTwoPoints(playerPosition, transformPosition);
        transform.rotation =  Quaternion.Euler(new Vector3(0f,0f,angle + 10));
        if (canAttack)
        {
            StartCoroutine(Attack());
        }
        
        if (GetComponent<Rigidbody2D>().velocity.magnitude == 0) GetComponent<Rigidbody2D>().angularVelocity = 0;

        if (health == 0)
        {
            Destroy(gameObject);
        }
    }
    
    float AngleBetweenTwoPoints(Vector3 a, Vector3 b) {
        return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }

    IEnumerator Attack()
    {
        canAttack = false;
        GetComponent<Rigidbody2D>().AddForce(transform.right * 700);
        yield return new WaitForSeconds(5);
        canAttack = true;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.transform.CompareTag("Player"))
        {
            col.transform.GetComponent<PlayerMovement>().TakeDamage(2);
        }
    }
}
