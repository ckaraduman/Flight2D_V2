using UnityEngine;
using UnityEngine.EventSystems;

public class TimeScaleIndependentButton : MonoBehaviour, IPointerClickHandler
{
    // Inspector'da atanacak aksiyonlar (fonksiyonlar) için UnityEvent kullanabiliriz.
    // Ancak Game Manager'ý direkt çaðýrmak daha basittir.

    public void OnPointerClick(PointerEventData eventData)
    {
        // Zaman 0 olsa bile bu metot çalýþýr.

        // Hangi butona týklandýðýný ismine göre kontrol ediyoruz:
        if (gameObject.name.Contains("Restart"))
        {
            // Zamaný tekrar aktif edip oyunu yeniden baþlat
            Time.timeScale = 1f;
            GameManager.Instance.RestartGame();
        }
        else if (gameObject.name.Contains("Quit"))
        {
            // Zamaný tekrar aktif edip oyunu kapat
            Time.timeScale = 1f;
            GameManager.Instance.QuitGame();
        }
    }
}