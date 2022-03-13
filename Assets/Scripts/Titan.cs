using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Titan : MonoBehaviour
{
    [SerializeField] private AudioSource smash;
    [SerializeField] private Transform eye, doorLeft, doorRight, body, handColliderLeft, handColliderRight;
    [SerializeField] private GameObject fish, jellyfish;
    [SerializeField] private Animator anim;
    private bool openingDoors = false, closingDoors = false, canTry = true;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
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


    }

    IEnumerator Smash(int side)
    {
        Transform temp;
        if (side == 0) body.localScale = new Vector3(-1.5f, 1.5f, 1.5f);
        else body.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        temp = side == 0 ? handColliderLeft : handColliderRight;
        for (int i = 0; i < 4; i++)
        {
            temp.GetComponent<SpriteRenderer>().enabled = true;
            yield return new WaitForSeconds(.1f);
            temp.GetComponent<SpriteRenderer>().enabled = false;
            yield return new WaitForSeconds(.1f);
        }
        
        anim.SetBool("Smash", true);
        yield return new WaitForSeconds(.5f);
        temp.GetComponent<Collider2D>().enabled = true;
        yield return new WaitForSeconds(.5f);
        anim.SetBool("Smash", false);
        smash.Play();
        yield return new WaitForSeconds(.5f);
        temp.GetComponent<Collider2D>().enabled = false;
        
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
                Instantiate(fish, new Vector2(233.26f, -14.85f), Quaternion.identity);
            }
            else {
                Instantiate(jellyfish, new Vector2(233.26f, -14.85f), Quaternion.identity);
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
            print("open doors");
            StartCoroutine(OpenDoors());
        }
        else
        {
            print("smash");
            StartCoroutine(Smash(random == 3 ? 0 : 1));
        }
    }
}
