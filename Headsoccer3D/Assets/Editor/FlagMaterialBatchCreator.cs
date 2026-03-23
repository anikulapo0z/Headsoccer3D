using UnityEngine;
using UnityEditor;
using System.IO;

public class FlagMaterialBatchCreator : EditorWindow
{
    private string texturesFolder = "Assets/Textures/Flags";
    private string materialsFolder = "Assets/Materials/CharacterFlags";
    private Shader fallbackShader;

    private const string ShaderName = "Saphead Studios/Principle Toon";
    private const string TexturePrefix = "Flag_";
    private const string MaterialPrefix = "M_CFlag_";

    [MenuItem("Tools/Flag Material Batch Creator")]
    public static void ShowWindow()
    {
        GetWindow<FlagMaterialBatchCreator>("Flag Material Creator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Batch Create Flag Materials", EditorStyles.boldLabel);

        texturesFolder = EditorGUILayout.TextField("Textures Folder", texturesFolder);
        materialsFolder = EditorGUILayout.TextField("Materials Folder", materialsFolder);

        fallbackShader = (Shader)EditorGUILayout.ObjectField(
            "Fallback Shader",
            fallbackShader,
            typeof(Shader),
            false
        );

        if (GUILayout.Button("Create Materials"))
        {
            CreateMaterials();
        }
    }

    private void CreateMaterials()
    {
        if (!AssetDatabase.IsValidFolder(materialsFolder))
        {
            Directory.CreateDirectory(materialsFolder);
            AssetDatabase.Refresh();
        }

        Shader shader = Shader.Find(ShaderName);

        if (shader == null)
        {
            Debug.LogWarning($"Shader '{ShaderName}' not found. Using fallback shader.");
            shader = fallbackShader;

            if (shader == null)
            {
                Debug.LogError("No valid shader assigned.");
                return;
            }
        }

        string[] textureGUIDs = AssetDatabase.FindAssets("t:Texture2D", new[] { texturesFolder });

        foreach (string guid in textureGUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string fileName = Path.GetFileNameWithoutExtension(path);

            if (!fileName.StartsWith(TexturePrefix))
                continue;

            string countryName = fileName.Substring(TexturePrefix.Length);
            string materialName = MaterialPrefix + countryName;

            string materialPath = Path.Combine(materialsFolder, materialName + ".mat");
            materialPath = materialPath.Replace("\\", "/");

            if (File.Exists(materialPath))
            {
                Debug.Log($"Material already exists: {materialName}");
                continue;
            }

            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);

            Material mat = new Material(shader);
            mat.name = materialName;

            if (mat.HasProperty("_MainTex"))
            {
                mat.SetTexture("_MainTex", texture);
            }
            else
            {
                Debug.LogWarning($"Shader does not have _MainTex property: {materialName}");
            }

            AssetDatabase.CreateAsset(mat, materialPath);
            Debug.Log($"Created material: {materialName}");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Flag material batch creation complete.");
    }
}