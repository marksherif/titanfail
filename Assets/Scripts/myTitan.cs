using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class myTitan : MonoBehaviour
{

    bool isgrounded = false;
    public Text embark_text;
    public GameObject TitanFPS;
    public GameObject PlayerFPS;
    // Start is called before the first frame update
    void Start()
    {
     
    }

    // Update is called once per frame
    void Update()
    {
        if (isgrounded)
        {
            Destroy(GetComponent<ConstantForce>());
            Destroy(GetComponent<Rigidbody>());
        }
    
        if (CrossPlatformInputManager.GetButtonDown("TitanEmbark") && embark_text.text == "Press 'E' to embark the titan")
        {
            TitanFPS.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y+5, gameObject.transform.position.z);
            TitanFPS.transform.rotation = gameObject.transform.rotation;
            Destroy(gameObject);
            PlayerFPS.SetActive(false);
            TitanFPS.SetActive(true);
        }
            
    }
    void OnCollisionEnter(Collision theCollision)
    {
        if (theCollision.gameObject.name == "Plane")
        {
            isgrounded = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        embark_text.text = "Press 'E' to embark the titan";
    }

    private void OnTriggerExit(Collider other)
    {
        embark_text.text = "";
    }
}
