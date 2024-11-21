using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
    [RequireComponent(typeof(CanvasRenderer))]
    [AddComponentMenu("UI/Curve", 12)]
    public class UICurve : MaskableGraphic
    {
        public Vector2[] points;
        public int resolution = 50;
        public Color curveColor = Color.white;
        public float thickness = 5f;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            if (points == null || points.Length < 3)
            {
                return;
            }

            // Calculate and add vertices
            for (int i = 0; i <= resolution; i++)
            {
                float t = i / (float)resolution;
                Vector2 point = CalculateBezierPoint(t, points);
                Vector2 perpendicular = Perpendicular(CalculateBezierTangent(t, points)).normalized;
                vh.AddVert(point + perpendicular * thickness / 2, curveColor, Vector2.zero);
                vh.AddVert(point - perpendicular * thickness / 2, curveColor, Vector2.zero);
            }

            // Add triangles to form the line
            for (int i = 0; i < resolution; i++)
            {
                vh.AddTriangle(2 * i, 2 * i + 1, 2 * i + 2);
                vh.AddTriangle(2 * i + 2, 2 * i + 1, 2 * i + 3);
            }
        }

        // Calculate Bezier point at time t
        private Vector2 CalculateBezierPoint(float t, Vector2[] points)
        {
            Vector2 point = Vector2.zero;
            int n = points.Length - 1;
            for (int i = 0; i <= n; i++)
            {
                point += BinomialCoefficient(n, i) * Mathf.Pow(1 - t, n - i) * Mathf.Pow(t, i) * points[i];
            }
            return point;
        }

        // Calculate Bezier tangent at time t
        private Vector2 CalculateBezierTangent(float t, Vector2[] points)
        {
            Vector2 tangent = Vector2.zero;
            int n = points.Length - 1;
            for (int i = 0; i <= n - 1; i++)
            {
                tangent += BinomialCoefficient(n - 1, i) * Mathf.Pow(1 - t, n - 1 - i) * Mathf.Pow(t, i) * (points[i + 1] - points[i]);
            }
            return tangent * n;
        }

        // Calculate perpendicular vector
        private Vector2 Perpendicular(Vector2 v)
        {
            return new Vector2(-v.y, v.x);
        }

        // Calculate binomial coefficient
        private int BinomialCoefficient(int n, int k)
        {
            if (k < 0 || k > n)
            {
                return 0;
            }
            if (k == 0 || k == n)
            {
                return 1;
            }
            return BinomialCoefficient(n - 1, k - 1) + BinomialCoefficient(n - 1, k);
        }
    }

}
