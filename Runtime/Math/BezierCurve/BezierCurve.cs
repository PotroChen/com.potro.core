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

        public static void TransformCurve(Vector3[] curvePoints, Vector3 newStartPos, Vector3 newEndPos)
        {
            if (curvePoints.Length < 2)
            {
                Debug.LogError("Curve must have at least two points.");
                return;
            }

            // ��ȡ���ߵ�ԭʼ�����յ�
            Vector3 oriStartPos = curvePoints[0];
            Vector3 oriEndPos = curvePoints[curvePoints.Length - 1];

            // ����ԭʼ��㵽�յ���������µ���㵽�յ������
            Vector3 oriDir = oriEndPos - oriStartPos;
            Vector3 newDir = newEndPos - newStartPos;

            // ����ƽ������
            Vector3 translation = newStartPos - oriStartPos;

            // ������������
            float scale = newDir.magnitude / oriDir.magnitude;

            // ������ת����
            Quaternion rotation = Quaternion.FromToRotation(oriDir, newDir);

            // ����ƽ�ƺ����ž���
            Matrix4x4 translationMatrix = Matrix4x4.Translate(translation);
            Matrix4x4 scalingMatrix = Matrix4x4.Scale(new Vector3(scale, scale, scale));
            Matrix4x4 rotationMatrix = Matrix4x4.Rotate(rotation);

            // �ϲ��任����
            Matrix4x4 transformMatrix = translationMatrix * rotationMatrix * scalingMatrix;

            // Ӧ�ñ任
            for (int i = 0; i < curvePoints.Length; i++)
            {
                // �����ߵ�ת��Ϊ�������
                Vector3 originalPoint = curvePoints[i] - oriStartPos; //ƽ�Ƶ�ԭʼ���
                Vector4 homogenousPoint = new Vector4(originalPoint.x, originalPoint.y, originalPoint.z, 1.0f);

                // Ӧ�ñ任����
                homogenousPoint = transformMatrix * homogenousPoint;

                // �������ߵ�λ��
                curvePoints[i] = new Vector3(homogenousPoint.x, homogenousPoint.y, homogenousPoint.z);
            }

            // ƽ�����е㣬ʹ�����������λ����ȷ
            Vector3 finalOffset = newStartPos - curvePoints[0];
            for (int i = 0; i < curvePoints.Length; i++)
            {
                curvePoints[i] += finalOffset;
            }
        }
    }
}
