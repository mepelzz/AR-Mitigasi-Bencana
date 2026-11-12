using UnityEngine;

// Untuk api yang membesar-mengecil, atau gelombang suara dari megafon.
public class PulseScale : MonoBehaviour
{
    public float minScale = 0.9f;
    public float maxScale = 1.15f;
    public float speed = 4f;
    public float offsetFase = 0f; // beda tiap objek agar tidak serempak

    Vector3 skalaAwal;

    void OnEnable() { skalaAwal = transform.localScale; }
    void OnDisable() { transform.localScale = skalaAwal; }

    void Update()
    {
        float t = (Mathf.Sin(Time.time * speed + offsetFase) + 1f) * 0.5f;
        float f = Mathf.Lerp(minScale, maxScale, t);
        transform.localScale = skalaAwal * f;
    }
}
