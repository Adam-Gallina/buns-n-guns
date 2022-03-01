using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(FiringMode), true)]
public class FiringMode_PropertyDrawer : PropertyDrawer
{
    private void DrawProp(Rect position, SerializedProperty property, int currLine)
    {
        Rect rect = new Rect(position.min.x, position.min.y + currLine++ * EditorGUIUtility.singleLineHeight, position.size.x, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(rect, property);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        Rect rectFoldout = new Rect(position.min.x, position.min.y, position.size.x, EditorGUIUtility.singleLineHeight);
        property.isExpanded = EditorGUI.Foldout(rectFoldout, property.isExpanded, label);

        if (property.isExpanded && property.hasVisibleChildren)
        {
            int total = property.Copy().CountInProperty() - 1;

            property.NextVisible(true);

            int i = 0;
            do
            {
                DrawProp(position, property, ++i);
                property.NextVisible(true);
            } while (i < total);
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int totalLines = property.CountInProperty();

        return EditorGUIUtility.singleLineHeight * totalLines + EditorGUIUtility.standardVerticalSpacing * (totalLines - 1);
    }
}
