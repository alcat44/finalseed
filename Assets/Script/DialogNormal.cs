using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogNormal : MonoBehaviour
{
    public TextMeshProUGUI textComponent; // Text untuk dialog
    public GameObject intText; // GameObject untuk "Press E"
    public string[] lines; // Array untuk menampung dialog
    public float textSpeed;
    private int index;
    public bool interactable = false; // Untuk mengecek apakah pemain ada di dalam area trigger
    private bool isDialogActive = false; // Untuk mengecek apakah dialog sedang aktif

    public MonoBehaviour SC_FPSController;
    public Pause pauseScript;
    public PauseApart pauseScript2;  // Tambahkan public PauseApart pauseScript2
    public AudioSource audioSource;
    public GameObject DialogBG;

    void Start()
    {
        textComponent.text = string.Empty;
        intText.SetActive(false); // Nonaktifkan teks "Press E" pada awalnya
    }

    void Update()
    {
        // Hanya memungkinkan interaksi jika pemain berada di area trigger dan dialog belum aktif
        if (interactable && !isDialogActive && Input.GetKeyDown(KeyCode.E))
        {
            StartDialog();
        }

        // Saat dialog aktif, pemain dapat menekan E untuk melanjutkan ke baris berikutnya
        if (isDialogActive && Input.GetKeyDown(KeyCode.E))
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

    void StartDialog()
    {
        index = 0;
        isDialogActive = true; // Tandai bahwa dialog aktif
        SC_FPSController.enabled = false; // Matikan script player movement
        audioSource.enabled = false;
        intText.SetActive(false);
        DialogBG.SetActive(true);

        // Hanya nonaktifkan pauseScript jika tidak null
        if (pauseScript != null) 
        {
            pauseScript.enabled = false;
        }

        // Hanya nonaktifkan pauseScript2 jika tidak null
        if (pauseScript2 != null)
        {
            pauseScript2.enabled = false;
        }

        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        // Mengetik tiap karakter satu per satu
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
            isDialogActive = false; // Tandai bahwa dialog sudah selesai
            textComponent.text = string.Empty; // Kosongkan teks setelah dialog selesai
            SC_FPSController.enabled = true; // Aktifkan kembali script player movement
            audioSource.enabled = true;

            // Hanya aktifkan kembali pauseScript jika tidak null
            if (pauseScript != null)
            {
                pauseScript.enabled = true;
            }

            // Hanya aktifkan kembali pauseScript2 jika tidak null
            if (pauseScript2 != null)
            {
                pauseScript2.enabled = true;
            }

            DialogBG.SetActive(false);
        }
    }

    // Ketika pemain berada di dalam area trigger
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            // Tampilkan "Press E" hanya jika dialog belum aktif
            if (!isDialogActive)
            {
                intText.SetActive(true);
                interactable = true;
            }
        }
    }

    // Ketika pemain keluar dari area trigger
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            intText.SetActive(false); // Sembunyikan "Press E"
            interactable = false; // Pemain tidak bisa berinteraksi
        }
    }

    public void ResetDialog()
    {
        // Reset dialog UI and interaction states
        textComponent.text = string.Empty; // Clear any active dialog text
        intText.SetActive(false); // Hide interaction prompt
        interactable = false; // Reset interaction flag
        isDialogActive = false; // Ensure dialog is not active
        DialogBG.SetActive(false); // Hide dialog background
    }
}