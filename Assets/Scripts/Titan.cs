using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Titan : MonoBehaviour
{
    [SerializeField] private Transform eye, doorLeft, doorRight, body, handCollider;
    [SerializeField] private GameObject fish, jellyfish;
    [SerializeField] private Animator anim;
    private bool openingDoors = false, closingDoors = false, canTry = true, isSmashing = false;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (openingDoors)
        {
            doorLeft.transform.position += Vector3.left/22;
            doorRight.transform.position += Vector3.right/22;
        }

        if (closingDoors)
        {
            doorLeft.transform.position += Vector3.right/22;
            doorRight.transform.position += Vector3.left/22;
        }

        if (canTry)
        {
            StartCoroutine(NewTry());
        }

        if (isSmashing)
        {
            handCollider.position -= Vector3.up * 5;
            if (handCollider.position.y < -1) handCollider.position = new Vector2(handCollider.position.x, -1);
        }
        
        
    }

    IEnumerator Smash(int side)
    {
        if (side == 0) body.localScale = new Vector3(-1.5f, 1.5f, 1.5f);
        else body.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        anim.SetBool("Smash", true);
        yield return new WaitForSeconds(1f);
        anim.SetBool("Smash", false);
        //isSmashing = true;
        handCollider.GetComponent<Collider2D>().enabled = true;
        handCollider.GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(.05f);
        //isSmashing = false;
        yield return new WaitForSeconds(1f);
        handCollider.GetComponent<SpriteRenderer>().color = Color.white;
        handCollider.GetComponent<Collider2D>().enabled = false;
        handCollider.transform.position = new Vector2(handCollider.transform.position.x, 5.47f);
        yield return new WaitForSeconds(1);
        canTry = true;

    }

    IEnumerator OpenDoors()
    {
        openingDoors = true;
        yield return new WaitForSeconds(1);
        openingDoors = false;
        int count = Random.Range(5, 11);
        for (int i = 0; i < count; i++)
        {
            if (Random.Range(0, 2) == 0)
            {
                //Instantiate(fish, new Vector2(233.26f, -14.85f), Quaternion.identity);
                Instantiate(fish, new Vector2(42.38f, -9.75f), Quaternion.identity);
            }
            else {
                //Instantiate(fish, new Vector2(42.38f, -9.75f), Quaternion.identity);
                //Instantiate(jellyfish, new Vector2(233.26f, -14.85f), Quaternion.identity);
            }
            yield return new WaitForSeconds(.5f);
        }
        closingDoors = true;
        yield return new WaitForSeconds(1);
        closingDoors = false;
        canTry = true;
    }

    IEnumerator NewTry()
    {
        canTry = false;
        int randomTime = Random.Range(3, 7);
        yield return new WaitForSeconds(randomTime);
        int random = Random.Range(0, 4);
        if (random < 2)
        {
            StartCoroutine(OpenDoors());
        }
        else
        {
            StartCoroutine(Smash(random == 3 ? 0 : 1));
        }
    }
}
