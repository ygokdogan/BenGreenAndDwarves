using UnityEngine;
using UnityEngine.SceneManagement; // Sahne işlemleri için gerekli kütüphane

public class SceneController : MonoBehaviour
{
    // Butona tıklandığında Inspector'dan gireceğimiz sahne ismini yükler
    public void LoadTargetScene(int sceneNumber)
    {
        SceneManager.LoadScene(2);
    }
}