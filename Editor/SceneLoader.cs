
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace lugu.SceneLoader
{
    public class SceneLoader : EditorWindow
    {
        public SceneAsset[] scenes;
        public SceneAsset sceneSelected;

        public string[] scenePaths;

        private SerializedObject so;
        private SerializedProperty propScenes;
        private SerializedProperty propSceneSelected;
        private SerializedProperty propScenePaths;


        [MenuItem("Tools/Lugu/Scene Loader")]
        public static void OpenWindow()
        { 
            EditorWindow.GetWindow<SceneLoader>();
        }

        private void OnEnable()
        {
            so = new SerializedObject(this);
            propScenes = so.FindProperty("scenes");
            propSceneSelected = so.FindProperty("sceneSelected");
            propScenePaths = so.FindProperty("scenePaths");

            Load();
        }

        private void OnDisable()
        {
            Save();
        }

        private void OnGUI()
        {
            GUIStyle style = EditorStyles.textField;

            EditorGUILayout.BeginVertical(style);

            so.Update();

            EditorGUILayout.ObjectField(propSceneSelected, typeof(SceneAsset));

            so.ApplyModifiedProperties();

            if(sceneSelected != null)
            if(scenes.Contains(sceneSelected))
            {
                if (GUILayout.Button("Remove"))
                {
                    so.Update();
                    int index = Array.FindIndex(scenes, 0, (SceneAsset match) => { return match == sceneSelected; } );
                    propScenes.DeleteArrayElementAtIndex(index);
                    propScenePaths.DeleteArrayElementAtIndex(index);
                    so.ApplyModifiedProperties();
                }
            }
            else if(sceneSelected != null) 
            {
                if (GUILayout.Button("Add"))
                {
                    so.Update();
                    propScenes.InsertArrayElementAtIndex(propScenes.arraySize);
                    so.ApplyModifiedProperties();
                    so.Update();
                    SerializedProperty pScene = propScenes.GetArrayElementAtIndex(propScenes.arraySize-1);
                    pScene.objectReferenceValue = propSceneSelected.objectReferenceValue;
                    so.ApplyModifiedProperties();

                    so.Update();
                    propScenePaths.InsertArrayElementAtIndex(propScenePaths.arraySize);
                    SerializedProperty path = propScenePaths.GetArrayElementAtIndex(propScenePaths.arraySize - 1);
                    path.stringValue = AssetDatabase.GetAssetPath(sceneSelected);
                    so.ApplyModifiedProperties();
                }
            }
           
            

            EditorGUILayout.EndVertical();

            if(scenes != null)
            for (int i = 0; i < scenes.Length; i++)
            {
                SceneAsset currentScene = scenes[i];

                if (currentScene != null)
                {
                    string sceneName = currentScene.name;

                    EditorGUILayout.BeginHorizontal();

                    so.Update();
                    
                    if (GUILayout.Button("↓", GUILayout.Width(20)))
                    {
                        if (i != scenes.Length - 1)
                            propScenes.MoveArrayElement(i, i+1);
                    }

                    if (GUILayout.Button("↑", GUILayout.Width(20)))
                    {
                        if (i != 0)
                            propScenes.MoveArrayElement(i, i - 1);
                    }
                    so.ApplyModifiedProperties();

                    if (GUILayout.Button(sceneName))
                    {
                        LoadScene(i);   
                    }
                    EditorGUILayout.EndHorizontal();
                }

            }

            
        }

        public int SearchScene(string sceneName)
        { 
            for (int i = 0;i < scenes.Length;i++)
            {
                if(scenes[i].name == sceneName)
                {
                    return i;
                }
            }

            return -1;
        }

        public void LoadScene(int index)
        {
            string scenePath = AssetDatabase.GetAssetPath(scenes[index]);
            
            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        }

        public void Load()
        {
            if (propScenes.arraySize == 0)
            {
                string path = Application.persistentDataPath + "/lugu_scene_loader_save.json";

                if (File.Exists(path))
                {
                    string json = File.ReadAllText(path);
                    SceneLoaderData data = JsonUtility.FromJson<SceneLoaderData>(json);

                    if (data.scenePaths != null)
                        for (int i = 0; i < data.scenePaths.Length; i++)
                        {
                            SceneAsset sceneToAdd = AssetDatabase.LoadAssetAtPath(data.scenePaths[i], typeof(SceneAsset)) as SceneAsset;
                            so.Update();
                            propScenes.InsertArrayElementAtIndex(i);
                            SerializedProperty pScene = propScenes.GetArrayElementAtIndex(i);
                            pScene.objectReferenceValue = sceneToAdd;

                            so.ApplyModifiedProperties();

                            so.Update();
                            propScenePaths.InsertArrayElementAtIndex(i);
                            SerializedProperty propPath = propScenePaths.GetArrayElementAtIndex(i);
                            propPath.stringValue = AssetDatabase.GetAssetPath(sceneSelected);
                            so.ApplyModifiedProperties();
                        }
                }
            }
        }
        

        public void Save()
        {
            if (!Application.isPlaying)
            {
                string path = Application.persistentDataPath + "/lugu_scene_loader_save.json";

                SceneLoaderData data = new SceneLoaderData();
                data.scenePaths = new string[scenes.Length];

                for (int i = 0; i < propScenePaths.arraySize; i++)
                {
                    data.scenePaths[i] = propScenePaths.GetArrayElementAtIndex(i).stringValue;
                }

                File.WriteAllText(path, JsonUtility.ToJson(data));
            }
        }
    }
}
