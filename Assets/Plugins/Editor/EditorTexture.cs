using System.IO;
using UnityEditor;
using UnityEngine;
public class EditorTexture : EditorWindow
{
    public Texture2D texture2D;
    Vector2Int UV;
    int Size = 1;
    Color preview;
    Color SetColor;
    bool isColors;
    Color[] colors = null;
    Vector2 scroll;
    [MenuItem("VX/EditorTexture")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        EditorTexture window = (EditorTexture)GetWindow(typeof(EditorTexture));
        window.Show();
    }
    void OnGUI()
    {
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);

        scroll = EditorGUILayout.BeginScrollView(scroll);
        texture2D = (Texture2D)EditorGUILayout.ObjectField(texture2D, typeof(Texture2D), new GUILayoutOption[0]);
        UV = EditorGUILayout.Vector2IntField("UV: ", UV, new GUILayoutOption[0]);
        Size = EditorGUILayout.IntField(Size, new GUILayoutOption[0]);
        EditorGUILayout.ColorField(preview);
        isColors = EditorGUILayout.Toggle("isColors", isColors, new GUILayoutOption[0]);

        if (isColors)
        {
            if (colors.Length < 1)
                colors = texture2D.GetPixels();
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = EditorGUILayout.ColorField("# " + i, colors[i]);
            }

        }
        if (texture2D != null)
        {
            if (!isColors)
                EditorGUI.DrawPreviewTexture(new Rect(new Vector2(100, 200), new Vector2(256, 256)), texture2D);
            SetColor = EditorGUILayout.ColorField(SetColor);
            preview = texture2D.GetPixel(UV.x, UV.y);
            texture2D.SetPixels(colors);
            texture2D.Apply();
            if (GUILayout.Button("Update"))
            {

                colors = texture2D.GetPixels();
            }
            if (GUILayout.Button("Apply"))
            {
                AssetDatabase.SaveAssets();
                string patch = Application.dataPath + $"/Textures/Texture_{(int)Time.time}.png";
            
                File.WriteAllBytes(patch, texture2D.EncodeToPNG());


                Debug.Log("Test");
            }
        }

        EditorGUILayout.EndScrollView();
    }
    void EditTexture()
    {

    }
}
