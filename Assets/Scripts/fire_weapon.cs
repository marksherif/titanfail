using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class fire_weapon : MonoBehaviour
{
    public Animator anim;
    public int ammo_capacity;
    public int current_ammo;
    public Text ammo_count;
    public GameObject bullet;
    private AudioSource audio_source_gun;
    public AudioClip gun_fire_audio;
    public AudioClip gun_reload_audio1;
    public AudioClip gun_reload_audio2;

    public GameObject player_hud_panel;
    public GameObject titan_hud_panel;

    // Start is called before the first frame update
    void Start()
    {
        audio_source_gun = GetComponent<AudioSource>();
        current_ammo = ammo_capacity;
    }

    // Update is called once per frame !Input.GetKey(KeyCode.C)
    void Update()

    {
        ammo_count.text = current_ammo + "/" + ammo_capacity;
        if (CrossPlatformInputManager.GetButton("Fire1") && current_ammo > 0 && (player_hud_panel.activeSelf || titan_hud_panel.activeSelf))
        {
            anim.SetBool("fire", true);
            if(!audio_source_gun.isPlaying)
                audio_source_gun.PlayOneShot(gun_fire_audio);
        }

        if ((current_ammo == 0 || (CrossPlatformInputManager.GetButtonDown("Reload")  && current_ammo < ammo_capacity)) && (player_hud_panel.activeSelf || titan_hud_panel.activeSelf))
        {
            anim.SetTrigger("reload");
            if (!audio_source_gun.isPlaying)
                audio_source_gun.PlayOneShot(gun_reload_audio1);
            //audio_source_gun.PlayOneShot(gun_fire_audio);
        }
    }

    public void fire()
    {
        var clone = Instantiate(bullet, transform.position, transform.rotation);
        clone.SetActive(true);
        clone.GetComponent<Rigidbody>().AddForce(clone.transform.forward * 1000);
        Destroy(clone, 3.0f);
    }

    public void fireAnimationEnded()
    {
        current_ammo -= 1;
        anim.SetBool("fire", false);
    }

    public void reloadAnimationEnded()
    {
        anim.ResetTrigger("reload");
        current_ammo = ammo_capacity;
    }
}
