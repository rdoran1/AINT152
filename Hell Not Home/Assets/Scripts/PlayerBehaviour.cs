﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    public delegate void UpdateHealth(int newHealth);
    public static event UpdateHealth OnUpdateHealth;

    public int health = 50;

    private void Start()
    {
        SendHealthData();
    }

    private void Update()
    {
        if(Input.GetKeyDown("w") || Input.GetKeyDown("a") || Input.GetKeyDown("s") || Input.GetKeyDown("d"))
        {

                GetComponent<AudioSource>().Play();

        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        SendHealthData();

        if (health <= 0)
        {
            Die();

        }
    }

    void Die()
    {
        Destroy(gameObject);
    }

    void SendHealthData()
    {
        if (OnUpdateHealth != null)
        {
            OnUpdateHealth(health);
        }
    }
}


