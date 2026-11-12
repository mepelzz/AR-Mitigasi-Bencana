using UnityEngine;

// Menggerakkan objek dari posisi awal ke posisi awal + offset.
// Contoh pemakaian:
// - Air banjir naik        : offset (0, 0.15, 0), pingPong = false
// - Gelombang tsunami maju : offset (0, 0, -0.5), pingPong = false
// - Panah evakuasi         : offset (0.1, 0, 0),  pingPong = true
// - Tanah longsor turun    : offset (0, -0.2, -0.2), pingPong = false
public class MoveBetween : MonoBehaviour
{
    public Vector3 offset = new Vector3(0f, 0.1f, 0f);
    public float durasi = 3f;
    public bool pingPong = false;   // true = bolak-balik, false = ulang dari awal
    public float jeda = 0.5f;       // jeda sebelum mengulang (saat pingPong = false)

    Vector3 awal;
    float waktu;

    void OnEnable() { awal = transform.localPosition; waktu = 0f; }
    void OnDisable() { transform.localPosition = awal; }

    void Update()
    {
        waktu += Time.deltaTime;
        float t;

        if (pingPong)
        {
            t = Mathf.PingPong(waktu / durasi, 1f);
        }
        else
        {
            float siklus = durasi + jeda;
            t = Mathf.Clamp01((waktu % siklus) / durasi);
        }

        t = Mathf.SmoothStep(0f, 1f, t);
        transform.localPosition = awal + offset * t;
    }
}
