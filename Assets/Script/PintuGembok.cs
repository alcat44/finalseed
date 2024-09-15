using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PintuGembok : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public string[] lines;
    public float textSpeed;
    public GameObject intText;
    public GameObject DialogBG;
    public MonoBehaviour SC_FPSController;
    public AudioSource audioSource;
    public AudioSource audioSource2;
    public AudioClip openSound;
    public AudioClip lockSound;
    public Collider doorCollider; // Collider pintu
    public GameObject Kunci;

    private int index, NumberDialog;
    private bool isDialogueActive = false; 
    public bool interactable = false;
    private bool hasKeyTouched = false; // Flag jika kunci sudah menyentuh pintu

    // Start is called before the first frame update
    void Start()
    {
        textComponent.text = string.Empty;
        intText.SetActive(false); // Nonaktifkan teks interaksi di awal
    }

    // Update is called once per frame
    void Update()
    {
        // Ubah nomor dialog jika kunci sudah menyentuh doorCollider
        if (hasKeyTouched)
        {
            NumberDialog = 2; // Dialog akan dimulai dari index 2 jika kunci menyentuh
        }
        else
        {
            NumberDialog = 0; // Jika hasTuru false, dialog mulai dari index 0
        }

        // Jika interaksi diizinkan dan pemain menekan E
        if (interactable && !isDialogueActive && Input.GetKeyDown(KeyCode.E))
        {
            StartDialog();
        }

        // Saat dialog aktif dan pemain menekan E untuk lanjut
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

    void StartDialog()
    {
        index = NumberDialog;
        isDialogueActive = true; // Tandai dialog sedang aktif
        SC_FPSController.enabled = false; // Nonaktifkan movement player
        audioSource.enabled = false;
        intText.SetActive(false); // Matikan intText saat dialog aktif
        DialogBG.SetActive(true);
        // Jika dialog dimulai dari NumberDialog == 0, putar suara lockSound
        
        if (NumberDialog == 0)
        {
            audioSource2.PlayOneShot(lockSound);
        }
        
        StartCoroutine(TypeLine());
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
        if (index == NumberDialog) // Lanjutkan ke dialog berikutnya jika masih ada
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            isDialogueActive = false; // Tandai dialog selesai
            textComponent.text = string.Empty; 
            SC_FPSController.enabled = true; // Aktifkan kembali kontrol pemain
            audioSource.enabled = true;
            DialogBG.SetActive(false); // Sembunyikan background dialog

            // Setelah dialog selesai, jika NumberDialog == 2, hancurkan objek ini
            if (NumberDialog == 2)
            {
                Destroy(gameObject); // Hancurkan objek setelah dialog yang dimulai dari NumberDialog == 2 selesai
            }
        }
    }


    // Deteksi jika doorCollider bersentuhan dengan objek bertag "Kunci"
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Kunci"))
        {
            hasKeyTouched = true; // Kunci menyentuh doorCollider
            Destroy(Kunci); // Hancurkan objek Kunci
            doorCollider.enabled = true; // Aktifkan collider pintu
            audioSource2.PlayOneShot(openSound);
        }
    }

    // Deteksi saat pemain mendekati pintu
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            if (!isDialogueActive)
            {
                intText.SetActive(true); // Tampilkan teks interaksi
                interactable = true; // Pemain bisa berinteraksi
            }
        }
    }

    // Deteksi saat pemain menjauh dari pintu
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            intText.SetActive(false); // Matikan teks interaksi
            interactable = false; // Pemain tidak bisa berinteraksi
        }
    }
}
