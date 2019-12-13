using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
public class myTitan : MonoBehaviour
{
    bool isgrounded = false;
   // public Text embark_text;
    public GameObject TitanFPS;
    public GameObject PlayerFPS;
    public GameObject PlayerHUD;
    public GameObject TitanHUD;

    public Text embark_text;
    private bool can_embark;
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

        if (CrossPlatformInputManager.GetButtonDown("TitanEmbark") && can_embark)
        {
            embark_text.text = "";
            TitanFPS.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 5, gameObject.transform.position.z);
            TitanFPS.transform.rotation = gameObject.transform.rotation;
            Destroy(gameObject);
            PlayerFPS.SetActive(false);
            PlayerHUD.SetActive(false);
            TitanFPS.SetActive(true);
            TitanHUD.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player")) {
            embark_text.text = "Press 'E' to embark";
            can_embark = true;
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {

            embark_text.text = "";
            can_embark = false;
        }
    }

    void OnCollisionEnter(Collision theCollision)
    {
        if (theCollision.gameObject.name == "Plane")
        {
            isgrounded = true;
        }
    }
}
