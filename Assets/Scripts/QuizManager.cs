using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class SoalKuis
{
    [TextArea(2, 4)] public string pertanyaan;
    public string[] pilihan = new string[4];
    [Range(0, 3)] public int jawabanBenar;
    [TextArea(1, 3)] public string penjelasan;
}

// Pasang di objek yang SELALU aktif (misalnya Canvas), BUKAN di panelKuis
// itu sendiri, karena objek nonaktif tidak menjalankan Awake.
public class QuizManager : MonoBehaviour
{
    [Header("UI Kuis")]
    public GameObject panelKuis;
    public TMP_Text teksJudul;
    public TMP_Text teksNomor;
    public TMP_Text teksPertanyaan;
    public TMP_Text teksUmpanBalik;
    public Button[] tombolPilihan;   // 4 tombol, tiap tombol punya child TMP_Text
    public Button tombolLanjut;
    public TMP_Text teksTombolLanjut;

    [Header("Warna umpan balik")]
    public Color warnaNormal = Color.white;
    public Color warnaBenar = new Color(0.55f, 0.85f, 0.55f);
    public Color warnaSalah = new Color(0.95f, 0.55f, 0.55f);

    SoalKuis[] daftar;
    int index, skor;
    bool sudahMenjawab, selesai;

    void Awake()
    {
        for (int i = 0; i < tombolPilihan.Length; i++)
        {
            int idx = i; // salin agar tidak tertukar di closure
            tombolPilihan[i].onClick.AddListener(() => Pilih(idx));
        }
        if (tombolLanjut != null) tombolLanjut.onClick.AddListener(Lanjut);
        if (panelKuis != null) panelKuis.SetActive(false);
    }

    public void Mulai(string judul, SoalKuis[] soal)
    {
        if (soal == null || soal.Length == 0) return;
        daftar = soal;
        index = 0;
        skor = 0;
        selesai = false;
        if (teksJudul != null) teksJudul.text = "Kuis: " + judul;
        panelKuis.SetActive(true);
        TampilkanSoal();
    }

    void TampilkanSoal()
    {
        sudahMenjawab = false;
        SoalKuis s = daftar[index];

        if (teksNomor != null) teksNomor.text = "Soal " + (index + 1) + " dari " + daftar.Length;
        teksPertanyaan.text = s.pertanyaan;
        if (teksUmpanBalik != null) teksUmpanBalik.text = "";
        tombolLanjut.gameObject.SetActive(false);

        for (int i = 0; i < tombolPilihan.Length; i++)
        {
            bool ada = i < s.pilihan.Length && !string.IsNullOrEmpty(s.pilihan[i]);
            tombolPilihan[i].gameObject.SetActive(ada);
            if (!ada) continue;
            tombolPilihan[i].interactable = true;
            tombolPilihan[i].image.color = warnaNormal;
            tombolPilihan[i].GetComponentInChildren<TMP_Text>().text = s.pilihan[i];
        }
    }

    void Pilih(int i)
    {
        if (sudahMenjawab || selesai) return;
        sudahMenjawab = true;

        SoalKuis s = daftar[index];
        bool benar = i == s.jawabanBenar;
        if (benar) skor++;

        // Tandai jawaban benar (hijau) dan pilihan salah pengguna (merah)
        tombolPilihan[s.jawabanBenar].image.color = warnaBenar;
        if (!benar) tombolPilihan[i].image.color = warnaSalah;
        foreach (Button b in tombolPilihan) b.interactable = false;

        if (teksUmpanBalik != null)
            teksUmpanBalik.text = (benar ? "Benar! " : "Kurang tepat. ") + s.penjelasan;

        tombolLanjut.gameObject.SetActive(true);
        if (teksTombolLanjut != null)
            teksTombolLanjut.text = (index == daftar.Length - 1) ? "Lihat hasil" : "Lanjut";
    }

    void Lanjut()
    {
        if (selesai) { Tutup(); return; }

        index++;
        if (index >= daftar.Length) TampilkanHasil();
        else TampilkanSoal();
    }

    void TampilkanHasil()
    {
        selesai = true;
        foreach (Button b in tombolPilihan) b.gameObject.SetActive(false);
        if (teksNomor != null) teksNomor.text = "";
        teksPertanyaan.text = "Selesai! Skor kamu " + skor + " dari " + daftar.Length + ".";
        if (teksUmpanBalik != null)
            teksUmpanBalik.text = skor == daftar.Length
                ? "Luar biasa, kamu siap siaga!"
                : "Baca lagi panel info untuk memperkuat pemahamanmu.";
        tombolLanjut.gameObject.SetActive(true);
        if (teksTombolLanjut != null) teksTombolLanjut.text = "Tutup";
    }

    public void Tutup()
    {
        panelKuis.SetActive(false);
    }
}
