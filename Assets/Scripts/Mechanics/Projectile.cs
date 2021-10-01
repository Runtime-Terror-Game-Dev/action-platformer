using System.Collections;
using System.Collections.Generic;
using UnityEngine;

<<<<<<< Updated upstream
public class Projectile : MonoBehaviour
{
=======
public class Projectile : MonoBehaviour {
>>>>>>> Stashed changes
    public float speed;
    private Transform player;
    private Vector2 target;
    // Start is called before the first frame update
<<<<<<< Updated upstream
    void Start()
    {
=======
    void Start() {
>>>>>>> Stashed changes
        player = GameObject.FindGameObjectWithTag("Player").transform;
        target = new Vector2(player.position.x, player.position.y);

    }

    // Update is called once per frame
<<<<<<< Updated upstream
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
        
        if (transform.position.x == target.x && transform.position.y == target.y){
=======
    void Update() {
        transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (transform.position.x == target.x && transform.position.y == target.y) {
>>>>>>> Stashed changes
            DestroyProjectile();
        }
    }

<<<<<<< Updated upstream
    void OnTriggerEnter2D(Collider2D other){
        if (other.CompareTag("Player")){
            DestroyProjectile();
        }
    }
    
    void DestroyProjectile(){
        Destroy(gameObject);
    }
}
=======
    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            DestroyProjectile();
        }
    }

    void DestroyProjectile() {
        Destroy(gameObject);
    }
}
>>>>>>> Stashed changes
