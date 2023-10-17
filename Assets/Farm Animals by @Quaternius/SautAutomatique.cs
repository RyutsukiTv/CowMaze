using System.Collections;
using UnityEngine;

public class SautAutomatique  : MonoBehaviour
{
    public float forceSaut = 10f; // Force du saut
    public float vitesseRotation = 180f; // Vitesse de rotation en degrés par seconde
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(SauterAutomatiquement());
    }

    private IEnumerator SauterAutomatiquement()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f); // Attendre 2 secondes

            // Rotation de 360 degrés pendant le saut
            float rotationY = 0f;
            float rotationAmount = 360f;
            while (rotationY < rotationAmount)
            {
                float rotationStep = vitesseRotation * Time.deltaTime;
                rb.MoveRotation(rb.rotation * Quaternion.Euler(Vector3.up * rotationStep));
                rotationY += rotationStep;
                yield return null;
            }

            // Appliquer une force vers le haut pour simuler le saut
            rb.AddForce(Vector3.up * forceSaut, ForceMode.Impulse);
        }
    }
}
