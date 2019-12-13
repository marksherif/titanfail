using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class next_level_platform : MonoBehaviour
{

    public Canvas canvas;
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
        var enemy_count = canvas.GetComponent<canvas_script>().enemy_count;
        var dead_enemies = canvas.GetComponent<canvas_script>().dead_enemies;
        var enemy_pilots = GameObject.FindGameObjectsWithTag("EnemyPilot");
        var enemy_titans = GameObject.FindGameObjectsWithTag("EnemyTitan");
        if (enemy_pilots.Length ==0 && enemy_titans.Length==0)
        {
            Debug.Log("Wadeeny");
            SceneManager.LoadScene("ParkourScene");
        }
    }
}
