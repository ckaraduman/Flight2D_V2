using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro; // Game Over yazısı için eklendi

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Oyun Ayarları")]
    public int lives = 3;
    public GameObject playerPrefab; // Player prefab'ı (Inspector'da ata)
    private GameObject playerInstance;
    private Vector3 startPosition;

    [Header("UI Referansları")]
    public TextMeshProUGUI gameOverText; // Game Over yazısı
    public TextMeshProUGUI copyrightText;
    public GameObject restartButton;
    public GameObject quitButton;
    public GameObject gameOverPanel;
    private bool isGameOver = false;

    private void Awake()
    {
        // Tekil instance (singleton)
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        // Başlangıçta sahnedeki Player'ı bul
        playerInstance = GameObject.FindWithTag("Player");
        if (playerInstance != null)
            startPosition = playerInstance.transform.position;

        // Başta Game Over yazısı gizli olsun
        if (gameOverText != null)
            gameOverText.gameObject.SetActive(false);
    }

    public void PlayerCrashed()
    {
        if (isGameOver) return; // oyun bitmişse işlem yapma

        lives--;

        if (lives <= 0)
        {
            Debug.Log("Game Over!");
            StartCoroutine(GameOverRoutine());
            return;
        }

        Debug.Log($"Crashed! Lives left: {lives}");
        RespawnPlayer();
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        // 1. Zamanı normale döndür (Zaman 0'da kalmasın)
        Time.timeScale = 1f;

        // 2. Platform Kontrolü

        // A. UNITY EDITOR İÇİN (Sadece Test)
#if UNITY_EDITOR
        // Sadece Editör içindeyken oyunu durdurur
        UnityEditor.EditorApplication.isPlaying = false;

        // B. WEBGL İÇİN (itch.io)
#elif UNITY_WEBGL
    // WebGL'de çıkış yapmak yerine, genellikle bir mesaj gösterilir
    // veya ana menüye dönülür, çünkü sekme kapatılamaz.
    // Şimdilik, konsola yazıp fonksiyondan çıkıyoruz.
    Debug.Log("WebGL'de Application.Quit() desteklenmez. Çıkış işlevi atlandı.");
    return; // Fonksiyonu burada sonlandır.
    
    // C. DİĞER PLATFORMLAR İÇİN (PC/Mac/Android/iOS)
#else
    // Windows, Mac, Android ve iOS için uygulamayı kapatır
    Application.Quit();
#endif
    }

    public IEnumerator RespawnPlayerDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        PlayerCrashed();
    }

    private void RespawnPlayer()
    {
        // Eski player sahnede varsa sil
        if (playerInstance != null)
            Destroy(playerInstance);

        // Yeni player oluştur (Prefab’tan)
        playerInstance = Instantiate(playerPrefab, startPosition, Quaternion.identity);

        // --- KAMERA AYARI: CameraFollow2D'ye yeni target ver ve kamerayı snap et ---
        CameraFollow2D camFollow = Camera.main.GetComponent<CameraFollow2D>();
        if (camFollow != null)
        {
            camFollow.target = playerInstance.transform;

            // Eğer CameraFollow2D'de bir Snap metodu yoksa, anlık taşı:
            Vector3 snapPos = playerInstance.transform.position + camFollow.offset;
            snapPos.z = Camera.main.transform.position.z; // kamera Z'sini koru
            Camera.main.transform.position = snapPos;
        }

        Debug.Log("Yeni uçak sahneye geldi ve kamera güncellendi.");
    }

    // --- 🎮 GAME OVER RUTİNİ ---
    private IEnumerator GameOverRoutine()
    {
        isGameOver = true;

        // Kamerayı merkeze taşı (örneğin sahnenin 0,0 noktasına)
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            mainCam.transform.position = new Vector3(0f, 0f, mainCam.transform.position.z);
        }

        // Oyun dursun
        Time.timeScale = 0f;

        // Paneli aktif et (kararma efekti)
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        // Game Over yazısını göster
        if (gameOverText != null)
            gameOverText.gameObject.SetActive(true);

        // Copyright yazısını göster
        if (copyrightText != null)
            copyrightText.gameObject.SetActive(true);

        // Butonları göster
        if (restartButton != null)
            restartButton.SetActive(true);

        if (quitButton != null)
            quitButton.SetActive(true);

        // Artık Input beklemiyoruz, butonlar bekleyecek.
        yield break; // Coroutine’i burada sonlandır
    }
}

