using System.Collections;
using System.Collections.Generic;
using Platformer.Mechanics;
using UnityEngine;
using System;

public class ShootingEnemy : EnemyController
{
    // Start is called before the first frame update
    private float timeBtwShots;
    public float startTimeBtwShots;
    public float shootingDistance;

    public GameObject projectile;
    public Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        timeBtwShots = startTimeBtwShots;
    }

    void ShootAtPlayer()
    {
        if (timeBtwShots <= 0)
        {
            Instantiate(projectile, transform.position, Quaternion.identity);
            timeBtwShots = startTimeBtwShots;
        }
        else
        {
            timeBtwShots -= Time.deltaTime;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Math.Abs(player.position.x - transform.position.x) <= shootingDistance){
            ShootAtPlayer();
        }
    }
}