using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Letakkan di Assets/Editor/KontenImporter.cs
// Membaca Assets/Data/konten_marker.json lalu mengisi semua MitigasiTarget
// di scene yang sedang terbuka. Pencocokan lewat bagian terakhir nama objek:
//   ImageTarget_01_Gempa -> "gempa",  ImageTarget_08_TasSiaga -> "tas", dst.
public static class KontenImporter
{
    const string PathJson = "Assets/Data/konten_marker.json";

    [Serializable]
    class SoalData
    {
        public string pertanyaan;
        public string[] pilihan;
        public int benar;
        public string penjelasan;
    }

    [Serializable]
    class MarkerData
    {
        public string kunci;
        public string judul, pengertian, sebelum, saat, sesudah, fakta, sumber;
        public string label1, label2, label3, label4;
        public SoalData[] soal;
    }

    [Serializable]
    class KontenData { public MarkerData[] marker; }

    [MenuItem("Tools/Isi Konten Marker dari JSON")]
    public static void Isi()
    {
        if (!File.Exists(PathJson))
        {
            EditorUtility.DisplayDialog("File tidak ditemukan",
                "Letakkan konten_marker.json di " + PathJson, "OK");
            return;
        }

        KontenData data = JsonUtility.FromJson<KontenData>(File.ReadAllText(PathJson));
        MitigasiTarget[] targets = UnityEngine.Object.FindObjectsOfType<MitigasiTarget>(true);

        int terisi = 0;
        string dilewati = "";

        foreach (MitigasiTarget t in targets)
        {
            MarkerData m = Cari(data, t.gameObject.name);
            if (m == null) { dilewati += "- " + t.gameObject.name + "\n"; continue; }

            Undo.RecordObject(t, "Isi konten marker");
            t.judul = m.judul;
            t.pengertian = m.pengertian;
            t.sebelum = m.sebelum;
            t.saat = m.saat;
            t.sesudah = m.sesudah;
            t.faktaLokal = m.fakta;
            t.sumber = m.sumber;
            t.labelTab1 = m.label1;
            t.labelTab2 = m.label2;
            t.labelTab3 = m.label3;
            t.labelTab4 = m.label4;

            int n = m.soal == null ? 0 : m.soal.Length;
            t.soalKuis = new SoalKuis[n];
            for (int i = 0; i < n; i++)
            {
                t.soalKuis[i] = new SoalKuis
                {
                    pertanyaan = m.soal[i].pertanyaan,
                    pilihan = (string[])m.soal[i].pilihan.Clone(),
                    jawabanBenar = m.soal[i].benar,
                    penjelasan = m.soal[i].penjelasan
                };
            }

            EditorUtility.SetDirty(t);
            terisi++;
        }

        EditorSceneManager.MarkAllScenesDirty();

        string pesan = terisi + " marker berhasil diisi.";
        if (dilewati.Length > 0)
            pesan += "\n\nDilewati (nama tidak cocok):\n" + dilewati;
        EditorUtility.DisplayDialog("Impor konten", pesan, "OK");
        Debug.Log(pesan);
    }

    static MarkerData Cari(KontenData data, string namaObjek)
    {
        string nama = namaObjek.ToLower();
        string token = nama.Substring(nama.LastIndexOf('_') + 1);
        if (token.Length < 3) return null;

        foreach (MarkerData m in data.marker)
        {
            if (token.StartsWith(m.kunci) || m.kunci.StartsWith(token))
                return m;
        }
        return null;
    }
}
