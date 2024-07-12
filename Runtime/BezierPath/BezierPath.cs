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
        public Vector3 MainPoint;
        public Vector3 ControlPoint1Local;
        public Vector3 ControlPoint1
        {
            get
            {
                return MainPoint + ControlPoint1Local;
            }
            set
            {
                ControlPoint1Local = value - MainPoint;
            }
        }
        public Vector3 ControlPoint2Local;
        public Vector3 ControlPoint2
        {
            get
            {
                return MainPoint + ControlPoint2Local;
            }
            set
            {
                ControlPoint2Local = value - MainPoint;
            }
        }
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
                var pathPoint = GameFramework.Math.CubicBezier(pathPoint1.MainPoint, pathPoint1.ControlPoint2,
                    pathPoint2.ControlPoint1, pathPoint2.MainPoint, t);
                tempPathPosition.Add(pathPoint);
            }
        }
        return tempPathPosition.ToArray();
    }

#if UNITY_EDITOR
    
    public bool showGizmos = false;
    private void OnDrawGizmos()
    {
        if (!showGizmos && Application.isPlaying)
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
