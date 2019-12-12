using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class rocket_script : MonoBehaviour
{
    public GameObject explosion;
    public float damage= 150;
    public float explosion_radius;
    public AudioSource explode;
    private bool do_it_once = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (do_it_once)
        {
            do_it_once = false;
            explode.Play();
            explosion.SetActive(true);
            ExplosionDamage(explosion_radius);
            StartCoroutine(DestroyOnColliding(1.0f));
        }
    }

    IEnumerator DestroyOnColliding(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }

    IEnumerator DestroyOnDying(float time, GameObject obj)
    {
        yield return new WaitForSeconds(time);
        Destroy(obj);
    }

    void ExplosionDamage(float radius)
    {
        Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, radius);
        int i = 0;
        while (i < hitColliders.Length)
        {
            if (hitColliders[i].gameObject.CompareTag("EnemyTitan") || hitColliders[i].gameObject.CompareTag("EnemyPilot")){
                hitColliders[i].gameObject.GetComponent<enemy_script>().health -= damage;
                var health = hitColliders[i].gameObject.GetComponent<enemy_script>().health;
                GameObject player = GameObject.Find("Player");
                RigidbodyFirstPersonController playerScript = player.GetComponent<RigidbodyFirstPersonController>();
                if (health <= 0)
                {
                    if (hitColliders[i].gameObject.CompareTag("EnemyTitan"))
                    {
                        hitColliders[i].gameObject.transform.GetChild(0).gameObject.SetActive(true);
                        StartCoroutine(DestroyOnDying(0.5f, hitColliders[i].gameObject));
                        playerScript.titanFallMeter += 50;

                    }
                    else
                    {
                        Destroy(hitColliders[i].gameObject);
                        playerScript.titanFallMeter += 10;
                    }
                    if (playerScript.titanFallMeter > 100)
                        playerScript.titanFallMeter = 100;
                }
            }
            if (i+2<hitColliders.Length && hitColliders[i].gameObject == hitColliders[i + 1].gameObject)
                i += 2;
            else
                i++;
        }
    }
}
