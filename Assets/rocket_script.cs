using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class rocket_script : MonoBehaviour
{
    public GameObject explosion;
    public float damage= 150;
    public AudioSource explode;
    private bool exploded;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (exploded && (collider.gameObject.CompareTag("EnemyTitan") || collider.gameObject.CompareTag("EnemyPilot")))
        {
            collider.gameObject.GetComponent<enemy_script>().health -= damage;
            var health = collider.gameObject.GetComponent<enemy_script>().health;
            GameObject player = GameObject.Find("Player");
            RigidbodyFirstPersonController playerScript = player.GetComponent<RigidbodyFirstPersonController>();
            if (health <= 0)
            {
                if (collider.gameObject.name == "EnemyTitan")
                {
                    collider.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                    StartCoroutine(DestroyOnDying(0.5f, collider.gameObject));
                    playerScript.titanFallMeter += 50;

                }
                else
                {
                    Destroy(collider.gameObject);
                    playerScript.titanFallMeter += 10;
                }
                if (playerScript.titanFallMeter > 100)
                    playerScript.titanFallMeter = 100;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        exploded = true;
        explode.Play();
        explosion.SetActive(true);
        StartCoroutine(DestroyOnColliding(1.0f));

        // FIX ME: Rocket doesnt inflict damage when it comes in direct contact with an enemy 3ashan on trigger happens before exploded bool is set
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
}
