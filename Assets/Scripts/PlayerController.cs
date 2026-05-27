using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private float movementX;
    private float movementY;

    public float speed = 10f;
    public float turnSpeed = 120f;

    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 20f;
    public int projectileDamage = 1;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    void OnJump()
    {
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        Bullet bullet = projectile.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.ownerTag = "Player";
            bullet.damage = projectileDamage;
        }

        Rigidbody projRb = projectile.GetComponent<Rigidbody>();
        projRb.linearVelocity = firePoint.forward * projectileSpeed;
        Destroy(projectile, 3f);
    }

    void FixedUpdate()
    {
        MoveTank();
        TurnTank();
    }

    private void MoveTank()
    {
        Vector3 movement = transform.forward * movementY * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);
    }

    private void TurnTank()
    {
        float turn = movementX * turnSpeed * Time.fixedDeltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
        }
    }
}