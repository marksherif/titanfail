using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy_script : MonoBehaviour
{
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
        if (collider.tag == "Player")
        {
            Component[] components = gameObject.GetComponents(typeof(Component));
            Behaviour castedToBehaviour = components[5] as Behaviour;
            castedToBehaviour.enabled = true;
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.tag == "Player")
        {
            Component[] components = gameObject.GetComponents(typeof(Component));
            Behaviour castedToBehaviour = components[5] as Behaviour;
            castedToBehaviour.enabled = false;
        }
    }
}
