using System;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class BPMScaler : MonoBehaviour
{
    public AnimationCurve animationCurve;
    public float bpm = 120f;

    public float minValue = 0f;
    public float maxValue = 1f;

    void Update()
    {
        float secondsPerBeat = 60f / bpm;
        float cyclePosition = (Time.time % secondsPerBeat) / secondsPerBeat;
        float curveValue = animationCurve.Evaluate(cyclePosition);
        float outputValue = Mathf.Lerp(minValue, maxValue, curveValue);
        transform.localScale = new Vector3(outputValue, outputValue, outputValue);
    }
}