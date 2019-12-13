using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.CrossPlatformInput;

public class canvas_script : MonoBehaviour
{
    public GameObject main_menu_panel;
    public GameObject options_panel;
    public GameObject loadout_panel;
    public GameObject pause_panel;
    public GameObject player_hud_panel;
    public GameObject titan_hud_panel;
    public GameObject game_over_panel;
    public GameObject player;
    public Toggle toggle;
    public Slider volume_slider;
    public AudioClip main_menu_music;
    


    private bool player_hud_before_pause;
    private EventSystem event_system;
    private AudioSource main_menu_audio_source;

    public Sprite[] primary_image_sprites;
    public Sprite[] heavy_weapon_sprites;

    public Image primary_weapon_image;
    public Image heavy_weapon_image;

    private bool toggle_primary_image = false;
    private bool toggle_heavy_image = false;

    // Start is called before the first frame update
    void Start()
    {
        primary_weapon_image.sprite = primary_image_sprites[0];
        heavy_weapon_image.sprite = heavy_weapon_sprites[0];
        Time.timeScale = 0f;
        main_menu_panel.SetActive(true);
        main_menu_audio_source = GetComponent<AudioSource>();
        if (toggle.isOn)
        {
            main_menu_audio_source.PlayOneShot(main_menu_music);
        }
        event_system = FindObjectOfType<EventSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (CrossPlatformInputManager.GetButtonDown("Pause"))
        {
            onPause();
        }
       
       if (player.transform.position.y < 335)
        {
            OnGameOver();
        }
           
        
    }

    public void onToggleUpdate()
    {
        if (toggle.isOn)
        {
            main_menu_audio_source.PlayOneShot(main_menu_music);
        }
        else
            main_menu_audio_source.Stop();
    }

    public void onNextPrimaryWeapon()
    {
        if (toggle_primary_image)
            primary_weapon_image.sprite = primary_image_sprites[0];
        else
            primary_weapon_image.sprite = primary_image_sprites[1];
        toggle_primary_image = !toggle_primary_image;
    }

    public void onNextHeavyWeapon()
    {
        if (toggle_heavy_image)
            heavy_weapon_image.sprite = heavy_weapon_sprites[0];
        else
            heavy_weapon_image.sprite = heavy_weapon_sprites[1];
        toggle_heavy_image = !toggle_heavy_image;
    }

    public void onStart()
    {
        //loadout_panel.SetActive(true);
        main_menu_panel.SetActive(false);
        player_hud_panel.SetActive(true);
        Time.timeScale = 1f;
        main_menu_audio_source.Stop();
    }

    public void onPause()
    {
        
        player.GetComponent<RigidbodyFirstPersonController>().mouseLook.SetCursorLock(false);
        //event_system.currentInputModule()
        event_system.UpdateModules();
        pause_panel.SetActive(true);
        if (player_hud_panel.activeSelf)
            player_hud_before_pause = true;
        else
            player_hud_before_pause = false;
        player_hud_panel.SetActive(false);
        titan_hud_panel.SetActive(false);
        Time.timeScale = 0f;
        if (toggle.isOn)
        {
            main_menu_audio_source.PlayOneShot(main_menu_music);
        }

    }

    public void OnResume()
    {
        player.GetComponent<RigidbodyFirstPersonController>().mouseLook.SetCursorLock(true);
        if (player_hud_before_pause)
            player_hud_panel.SetActive(true);
        else
            titan_hud_panel.SetActive(true);
        pause_panel.SetActive(false);
        Time.timeScale = 1f;
        main_menu_audio_source.Stop();
    }
    public void onOptions()
    {
        options_panel.SetActive(true);
        main_menu_panel.SetActive(false);
    }
    public void onBackFromLoadout()
    {
        main_menu_panel.SetActive(true);
        loadout_panel.SetActive(false);
    }

    public void onBackFromOptions()
    {
        main_menu_panel.SetActive(true);
        options_panel.SetActive(false);
       
    }

    public void onRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void onMainMenuFromPause()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void OnVolumeChange()
    {
        AudioListener.volume = volume_slider.value;
    }

    public void OnGameOver()
    {
        player.GetComponent<RigidbodyFirstPersonController>().mouseLook.SetCursorLock(false);
        game_over_panel.SetActive(true);
        player_hud_panel.SetActive(false);
        titan_hud_panel.SetActive(false);
        Time.timeScale = 0f;
    }

    public void onQuit()
    {
        Application.Quit();
    }
}
