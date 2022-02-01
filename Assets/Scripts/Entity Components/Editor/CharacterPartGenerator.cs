using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.IO;

public enum ECategory { chest, legs, head, eye, hair, arms, none };
public enum EDirection { side, front, back };

[CreateAssetMenu]
public class CharacterPartGenerator : ScriptableObject
{
    public bool sliceTexture = true;
    public bool generateAssets = true;
    public bool overWriteData = false;
    public Vector2Int gridSize = new Vector2Int(16, 16);

    public SliceConfigurationData sliceConfigurationData;
    public List<SliceConfigurationData.Configuration> sliceConfigurations => sliceConfigurationData.configurations;

    public List<Texture2D> textures = new List<Texture2D>();

    public string assetName;
}

[CustomEditor(typeof(CharacterPartGenerator))]
public class CharacterPartGeneratorEditor : Editor
{
    CharacterPartGenerator data => (CharacterPartGenerator)target;

    public void OnEnable()
    {
        EditorUtility.SetDirty(data);
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        EditorGUILayout.Space();

        data.assetName = EditorGUILayout.TextField("Asset Name", data.assetName);
        data.gridSize.x = EditorGUILayout.IntField("Grid Size X", data.gridSize.x);
        data.gridSize.y = EditorGUILayout.IntField("Grid Size Y", data.gridSize.y);

        data.sliceConfigurationData = EditorGUILayout.ObjectField("Slice Configuration", data.sliceConfigurationData, typeof(SliceConfigurationData), false) as SliceConfigurationData;

        EditorGUI.BeginChangeCheck();

        var serializedObject = new SerializedObject(data);
        var property = serializedObject.FindProperty("textures");
        serializedObject.Update();
        EditorGUILayout.PropertyField(property, true);

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }

        data.overWriteData = EditorGUILayout.Toggle("Overwrite asset files", data.overWriteData);

        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

        data.sliceTexture = EditorGUILayout.Toggle("Slice Texture", data.sliceTexture);

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

        data.generateAssets = EditorGUILayout.Toggle("Generate Assets", data.generateAssets);

        EditorGUILayout.EndHorizontal();

        bool hasTexture = data.textures.Count != 0;
        bool hasBodyText = !string.IsNullOrEmpty(data.assetName);
        bool hasSliceConfiguration = data.sliceConfigurationData != null;

        if (!hasTexture)
            EditorGUILayout.HelpBox("Texture Sheet needs to be set.", MessageType.Error);

        if (!hasBodyText)
            EditorGUILayout.HelpBox("Sheet item name cannot be empty.", MessageType.Error);

        if (!hasSliceConfiguration)
            EditorGUILayout.HelpBox("A Slice Configuration needs to be set .", MessageType.Error);

        if (!hasTexture || !hasBodyText || !hasSliceConfiguration)
            GUI.enabled = false;

        if (GUILayout.Button("Process"))
        {
            if (data.sliceTexture)
            {
                ProcessCharacterSheet();
            }
        }

