using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Titan : MonoBehaviour
{
    [SerializeField] private Transform eye, doorLeft, doorRight;
    [SerializeField] private GameObject fish, jellyfish;
    private bool openingDoors = false, closingDoors = false;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (openingDoors)
        {
            doorLeft.transform.position += Vector3.left;
            doorRight.transform.position += Vector3.right;
        }

        if (closingDoors)
        {
            doorLeft.transform.position += Vector3.right;
            doorRight.transform.position += Vector3.left;
        }
    }

    /*IEnumerator Smash()
    {
        yield return new WaitForSeconds();
    }*/

    IEnumerator OpenDoors()
    {
        openingDoors = true;
        yield return new WaitForSeconds(1);
        openingDoors = false;
        int count = Random.Range(2, 7);
        for (int i = 0; i < count; i++)
        {
            if (Random.Range(0, 2) == 0)
            {
                GameObject.Instantiate(fish, new Vector2(233.26f, -14.85f), Quaternion.identity);
            }
            else{
                GameObject.Instantiate(jellyfish, new Vector2(233.26f, -14.85f), Quaternion.identity);
            }
            yield return new WaitForSeconds(.5f);
        }
    }
}
