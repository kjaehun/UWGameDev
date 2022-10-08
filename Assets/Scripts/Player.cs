using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    Vector2 movementDirection;
    [Range(0.1f,3.0f)]
    public float moveSpeed;

    Rigidbody2D rigidbody2d;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = gameObject.GetComponent<Rigidbody2D>();
        movementDirection = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        movementDirection = Vector2.zero;
        if (Input.GetKey(KeyCode.W)) {
            movementDirection.y += 1;
        }
        if (Input.GetKey(KeyCode.S)) {
            movementDirection.y -= 1;
        }
        if (Input.GetKey(KeyCode.D)) {
            movementDirection.x += 1;
        }
        if (Input.GetKey(KeyCode.A)) {
            movementDirection.x -= 1;
        }

        movementDirection.Normalize();
        
    }

    void FixedUpdate() {
        rigidbody2d.velocity = moveSpeed * movementDirection;
    }
}
