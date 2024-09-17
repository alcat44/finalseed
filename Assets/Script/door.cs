using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public GameObject intText; // Interact text for player
    public bool interactable, toggle;
    public Animator doorAnim;
    public AudioSource audioSource;
    public AudioClip openSound;
    public AudioClip closeSound;
    public Collider Object;

    private bool isEnemyNearby = false; // To check if the enemy is already near the door
    // Hapus flag isActionInProgress untuk mengizinkan interaksi berulang
    // private bool isActionInProgress = false; 

    void OnTriggerStay(Collider other)
    {
        // Player interaction (using "MainCamera" tag)
        if (other.CompareTag("MainCamera"))
        {
            intText.SetActive(true);
            interactable = true;
        }

        // Enemy interaction (using "Enemy" tag)
        if (other.CompareTag("Enemy"))
        {
            if (!toggle && !isEnemyNearby)
            {
                isEnemyNearby = true; // Set flag to prevent repeated triggers
                StartCoroutine(OpenDoor());
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Player exits trigger (using "MainCamera" tag)
        if (other.CompareTag("MainCamera"))
        {
            intText.SetActive(false);
            interactable = false;
        }

        // Enemy exits trigger (using "Enemy" tag)
        if (other.CompareTag("Enemy"))
        {
            if (toggle && isEnemyNearby)
            {
                isEnemyNearby = false; // Reset flag when enemy leaves
                StartCoroutine(CloseDoor());
            }
        }
    }

    void Update()
    {
        // Player manual interaction (using Key "E")
        if (interactable)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                toggle = !toggle;
                Debug.Log("Tombol E ditekan. toggle: " + toggle);

                if (toggle)
                {
                    Debug.Log("Membuka pintu.");
                    StartCoroutine(OpenDoor());
                }
                else
                {
                    Debug.Log("Menutup pintu.");
                    StartCoroutine(CloseDoor());
                }
                // Tetap aktifkan teks interaksi agar dapat diinteraksi kembali
            }
        }
    }

    IEnumerator OpenDoor()
    {
        // isActionInProgress tidak digunakan lagi
        Debug.Log("Animasi membuka pintu dimulai.");
        doorAnim.ResetTrigger("close");
        doorAnim.SetTrigger("open");
        audioSource.PlayOneShot(openSound);
        Object.enabled = true;
        toggle = true;
        yield return null; // Tidak perlu menunggu penuh
        Debug.Log("Animasi membuka pintu selesai.");
    }

    IEnumerator CloseDoor()
    {
        // isActionInProgress tidak digunakan lagi
        Debug.Log("Animasi menutup pintu dimulai.");
        doorAnim.ResetTrigger("open");
        doorAnim.SetTrigger("close");
        audioSource.PlayOneShot(closeSound);
        Object.enabled = false;
        toggle = false;
        yield return null; // Tidak perlu menunggu penuh
        Debug.Log("Animasi menutup pintu selesai.");
    }

    public void ResetInteraction()
    {
        intText.SetActive(false); // Make sure interaction text is off
        interactable = false; // Reset interactable flag
        Debug.Log("Interaction reset after player respawn.");
    }
}
