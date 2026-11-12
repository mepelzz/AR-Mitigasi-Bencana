using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Pasang di satu objek yang SELALU aktif (misalnya Canvas_UI).
// Satu panel dipakai bersama oleh semua marker.
public class InfoPanelController : MonoBehaviour
{
    public static InfoPanelController Instance;

    [Header("Panel info")]
    public GameObject panelInfo;
    public TMP_Text teksJudul;
    public TMP_Text teksPengertian;
    public TMP_Text teksIsi;

    [Header("Tombol tab (urutan: tab 1, 2, 3, 4)")]
    public Button[] tombolTab;

    [Header("Warna tab")]
    public Color warnaTabAktif = new Color(0f, 0.659f, 0.588f, 1f);       // Teal #00A896
    public Color warnaTabNonaktif = new Color(1f, 1f, 1f, 0.15f);         // putih transparan

    [Header("Audio dan kuis")]
    public AudioSource audioSource;
    public QuizManager quizManager;

    enum Tab { Satu, Dua, Tiga, Empat }

    MitigasiTarget aktif;

    void Awake()
    {
        Instance = this;
        if (panelInfo != null) panelInfo.SetActive(false);
    }

    // Dipanggil oleh MitigasiTarget
    public void TargetTerdeteksi(MitigasiTarget t)
    {
        aktif = t;
        if (panelInfo != null) panelInfo.SetActive(true);
        if (teksJudul != null) teksJudul.text = t.judul;
        if (teksPengertian != null) teksPengertian.text = t.pengertian;

        TerapkanLabelTab(t);
        TampilkanTab(Tab.Satu);

        if (audioSource != null && t.narasi != null)
        {
            audioSource.Stop();
            audioSource.clip = t.narasi;
            audioSource.Play();
        }
    }

    public void TargetHilang(MitigasiTarget t)
    {
        if (aktif != t) return;
        aktif = null;
        if (panelInfo != null) panelInfo.SetActive(false);
        if (audioSource != null) audioSource.Stop();
    }

    // Ubah teks tombol tab sesuai marker. Label kosong = tab disembunyikan.
    void TerapkanLabelTab(MitigasiTarget t)
    {
        if (tombolTab == null) return;
        string[] label = { t.labelTab1, t.labelTab2, t.labelTab3, t.labelTab4 };

        for (int i = 0; i < tombolTab.Length && i < label.Length; i++)
        {
            if (tombolTab[i] == null) continue;
            bool ada = !string.IsNullOrWhiteSpace(label[i]);
            tombolTab[i].gameObject.SetActive(ada);
            if (!ada) continue;

            TMP_Text teks = tombolTab[i].GetComponentInChildren<TMP_Text>();
            if (teks != null) teks.text = label[i];
        }
    }

    // === Dihubungkan ke tombol tab (OnClick) ===
    // Nama fungsi dipertahankan agar OnClick yang sudah dibuat tetap jalan.
    public void TabSebelum() { TampilkanTab(Tab.Satu); }
    public void TabSaat() { TampilkanTab(Tab.Dua); }
    public void TabSesudah() { TampilkanTab(Tab.Tiga); }
    public void TabFakta() { TampilkanTab(Tab.Empat); }

    // Tab yang sedang dibuka berwarna aktif, lainnya nonaktif
    void WarnaiTab(int indeksAktif)
    {
        if (tombolTab == null) return;
        for (int i = 0; i < tombolTab.Length; i++)
        {
            if (tombolTab[i] == null) continue;
            Image gambar = tombolTab[i].targetGraphic as Image;
            if (gambar != null)
                gambar.color = (i == indeksAktif) ? warnaTabAktif : warnaTabNonaktif;
        }
    }

    void TampilkanTab(Tab tab)
    {
        WarnaiTab((int)tab);
        if (aktif == null || teksIsi == null) return;

        switch (tab)
        {
            case Tab.Satu: teksIsi.text = JadikanPoin(aktif.sebelum); break;
            case Tab.Dua: teksIsi.text = JadikanPoin(aktif.saat); break;
            case Tab.Tiga: teksIsi.text = JadikanPoin(aktif.sesudah); break;
            case Tab.Empat:
                teksIsi.text = aktif.faktaLokal +
                    (string.IsNullOrEmpty(aktif.sumber) ? "" : "\n\nSumber: " + aktif.sumber);
                break;
        }
    }

    // Ubah tiap baris menjadi poin "• ..."
    string JadikanPoin(string teks)
    {
        if (string.IsNullOrWhiteSpace(teks)) return "";
        string[] baris = teks.Split('\n');
        string hasil = "";
        foreach (string b in baris)
        {
            string bersih = b.Trim();
            if (bersih.Length == 0) continue;
            hasil += "• " + bersih + "\n";
        }
        return hasil.TrimEnd();
    }

    // === Tombol audio (putar/jeda) ===
    public void ToggleAudio()
    {
        if (audioSource == null || audioSource.clip == null) return;
        if (audioSource.isPlaying) audioSource.Pause();
        else if (audioSource.time > 0f) audioSource.UnPause();
        else audioSource.Play();
    }

    // === Tombol kuis ===
    public void BukaKuis()
    {
        if (aktif == null || quizManager == null) return;
        quizManager.Mulai(aktif.judul, aktif.soalKuis);
    }
}
