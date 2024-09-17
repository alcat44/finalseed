using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hidingPlace : MonoBehaviour
{
    public GameObject hideText, stopHideText;
    public GameObject normalPlayer, hidingPlayer;
    public enemyAI1 monsterScript;
    public Transform monsterTransform;
    bool interactable, hiding;
    public float loseDistance;

    // Cache SC_FPSController dynamically
    private SC_FPSController playerScript;

    void Start()
    {
        interactable = false;
        hiding = false;

        // Dynamically find the SC_FPSController on the normal player
        playerScript = normalPlayer.GetComponent<SC_FPSController>();
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            hideText.SetActive(true);
            interactable = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            hideText.SetActive(false);
            interactable = false;
        }
    }

    void Update()
    {
        if (interactable)
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                hideText.SetActive(false);
                hidingPlayer.SetActive(true);
                float distance = Vector3.Distance(monsterTransform.position, normalPlayer.transform.position);
                if (distance > loseDistance)
                {
                    if (monsterScript.chasing)
                    {
                        monsterScript.stopChase();
                    }
                }
                stopHideText.SetActive(true);
                hiding = true;
                normalPlayer.SetActive(false);
                interactable = false;

                // Reset exhaustion text during hiding, if player is not exhausted
                if (playerScript != null && playerScript.exhaustionText != null)
                {
                    playerScript.exhaustionText.SetActive(false);
                }
            }
        }

        if (hiding)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                stopHideText.SetActive(false);
                normalPlayer.SetActive(true);
                hidingPlayer.SetActive(false);
                hiding = false;

                // Handle exhaustion state after exiting hiding
                if (playerScript != null)
                {
                    playerScript.ResetPlayerMovement();
                }
            }
        }
    }

    public void ResetHiding()
    {
        // Reset flags
        interactable = false;
        hiding = false;

        // Ensure player exits hiding mode
        normalPlayer.SetActive(true);
        hidingPlayer.SetActive(false);

        // Disable all UI texts related to hiding
        hideText.SetActive(false);
        stopHideText.SetActive(false);
    }
}
