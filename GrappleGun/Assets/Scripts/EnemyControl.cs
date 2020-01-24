using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour {

    public Transform player;
    public Transform car;
    new Animator animation;
    public bool hit;

    private Rigidbody rb;

    // Use this for initialization
    void Start () {
        animation = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Car")
        {
            rb.AddForce(transform.forward * -3000);
        }
    }

    // Update is called once per frame
    void Update () {

        if (animation.enabled != false)
        {
            //Is enemy position close to player position?
            if (Vector3.Distance(player.position, this.transform.position) < 400 || Vector3.Distance(car.position, this.transform.position) < 400)
            {
                //Direction from player to enemy
                Vector3 direction = player.position - this.transform.position;
                //Prevent enemy from rotation upwards
                direction.y = 0;
                //Rotate towards player
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), 1f * Time.deltaTime);

                animation.SetBool("isIdle", false);

                //Move enemy towards player if position is close to player
                if (direction.magnitude > 5)
                {
                    this.transform.Translate(0, 0, 0.025f);
                    animation.SetBool("isWalking", true);
                }
                //Attack 
                else
                {
                    animation.SetBool("isAttacking", true);
                    animation.SetBool("isWalking", false);
                }
            }
            //Idle
            else
            {
                animation.SetBool("isIdle", true);
                animation.SetBool("isWalking", false);
                animation.SetBool("isAttacking", false);
            }
        }
	}
}
