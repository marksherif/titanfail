using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet_hit : MonoBehaviour

{

    public int bullet_damage = 10;

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
        if (collision.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }

        // TODO Collision with tag player --> damage player by bullet damage amount
    }
}
