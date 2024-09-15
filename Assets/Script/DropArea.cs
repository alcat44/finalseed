using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Playables;

public class DropArea : MonoBehaviour
{
    public GameObject objectToSpawn; // Objek yang akan muncul di area trigger
    public float spawnDelay = 1f; // Waktu tunda sebelum objek muncul
    public float activeDuration = 10f; // Durasi objek aktif sebelum dinonaktifkan

    public TextMeshProUGUI textComponent; // Komponen teks untuk dialog
    public string[] lines; // Baris-baris teks dialog
    public float textSpeed; // Kecepatan pengetikan teks
    public GameObject DialogBG; // Background dialog
    public MonoBehaviour SC_FPSController; // Kontrol player
    public AudioSource audioSource;

    public PlayableDirector timeline; // Timeline yang akan dipanggil setelah 2 trigger
    public GameObject player; // Player object yang akan disembunyikan selama timeline berjalan
    public Camera playerCamera; // Kamera pemain
    public Camera cutsceneCamera; // Kamera cutscene

    private int index, NumberDialog; // Index baris dialog saat ini
    private bool isDialogueActive = false; // Apakah dialog sedang aktif
    private int triggerCount = 0; // Variabel untuk menghitung jumlah trigger
    private bool hasObjectSpawned = false; // Flag untuk memastikan objek hanya muncul sekali

    public GameObject intText; // UI "Press E" untuk interaksi
    public bool interactable; // Apakah pemain dapat berinteraksi
    public Collider Object;
    
    public GameObject objectToDisable1;
    public GameObject objectToDisable2;

    private void Start()
    {
        // Pastikan objek yang akan muncul dimulai dalam keadaan tidak aktif
        if (objectToSpawn != null)
        {
            objectToSpawn.SetActive(false);
        }

        if (timeline != null)
        {
            timeline.gameObject.SetActive(false); // Pastikan timeline dimulai dalam keadaan tidak aktif
            timeline.stopped += OnTimelineStopped; // Berlangganan event ketika timeline berhenti
        }

        // Pastikan kamera cutscene dimulai dalam keadaan tidak aktif
        if (cutsceneCamera != null)
        {
            cutsceneCamera.gameObject.SetActive(false);
        }

        textComponent.text = string.Empty;
        DialogBG.SetActive(false); // Nonaktifkan background dialog di awal
        intText.SetActive(false); // Nonaktifkan teks "Press E" di awal
    }

