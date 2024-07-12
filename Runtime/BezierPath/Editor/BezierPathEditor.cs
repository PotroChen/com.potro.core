using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(BezierPath))]
public class BezierPathEditor : Editor
{
    class Styles
    {
        public GUIContent MainPoint_Label = new GUIContent("MainPoint");
        public GUIContent ControlPoint1_Label = new GUIContent("ControlPoint1");
        public GUIContent ControlPoint2_Label = new GUIContent("ControlPoint2");
    }
    private BezierPath targetObject;
    private SerializedProperty pathDetailLevel;
    private SerializedProperty pathPoints;

    private ReorderableList pathPointsDrawer = null;

    private Styles styles = new Styles();

    private void OnEnable()
    {
        targetObject = (BezierPath)target;
        pathDetailLevel = serializedObject.FindProperty(nameof(BezierPath.pathDetailLevel));
        pathPoints = serializedObject.FindProperty(nameof(BezierPath.pathPoints));

        pathPointsDrawer = new ReorderableList(serializedObject, pathPoints, true, true, true, true);
        pathPointsDrawer.elementHeight = 3 * EditorGUIUtility.singleLineHeight;
        pathPointsDrawer.drawElementCallback = DrawBezierPoint;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(pathDetailLevel);
        pathPointsDrawer.DoLayoutList();

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }
    }

    private void OnSceneGUI()
    {
        if (pathPointsDrawer.selectedIndices.Count > 0)
        {
            foreach (var selectedIndex in pathPointsDrawer.selectedIndices)
            {
                DrawBezierPointHandle(targetObject, selectedIndex);
            }
        }
    }



    private void OnDisable()
    {

    }

    private void DrawBezierPoint(Rect rect, int index, bool isActive, bool isFocused)
    {
        var serializedProperty = pathPoints.GetArrayElementAtIndex(index);

        var mainPoint = serializedProperty.FindPropertyRelative(nameof(BezierPath.BezierPoint.MainPoint));
        var controlPoint1Local = serializedProperty.FindPropertyRelative(nameof(BezierPath.BezierPoint.ControlPoint1Local));
        var controlPoint2Local = serializedProperty.FindPropertyRelative(nameof(BezierPath.BezierPoint.ControlPoint2Local));

        Rect currentPosition = rect;
        currentPosition.height = EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(currentPosition, mainPoint);

        currentPosition.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(currentPosition, controlPoint1Local);

        currentPosition.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(currentPosition, controlPoint2Local);
    }

    private void DrawBezierPointHandle(BezierPath bezierPath, int index)
    {
        var pathPointArray = bezierPath.pathPoints;
        var currentMainPoint = pathPointArray[index].MainPoint;
        var currentControlPoint1 = pathPointArray[index].ControlPoint1;
        var currentControlPoint2 = pathPointArray[index].ControlPoint2;

        var oriHandleColor = Handles.color;

        Handles.color = Color.red;
        var newMainPointValue = Handles.PositionHandle(currentMainPoint, Quaternion.identity);
        Handles.Label(newMainPointValue, styles.MainPoint_Label);
        if (newMainPointValue != currentMainPoint)
        {
            pathPointArray[index].MainPoint = newMainPointValue;
        }

        Handles.color = Color.green;
        var newControlPoint1Value = Handles.PositionHandle(currentControlPoint1, Quaternion.identity);
        Handles.Label(newControlPoint1Value, styles.ControlPoint1_Label);
        Handles.DrawLine(newMainPointValue, newControlPoint1Value);
        if (newControlPoint1Value != currentControlPoint1)
        {
            pathPointArray[index].ControlPoint1 = newControlPoint1Value;
        }

        Handles.color = Color.blue;
        var newControlPoint2Value = Handles.PositionHandle(currentControlPoint2, Quaternion.identity);
        Handles.Label(newControlPoint2Value, styles.ControlPoint2_Label);
        Handles.DrawLine(newMainPointValue, newControlPoint2Value);
        if (newControlPoint2Value != currentControlPoint2)
        {
            pathPointArray[index].ControlPoint2 = newControlPoint2Value;
        }

        Handles.color = oriHandleColor;
    }

}