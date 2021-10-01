using System.Collections;
using System.Collections.Generic;
using UnityEngine;

<<<<<<< Updated upstream
public class ShootingEnemy : MonoBehaviour
{
=======
public class ShootingEnemy : MonoBehaviour {
>>>>>>> Stashed changes
    // Start is called before the first frame update
    private float timeBtwShots;
    public float startTimeBtwShots;

    public GameObject projectile;
    public Transform player;

<<<<<<< Updated upstream
    void Start()
    {
=======
    void Start() {
>>>>>>> Stashed changes
        player = GameObject.FindGameObjectWithTag("Player").transform;
        timeBtwShots = startTimeBtwShots;
    }

    // Update is called once per frame
<<<<<<< Updated upstream
    void Update()
    {
        if (timeBtwShots <= 0){
            Instantiate(projectile, transform.position, Quaternion.identity);
            timeBtwShots = startTimeBtwShots;
        }
        else{
            timeBtwShots -= Time.deltaTime;
        }
    }
}
=======
    void Update() {
        if (timeBtwShots <= 0) {
            Instantiate(projectile, transform.position, Quaternion.identity);
            timeBtwShots = startTimeBtwShots;
        }
        else {
            timeBtwShots -= Time.deltaTime;
        }
    }
}
>>>>>>> Stashed changes
