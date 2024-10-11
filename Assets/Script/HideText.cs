using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection; // Perlu untuk menggunakan Reflection

public class HideText : MonoBehaviour
{
    public GameObject intText2; // Ubah dari intText menjadi intText2
    public bool interactable;
    public MonoBehaviour door;  // Script apa saja
    public MonoBehaviour door2; // Script apa saja

    private bool wasInteractableBefore = false; // Untuk menyimpan status sebelum collider

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            // Simpan status "interactable" sebelum disembunyikan
            wasInteractableBefore = interactable;

            // Nonaktifkan teks interaksi dan skrip pintu (script apapun yang digunakan oleh door dan door2)
            intText2.SetActive(false);  // Ubah dari intText menjadi intText2
            door.enabled = false;
            door2.enabled = false;
            interactable = false;

            // Ubah interaksi jika script door memiliki variabel interactable
            SetInteractable(door, false);
            SetInteractable(door2, false);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            // Kembalikan status pintu dan interaksi
            door.enabled = true;
            door2.enabled = true;
            interactable = wasInteractableBefore;

            // Kembalikan interaksi jika script door memiliki variabel interactable
            SetInteractable(door, wasInteractableBefore);
            SetInteractable(door2, wasInteractableBefore);

            // Kembalikan teks interaksi hanya jika sebelumnya bisa diinteraksi
            if (wasInteractableBefore)
            {
                intText2.SetActive(true);  // Ubah dari intText menjadi intText2
            }
        }
    }

    // Method untuk mengatur interactable menggunakan Reflection
    void SetInteractable(MonoBehaviour script, bool value)
    {
        // Gunakan reflection untuk memeriksa apakah script memiliki field atau properti "interactable"
        FieldInfo interactableField = script.GetType().GetField("interactable");

        if (interactableField != null && interactableField.FieldType == typeof(bool))
        {
            // Jika ditemukan, atur nilai interactable sesuai parameter
            interactableField.SetValue(script, value);
        }
    }
}
