using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class enemyAI1 : MonoBehaviour
{
    public NavMeshAgent ai;
    public List<Transform> destinations;
    public Animator aiAnim;
    public float walkSpeed, chaseSpeed, minIdleTime, maxIdleTime, idleTime, detectionDistance, catchDistance, searchDistance, minChaseTime, maxChaseTime, minSearchTime, maxSearchTime, jumpscareTime;
    public bool walking, chasing, searching;
    public Transform player;
    public Transform respawnPoint;
    public Vector3 respawnRotation;
    public Transform enemyRespawnPoint;
    Transform currentDest;
    Vector3 dest;
    public Vector3 rayCastOffset;
    public GameObject mainCamera;
    public GameObject jumpscareCamera;
    public GameObject Dialog;
    public GameObject exhaustionText;
    public GameObject intText;
    public GameObject hide;
    public GameObject stopHide;
    public float aiDistance;
    public bool isDead;
    public Image fadeImage;
    public float fadeDuration = 1f;

    // Audio source dan clip untuk walking dan running
    public AudioSource audioSource;
    public AudioClip walkClip;
    public AudioClip runClip;
    

    void Start()
    {
        walking = true;
        currentDest = destinations[Random.Range(0, destinations.Count)];
        ai.destination = currentDest.position;

        // Memastikan layar hitam di awal tidak terlihat (alpha = 0)
        SetImageAlpha(0);

        // Pastikan jumpscareCamera tidak aktif pada awalnya
        jumpscareCamera.SetActive(false);
        mainCamera.SetActive(true);  // Pastikan kamera utama aktif

        // Pastikan AudioSource tidak memutar suara apapun saat awal
        audioSource.Stop();
    }

    void Update()
    {
        if (isDead) return;

        Vector3 direction = (player.position - transform.position).normalized;
        RaycastHit hit;
        aiDistance = Vector3.Distance(player.position, this.transform.position);

        Debug.DrawRay(transform.position, direction * detectionDistance, Color.red);
        if (Physics.Raycast(transform.position, direction, out hit, detectionDistance))
        {
            if (hit.collider.gameObject.tag == "Player")
            {
                Debug.Log("Player detected!");
                walking = false;
                StopAllCoroutines();
                StartCoroutine(searchRoutine());
                searching = true;
            }
        }

        if (searching == true)
        {
            ai.speed = 0;
            StopAudio();  // Stop audio saat AI berhenti
            aiAnim.ResetTrigger("walk");
            aiAnim.ResetTrigger("idle");
            aiAnim.ResetTrigger("sprint");
            aiAnim.SetTrigger("search");

            if (aiDistance <= searchDistance)
            {
                StopAllCoroutines();
                StartCoroutine(chaseRoutine());
                chasing = true;
                searching = false;
            }
        }

        if (chasing == true)
        {
            dest = player.position;
            ai.destination = dest;
            ai.speed = chaseSpeed;
            PlayRunSound();  // Mainkan suara berlari
            aiAnim.ResetTrigger("walk");
            aiAnim.ResetTrigger("idle");
            aiAnim.ResetTrigger("search");
            aiAnim.SetTrigger("sprint");

            if (aiDistance <= catchDistance)
            {
                player.gameObject.SetActive(false);
                StopAllCoroutines();
                StartCoroutine(deathRoutine());
                chasing = false;
            }
        }

        if (walking == true)
        {
            dest = currentDest.position;
            ai.destination = dest;
            ai.speed = walkSpeed;
            PlayWalkSound();  // Mainkan suara berjalan
            aiAnim.ResetTrigger("sprint");
            aiAnim.ResetTrigger("idle");
            aiAnim.ResetTrigger("search");
            aiAnim.SetTrigger("walk");

            if (ai.remainingDistance <= ai.stoppingDistance)
            {
                aiAnim.ResetTrigger("walk");
                aiAnim.SetTrigger("idle");
                ai.speed = 0;
                StopAudio();  // Stop audio saat AI berhenti
                StartCoroutine(stayIdle());
                walking = false;
            }
        }
    }

    public void stopChase()
    {
        walking = true;
        chasing = false;
        StopAllCoroutines();
        currentDest = destinations[Random.Range(0, destinations.Count)];
        ai.destination = currentDest.position;
    }

    IEnumerator stayIdle()
    {
        idleTime = Random.Range(minIdleTime, maxIdleTime);
        yield return new WaitForSeconds(idleTime);
        walking = true;
        currentDest = destinations[Random.Range(0, destinations.Count)];
        ai.destination = currentDest.position;
    }

    IEnumerator searchRoutine()
    {
        yield return new WaitForSeconds(Random.Range(minSearchTime, maxSearchTime));
        searching = false;
        walking = true;
        currentDest = destinations[Random.Range(0, destinations.Count)];
        ai.destination = currentDest.position;
    }

    IEnumerator chaseRoutine()
    {
        yield return new WaitForSeconds(Random.Range(minChaseTime, maxChaseTime));
        stopChase();
    }

    IEnumerator deathRoutine()
    {
        exhaustionText.SetActive(false);
        intText.SetActive(false);
        hide.SetActive(false);
        isDead = true;

        // Nonaktifkan kamera utama dan aktifkan kamera jumpscare
        mainCamera.SetActive(false);
        jumpscareCamera.SetActive(true);
        stopHide.SetActive(false);

        // Pastikan animasi jumpscare diputar
        Debug.Log("Jumpscare triggered!");
        aiAnim.SetTrigger("jumpscare");

        // Tunggu durasi jumpscare
        yield return new WaitForSeconds(jumpscareTime);

        // Mulai fade-out (menuju hitam penuh)
        yield return StartCoroutine(FadeOut());

        // Respawn player dan musuh
        Debug.Log("Respawning player...");
        player.position = respawnPoint.position;
        transform.rotation = Quaternion.Euler(respawnRotation);
        player.gameObject.SetActive(true);

        Door[] doors = FindObjectsOfType<Door>(); // Find all instances of Door script
        foreach (Door door in doors)
        {
            door.ResetInteraction(); // Call reset on each door
        }

        DialogText[] allDialogs = FindObjectsOfType<DialogText>();
        foreach (DialogText dialog in allDialogs)
        {
            dialog.ResetDialog();
        }

        // Reset hiding setelah respawn
        hidingPlace[] hidingPlaces = FindObjectsOfType<hidingPlace>();
        foreach (hidingPlace hideSpot in hidingPlaces)
        {
            hideSpot.ResetHiding();
        }


        // Reset gerakan player
        SC_FPSController playerController = player.GetComponent<SC_FPSController>();
        if (playerController != null)
        {
            playerController.ResetPlayerMovement();  // Reset script gerakan player
        }

        Debug.Log("Respawning enemy...");
        transform.position = enemyRespawnPoint.position;

        

        // Nonaktifkan kamera jumpscare dan aktifkan kembali kamera utama
        jumpscareCamera.SetActive(false);
        mainCamera.SetActive(true);

        // Set parameter isRespawning agar musuh kembali ke state awal
        aiAnim.SetBool("isRespawning", true);

        // Jeda kecil untuk memastikan animator kembali ke state awal
        yield return new WaitForSeconds(0.2f);

        // Reset state respawning
        aiAnim.SetBool("isRespawning", false);

        // Reset state AI ke keadaan awal
        ResetAIState();

        // Mulai fade-in (layar hitam perlahan menghilang)
        yield return StartCoroutine(FadeIn());

        isDead = false;  // Izinkan Update() berjalan lagi
    }


    void ResetAIState()
    {
        Dialog.SetActive(true);
        walking = true;
        chasing = false;
        searching = false;

        aiAnim.ResetTrigger("jumpscare");
        aiAnim.ResetTrigger("sprint");
        aiAnim.ResetTrigger("search");
        aiAnim.ResetTrigger("walk");
        aiAnim.ResetTrigger("idle");

        aiAnim.CrossFade("Walk", 0.1f);

        currentDest = destinations[Random.Range(0, destinations.Count)];
        ai.destination = currentDest.position;
        ai.speed = walkSpeed;
    }

    IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }
        color.a = 0f;
        fadeImage.color = color;
    }

    IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }
        color.a = 1f;
        fadeImage.color = color;
    }

    void SetImageAlpha(float alpha)
    {
        Color color = fadeImage.color;
        color.a = alpha;
        fadeImage.color = color;
    }

    void PlayWalkSound()
    {
        if (audioSource.clip != walkClip || !audioSource.isPlaying)
        {
            audioSource.Stop(); // Pastikan audio sebelumnya berhenti
            audioSource.clip = walkClip;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    // Fungsi untuk memutar suara berlari
    void PlayRunSound()
    {
        if (audioSource.clip != runClip || !audioSource.isPlaying)
        {
            audioSource.Stop(); // Pastikan audio sebelumnya berhenti
            audioSource.clip = runClip;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    void StopAudio()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

}