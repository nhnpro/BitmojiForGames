// MobileFacialAnimationEvent.cs v0.2
// -will load up face textures by name
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Siccity.GLTFUtility;
using UnityEditor;
using System;
using System.Text;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.Scripting;
using GLBUtil;
using Newtonsoft.Json.Linq;

public class MobileFacialAnimationEvent : MonoBehaviour
{
    public static MobileFacialAnimationEvent main;
    // receive value from LOD3GLBLoader. ImportGLTF
    public static string glb_Object;
    // receive value from LOD3GLBLoader. ImportGLTF
    public static string head_path;
    string jsonString;
    void Start()
    {
    }

    // read jpg from glb and switch character facial texture.
    public void FacialTextureJson2JPG(string FacialNum)
    {
        // read glb
        FileStream stream = File.OpenRead(glb_Object);
        long binChunkStart;
        string json = GLBLoader.GetGLBJson4Face(stream, out binChunkStart);
        // set texture
        Texture2D tex = new Texture2D(2, 2);
        // glb file parsing
        var data = JObject.Parse(json);
        // pull out face jpg info
        var faceTex = data["extras"]["facialExpressionTextures"];
        for (int i = 0; i < faceTex.Count(); i++)
        {
            //string faceTexName = (string)faceTex[i]["name"];
            if (FacialNum == (string)faceTex[i]["name"])
            {
                string versionString = (string)faceTex[i]["uri"];
                string face_tex_encoded = versionString.Split(',').Last();
                // encode JPG
                byte[] decodedBytes = Convert.FromBase64String(face_tex_encoded);
                tex.LoadImage(decodedBytes);
                // switch face texture as animated.
                GameObject head_object = GameObject.Find(head_path);
                head_object.GetComponent<Renderer>().material.mainTexture = tex;
            }
        }
    }

    // read animation event and trigger facial texture animation.
    public void AnimationEventHandler(AnimationEvent animationEvent)
    {
        // Get animation event paramenter
        string stringParm = animationEvent.stringParameter;
        float floatParam = animationEvent.floatParameter;
        int intParam = animationEvent.intParameter;
        // facial texture animation function
        FacialTextureJson2JPG(stringParm);
    }
    void Update()
    {
    }
}
