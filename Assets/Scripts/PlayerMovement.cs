using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;

public class PlayerMovement : Score
{
    private const string Horizontal = "Horizontal";
    private const string Vertical = "Vertical";

    private float horizontalInput, verticalInput;

    private float speed = 7, rotationSpeed = 300, dashCount = 2, remainingPropulsion = 1000;

    private bool inWater = false,
        canGetHit = true,
        canShoot = true,
        isDashing = false,
        isRecharging = true,
        waitingForRecharge = false;

    private int health = 20, currentCheckpoint = 0;
    private List<Vector2> checkpoints = new List<Vector2>();
    [SerializeField] private AudioSource core, soy, waterIn, waterOut;
    [SerializeField] private Transform tail, head;
    [SerializeField] private GameObject dashParticles, jetParticles, bullet, titan;
    [SerializeField] private Camera cam;
    [SerializeField] private Animator anim;
    [SerializeField] private Slider healthSlider, inkSlider;
    [SerializeField] private TextMeshProUGUI scoreText;

    // Start is called before the first frame update
    void Start()
    {
        checkpoints.Add(new Vector2(-5, -15));
        checkpoints.Add(new Vector2(108, -15));
        checkpoints.Add(new Vector2(195, -25));
        core.Play();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //print("Health: " + health);
        //print("horizontal: " + horizontalInput);
        //print("vertical: " + verticalInput);
        //print("Remaining propulsion: " + remainingPropulsion);
        //print(isRecharging);
        //print(GetComponent<Rigidbody2D>().velocity.magnitude);
        //print(score);

        healthSlider.value = (float) health / 20;
        inkSlider.value = remainingPropulsion / 1000;
        if (score > 99999) score = 99999;
        scoreText.text = score.ToString();

        if (Input.GetMouseButtonDown(0) && canShoot)
        {
            StartCoroutine(Shoot());
        }

        //Main movement
        horizontalInput = Input.GetAxis(Horizontal);
        verticalInput = Input.GetAxis(Vertical);
        if (horizontalInput == 0 && verticalInput == 0) GetComponent<Rigidbody2D>().angularVelocity = 0;

        GetComponent<Rigidbody2D>().AddForce(Vector2.right * horizontalInput * speed);
        if (inWater) GetComponent<Rigidbody2D>().AddForce(Vector2.up * verticalInput * speed);

        if (Input.GetKeyDown(KeyCode.LeftShift) && dashCount > 0 && !isDashing)
        {
            StartCoroutine(Dash());
        }

        if (Input.GetKey(KeyCode.Space))
        {
            isRecharging = false;
            remainingPropulsion -= Time.deltaTime * 700;
            if (remainingPropulsion > 1)
            {
                jetParticles.SetActive(true);
                GetComponent<Rigidbody2D>().AddForce(tail.up * 10);
            }
        }
        else if (!waitingForRecharge && remainingPropulsion < 999)
        {
            StartCoroutine(Recharge());
        }
        if (!Input.GetKey(KeyCode.Space)) jetParticles.SetActive(false);
        if (isRecharging) remainingPropulsion += 300 * Time.deltaTime;
        if (remainingPropulsion < 0) remainingPropulsion = 0;
        if (remainingPropulsion > 1000) remainingPropulsion = 1000;

        //Change drag based on water/air
        inWater = transform.position.y < 0;
        GetComponent<Rigidbody2D>().gravityScale = inWater ? .3f : 1;
        if (Mathf.Abs(transform.position.y) < .1f)
        {
            if (GetComponent<Rigidbody2D>().velocity.y > 0) waterOut.Play();
            else waterIn.Play();
        }

        //Rotate tail in movement direction
        Vector2 movementDirection = new Vector2(horizontalInput, verticalInput);
        movementDirection.Normalize();
        if (movementDirection != Vector2.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, movementDirection);
            tail.rotation =
                Quaternion.RotateTowards(tail.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        //Rotate head towards mouse
        Vector3 pos = cam.WorldToScreenPoint(head.position);
        Vector3 dir = Input.mousePosition - pos;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        head.rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);

        //Manage health
        if (health == 0)
        {
            StartCoroutine(Die());
        }

        if (currentCheckpoint == 2 && !soy.isPlaying) {
            core.Stop();
            soy.Play();
        }
        
        if (currentCheckpoint == 2) titan.SetActive(true);
    }

    IEnumerator Dash()
    {
        dashCount--;
        isDashing = true;
        canGetHit = false;
        GetComponent<Rigidbody2D>().AddForce(Vector2.right * horizontalInput * 200);
        GetComponent<Rigidbody2D>().AddForce(Vector2.up * verticalInput * 200);
        dashParticles.SetActive(true);
        anim.SetBool("Dash", true);
        yield return new WaitForSeconds(.75f);
        anim.SetBool("Dash", false);
        isDashing = false;
        canGetHit = true;
        dashParticles.SetActive(false);
        yield return new WaitForSeconds(3.5f);
        dashCount++;
    }

    IEnumerator Shoot()
    {
        canShoot = false;
        GameObject spawnedBullet = GameObject.Instantiate(bullet, head.position + head.up * .25f, head.rotation);
        spawnedBullet.GetComponent<Rigidbody2D>().AddForce(head.up * 1000);
        yield return new WaitForSeconds(.1f);
        canShoot = true;
    }

    public void TakeDamage(int damage)
    {
        if (canGetHit)
        {
            health -= damage;
            StartCoroutine(Recover());
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
            renderer.color = Color.white;
        }

        health = 20;
        transform.position = checkpoints[currentCheckpoint];
    }

    IEnumerator Recover()
    {
        foreach (var renderer in GetComponentsInChildren<SpriteRenderer>())
        {
            renderer.color = Color.red;
        }
        yield return new WaitForSeconds(.1f);
        foreach (var renderer in GetComponentsInChildren<SpriteRenderer>())
        {
            renderer.color = Color.white;
        }
        canGetHit = false;
        yield return new WaitForSeconds(3);
        canGetHit = true;
    }

    IEnumerator Recharge()
    {
        waitingForRecharge = true;
        yield return new WaitForSeconds(1);
        waitingForRecharge = false;
        isRecharging = true;
    }

    public void SetCheckpoint(int index)
    {
        currentCheckpoint = index;
    }

    public void IncrementScore(int value) {
        score += value;
    }
}
