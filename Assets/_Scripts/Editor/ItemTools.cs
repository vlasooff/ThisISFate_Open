using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;
using UnityEngine.UI;
using System;
using System.IO;
using Network;

public class ItemTools : EditorWindow {

    public Color newcolor;
    public Color currentColor; 
    GameObject obj;
    public Mesh mesh;

    public MeshFilter meshfilter;
    public MeshRenderer renderer;
    public Transform icon;
    public ushort id;
    public int width, height, orthoSize;
    [MenuItem("VX/ItemTools")]
    public static void OpenEditor()
    {
        EditorWindow.GetWindow<ItemTools>("ItemTools");
    }
    private void OnSelectionChange()
    {
        Repaint();
    }
    protected void OnGUI()
    {
        if (Application.isPlaying)
        {
            GUILayout.Label("PLAY MODE ACTIVE", GUI.skin.box, GUILayout.ExpandWidth(true), GUILayout.MinHeight(32));
            return;
        }

        GUILayout.Label("OPTIONS", GUI.skin.box, GUILayout.ExpandWidth(true), GUILayout.MinHeight(32));


        GUILayout.Space(20);
      //  iconnewColor = EditorGUILayout.ObjectField("objectIcons", iconnewColor, typeof(VXIconsObject), true) as VXIconsObject;
        newcolor = EditorGUILayout.ColorField("new",newcolor);
        currentColor = EditorGUILayout.ColorField("curent",currentColor); 
        id = (ushort)EditorGUILayout.IntField("idItem", id);
        mesh = EditorGUILayout.ObjectField("mesh", mesh, typeof(Mesh), true) as Mesh;
        renderer = EditorGUILayout.ObjectField("meshRender", renderer, typeof(MeshRenderer), true) as MeshRenderer;
        meshfilter = EditorGUILayout.ObjectField("meshf", meshfilter, typeof(MeshFilter), true) as MeshFilter; 
        icon = EditorGUILayout.ObjectField("icon", icon, typeof(Transform), true) as Transform;

        width = EditorGUILayout.IntField("widht", width);
        height = EditorGUILayout.IntField("height", height);
        orthoSize = EditorGUILayout.IntField("orthoSize", orthoSize);
        if(icon == null)
        {
            if (Selection.activeGameObject != null)
        {
            //hack to not select preview chunks OR Points OR Destructible :)
            if (Selection.activeGameObject.hideFlags != HideFlags.NotEditable)
            {
                if (Selection.activeGameObject != null) icon = Selection.activeGameObject.transform;

            }
        }
        }
        ItemAsset asset = Resources.Load<ItemAsset>($"Prefabs/items/item_{id}");
        if (meshfilter != null) meshfilter.mesh = asset.mesh;
        if (!icon) return;
        Texture2D texture = null;
        if (GUILayout.Button("Capute Icon"))
        { 
           
            texture = captureIcon(id, icon, width, height, orthoSize);
            
            SaveImg(texture);
            id += 1;
        }
        if (GUILayout.Button("Cheack icon"))
        {
            texture = LoadImg(id);
  
        }
        if(GUILayout.Button("Capute set color"))
        {
            texture = captureIcon(id, icon, width, height, orthoSize);
            captureWriteIcon(texture, newcolor, currentColor, id);
        }
       
    }
    public static Texture2D captureWriteIcon(Texture2D outTexture,Color newcolor,Color notcolor,int id)
    {
        Texture2D texture = outTexture;
        for (int i = 0; i < texture.width; i++)
        {
            for (int j = 0; j < texture.height; j++)
            {
                if (texture.GetPixel(i, j) != notcolor)
                {
                    texture.SetPixel(i, j, newcolor);
              
                     
                }
            }
        }
        
        texture.Apply();
        Texture tex = texture;
     //   iconnewColor.icons[id].bytes  = texture.EncodeToPNG();
        return texture;
    }

 
    

    public Texture2D captureIcon(ushort id,Transform icon, int width, int height, float orthoSize)
    {
      //  if(obj != null) Destroy(obj);  
      //  obj = Instantiate(objectIcons.list[id],icon);
        Camera cam;
        if (icon.GetComponent<Camera>() != null)
        {

            cam = icon.GetComponent<Camera>();
        }
        else
        {
            cam = icon.gameObject.AddComponent<Camera>();
        }
        RenderTexture temporary = RenderTexture.GetTemporary(width, height, 16, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
        temporary.name = string.Concat(new object[]
        {
                "Render_",
                id,
                "_" 
        });
        RenderTexture.active = temporary;
        cam.targetTexture = temporary;
        cam.orthographicSize = orthoSize;
        bool fog = RenderSettings.fog;
        AmbientMode ambientMode = RenderSettings.ambientMode;
        Color ambientSkyColor = RenderSettings.ambientSkyColor;
        Color ambientEquatorColor = RenderSettings.ambientEquatorColor;
        Color ambientGroundColor = RenderSettings.ambientGroundColor;
        RenderSettings.fog = false;
        RenderSettings.ambientMode = AmbientMode.Trilight;
        RenderSettings.ambientSkyColor = Color.white;
        RenderSettings.ambientEquatorColor = Color.white;
        RenderSettings.ambientGroundColor = Color.white;


        cam.farClipPlane = 16f;
        cam.Render();
      
 
        RenderSettings.fog = fog;
        RenderSettings.ambientMode = ambientMode;
        RenderSettings.ambientSkyColor = ambientSkyColor;
        RenderSettings.ambientEquatorColor = ambientEquatorColor;
        RenderSettings.ambientGroundColor = ambientGroundColor;
        Texture2D texture2D = new Texture2D(width, height, TextureFormat.ARGB32, false, true);
        texture2D.name = string.Concat(new object[]
        {
                "Icon_",
                id
                
        });
        Debug.Log(texture2D.name);
        
        texture2D.filterMode = FilterMode.Point;
        texture2D.ReadPixels(new Rect(0f, 0f, (float)width, (float)height), 0, 0);
    
        texture2D.Apply();
        RenderTexture.ReleaseTemporary(temporary);
        
        return texture2D;
    } 
    public void SaveImg(Texture2D texture)
    {
        string finalPath = Application.dataPath + $"/Resources/Prefabs/items/icons/{texture.name}.png";
		File.WriteAllBytes(finalPath, texture.EncodeToPNG());
    }
    public Texture2D LoadImg(int id)
    {
        string finalPath = Application.dataPath + $"/Resources/Prefabs/items/icons/Icon_{id}.png";
		
        Texture2D texture2D = new Texture2D(width, height, TextureFormat.ARGB32, false, true);
        Debug.Log(texture2D.LoadImage(File.ReadAllBytes(finalPath)));
        texture2D.ReadPixels(new Rect(0f, 0f, (float)width, (float)height), 0, 0);
    
        texture2D.Apply();

        return texture2D;
    }
 
}
