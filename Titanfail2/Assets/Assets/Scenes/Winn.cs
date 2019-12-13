using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Winn : MonoBehaviour
{
    void onTriggerEnter(Collider d)
    {
        SceneManager.LoadScene(1);
    }
}
