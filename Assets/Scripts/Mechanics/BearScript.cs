using System.Collections;
using System.Collections.Generic;
using Platformer.Mechanics;
using UnityEngine;

public class BearScript : EnemyController
{
    
    [SerializeField]
    private Transform player;
    
    [SerializeField]
    private float bearRange;

    [SerializeField]
    private float moveSpeed;

    private Rigidbody2D rb2d;
    
    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // check distance to player
        float distToPlayer = Vector2.Distance(transform.position, player.position);
        
        // log distance
        // print("distToPlayer:" + distToPlayer);

        if (distToPlayer < bearRange)
        {
            // code to chase player
            ChasePlayer();
        }
        else
        {
            // stop chasing player
            StopChasingPlayer();
        }


    }

    private void ChasePlayer()
    {
        if (transform.position.x < player.position.x)
        {
            // enemy is to the left hand side of player - so move right
            rb2d.velocity = new Vector2(moveSpeed, 0);
        }
        else if (transform.position.x > player.position.x)
        {
            // enemy is to the right hand side of player - so move left
            rb2d.velocity = new Vector2(-moveSpeed, 0);
        }
    }
    private void StopChasingPlayer()
    {
        rb2d.velocity = new Vector2(0, 0);
    }
}