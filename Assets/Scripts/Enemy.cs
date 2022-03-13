using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Score
{  
    protected int rarity;
    protected bool inWater = true, canGetHit = true;
    protected float health = 4;
    [SerializeField] protected Camera cam;
    [SerializeField] protected Transform player;
    [SerializeField] protected Animator anim;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    protected void SuperUpdate()
    {
        inWater = transform.position.y < 0;
        GetComponent<Rigidbody2D>().gravityScale = inWater ? 0 : 1;
        if (health == 0)
        {
            StartCoroutine(Die());
        }
    }
    
    IEnumerator Die()
    {
        foreach (var renderer in GetComponentsInChildren<SpriteRenderer>())
        {
            renderer.color = Color.red;
        }
        yield return new WaitForSeconds(.1f);
        foreach (var renderer in GetComponentsInChildren<SpriteRenderer>())
        {
            renderer.enabled = false;
        }
        GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(1);
        score += GetRarity();
        Destroy(gameObject);
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
        foreach (var renderer in GetComponentsInChildren<SpriteRenderer>())
        {
            renderer.color = Color.red;
        }
        yield return new WaitForSeconds(.1f);
        foreach (var renderer in GetComponentsInChildren<SpriteRenderer>())
        {
            renderer.color = Color.white;
        }
        yield return new WaitForSeconds(1);
        canGetHit = true;
    }

    public int GetRarity() {
        return rarity;
    }
}
