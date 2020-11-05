//FacialAnimConverter.cs v0.2
//Universal facial anim converter for both Console/Mobile
//Use single float when creating animation clip for Console
//Enable address updates from text input
﻿using System.IO;
using TMPro;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Object = UnityEngine.Object;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using Newtonsoft.Json.Linq;

public class FacialAnimConverter : EditorWindow
{
    //var for mobile
    GameObject m_animationclip;
    TextAsset objectType;
    UnityEngine.Object[] m_data;
    string m_jsonString;
    //var for console
    TextAsset JSONObject;
    UnityEngine.Object[] c_data;
    string outputPath = "Assets/BitmojiForGamesSample/AnimationLibrary/Console/BodyAnimations/";//
    string path;
    string c_jsonString;
    public string FacialAnimSufix = "_facial_anim.anim";

    [MenuItem("Window/Bitmoji For Games:Facial Animation Converter")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(FacialAnimConverter));
    }

    void OnGUI()
    {
        //menu section: mobile
        GUILayout.Label("", EditorStyles.boldLabel);
        GUILayout.Label("Mobile: Convert facial animation json to animation events", EditorStyles.boldLabel);
        objectType = EditorGUILayout.ObjectField("Facial Animation (json)", objectType, typeof(TextAsset), false) as TextAsset;
        m_animationclip = EditorGUILayout.ObjectField("Animation File (fbx)", m_animationclip, typeof(GameObject), false) as GameObject;

        if (GUILayout.Button("Mobile: Convert facial animation"))
        {
            // get animation path
            string animPath = AssetDatabase.GetAssetPath(m_animationclip);
            ModelImporter modelImporter = (ModelImporter)AssetImporter.GetAtPath(animPath);
            InitEvent(modelImporter);
            // animation properties
            SerializedObject so = new SerializedObject(modelImporter);
            SerializedProperty clips = so.FindProperty("m_ClipAnimations");
            // list for facial animation
            List<AnimationEvent[]> animationEvents = new List<AnimationEvent[]>(modelImporter.clipAnimations.Length);

            for (int i = 0; i < modelImporter.clipAnimations.Length; i++)
            {
                animationEvents.Add(GetEvents(clips.GetArrayElementAtIndex(i)));
            }
            for (int i = 0; i < modelImporter.clipAnimations.Length; i++)
            {
                SetEvents(clips.GetArrayElementAtIndex(i), animationEvents[i]);
            }
            so.ApplyModifiedProperties();
        }

        //menu section: console
        GUILayout.Label("", EditorStyles.boldLabel);
        GUILayout.Label("Console: Convert facial animation json to facial animation clips", EditorStyles.boldLabel);
        JSONObject = EditorGUILayout.ObjectField("Facial animation (json)", JSONObject, typeof(TextAsset), false) as TextAsset;
        outputPath = EditorGUILayout.TextField("Save to ", outputPath);//enbaled updates from text input

        if (GUILayout.Button("Console: Convert facial animation"))
        {
            CreateAnimationClip();
        }
    }

    public void InitEvent(ModelImporter modelImporter)
    {
        List<AnimationEvent> animationEvents = new List<AnimationEvent>();
        AnimationEvent evt = new AnimationEvent();
        evt.time = 0.5f;
        evt.functionName = "InitEvent";
        animationEvents.Add(evt);
        ModelImporterClipAnimation[] clips = modelImporter.defaultClipAnimations;
        clips[0].events = animationEvents.ToArray();
        modelImporter.clipAnimations = clips;
    }

    public AnimationEvent[] GetEvents(SerializedProperty sp)
    {
        // JSON data for facial animation
        string assetPath = AssetDatabase.GetAssetPath(objectType);
        m_jsonString = File.ReadAllText(assetPath);
        var m_data = JObject.Parse(m_jsonString);
        // animation path
        string animPath = AssetDatabase.GetAssetPath(m_animationclip);
        ModelImporter modelImporter = (ModelImporter)AssetImporter.GetAtPath(animPath);
        AnimationClip animationClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(animPath);
        float frameRate = animationClip.frameRate;
        // first animation clip
        ModelImporterClipAnimation clip = modelImporter.clipAnimations[0];
        float animFrameLength = (clip.lastFrame - clip.firstFrame);
        // search event property
        SerializedProperty serializedProperty = sp.FindPropertyRelative("events");
        AnimationEvent[] array = null;
        int KeyNum = (int)m_data["frames"].Count();
        array = new AnimationEvent[KeyNum];
        // facial animation to list
        int i = 0;
        foreach (var record in m_data["frames"])
        {
            AnimationEvent animationEvent = new AnimationEvent();
            animationEvent.functionName = "AnimationEventHandler";
            animationEvent.time = ((float)record / animFrameLength);
            animationEvent.stringParameter = (string)m_data["expressions"][i];
            Debug.Log(m_data["expressions"][i]);
            array[i] = animationEvent;
            i++;
        }
        return array;
    }

    // add event
    public void SetEvents(SerializedProperty sp, AnimationEvent[] newEvents)
    {
        SerializedProperty serializedProperty = sp.FindPropertyRelative("events");
        if (serializedProperty != null && serializedProperty.isArray && newEvents != null && newEvents.Length > 0)
        {
            serializedProperty.ClearArray();
            for (int i = 0; i < newEvents.Length; i++)
            {
                AnimationEvent animationEvent = newEvents[i];
                serializedProperty.InsertArrayElementAtIndex(serializedProperty.arraySize);
                SerializedProperty eventProperty = serializedProperty.GetArrayElementAtIndex(i);
                eventProperty.FindPropertyRelative("functionName").stringValue = animationEvent.functionName;
                eventProperty.FindPropertyRelative("data").stringValue = animationEvent.stringParameter;
                eventProperty.FindPropertyRelative("time").floatValue = animationEvent.time;
                eventProperty.FindPropertyRelative("intParameter").intValue = animationEvent.intParameter;
                eventProperty.FindPropertyRelative("objectReferenceParameter").objectReferenceValue = animationEvent.objectReferenceParameter;
            }
        }
    }

    public void CreateAnimationClip()
    {
        // designed for 30 frame/second animation
        AnimationClip clip = new AnimationClip();
        clip.frameRate = 30;
        // get full path of facial animation file
        string assetPath = AssetDatabase.GetAssetPath(JSONObject);
        var fileName = Path.GetFileNameWithoutExtension(assetPath);
        c_jsonString = File.ReadAllText(assetPath);
        var c_data = JObject.Parse(c_jsonString);
        // int i = 0;
        foreach (var kv in c_data)
        {
            var BlendShapeNodeName = kv.Key;
            var frameNum = kv.Value["frames"];
            Keyframe[] keys;
            keys = new Keyframe[(int)frameNum.Count()];
            // read each of blendshape values
            for (int item = 0; item < frameNum.Count(); item++)
            {
                // get facial animation data from json
                AnimationCurve curve;
                Type type = typeof(float);
                Type stringType = typeof(string);
                var f1 = System.Convert.ChangeType(kv.Value["frames"][item].ToString(), type);
                var v1 = System.Convert.ChangeType(kv.Value["value"][item].ToString(), type);
                var p1 = System.Convert.ChangeType(kv.Value["path"].ToString(), stringType);
                int FrameN = (int)Convert.ChangeType(f1, TypeCode.Int32);
                float flt1 = (float)FrameN;
                float Val = (float)Convert.ChangeType(v1, TypeCode.Single); //value changed to single float here
                // switch to unity time scale
                keys[item] = new Keyframe(flt1 / 30, Val);
                curve = new AnimationCurve(keys);
                string path = Convert.ToString(kv.Value["path"]);
                string attributeNode = "blendShape.";
                string attribute = attributeNode + BlendShapeNodeName;
                var binding = EditorCurveBinding.FloatCurve(path, typeof(SkinnedMeshRenderer), attribute);
                clip.SetCurve(path, typeof(SkinnedMeshRenderer), attribute, curve);
                AnimationUtility.SetEditorCurve(clip, binding, curve);
            }
        }
        // create facial animation clip
        AssetDatabase.CreateAsset(clip, outputPath + fileName + FacialAnimSufix);
    }
}