    void OnTriggerStay(Collider other)
    {

        if (other.CompareTag("MainCamera"))
        {
            if (!isDialogueActive)
            {
                intText.SetActive(true);
                interactable = true;
            }
        }
    }


    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            intText.SetActive(false);
            interactable = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pickupable") || other.CompareTag("Pickupable2"))
        {
            triggerCount++; // Tambahkan jumlah trigger setiap kali objek mengenai trigger

            if (other.CompareTag("Pickupable"))
            {
                NumberDialog = 0; // Set NumberDialog untuk Pickupable
            }
            else if (other.CompareTag("Pickupable2"))
            {
                NumberDialog = 4; // Set NumberDialog untuk Pickupable2
            }

            if (!hasObjectSpawned)
            {
                hasObjectSpawned = true; // Set flag agar objek tidak muncul lagi
                StartCoroutine(ActivateAndDeactivateObject());
            }

            Destroy(other.gameObject); // Hapus objek yang dibuang

            if (triggerCount >= 2 && timeline != null)
            {
                // Aktifkan timeline
                timeline.gameObject.SetActive(true);
                timeline.Play();

                // Nonaktifkan semua interaksi
                ToggleInteractions(false);

                // Sembunyikan player
                HidePlayer();

                // Beralih ke kamera cutscene
                SwitchToCutsceneCamera();
            }
            else if (triggerCount == 1) // Mulai dialog jika triggerCount == 1
            {
                StartDialog(); // Mulai dialog dari NumberDialog yang sudah ditentukan
            }
        }
    }


    private void Update()
    {
        // Menangani input untuk melanjutkan dialog
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
        index = NumberDialog; // Mulai dialog dari nilai NumberDialog
        isDialogueActive = true; // Tandai dialog sedang aktif
        SC_FPSController.enabled = false; // Nonaktifkan movement player
        audioSource.enabled = false;
        intText.SetActive(false); // Sembunyikan teks "Press E"
        DialogBG.SetActive(true); // Tampilkan background dialog

        StartCoroutine(TypeLine()); // Mulai dialog pertama
    }

    void NextLine()
    {
        // Pastikan index tidak melebihi jumlah baris dalam array lines
        if (index < lines.Length - 1 && index < NumberDialog + 3) 
        {
            index++; // Tambah index
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine()); // Tampilkan dialog berikutnya
        }
        else
        {
            EndDialog(); // Akhiri dialog jika sudah tidak ada lagi line atau index melebihi batas
        }
    }
    
    IEnumerator TypeLine()
    {
        textComponent.text = string.Empty;
        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }


    void EndDialog()
    {
        isDialogueActive = false; // Tandai dialog selesai
        textComponent.text = string.Empty;
        SC_FPSController.enabled = true; // Aktifkan kembali kontrol pemain
        audioSource.enabled = true;
        DialogBG.SetActive(false); // Sembunyikan background dialog

        // Kembalikan kamera ke player jika ada timeline
        if (timeline != null)
        {
            SwitchToPlayerCamera();
        }
    }

    private IEnumerator ActivateAndDeactivateObject()
    {
        // Tunggu beberapa detik sebelum objek muncul
        yield return new WaitForSeconds(spawnDelay);

        // Aktifkan objek
        if (objectToSpawn != null)
        {
            objectToSpawn.SetActive(true);
            // Set objek muncul di posisi area trigger
            objectToSpawn.transform.position = transform.position;
            objectToSpawn.transform.rotation = transform.rotation;

            // Tunggu beberapa detik sebelum menonaktifkan objek
            yield return new WaitForSeconds(activeDuration);

            // Nonaktifkan objek
            objectToSpawn.SetActive(false);

            // Reset flag untuk memungkinkan penggunaan kembali jika diperlukan
            hasObjectSpawned = false;
        }
    }

    // Fungsi untuk menyembunyikan player
    private void HidePlayer()
    {
        // Menonaktifkan kontrol player (misalnya script movement)
        if (player != null)
        {
            player.GetComponent<SC_FPSController>().enabled = false;
            player.GetComponent<AudioSource>().enabled = false;

            // Nonaktifkan mesh renderer (atau komponen visual lainnya) untuk "menyembunyikan" player
            foreach (Renderer rend in player.GetComponentsInChildren<Renderer>())
            {
                rend.enabled = false;
            }
        }
    }

    // Fungsi untuk mengembalikan player ke kondisi semula
    private void ShowPlayer()
    {
        // Mengaktifkan kembali kontrol player
        if (player != null)
        {
            player.GetComponent<SC_FPSController>().enabled = true;
            player.GetComponent<AudioSource>().enabled = true;

            // Aktifkan kembali mesh renderer untuk "menampilkan" player
            foreach (Renderer rend in player.GetComponentsInChildren<Renderer>())
            {
                rend.enabled = true;
            }
        }
    }

    // Event ketika timeline berhenti
    private void OnTimelineStopped(PlayableDirector director)
    {
        // Ketika timeline selesai, kembalikan player
        ShowPlayer();

        // Kembalikan ke kamera player
        SwitchToPlayerCamera();

        // Aktifkan kembali semua interaksi
        ToggleInteractions(true);

        if (objectToDisable1 != null)
        {
        objectToDisable1.SetActive(false);
        }

        if (objectToDisable2 != null)
        {
            objectToDisable2.SetActive(false);
        }
    }

    // Fungsi untuk beralih ke kamera cutscene
    private void SwitchToCutsceneCamera()
    {
        if (cutsceneCamera != null && playerCamera != null)
        {
            playerCamera.gameObject.SetActive(false); // Nonaktifkan kamera player
            cutsceneCamera.gameObject.SetActive(true); // Aktifkan kamera cutscene
        }
    }

    // Fungsi untuk mengembalikan ke kamera player
    private void SwitchToPlayerCamera()
    {
        if (cutsceneCamera != null && playerCamera != null)
        {
            cutsceneCamera.gameObject.SetActive(false); // Nonaktifkan kamera cutscene
            playerCamera.gameObject.SetActive(true); // Aktifkan kamera player
        }
    }

    // Fungsi untuk mengaktifkan atau menonaktifkan semua interaksi
    private void ToggleInteractions(bool state)
    {
        intText.SetActive(state);
        interactable = state;
    }
}
