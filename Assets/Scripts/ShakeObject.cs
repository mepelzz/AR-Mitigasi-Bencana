using UnityEngine;

// Untuk rumah gempa: bergetar kiri-kanan. Pasang di objek rumah (bukan alas).
public class ShakeObject : MonoBehaviour
{
    public float amplitude = 0.01f; // jarak getar (satuan Unity)
    public float speed = 40f;

    Vector3 posisiAwal;

    void OnEnable() { posisiAwal = transform.localPosition; }
    void OnDisable() { transform.localPosition = posisiAwal; }

    void Update()
    {
        float x = Mathf.Sin(Time.time * speed) * amplitude;
        float z = Mathf.Cos(Time.time * speed * 0.8f) * amplitude * 0.5f;
        transform.localPosition = posisiAwal + new Vector3(x, 0f, z);
    }
}
