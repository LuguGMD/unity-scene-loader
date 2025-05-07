
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace lugu.SceneLoader
{
    public class SceneLoader : EditorWindow
    {
        public List<SceneAsset> scenes = new List<SceneAsset>();
        public SceneAsset sceneSelected;

        public List<string> scenePaths;

        private SerializedObject so;
        private SerializedProperty propScenes;
        private SerializedProperty propSceneSelected;
        private SerializedProperty propScenePaths;


        [MenuItem("Tools/Scene Loader")]
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

            //Load();
        }

        private void OnDisable()
        {
            //Save();
        }

        private void OnGUI()
        {
            GUIStyle style = EditorStyles.textField;

            EditorGUILayout.BeginVertical(style);

            so.Update();

            EditorGUILayout.ObjectField(propSceneSelected, typeof(SceneAsset));

            so.ApplyModifiedProperties();

            so.Update();
            if(scenes.Contains(sceneSelected))
            {
                if (GUILayout.Button("Remove"))
                {
                    int index = scenes.FindIndex(0, scenes.Count, (SceneAsset match) => { return match == sceneSelected; } );
                    scenes.RemoveAt(index);
                    //propScenePaths.DeleteArrayElementAtIndex(index);
                }
            }
            else if(sceneSelected != null) 
            {
                if (GUILayout.Button("Add"))
                {
                    scenes.Add(sceneSelected);

                    /*propScenePaths.InsertArrayElementAtIndex(scenes.Count - 1);
                    SerializedProperty path = propScenePaths.GetArrayElementAtIndex(scenes.Count - 1);
                    path.stringValue = AssetDatabase.GetAssetPath(sceneSelected);*/

                }
            }
            so.ApplyModifiedProperties();
            

            EditorGUILayout.EndVertical();

            for (int i = 0; i < scenes.Count; i++)
            {
                SceneAsset currentScene = scenes[i];

                if (currentScene != null)
                {
                    string sceneName = currentScene.name;

                    EditorGUILayout.BeginHorizontal();

                    so.Update();
                    
                    if (GUILayout.Button("↓", GUILayout.Width(20)))
                    {
                        if (i != scenes.Count - 1)
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
            for (int i = 0;i < scenes.Count;i++)
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
            string path = Application.persistentDataPath + "/lugu_scene_loader_save.json";
            Debug.Log(path);
            if (File.Exists(path))
            {
                SceneLoaderData data = JsonUtility.FromJson(path, typeof(SceneLoaderData)) as SceneLoaderData;

                for (int i = 0; i < data.scenePaths.Length; i++)
                {
                    SceneAsset sceneToAdd = AssetDatabase.LoadAssetAtPath(data.scenePaths[i], typeof(SceneAsset)) as SceneAsset;
                    scenes.Add(sceneToAdd);
                }
            }

        }
        

        public void Save()
        {
            string path = Application.persistentDataPath + "/lugu_scene_loader_save.json";
            
            SceneLoaderData data = new SceneLoaderData();
            data.scenePaths = new string[scenes.Count];

            for (int i = 0; i < propScenePaths.arraySize; i++)
            {
                data.scenePaths[i] = propScenePaths.GetArrayElementAtIndex(i).stringValue;
            }
            
            File.WriteAllText(path, JsonUtility.ToJson(data));
        }
    }
}
