/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using UnityEngine;

namespace BinaryEgo.ToolBox
{
    public class GeometryUtils
    {
        public Vector3 QuadLerp(Vector3 p_a, Vector3 p_b, Vector3 p_c, Vector3 p_d, float p_u, float p_v)
        {
            Vector3 abu = Vector3.Lerp(p_a, p_b, p_u);
            Vector3 cdu = Vector3.Lerp(p_c, p_d, p_u);
            return Vector3.Lerp(cdu, abu, p_v);
        }

        static float CheckLineSide(Vector2 p_v1, Vector2 p_v2, Vector2 p_v)
        {
            return (p_v2.y - p_v1.y) * (p_v.x - p_v1.x) + (-p_v2.x + p_v1.x) * (p_v.y - p_v1.y);
        }

        static float CalculateTriangleArea(Vector2 p_v1, Vector2 p_v2, Vector2 p_v3)
        {
            return Mathf.Abs((p_v1.x * (p_v2.y - p_v3.y) +
                              p_v2.x * (p_v3.y - p_v1.y) +
                              p_v3.x * (p_v1.y - p_v2.y)) / 2.0f);
        }

        static float CalculateTriangleArea(float p_x1, float p_y1, float p_x2, float p_y2, float p_x3, float p_y3)
        {
            return Mathf.Abs((p_y1 * (p_y2 - p_y3) +
                              p_x2 * (p_y3 - p_y1) +
                              p_x3 * (p_y1 - p_y2)) / 2.0f);
        }

        static bool IsInsideTriangle(Vector2 p_v1, Vector2 p_v2, Vector2 p_v3, Vector2 p_v)
        {
            float d = ((p_v2.y - p_v3.y) * (p_v1.x - p_v3.x) + (p_v3.x - p_v2.x) * (p_v1.y - p_v3.y));
            float a = ((p_v2.y - p_v3.y) * (p_v.x - p_v3.x) + (p_v3.x - p_v2.x) * (p_v.y - p_v3.y)) / d;
            float b = ((p_v3.y - p_v1.y) * (p_v.x - p_v3.x) + (p_v1.x - p_v3.x) * (p_v.y - p_v3.y)) / d;
            float c = 1 - a - b;

            return 0 <= a && a <= 1 && 0 <= b && b <= 1 && 0 <= c && c <= 1;
        }

        // Sometimes it is handly to have direct floats when parsing vertex buffers directly
        static bool IsInsideTriangle(float p_x1, float p_y1, float p_x2, float p_y2, float p_x3, float p_y3, float p_x,
            float p_y)
        {
            float d = ((p_y2 - p_y3) * (p_x1 - p_x3) + (p_x3 - p_x2) * (p_y1 - p_y3));
            float a = ((p_y2 - p_y3) * (p_x - p_x3) + (p_x3 - p_x2) * (p_y - p_y3)) / d;
            float b = ((p_y3 - p_y1) * (p_x - p_x3) + (p_x1 - p_x3) * (p_y - p_y3)) / d;
            float c = 1 - a - b;

            return 0 <= a && a <= 1 && 0 <= b && b <= 1 && 0 <= c && c <= 1;
        }
    }
}