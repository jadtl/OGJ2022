using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitanLaser : Enemy
{
    private bool inPhase = false, isAttacking = false, pair = false;
    [SerializeField] private GameObject laser, eyeSprite;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SuperUpdate();
        
        if (!inPhase)
        {
            if (Random.Range(0, 2) == 0)
            {
                StartCoroutine(Passive());
            }
            else
            {
                StartCoroutine(Attack());
            }
        }

        if (isAttacking)
        {
            transform.RotateAround(eyeSprite.transform.position, pair? -transform.forward : transform.forward, 5*Time.deltaTime);
        }
    }

    IEnumerator Passive()
    {
        inPhase = true;
        yield return new WaitForSeconds(Random.Range(5, 10));
        inPhase = false;
    }
    
    IEnumerator Attack()
    {
        isAttacking = true;
        laser.SetActive(true);
        yield return new WaitForSeconds(5);
        laser.SetActive(false);
        isAttacking = false;
        pair = !pair;
    }
    
    
}
