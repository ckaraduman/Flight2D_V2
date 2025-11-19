using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    private float moveSpeed = 50f;       // sağa-sola hız
    private float forwardSpeed = 5f;     // yukarı sabit hız
    private float horizontalLimit = 23.5f; // sahne kenar sınırı

    [Header("Efekt Ayarları")]
    public GameObject explosionPrefab;
    public GameObject brokenPlanePrefab;

    private bool crashed = false;

    void Update()
    {
        if (crashed) return; // çarpmışsa kontrol etme

        // Sağa-sola kontrol
        float moveInput = Input.GetAxis("Horizontal");
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

