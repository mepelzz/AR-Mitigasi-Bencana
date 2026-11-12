using UnityEngine;
using UnityEngine.SceneManagement;

// Untuk tombol di menu utama dan layar AR.
public class MenuController : MonoBehaviour
{
    public string namaSceneAR = "SceneAR";
    public string namaSceneMenu = "MainMenu";

    public void Mulai() { SceneManager.LoadScene(namaSceneAR); }
    public void KembaliKeMenu() { SceneManager.LoadScene(namaSceneMenu); }
    public void Keluar() { Application.Quit(); }
}
