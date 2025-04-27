using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBehavior : MonoBehaviour
{
    [SerializeField] private float stabilizationForce = 0.5f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Slight delay before self-stabilization kicks in
        StartCoroutine(EnableStabilization());
    }

    IEnumerator EnableStabilization()
    {
        // Wait a moment to let physics settle
        yield return new WaitForSeconds(1.0f);

        // Enable stabilization
        StartCoroutine(StabilizeRotation());
    }

    IEnumerator StabilizeRotation()
    {
        while (true)
        {
            // Apply a very slight force to keep the block from tipping over too easily
            // This makes the game slightly more forgiving
            if (rb != null)
            {
                Vector3 currentUp = transform.up;
                Vector3 targetUp = Vector3.up;

                // Calculate rotation to align with world up
                Quaternion targetRotation = Quaternion.FromToRotation(currentUp, targetUp);
                Vector3 rotationAxis = new Vector3(targetRotation.x, targetRotation.y, targetRotation.z);

                // Apply stabilization torque (weak - just enough to fight minor instability)
                rb.AddTorque(rotationAxis * stabilizationForce * Time.deltaTime);
            }

            yield return null;
        }
    }
}