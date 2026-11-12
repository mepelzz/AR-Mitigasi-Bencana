using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Letakkan di Assets/Editor/ApplyUITheme.cs
// Tema UI "Gelap konsisten" berbasis Design System Orang 3 (palet + Roboto).
// Jalankan di SceneAR dan juga di MainMenu. Menu: Tools > Terapkan Tema UI
public static class ApplyUITheme
{
    // Palet dari dokumen Design System
    static readonly Color Navy    = Hex("0B2D5B");
    static readonly Color Teal    = Hex("00A896");
    static readonly Color Blue    = Hex("3B82F6");
    static readonly Color Putih   = Hex("F8FAFC");
    static readonly Color AbuMuda = Hex("E2E8F0");

    // Turunan lebih terang agar teks Navy tetap terbaca pada jawaban kuis
    static readonly Color BenarTint = Hex("7DD3C8");
    static readonly Color SalahTint = Hex("FCA5A5");

    // Tab tidak aktif: putih transparan di atas panel Navy (tampak Navy lebih terang)
    static readonly Color TabNonaktif = new Color(1f, 1f, 1f, 0.15f);

    const string PathSprite = "Assets/UI/Sprites/rounded.png";
    const string PathOutline = "Assets/UI/Sprites/rounded_outline.png";

    [MenuItem("Tools/Terapkan Tema UI")]
    public static void Terapkan()
    {
        Sprite bulat = SiapkanSprite(PathSprite);
        Sprite garis = SiapkanSprite(PathOutline);
        TMP_FontAsset font = CariFont();

        int jmlTombol = 0, jmlTeks = 0, jmlPanel = 0;

        foreach (Canvas c in Object.FindObjectsOfType<Canvas>(true))
        {
            // 1) Panel dan tombol
            foreach (Transform t in c.GetComponentsInChildren<Transform>(true))
            {
                Image img = t.GetComponent<Image>();
                Button btn = t.GetComponent<Button>();
                string nama = t.name;

                if (btn != null)
                {
                    if (GayaTombol(btn, nama, bulat, garis)) jmlTombol++;
                }
                else if (img != null && nama == "PanelInfo")
                {
                    // Navy semi-transparan: kontras terhadap kamera terang maupun gelap
                    AturGambar(img, new Color(Navy.r, Navy.g, Navy.b, 0.93f), bulat, false);
                    jmlPanel++;
                }
                else if (img != null && (nama == "PanelKuis" || nama == "PanelMenu"))
                {
                    AturGambar(img, Navy, null, false);
                    jmlPanel++;
                }
            }

            // 2) Teks
            foreach (TMP_Text tx in c.GetComponentsInChildren<TMP_Text>(true))
            {
                Undo.RecordObject(tx, "Tema UI");
                if (font != null) tx.font = font;

                if (tx.GetComponentInParent<Button>(true) == null)
                {
                    AturTeks(tx);
                    jmlTeks++;
                }
                EditorUtility.SetDirty(tx);
            }
        }

        // 3) Warna umpan balik kuis
        QuizManager qm = Object.FindObjectOfType<QuizManager>(true);
        if (qm != null)
        {
            Undo.RecordObject(qm, "Tema UI");
            qm.warnaNormal = Putih;
            qm.warnaBenar = BenarTint;
            qm.warnaSalah = SalahTint;
            EditorUtility.SetDirty(qm);
        }

        EditorSceneManager.MarkAllScenesDirty();

        string pesan = "Tema gelap diterapkan.\n\nPanel: " + jmlPanel + "\nTombol: " + jmlTombol +
                       "\nTeks: " + jmlTeks +
                       "\n\nFont Roboto: " + (font != null ? font.name : "TIDAK DITEMUKAN (font lama dipakai)") +
                       "\nSprite rounded: " + (bulat != null ? "OK" : "TIDAK DITEMUKAN") +
                       "\nSprite outline: " + (garis != null ? "OK" : "TIDAK DITEMUKAN");
        EditorUtility.DisplayDialog("Tema UI", pesan, "OK");
        Debug.Log(pesan);
    }

