using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    public GameObject Pausemenu;
    public GameObject howToPlayCanvas;
    public string sceneName;
    public bool toggle;
    public SC_FPSController playerScript;
    public AudioSource audioSource;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            toggle = !toggle;

            if (toggle == false)
            {
                Pausemenu.SetActive(false);
                Time.timeScale = 1;
                playerScript.enabled = true;
                audioSource.enabled = true;
                Cursor.visible = false;  // Pastikan kursor tersembunyi ketika game berjalan
                Cursor.lockState = CursorLockMode.Locked;  // Kunci kursor dalam game
            }
            else
            {
                Pausemenu.SetActive(true);
                Time.timeScale = 0;
                playerScript.enabled = false;
                audioSource.enabled = false;
                Cursor.visible = true;  // Tampilkan kursor di menu pause
                Cursor.lockState = CursorLockMode.None;  // Bebaskan kursor
            }
        }
    }


    public void resumeGame()
    {
        toggle = false;
        Pausemenu.SetActive(false);
        Time.timeScale = 1;
        playerScript.enabled = true;
        audioSource.enabled = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void Mainmenu()
    {
        SceneManager.LoadScene(sceneName);
        playerScript.enabled = true;
        audioSource.enabled = true;
        Time.timeScale = 1;
    }
    public void Exit()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }

    public void ShowHowToPlay()
    {
        howToPlayCanvas.SetActive(true);
        Pausemenu.SetActive(false);
    }

    public void HideHowToPlay()
    {
        howToPlayCanvas.SetActive(false);
        Pausemenu.SetActive(true);
    }
}
