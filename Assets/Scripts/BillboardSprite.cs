using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardSprite : MonoBehaviour
{
    public Camera cameraToFace; // Assign the main camera in the inspector
    public bool wobbleEnabled = true; // Toggle for wobble effect
    public float wobbleAmount = 5f; // Intensity of the wobble
    public float wobbleSpeed = 2f; // Speed of the wobble

    void Update()
    {
        if (cameraToFace == null)
        {
            cameraToFace = Camera.main; 
            if (cameraToFace == null) return; // No camera to look at!
        }

        // Make the quad face the camera
        transform.LookAt(cameraToFace.transform.position, Vector3.up); 
        transform.localRotation *= Quaternion.Euler(0f, 180f, 0f); 

        // Apply wobble if enabled
        if (wobbleEnabled)
        {
            // Calculate the wobble rotation
            Quaternion wobbleRotation = Quaternion.Euler(
                0f, 
                Mathf.Sin(Time.time * wobbleSpeed) * wobbleAmount, 
                0f
            );

            // Apply the wobble rotation to the quad's local rotation
            transform.localRotation *= wobbleRotation; 
        }
    }
}
