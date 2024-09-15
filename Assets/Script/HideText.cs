using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HideText : MonoBehaviour
{
    public GameObject intText; 
    public bool interactable;
    public MonoBehaviour door;
    public MonoBehaviour door2;

    void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("MainCamera"))
        {
            intText.SetActive(false);
            door.enabled = false;
            door2.enabled = false;
            interactable = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("MainCamera"))
        {
            door.enabled = true;
            door2.enabled = true;
            interactable = false;
        }
    }
}
