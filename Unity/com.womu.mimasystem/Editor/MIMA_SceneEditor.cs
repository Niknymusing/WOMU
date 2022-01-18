using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MIMA
{
    [CustomEditor(typeof(MIMA_Scene), true)]
    public class MIMA_SceneEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            MIMA_Scene sceneSettings = target as MIMA_Scene;
            SceneAsset scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(sceneSettings._scenePath);

            if (scene == null)
            {
                scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(AssetDatabase.GUIDToAssetPath(sceneSettings._sceneGUID));
            }

            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            scene = EditorGUILayout.ObjectField("Scene", scene, typeof(SceneAsset), false) as SceneAsset;

            if (EditorGUI.EndChangeCheck())
            {
                string newPath = AssetDatabase.GetAssetPath(scene);
                SerializedProperty scenePathProperty = serializedObject.FindProperty("_scenePath");
                scenePathProperty.stringValue = newPath;
                string newGUID = AssetDatabase.AssetPathToGUID(newPath);
                SerializedProperty sceneGUIDProperty = serializedObject.FindProperty("_sceneGUID");
                sceneGUIDProperty.stringValue = newGUID;
                SerializedProperty sceneNameProperty = serializedObject.FindProperty("_sceneName");
                sceneNameProperty.stringValue = scene.name;
                Debug.Log("Set scene name property to " + sceneNameProperty.stringValue);
            }
            serializedObject.ApplyModifiedProperties();
        }
    } 
    
    
}