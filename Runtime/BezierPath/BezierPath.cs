using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BezierPath : MonoBehaviour
{
    [Serializable]
    public struct BezierPoint
    {
        public Vector3 mainPoint;
        public Vector3 controlPoint1;
        public Vector3 controlPoint2;
    }

    public BezierPoint[] pathPoints;

    [Min(1)]
    public int pathDetailLevel = 15;

    private List<Vector3> tempPathPosition = new List<Vector3>();
    public Vector3[] GetPathPositions()
    {
        tempPathPosition.Clear();
        for (int i = 0; i < pathPoints.Length -1; i++)
        {
            for (int sectionIndex = 0; sectionIndex < pathDetailLevel; sectionIndex++)
            {
                var pathPoint1 = pathPoints[i];
                var pathPoint2 = pathPoints[i + 1];
                float t = (float)sectionIndex / pathDetailLevel;
                var pathPoint = GameFramework.Math.CubicBezier(pathPoint1.mainPoint, pathPoint1.controlPoint2,
                    pathPoint2.controlPoint1, pathPoint2.mainPoint, t);
                tempPathPosition.Add(pathPoint);
            }
        }
        return tempPathPosition.ToArray();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
            return;
        var pathPositions = GetPathPositions();
        for (int i = 0; i < pathPositions.Length - 1; i++)
        {
            var point1 = pathPositions[i];
            var point2 = pathPositions[i + 1];
            Gizmos.DrawLine(point1, point2);
            Gizmos.DrawWireSphere(point1, 0.2f);
            Gizmos.DrawWireSphere(point2, 0.2f);

        }
    }
#endif

}
