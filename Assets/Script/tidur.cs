using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;  // Pastikan untuk menambahkan namespace ini untuk PlayableDirector

public class tidur : MonoBehaviour
{
    public Dialogpintu dialogPintu;
    public GameObject intText;
    public bool interactable;
    public Collider laci, laci2, meja, lemari1, lemari2, laci4;
    public PlayableDirector cutsceneDirector; // Referensi ke PlayableDirector
    public Camera mainCamera; // Referensi ke kamera utama
    public Camera cutsceneCamera; // Referensi ke kamera cutscene (jika ada)
    public SC_FPSController SC_FPSController; // Referensi ke skrip PlayerController (jika ada)
    public float cutsceneDuration = 5f; // Durasi cutscene sebelum bisa dilewati

    private bool cutscenePlaying = false;
    private Vector3 originalCameraPosition;
    private Quaternion originalCameraRotation;
    private bool cutsceneSkipped = false;

    void Start()
    {
        // Simpan posisi dan rotasi kamera asli
        originalCameraPosition = mainCamera.transform.position;
        originalCameraRotation = mainCamera.transform.rotation;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("MainCamera") && !cutscenePlaying) // Cek apakah cutscene sedang diputar
        {
            intText.SetActive(true);
            interactable = true;
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

    void Update()
    {
        if (interactable && !cutscenePlaying)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                StartCoroutine(PlayCutscene());
            }
        }

        if (cutscenePlaying && !cutsceneSkipped && Input.GetKeyDown(KeyCode.Space))
        {
            SkipCutscene();
        }
    }

    IEnumerator PlayCutscene()
    {
        cutscenePlaying = true;
        cutsceneSkipped = false;

        // Nonaktifkan pergerakan pemain dan ubah kamera
        if (SC_FPSController != null)
            SC_FPSController.enabled = false;

        if (cutsceneCamera != null)
        {
            mainCamera.gameObject.SetActive(false);
            cutsceneCamera.gameObject.SetActive(true);
        }

        // Nonaktifkan teks interaksi saat cutscene diputar
        intText.SetActive(false);

        // Mulai cutscene
        cutsceneDirector.Play();

        // Tunggu durasi cutscene sebelum bisa dilewati
        yield return new WaitForSeconds(cutsceneDuration);

        // Jika cutscene tidak dilewati, langsung kembali ke kondisi normal
        if (!cutsceneSkipped)
        {
            EndCutscene();
        }
    }

    void SkipCutscene()
    {
        cutsceneSkipped = true;
        cutsceneDirector.Stop(); // Hentikan cutscene
        EndCutscene();
    }

    void EndCutscene()
    {
        cutscenePlaying = false;

        // Kembalikan kamera ke posisi semula
        if (cutsceneCamera != null)
        {
            cutsceneCamera.gameObject.SetActive(false);
            mainCamera.gameObject.SetActive(true);
            mainCamera.transform.position = originalCameraPosition;
            mainCamera.transform.rotation = originalCameraRotation;
        }

        // Aktifkan kembali pergerakan pemain
        if (SC_FPSController != null)
            SC_FPSController.enabled = true;

        // Aktifkan kembali pintu
        laci.enabled = true;
        laci2.enabled = true;
        meja.enabled = true;
        lemari1.enabled = true;
        lemari2.enabled = true;
        laci4.enabled = true;

        // Set dialogPintu dan barang yang diatur sebelumnya
        dialogPintu.hasTuru = true;

        // Aktifkan teks interaksi setelah cutscene selesai
        intText.SetActive(true);
    }
}
