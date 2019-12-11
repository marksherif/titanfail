using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class enemy_script : MonoBehaviour
{

    public GameObject bullet;
    public Image health_bar;
    public GameObject explosion;
    private bool player_in_region;
    private float health = 100;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        health_bar.fillAmount = health / 100f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            health -= collision.gameObject.GetComponent<bullet_hit>().bullet_damage;
            Destroy(collision.gameObject);
            if (health <= 0)
            {
                explosion.SetActive(true);
                StartCoroutine(DestroyOnDying(0.5f));
            }
                
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player")
        {
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
