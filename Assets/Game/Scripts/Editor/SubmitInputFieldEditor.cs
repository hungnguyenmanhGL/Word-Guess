using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(SubmitInputField), true)]
[CanEditMultipleObjects]
public class SubmitInputFieldEditor : Editor
{
    SerializedProperty m_KeyboardDoneProperty;
    SerializedProperty m_TextComponent;

    protected void OnEnable() {
        m_KeyboardDoneProperty = serializedObject.FindProperty("keyboardDoneEvent");
        m_TextComponent = serializedObject.FindProperty("m_TextComponent");
    }


    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        EditorGUI.BeginDisabledGroup(m_TextComponent == null || m_TextComponent.objectReferenceValue == null);

        EditorGUILayout.Space();

        serializedObject.Update();
        EditorGUILayout.PropertyField(m_KeyboardDoneProperty);
        serializedObject.ApplyModifiedProperties();

        EditorGUI.EndDisabledGroup();
        serializedObject.ApplyModifiedProperties();
    }
}
