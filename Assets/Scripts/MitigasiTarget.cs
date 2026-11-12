using UnityEngine;
using Vuforia;

// Pasang di SETIAP Image Target (satu per marker).
// Menyimpan data marker dan melapor ke InfoPanelController saat marker
// terdeteksi atau hilang.
[RequireComponent(typeof(ObserverBehaviour))]
public class MitigasiTarget : MonoBehaviour
{
    [Header("Identitas")]
    public string judul = "Gempa Bumi";
    [TextArea(2, 4)] public string pengertian;

    [Header("Label tab (kosongkan untuk menyembunyikan tab)")]
    public string labelTab1 = "Sebelum";
    public string labelTab2 = "Saat";
    public string labelTab3 = "Sesudah";
    public string labelTab4 = "Fakta";

    [Header("Isi tab 1 sampai 3 (satu poin per baris)")]
    [TextArea(4, 8)] public string sebelum;
    [TextArea(4, 8)] public string saat;
    [TextArea(4, 8)] public string sesudah;

    [Header("Isi tab 4 dan sumber")]
    [TextArea(2, 6)] public string faktaLokal;
    [TextArea(1, 3)] public string sumber;

    [Header("Audio narasi (rekaman sendiri)")]
    public AudioClip narasi;

    [Header("Kuis")]
    public SoalKuis[] soalKuis;

    ObserverBehaviour observer;

    void Awake()
    {
        observer = GetComponent<ObserverBehaviour>();
        observer.OnTargetStatusChanged += OnStatusChanged;
    }

    void OnDestroy()
    {
        if (observer != null)
            observer.OnTargetStatusChanged -= OnStatusChanged;
    }

    void OnStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        bool terdeteksi = status.Status == Status.TRACKED ||
                          status.Status == Status.EXTENDED_TRACKED;

        if (InfoPanelController.Instance == null) return;

        if (terdeteksi) InfoPanelController.Instance.TargetTerdeteksi(this);
        else InfoPanelController.Instance.TargetHilang(this);
    }
}
