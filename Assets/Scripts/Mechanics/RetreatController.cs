using System.Collections;
using System.Collections.Generic;
using UnityEngine;

<<<<<<< Updated upstream
public class RetreatController : MonoBehaviour
{
=======
public class RetreatController : MonoBehaviour {
>>>>>>> Stashed changes
    public float speed;
    public float stoppingDistance;
    public float retreatDistance;

    public Transform player;

<<<<<<< Updated upstream
    void Start()
    {
=======
    void Start() {
>>>>>>> Stashed changes
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
<<<<<<< Updated upstream
    void Update()
    {
        if (Vector2.Distance(transform.position, player.position) > stoppingDistance){
            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);

        }
        else if (Vector2.Distance(transform.position, player.position) < stoppingDistance && Vector2.Distance(transform.position, player.position) > retreatDistance){
            transform.position = this.transform.position;
        }
        else if (Vector2.Distance(transform.position, player.position) < retreatDistance){
            transform.position = Vector2.MoveTowards(transform.position, player.position, -speed * Time.deltaTime);
        }
    }
}
=======
    void Update() {
        if (Vector2.Distance(transform.position, player.position) > stoppingDistance) {
            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);

        }
        else if (Vector2.Distance(transform.position, player.position) < stoppingDistance && Vector2.Distance(transform.position, player.position) > retreatDistance) {
            transform.position = this.transform.position;
        }
        else if (Vector2.Distance(transform.position, player.position) < retreatDistance) {
            transform.position = Vector2.MoveTowards(transform.position, player.position, -speed * Time.deltaTime);
        }
    }
}
>>>>>>> Stashed changes
