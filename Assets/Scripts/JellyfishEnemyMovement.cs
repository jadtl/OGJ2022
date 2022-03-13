using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellyfishEnemyMovement : Enemy
{
    [SerializeField] private Transform player;
    [SerializeField] private Camera cam;
    [SerializeField] private AudioSource source;
    private Vector2 initialPosition;
    private Vector2 destination;
    private bool reachedTarget = true;
    private float minWaitTime = 5f;
    private float maxWaitTime = 10f;
    private float waitTimeCountdown = -1f;
    
    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {    
        SuperUpdate();
        if (reachedTarget)
        {
            destination = initialPosition + Random.insideUnitCircle * 5;
            reachedTarget = false;
        }

        transform.position = Vector2.Lerp(transform.position, destination, Time.deltaTime/4);
        if (Vector2.Distance(transform.position, destination) < 1)
        {
            reachedTarget = true;
        }

        // Random sounds if close enough
        Vector2 playerPosition = cam.WorldToViewportPoint(player.position);
        Vector2 transformPosition = (Vector2)cam.WorldToViewportPoint(transform.position);
        print(Vector2.Distance(playerPosition, transformPosition));
        if (Vector2.Distance(playerPosition, transformPosition) < 0.6f) {
            if (!source.isPlaying) {
                if (waitTimeCountdown < 0f) {
                    source.Play(0);
                    waitTimeCountdown = Random.Range(minWaitTime, maxWaitTime);
                }
                else {
                    waitTimeCountdown -= Time.deltaTime;
                }
            }
        }
    }
    
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.transform.CompareTag("Player"))
        {
            col.transform.GetComponent<PlayerMovement>().TakeDamage(4);
        }
    }
}
