
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace lugu.SceneLoader
{
    public class SceneLoader : EditorWindow
    {
        public List<SceneAsset> scenes = new List<SceneAsset>();
        public SceneAsset sceneSelected;

        private SerializedObject so;
        private SerializedProperty propScenes;
        private SerializedProperty propSceneSelected;


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
        }

        private void OnGUI()
        {
            GUIStyle style = EditorStyles.textField;

            EditorGUILayout.BeginVertical(style);

            so.Update();

            EditorGUILayout.ObjectField(propSceneSelected, typeof(SceneAsset));

            so.ApplyModifiedProperties();

            if(scenes.Contains(sceneSelected))
            {
                if (GUILayout.Button("Remove"))
                {
                    scenes.Remove(sceneSelected);
                }
            }
            else if(sceneSelected != null) 
            {
                if (GUILayout.Button("Add"))
                {
                    scenes.Add(sceneSelected);
                }
            }
            

            EditorGUILayout.EndVertical();

            for (int i = 0; i < scenes.Count; i++)
            {
                SceneAsset currentScene = scenes[i];

                if (currentScene != null)
                {
                    string sceneName = currentScene.name;

                    EditorGUILayout.BeginHorizontal();

                    so.Update();
                    if(i != scenes.Count-1)
                        if (GUILayout.Button("↓", GUILayout.Width(20)))
                        {
                             propScenes.MoveArrayElement(i, i+1);
                        }

                    if (i != 0)
                        if (GUILayout.Button("↑", GUILayout.Width(20)))
                        {
                            propScenes.MoveArrayElement(i, i - 1);
                        }
                    

                    if (GUILayout.Button(sceneName))
                    {
                        var scenePath = AssetDatabase.GetAssetPath(scenes[i]);
                        EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                        
                    }
                    EditorGUILayout.EndHorizontal();
                }

            }

            so.ApplyModifiedProperties();
        }
    }
}
