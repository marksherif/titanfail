using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class enemy_script : MonoBehaviour
{

    public GameObject bullet;
    public Image health_bar;
    public GameObject explosion;
    private bool player_in_region;
    public float initial_enemy_health;
    public float health;
    private bool living = true;
    public Animator anim;


    void Start()
    {
        health = initial_enemy_health;
    }

    // Update is called once per frame
    void Update()
    {
        health_bar.fillAmount = health / initial_enemy_health;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            health -= collision.gameObject.GetComponent<bullet_hit>().bullet_damage;
            Destroy(collision.gameObject);
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            RigidbodyFirstPersonController playerScript = player.GetComponent<RigidbodyFirstPersonController>();
            if (health <= 0)
            {
                if (gameObject.CompareTag("EnemyTitan") && living)
                {
                    explosion.SetActive(true);
                    StartCoroutine(DestroyOnDying(0.5f));
                    playerScript.titanFallMeter += 50f;
                    living = false;

                }
                else if (gameObject.CompareTag("EnemyPilot"))
                {
                    Destroy(gameObject);
                    playerScript.titanFallMeter += 10f;
                }
                if (playerScript.titanFallMeter > 100)
                    playerScript.titanFallMeter = 100;
            }
      
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player")
        {
            anim.SetBool("player_near", true);
            Component[] components = gameObject.GetComponents(typeof(Component));
            Behaviour castedToBehaviour = components[5] as Behaviour;
            castedToBehaviour.enabled = true;
            player_in_region = true;
            StartCoroutine(ShootAtPlayer(1.7f));
        }
    }


    private void OnTriggerExit(Collider collider)
    {
        if (collider.tag == "Player")
        {
            anim.SetBool("player_near", false);
            Component[] components = gameObject.GetComponents(typeof(Component));
            Behaviour castedToBehaviour = components[5] as Behaviour;
            castedToBehaviour.enabled = false;
            player_in_region = false;
        }
    }
    IEnumerator ShootAtPlayer(float time)
    {
        yield return new WaitForSeconds(time);
        var clone = Instantiate(bullet, bullet.transform.position, bullet.transform.rotation);
        clone.SetActive(true);
        clone.GetComponent<Rigidbody>().AddForce(clone.transform.forward * 1000);
        Destroy(clone, 3.0f);
        if (player_in_region)
        {
            StartCoroutine(ShootAtPlayer(1.7f));
        }
    }

    IEnumerator DestroyOnDying(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
