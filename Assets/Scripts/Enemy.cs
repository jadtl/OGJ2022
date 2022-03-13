using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected bool inWater = true, canGetHit = true;
    protected float health = 4;
    [SerializeField] protected Animator anim;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        inWater = transform.position.y < 0;
        GetComponent<Rigidbody2D>().gravityScale = inWater ? 0 : 1;
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
        yield return new WaitForSeconds(1);
        canGetHit = true;
    }
}
