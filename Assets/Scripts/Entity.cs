using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    int health;
    public int maxHealth;


    StatusBarController healthBar;
    // Start is called before the first frame update
    void Start()
    {
        healthBar = StatusBarController.GenerateBar(gameObject);
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void takeDamage(int damage) {
        health -= Mathf.Min(health, damage);
        healthBar.SetPercent((float)health / (float)maxHealth);
    }

    void OnMouseOver() {
        if (Input.GetMouseButtonDown(0)) {
            takeDamage(1);
        }
    }
}
