using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    public float parallaxAmount = 0.1f; // Parallax effect multiplier

    private Vector3 previousMousePosition;

    private void Start()
    {
        previousMousePosition = Input.mousePosition;
    }

    private void Update()
    {
        Vector3 mouseDelta = Input.mousePosition - previousMousePosition;
        Vector3 parallaxOffset = mouseDelta * parallaxAmount;

        transform.position += parallaxOffset;

        previousMousePosition = Input.mousePosition;
    }
}
