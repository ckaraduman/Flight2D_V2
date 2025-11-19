using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [Header("Takip Edilecek Nesne")]
    public Transform target;         // Player objesi

    [Header("Takip Ayarlarý")]
    public Vector3 offset = new Vector3(0f, 5f, -10f);
    public float smoothSpeed = 5f;

    void LateUpdate()
    {
        // Eðer target yoksa otomatik bulmaya çalýþ
        if (target == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) target = p.transform;
            else return; // hedef yoksa çýk
        }

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }

    // Instantly move camera to target (oyuncu spawn olduktan sonra çaðýr)
    public void SnapToTarget()
    {
        if (target == null) return;
        Vector3 snapPos = target.position + offset;
        snapPos.z = transform.position.z;
        transform.position = snapPos;
    }
}
