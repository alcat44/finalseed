using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpController : MonoBehaviour
{
    public GameObject intText;
    public Rigidbody rb;
    public BoxCollider coll;
    public Transform player, gunContainer, fpsCam;

    public float dropForwardForce, dropUpwardForce;

    public bool equipped;
    public bool withinRange;

    // Static variable to ensure only one object can be picked up at a time
    public static bool isCarryingObject = false;

    private void Start()
    {
        intText.SetActive(false); // Pastikan intText dimulai dalam keadaan tidak aktif

        if (!equipped)
        {
            rb.isKinematic = false;
            coll.isTrigger = false;
        }
        else
        {
            rb.isKinematic = true;
            coll.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Hanya aktifkan intText jika tidak sedang membawa object lain dan item ini belum diambil
        if (other.CompareTag("MainCamera") && !equipped && !isCarryingObject)
        {
            intText.SetActive(true);
            withinRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Sembunyikan intText saat pemain keluar dari collider
        if (other.CompareTag("MainCamera"))
        {
            intText.SetActive(false);
            withinRange = false;
        }
    }

    private void Update()
    {
        // Hanya ambil object jika pemain berada dalam range, tidak membawa object lain, dan menekan tombol E
        if (withinRange && Input.GetKeyDown(KeyCode.E) && !isCarryingObject)
        {
            PickUp();
        }

        // Jika pemain sudah mengambil item, tekan tombol F untuk drop
        if (equipped && Input.GetKeyDown(KeyCode.F))
        {
            Drop();
        }

        // Jika object yang dipegang sudah tidak ada (misalnya dihancurkan), reset isCarryingObject
        if (equipped && gameObject == null)
        {
            isCarryingObject = false;
        }
    }

    private void OnDestroy()
    {
        // Pastikan saat objek dihancurkan, isCarryingObject di-reset
        if (equipped)
        {
            isCarryingObject = false;
        }
    }

    private void PickUp()
    {
        equipped = true;
        isCarryingObject = true; // Tandai bahwa pemain sedang membawa objek

        // Menempatkan item di container
        transform.SetParent(gunContainer);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        transform.localScale = Vector3.one;

        // Nonaktifkan physics dan trigger collider
        rb.isKinematic = true;
        coll.isTrigger = true;

        intText.SetActive(false); // Sembunyikan intText setelah di-pickup
    }

    private void Drop()
    {
        equipped = false;
        isCarryingObject = false; // Bebaskan pemain untuk mengambil objek lain

        // Lepaskan item dari pemain
        transform.SetParent(null);

        // Aktifkan kembali physics
        rb.isKinematic = false;
        coll.isTrigger = false;

        // Berikan velocity yang sama dengan player
        rb.velocity = player.GetComponent<Rigidbody>().velocity;

        // Tambahkan gaya dorong ke arah depan dan atas saat drop
        rb.AddForce(fpsCam.forward * dropForwardForce, ForceMode.Impulse);
        rb.AddForce(fpsCam.up * dropUpwardForce, ForceMode.Impulse);

        // Tambahkan putaran acak
        float random = Random.Range(-1f, 1f);
        rb.AddTorque(new Vector3(random, random, random) * 10);

        // Pastikan object dihapus dengan benar, jika memang ada sistem penghancuran
        // Destroy(gameObject); // Jika diperlukan untuk menghancurkan objek
    }

}