        GUI.enabled = true;
    }

    private void ProcessCharacterSheet()
    {
        foreach (Texture2D texture in data.textures)
        {

            string path = AssetDatabase.GetAssetPath(texture);
            TextureImporter textureImporter = TextureImporter.GetAtPath(path) as TextureImporter;
            textureImporter.isReadable = true;

            if (textureImporter.spriteImportMode == SpriteImportMode.Multiple)
            {
                textureImporter.spriteImportMode = SpriteImportMode.Single;
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                textureImporter.spriteImportMode = SpriteImportMode.Multiple;
            }
            else
            {
                textureImporter.spriteImportMode = SpriteImportMode.Multiple;
            }

            List<SpriteMetaData> metaData = new List<SpriteMetaData>();

            int textureHeight = ((Texture2D)texture).height;
            int textureWidth = ((Texture2D)texture).width;

            for (int i = 0; i < data.sliceConfigurations.Count; i++)
            {
                for (int x = data.sliceConfigurations[i].startLocation.x; x < data.sliceConfigurations[i].Cells + data.sliceConfigurations[i].startLocation.x; x++)
                {
                    int rectX = data.gridSize.x * x;
                    int rectY = (texture.height - data.gridSize.y) - (data.gridSize.y * data.sliceConfigurations[i].startLocation.y);

                    /*
                    if (!HasPixels(data.texture.GetPixels(rectX, rectY, data.gridSize.x, data.gridSize.y)))
                    {
                        continue;
                    }
                    */

                    SpriteMetaData spriteMetaData = new SpriteMetaData();
                    spriteMetaData.alignment = 9;

                    spriteMetaData.rect = new Rect(rectX, rectY, data.gridSize.x, data.gridSize.y);

                    string categoryName = Enum.GetName(typeof(ECategory), (int)data.sliceConfigurations[i].category);
                    string directionName = Enum.GetName(typeof(EDirection), (int)data.sliceConfigurations[i].direction);

                    spriteMetaData.name = $"{categoryName}_{directionName}{(x - data.sliceConfigurations[i].startLocation.x).ToString()}";
                    spriteMetaData.pivot = new Vector2(0.5f, 0.5f);

                    metaData.Add(spriteMetaData);
                }
            }

            textureImporter.spritesheet = metaData.ToArray();
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

            if (data.generateAssets)
            {
                GenerateAssets(path);
            }
        }
    }

    private bool HasPixels(Color[] colors)
    {
        for (int i = 0; i < colors.Length; i++)
        {
            if (colors[i].a != 0)
            {
                return true;
            }
        }

        return false;
    }

    private void GenerateAssets(string path)
    {
        string targetPath = AssetDatabase.GetAssetPath(data);

        targetPath = targetPath.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(data)), "");

        Dictionary<string, Sprite> sprites = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().ToDictionary(v => v.name, v => v);

        ECategory currentCatagory = ECategory.none;
        BodyData characterBodyData = ScriptableObject.CreateInstance<BodyData>();

        for (int i = 0; i < data.sliceConfigurations.Count; i++)
        {
            if (data.sliceConfigurations[i].category != currentCatagory)
            {
                // Save current data, and create another once.
                currentCatagory = data.sliceConfigurations[i].category;
                characterBodyData = ScriptableObject.CreateInstance<BodyData>();

                string categoryName = Enum.GetName(typeof(ECategory), (int)data.sliceConfigurations[i].category);
                categoryName = char.ToUpper(categoryName[0]) + categoryName.Substring(1).ToLower();

                string assetPathAndName;

                if (!data.overWriteData)
                {
                    assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(targetPath + $"{data.assetName}" + ".asset");
                }
                else
                {
                    assetPathAndName = targetPath + $"{data.assetName}" + ".asset";
                }

                AssetDatabase.CreateAsset(characterBodyData, assetPathAndName);

                EditorUtility.SetDirty(characterBodyData);
            }

            // Storage in SO is based on category, once it changes. There is a new SO

            for (int x = 0; x < data.sliceConfigurations[i].Cells; x++)
            {
                string categoryName = Enum.GetName(typeof(ECategory), (int)data.sliceConfigurations[i].category);
                string directionName = Enum.GetName(typeof(EDirection), (int)data.sliceConfigurations[i].direction);
                string keyPath = $"{categoryName}_{directionName}{(x).ToString()}";

                if (sprites.ContainsKey(keyPath))
                {
                    Sprite sprite;
                    sprites.TryGetValue(keyPath, out sprite);

                    switch (data.sliceConfigurations[i].direction)
                    {
                        case EDirection.side:
                            characterBodyData.side.Add(sprite);
                            break;
                        case EDirection.front:
                            characterBodyData.front.Add(sprite);
                            break;
                        case EDirection.back:
                            characterBodyData.back.Add(sprite);
                            break;
                        default:
                            break;
                    }

                }
            }
        }
    }

}
