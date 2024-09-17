using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DIalogMasuk : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public string[] lines;
    public float textSpeed;
    public GameObject Dialog;
    public GameObject DialogBG;
    public GameObject player;  // Reference ke Player GameObject
    public GameObject pauseMenuObject;  // Reference ke GameObject Pause
    public AudioSource audioSource;

    public bool isDialogueActive = false;  // Public flag untuk pengecekan status dialog

    private int index;
    private SC_FPSController scFpsController;
    private Pause pauseScript;

    void Start()
    {
        // Inisialisasi komponen pada Start
        scFpsController = player.GetComponent<SC_FPSController>();  // Ambil komponen SC_FPSController dari player
        pauseScript = pauseMenuObject.GetComponent<Pause>();  // Ambil komponen Pause dari pauseMenuObject

        textComponent.text = string.Empty;
        DialogBG.SetActive(false);  // Pastikan dialog background tidak aktif di awal
    }

    void OnTriggerEnter(Collider other)
    {
        if ((other.CompareTag("Player") || other.CompareTag("MainCamera")) && !isDialogueActive)
        {
            StartDialogue();
            isDialogueActive = true;
        }
    }

    void Update()
    {
        if (isDialogueActive && Input.GetKeyDown(KeyCode.E))
        {
            if (textComponent.text == lines[index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = lines[index];
            }
        }
    }

    void StartDialogue()
    {
        index = 0;
        StartCoroutine(TypeLine());
        DialogBG.SetActive(true);  // Tampilkan background dialog
        Dialog.SetActive(true);    // Tampilkan dialog object

        if (scFpsController != null) scFpsController.enabled = false;  // Nonaktifkan kontrol player
        if (pauseScript != null) pauseScript.enabled = false;  // Nonaktifkan pause menu
        if (audioSource != null) audioSource.enabled = false;  // Nonaktifkan audio source jika ada
    }

    IEnumerator TypeLine()
    {
        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        textComponent.text = string.Empty;
        DialogBG.SetActive(false);  // Sembunyikan background dialog
        Dialog.SetActive(false);    // Sembunyikan dialog object
        isDialogueActive = false;   // Reset flag setelah dialog selesai

        if (scFpsController != null) scFpsController.enabled = true;  // Aktifkan kontrol player
        if (pauseScript != null) pauseScript.enabled = true;  // Aktifkan pause menu
        if (audioSource != null) audioSource.enabled = true;  // Aktifkan audio source jika ada
    }
}

