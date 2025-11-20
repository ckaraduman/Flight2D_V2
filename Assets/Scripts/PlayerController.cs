using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    private float moveSpeed = 28f;       // sağa-sola hız
    private float forwardSpeed = 5f;     // yukarı sabit hız
    private float horizontalLimit = 23.5f; // sahne kenar sınırı

    [Header("Efekt Ayarları")]
    public GameObject explosionPrefab;
    public GameObject brokenPlanePrefab;

    [Header("Sprite Görselleri")]
    public Sprite defaultSprite;    // Düz uçuş
    public Sprite rollRightSprite;  // Sağa yatış
    public Sprite rollLeftSprite;   // Sola yatış

    private bool crashed = false;

    // === Dahili Bileşen ===
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        // SpriteRenderer bileşenini al
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Eğer Start'ta Sprite'ı set etmediyseniz, default'u buraya atayabilirsiniz.
        if (spriteRenderer != null && defaultSprite != null)
        {
            spriteRenderer.sprite = defaultSprite;
        }
    }

    void Update()
    {
        // 1. Yatay Girişi Hesapla (Hem Klavye Hem Dokunmatik)
        // Mobil desteği için dokunmatik giriş kontrolü
        float horizontalInput = 0f;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0); // İlk dokunuşu al

            // Dokunulan noktanın ekranın neresinde olduğunu kontrol et
            // Ekranın yarısını al
            float screenCenter = Screen.width / 2f;

            if (touch.position.x < screenCenter)
            {
                // Sol tarafa dokunuluyor
                horizontalInput = -1f; // Sola git
            }
            else if (touch.position.x > screenCenter)
            {
                // Sağ tarafa dokunuluyor
                horizontalInput = 1f; // Sağa git
            }
        }
        // Klavye/Gamepad desteğini korumak için, dokunma yoksa klavyeyi kontrol et
        else
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");
        }

        // 2. Sprite Değişim Mantığı
        // SpriteRenderer yoksa veya atanmamışsa çık.
        if (spriteRenderer == null) return;

        if (horizontalInput > 0.1f) // Sağa basılıyor/dokunuluyor
        {
            spriteRenderer.sprite = rollRightSprite;
        }
        else if (horizontalInput < -0.1f) // Sola basılıyor/dokunuluyor
        {
            spriteRenderer.sprite = rollLeftSprite;
        }
        else // Tuş/Dokunuş bırakıldı (Giriş 0'a yakın)
        {
            spriteRenderer.sprite = defaultSprite;
        }

        if (crashed) return; // çarpmışsa kontrol etme

        // 3. Hareket Hesaplama
        // NOT: Harekette kullandığınız Input.GetAxis("Horizontal") yerine,
        // yukarıda hesapladığımız 'horizontalInput' değişkenini kullanıyoruz!

        float moveInput = horizontalInput; // Dokunmatik değeri doğrudan hareket için kullan

        Vector3 newPosition = transform.position + Vector3.right * moveInput * moveSpeed * Time.deltaTime;

        // Sürekli ileri hareket (yukarı)
        newPosition += Vector3.up * forwardSpeed * Time.deltaTime;

        // Sınır kontrolü
        newPosition.x = Mathf.Clamp(newPosition.x, -horizontalLimit, horizontalLimit);

        transform.position = newPosition;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (crashed) return;

        if (other.CompareTag("Wall"))
        {
            crashed = true;
            Debug.Log("Uçak duvara çarptı!");

            // 💥 Patlama efekti
            if (explosionPrefab != null)
            {
                GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
                Destroy(explosion, 0.5f);
            }

            // 🛩️ Kırık uçak
            if (brokenPlanePrefab != null)
            {
                foreach (SpriteRenderer sr in brokenPlanePrefab.GetComponentsInChildren<SpriteRenderer>())
                {
                    sr.sortingOrder = 5;
                }
                GameObject brokenPlane = Instantiate(brokenPlanePrefab, transform.position, Quaternion.identity);

                foreach (Rigidbody2D rb in brokenPlane.GetComponentsInChildren<Rigidbody2D>())
                {
                    rb.AddForce(Random.insideUnitCircle * 0.4f, ForceMode2D.Impulse);
                    rb.AddTorque(Random.Range(-2f, 2f), ForceMode2D.Impulse);
                }

                Destroy(brokenPlane, 1.2f);
            }

            // 🧠 GameManager’a haber ver (burada çağır, çünkü az sonra kendini devre dışı bırakacağız)
            GameManager.Instance.StartCoroutine(GameManager.Instance.RespawnPlayerDelayed(1.3f));

            // Uçağı gizle
            gameObject.SetActive(false);
        }
    }
}

