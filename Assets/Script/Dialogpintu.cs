using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class Dialogpintu : MonoBehaviour
{
    public TextMeshProUGUI textComponent; // Text untuk dialog
    public GameObject intText; // GameObject untuk "Press E"
    public string[] lines; // Array untuk menampung dialog
    public float textSpeed;
    private int index, NumberDialog;
    public Image blackScreen; // Assign ini di Inspector dengan Image hitam di Canvas
    public float fadeDuration = 1f; // Durasi fade in dan fade out
    public bool interactable = false; // Untuk mengecek apakah pemain ada di dalam area trigger
    private bool isDialogActive = false; // Untuk mengecek apakah dialog sedang aktif

    public MonoBehaviour SC_FPSController; // Script player movement
    public AudioSource audioSource;
    public GameObject DialogBG;
    public AudioSource doorSFX;
    public bool hasKey = false; // Tandai apakah pemain sudah memiliki kunci
    public bool hasTuru = false;

    void Start()
    {
        textComponent.text = string.Empty;
        intText.SetActive(false); // Nonaktifkan teks "Press E" pada awalnya
    }

    void Update()
    {
        if (hasTuru)
        {
            NumberDialog = 2; // Jika hasTuru true, dialog mulai dari index 1
        }
        else
        {
            NumberDialog = 0; // Jika hasTuru false, dialog mulai dari index 0
        }

        // Jika pemain sudah berinteraksi dengan "Kunci" dan menekan E, ganti scene
        if (hasKey && Input.GetKeyDown(KeyCode.E) && interactable)
        {
            StartCoroutine(TransitionToScene("SampleScene")); // Ganti "SampleScene" dengan nama scene tujuan

            return;
        }

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
        index = NumberDialog;
        isDialogActive = true; // Tandai bahwa dialog aktif
        SC_FPSController.enabled = false; // Matikan script player movement
        audioSource.enabled = false;
        intText.SetActive(false);
        DialogBG.SetActive(true);
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
        if (index == NumberDialog)
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
            DialogBG.SetActive(false);
        }
    }

    // Ketika pemain berada di dalam area trigger pintu
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

    // Ketika pemain keluar dari area trigger pintu
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            intText.SetActive(false); // Sembunyikan "Press E"
            interactable = false; // Pemain tidak bisa berinteraksi
        }
    }

    IEnumerator TransitionToScene(string sceneName)
    {
        if (doorSFX != null)
        {
            doorSFX.Play();
        }
        // Fade in layar hitam
        yield return StartCoroutine(FadeScreen(0f, 1f)); // Fade in (layar hitam muncul)
        
        // Tunggu 3 detik
        yield return new WaitForSeconds(4f);

        // Ganti scene
        SceneManager.LoadScene(sceneName);

        // Setelah scene berubah, fade out ke tampilan baru
        yield return StartCoroutine(FadeScreen(1f, 0f)); // Fade out (layar hitam menghilang)
    }

    IEnumerator FadeScreen(float startAlpha, float endAlpha)
    {
        float elapsedTime = 0f;
        Color color = blackScreen.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            blackScreen.color = color;
            yield return null;
        }

        // Pastikan alpha tepat sesuai nilai akhir
        color.a = endAlpha;
        blackScreen.color = color;
    }
}
