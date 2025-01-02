using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Wave.EnemyDetail))]
public class EnemyDetailDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Use the index to name the element as "Enemy Set [n]"
        SerializedProperty prefabProp = property.FindPropertyRelative("prefab");
        SerializedProperty countProp = property.FindPropertyRelative("count");

        // Draw fields for prefab and count
        float halfWidth = position.width / 2;
        Rect prefabRect = new Rect(position.x, position.y, halfWidth - 5, position.height);
        Rect countRect = new Rect(position.x + halfWidth + 5, position.y, halfWidth - 5, position.height);

        EditorGUI.PropertyField(prefabRect, prefabProp, GUIContent.none);
        EditorGUI.PropertyField(countRect, countProp, GUIContent.none);

        EditorGUI.EndProperty();
    }
}
