using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBounds : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float currentX = transform.position.x; 
        float currentY = transform.position.y;
        float currentZ = transform.position.z;
        //print(currentX);
        print(currentY);
        if (currentY < -10)
        {
            transform.position = new Vector3(currentX, -10, currentZ);
        }
    }
}
