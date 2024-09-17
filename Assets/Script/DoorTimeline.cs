using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTimeline : MonoBehaviour
{
    public GameObject intText; // Interact text for player
    public bool interactable, toggle;
    public Animator doorAnim;
    public AudioSource audioSource;
    public AudioClip openSound;
    public AudioClip closeSound;
    public GameObject Timeline;

    private bool isEnemyNearby = false; // To check if the enemy is already near the door
    private bool isActionInProgress = false; // Flag to prevent spam triggering

    void OnTriggerStay(Collider other)
    {
        // Player interaction (using "MainCamera" tag)
        if (other.CompareTag("MainCamera"))
        {
            intText.SetActive(true);
            interactable = true;
        }

        // Enemy interaction (using "Enemy" tag)
        if (other.CompareTag("Enemy") && !isActionInProgress)
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
        if (other.CompareTag("Enemy") && !isActionInProgress)
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
        if (interactable == true && !isActionInProgress)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                
                toggle = !toggle;
                if (toggle == true)
                {
                    Timeline.SetActive(true);
                    StartCoroutine(OpenDoor());
                }
                else
                {
                    StartCoroutine(CloseDoor());
                }
                intText.SetActive(false);
                interactable = false;
            }
        }
    }

    IEnumerator OpenDoor()
    {
        isActionInProgress = true; // Set the flag to prevent action spam
        doorAnim.ResetTrigger("close");
        doorAnim.SetTrigger("open");
        audioSource.PlayOneShot(openSound);
        toggle = true;
        yield return new WaitForSeconds(1f); // Delay to prevent spam (adjust the delay as needed)
        isActionInProgress = false; // Reset the flag after door fully opens
    }

    IEnumerator CloseDoor()
    {
        isActionInProgress = true; // Set the flag to prevent action spam
        doorAnim.ResetTrigger("open");
        doorAnim.SetTrigger("close");
        audioSource.PlayOneShot(closeSound);
        toggle = false;
        yield return new WaitForSeconds(1f); // Delay to prevent spam (adjust the delay as needed)
        isActionInProgress = false; // Reset the flag after door fully closes
    }
}
