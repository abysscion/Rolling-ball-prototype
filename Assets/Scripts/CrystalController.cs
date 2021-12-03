using UnityEngine;

public class CrystalController : MonoBehaviour
{
    public float rotationSpeed = 100f;

    private void Update()
    {
        HandleRotation();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        GameController.Instance.CrystalsCount++;
        Destroy(this.gameObject);
    }

    private void HandleRotation()
    {
        transform.Rotate(transform.up, rotationSpeed * Time.deltaTime);
    }
}
