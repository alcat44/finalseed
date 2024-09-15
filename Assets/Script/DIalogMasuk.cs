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
    public MonoBehaviour SC_FPSController;
    public AudioSource audioSource;

    private int index;
    private bool isDialogueActive = false; // Menambahkan flag agar dialog hanya dimulai sekali

    // Start is called before the first frame update
    void Start()
    {
        textComponent.text = string.Empty;
        DialogBG.SetActive(false); // Pastikan dialog background tidak aktif di awal
    }

    void OnTriggerEnter(Collider other)
    {
        if ((other.CompareTag("Player") || other.CompareTag("MainCamera")) && !isDialogueActive) // Cek apakah dialog sudah aktif
        {
            StartDialogue();
            isDialogueActive = true; // Tandai bahwa dialog sudah dimulai
        }
    }

    // Update is called once per frame
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
        DialogBG.SetActive(true); // Menampilkan dialog background
        Dialog.SetActive(true);   // Menampilkan dialog object
        SC_FPSController.enabled = false;
        audioSource.enabled = false;
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
            textComponent.text = string.Empty;
            DialogBG.SetActive(false); // Menonaktifkan dialog background
            Destroy(Dialog); // Hapus dialog setelah selesai
            isDialogueActive = false; // Reset flag setelah dialog selesai
            SC_FPSController.enabled = true;
            audioSource.enabled = true;
        }
    }
}
