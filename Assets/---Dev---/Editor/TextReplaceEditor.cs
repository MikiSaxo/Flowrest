using UnityEditor;

[CustomEditor(typeof(TextReplace))]
public class TextReplaceEditor : Editor
{
    private SerializedProperty Text;
    private SerializedProperty IsColor;
    private SerializedProperty Color;
    private SerializedProperty IsBold;
    private SerializedProperty IsItalic;
    
    
    void OnEnable()
    {
        Text = serializedObject.FindProperty("Text");
        IsColor = serializedObject.FindProperty("IsColor");
        Color = serializedObject.FindProperty("Color");
        IsBold = serializedObject.FindProperty("IsBold");
        IsItalic = serializedObject.FindProperty("IsItalic");
    }
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        EditorGUILayout.PropertyField(Text);
        
        EditorGUILayout.Space(10);
        
        EditorGUILayout.PropertyField(IsColor);
        if (IsColor.boolValue)
            EditorGUILayout.PropertyField(Color);
        
        EditorGUILayout.PropertyField(IsBold);
        EditorGUILayout.PropertyField(IsItalic);
        
        serializedObject.ApplyModifiedProperties();
        
        // TextModifier myTarget = (TextModifier)target;
        //
        // myTarget.Text = EditorGUILayout.TextField("My String", myTarget.Text);
        // myTarget.IsColor = EditorGUILayout.Toggle("My bool", myTarget.IsColor);
        // myTarget.Color = EditorGUILayout.ColorField("My Color", myTarget.Color);
        // myTarget.myFloat = EditorGUILayout.FloatField("My Float", myTarget.myFloat);
    }
}
