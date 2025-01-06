using UnityEditor;
using UnityEngine;

// Custom editor for the SoundManager class
// This editor displays the SoundType enum names next to the SoundData elements
[CustomEditor(typeof(SoundManager))]
public class SoundManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SoundManager soundManager = (SoundManager)target;

        SerializedProperty soundListProp = serializedObject.FindProperty("soundList");

        // Ensure the array length matches the number of enum values
        int enumLength = System.Enum.GetValues(typeof(SoundType)).Length;

        if (soundListProp.arraySize != enumLength)
        {
            soundListProp.arraySize = enumLength;
        }

        // Display each element with its enum name
        for (int i = 0; i < enumLength; i++)
        {
            string enumName = System.Enum.GetName(typeof(SoundType), i);
            SerializedProperty element = soundListProp.GetArrayElementAtIndex(i);

            EditorGUILayout.PropertyField(element, new GUIContent(enumName));
        }

        serializedObject.ApplyModifiedProperties();
    }
}
