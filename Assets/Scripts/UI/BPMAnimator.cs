using System;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class BPMAnimator : MonoBehaviour
{
    public AnimationCurve animationCurve;
    public float bpm = 120f;

    public float minValue = 0f;
    public float maxValue = 1f;


    public UITorus torus;
    void Update()
    {
        if (torus != null)
        {
            float secondsPerBeat = 60f / bpm;
            float cyclePosition = (Time.time % secondsPerBeat) / secondsPerBeat;
            float curveValue = animationCurve.Evaluate(cyclePosition);
            float outputValue = Mathf.Lerp(minValue, maxValue, curveValue);
            torus.outerRadius = outputValue;
            torus.SetVerticesDirty();
        }
    }
}