    // ---------- tombol ----------
    static bool GayaTombol(Button btn, string nama, Sprite bulat, Sprite garis)
    {
        Color bg, fg;
        int ukuran = 0;      // 0 = biarkan ukuran font yang ada
        bool outline = false;

        if (nama.StartsWith("Tab")) { bg = TabNonaktif; fg = Putih; ukuran = 28; }
        else if (nama == "TombolAudio") { bg = Putih; fg = Putih; ukuran = 30; outline = true; }
        else if (nama == "TombolKuis") { bg = Teal; fg = Putih; ukuran = 30; }
        else if (nama == "TombolKembali") { bg = Navy; fg = Putih; ukuran = 28; }
        else if (nama == "TombolLanjut") { bg = Teal; fg = Putih; ukuran = 34; }
        else if (nama == "TombolTutup") { bg = Putih; fg = Putih; ukuran = 30; outline = true; }
        else if (nama.StartsWith("Pilihan")) { bg = Putih; fg = Navy; ukuran = 32; }
        else if (nama == "TombolMulai") { bg = Teal; fg = Putih; }
        else if (nama == "TombolKeluar") { bg = Putih; fg = Putih; outline = true; }
        else return false;

        Image img = btn.targetGraphic as Image;
        if (img == null) img = btn.GetComponent<Image>();
        if (img != null)
        {
            if (outline && garis != null) AturGambar(img, bg, garis, true);
            else AturGambar(img, bg, bulat, false);
        }

        Undo.RecordObject(btn, "Tema UI");
        ColorBlock cb = btn.colors;
        cb.normalColor = Color.white;
        cb.highlightedColor = new Color(0.93f, 0.93f, 0.93f, 1f);
        cb.pressedColor = new Color(0.80f, 0.80f, 0.80f, 1f);
        cb.selectedColor = Color.white;
        cb.disabledColor = Color.white; // tetap solid setelah dijawab
        cb.colorMultiplier = 1f;
        btn.colors = cb;
        EditorUtility.SetDirty(btn);

        TMP_Text label = btn.GetComponentInChildren<TMP_Text>(true);
        if (label != null)
        {
            Undo.RecordObject(label, "Tema UI");
            label.color = fg;
            if (ukuran > 0) { label.enableAutoSizing = false; label.fontSize = ukuran; }
            EditorUtility.SetDirty(label);
        }
        return true;
    }

    static void AturGambar(Image img, Color warna, Sprite sprite, bool hanyaGaris)
    {
        Undo.RecordObject(img, "Tema UI");
        img.sprite = sprite;
        img.type = sprite != null ? Image.Type.Sliced : Image.Type.Simple;
        img.fillCenter = !hanyaGaris;   // false = hanya garis tepi (outline)
        img.color = warna;
        EditorUtility.SetDirty(img);
    }

    // ---------- teks (di luar tombol) ----------
    static void AturTeks(TMP_Text tx)
    {
        bool diPanelInfo = DiBawah(tx.transform, "PanelInfo");
        bool diPanelKuis = DiBawah(tx.transform, "PanelKuis");

        if (diPanelInfo || diPanelKuis)
        {
            switch (tx.name)
            {
                case "TeksJudul": SetUkuran(tx, 56, true); break;
                case "TeksJudulKuis": SetUkuran(tx, 52, true); break;
                case "TeksPengertian": SetUkuran(tx, 28, false); break;
                case "TeksNomor": SetUkuran(tx, 28, false); break;
                case "TeksPertanyaan": SetUkuran(tx, 38, false); break;
                case "TeksUmpanBalik": SetUkuran(tx, 30, false); break;
            }
        }

        // Putih di latar gelap, Navy di latar terang
        Image latar = tx.GetComponentInParent<Image>(true);
        if (latar != null)
            tx.color = Luminans(latar.color) < 0.5f ? Putih : Navy;
    }

    static void SetUkuran(TMP_Text tx, float ukuran, bool tebal)
    {
        tx.enableAutoSizing = false;
        tx.fontSize = ukuran;
        tx.fontStyle = tebal ? FontStyles.Bold : FontStyles.Normal;
    }

    // ---------- util ----------
    static bool DiBawah(Transform t, string namaInduk)
    {
        for (Transform p = t.parent; p != null; p = p.parent)
            if (p.name == namaInduk) return true;
        return false;
    }

    static float Luminans(Color c)
    {
        return 0.2126f * c.r + 0.7152f * c.g + 0.0722f * c.b;
    }

    static Color Hex(string h)
    {
        ColorUtility.TryParseHtmlString("#" + h, out Color c);
        return c;
    }

    static Sprite SiapkanSprite(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogWarning("Sprite tidak ditemukan: " + path);
            return null;
        }
        TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
        if (ti != null)
        {
            ti.textureType = TextureImporterType.Sprite;
            ti.spriteImportMode = SpriteImportMode.Single;
            ti.spriteBorder = new Vector4(48, 48, 48, 48);
            ti.alphaIsTransparency = true;
            ti.mipmapEnabled = false;
            ti.SaveAndReimport();
        }
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }

    static TMP_FontAsset CariFont()
    {
        string[] guid = AssetDatabase.FindAssets("Roboto t:TMP_FontAsset");
        TMP_FontAsset pilihan = null;
        foreach (string g in guid)
        {
            TMP_FontAsset f = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(AssetDatabase.GUIDToAssetPath(g));
            if (f == null) continue;
            if (pilihan == null) pilihan = f;
            if (f.name.ToLower().Contains("regular")) return f;
        }
        return pilihan;
    }
}
