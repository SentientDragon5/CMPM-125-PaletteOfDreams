using System;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class BPMAnimator : MonoBehaviour
{
    public AnimationCurve animationCurve;
    public float bpm = 120f;

    public static AnimationCurve g_animationCurve;
    public static float g_bpm = 120f;

    private void OnValidate()
    {
        g_animationCurve = animationCurve;
        g_bpm = bpm;
    }
    private void Awake()
    {
        animationCurve = BPMAnimator.g_animationCurve;
        bpm = BPMAnimator.g_bpm;
    }


    public float minValue = 0f;
    public float maxValue = 1f;


    public UITorus torus;
    void Update()
    {
        float secondsPerBeat = 60f / bpm;
        float cyclePosition = (Time.time % secondsPerBeat) / secondsPerBeat;
        float curveValue = animationCurve.Evaluate(cyclePosition);
        float outputValue = Mathf.Lerp(minValue, maxValue, curveValue);
        torus.outerRadius = outputValue;
        torus.SetVerticesDirty();
    }
}