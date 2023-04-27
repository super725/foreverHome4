using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class TrailConfig : MonoBehaviour
{
    public float startWidth = 0.1f; // The starting width of the trail.
    public float endWidth = 0.01f; // The ending width of the trail.
    public Gradient colorGradient; // The color gradient for the trail.
    public float timeToLive = 1.0f; // The duration of the trail in seconds.

    private TrailRenderer trailRenderer;

    private void Awake()
    {
        trailRenderer = GetComponent<TrailRenderer>();
    }

    private void Start()
    {
        ConfigureTrail();
    }

    private void ConfigureTrail()
    {
        trailRenderer.startWidth = startWidth;
        trailRenderer.endWidth = endWidth;
        trailRenderer.colorGradient = colorGradient;
        trailRenderer.time = timeToLive;
    }
}
