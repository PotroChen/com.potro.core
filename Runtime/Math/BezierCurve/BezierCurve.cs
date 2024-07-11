using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

/*
 * �˴��ı���������ָ����ʵ�����ױ���������
 */
namespace GameFramework
{
    public static partial class Math
    {

        /// <summary>
        /// CubicBezier(���ױ���������)
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="t">The interpolation value</param>
        /// <returns></returns>
        public static Vector3 CubicBezier(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            Vector3 a = p0 + (p1 - p0) * t;
            Vector3 b = p1 + (p2 - p1) * t;
            Vector3 c = p2 + (p3 - p2) * t;

            Vector3 aa = a + (b - a) * t;
            Vector3 bb = b + (c - b) * t;
            return aa + (bb - aa) * t;
        }
    }
}
