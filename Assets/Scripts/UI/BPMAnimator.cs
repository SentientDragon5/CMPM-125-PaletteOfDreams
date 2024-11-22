using UnityEngine;
using UnityEngine.UI.Extensions;

public class BPMAnimator : MonoBehaviour
{
    public AnimationCurve animationCurve;
    public float bpm = 120f;
    public float minValue = 0f;
    public float maxValue = 1f;

    private float currentTime = 0f;

    public UITorus torus;
    void Update()
    {
        // Calculate the time in seconds per beat
        float secondsPerBeat = 60f / bpm;

        // Increment the current time based on the time passed since the last frame
        currentTime = Time.time;

        // Calculate the current position in the animation cycle (0 to 1)
        float cyclePosition = (currentTime % secondsPerBeat) / secondsPerBeat;

        // Evaluate the animation curve at the current position
        float curveValue = animationCurve.Evaluate(cyclePosition);

        // Map the curve value to the desired range
        float outputValue = Mathf.Lerp(minValue, maxValue, curveValue);

        // Do something with the outputValue, e.g., update a transform position
        // transform.position = new Vector3(0f, outputValue, 0f);
        torus.outerRadius = outputValue;
        torus.SetVerticesDirty();
    }
}