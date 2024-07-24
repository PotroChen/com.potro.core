using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

/*
 * 此处的贝塞尔曲线指的其实是三阶贝塞尔曲线
 */
namespace GameFramework
{
    public static partial class Math
    {

        /// <summary>
        /// CubicBezier(三阶贝塞尔曲线)
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

            // 获取曲线的原始起点和终点
            Vector3 oriStartPos = curvePoints[0];
            Vector3 oriEndPos = curvePoints[curvePoints.Length - 1];

            // 计算原始起点到终点的向量和新的起点到终点的向量
            Vector3 oriDir = oriEndPos - oriStartPos;
            Vector3 newDir = newEndPos - newStartPos;

            // 计算平移向量
            Vector3 translation = newStartPos - oriStartPos;

            // 计算缩放因子
            float scale = newDir.magnitude / oriDir.magnitude;

            // 计算旋转矩阵
            Quaternion rotation = Quaternion.FromToRotation(oriDir, newDir);

            // 创建平移和缩放矩阵
            Matrix4x4 translationMatrix = Matrix4x4.Translate(translation);
            Matrix4x4 scalingMatrix = Matrix4x4.Scale(new Vector3(scale, scale, scale));
            Matrix4x4 rotationMatrix = Matrix4x4.Rotate(rotation);

            // 合并变换矩阵
            Matrix4x4 transformMatrix = translationMatrix * rotationMatrix * scalingMatrix;

            // 应用变换
            for (int i = 0; i < curvePoints.Length; i++)
            {
                // 将曲线点转换为齐次坐标
                Vector3 originalPoint = curvePoints[i] - oriStartPos; //平移到原始起点
                Vector4 homogenousPoint = new Vector4(originalPoint.x, originalPoint.y, originalPoint.z, 1.0f);

                // 应用变换矩阵
                homogenousPoint = transformMatrix * homogenousPoint;

                // 更新曲线点位置
                curvePoints[i] = new Vector3(homogenousPoint.x, homogenousPoint.y, homogenousPoint.z);
            }

            // 平移所有点，使其相对新起点的位置正确
            Vector3 finalOffset = newStartPos - curvePoints[0];
            for (int i = 0; i < curvePoints.Length; i++)
            {
                curvePoints[i] += finalOffset;
            }
        }
    }
}
