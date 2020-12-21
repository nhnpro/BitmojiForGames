// MobileFacialAnimationEvent.cs v0.2
// -will load up face textures by name
using UnityEngine;
using System;
using System.Text;
using System.IO;
using System.Linq;
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
        string json = GetGLBJson4Face(stream, out binChunkStart);
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
                var head_object = this.transform.Find(head_path);                
                head_object.GetComponent<Renderer>().material.mainTexture = tex;
            }
        }
    }

    // read animation event and trigger facial texture animation.
    public void AnimationEventHandler(AnimationEvent animationEvent)
    {
        // Get animation event paramenter
        string stringParm = animationEvent.stringParameter;
        // facial texture animation function
        FacialTextureJson2JPG(stringParm);
    }

    public static string GetGLBJson4Face(Stream stream, out long binChunkStart)
    {
        byte[] buffer = new byte[12];
        stream.Read(buffer, 0, 12);
        // 12 byte header
        // 0-4  - magic = "glTF"
        // 4-8  - version = 2
        // 8-12 - length = total length of glb, including Header and all Chunks, in bytes.
        string magic = Encoding.Default.GetString(buffer, 0, 4);
        if (magic != "glTF")
        {
            Debug.LogWarning("Input does not look like a .glb file");
            binChunkStart = 0;
            return null;
        }
        uint version = System.BitConverter.ToUInt32(buffer, 4);
        if (version != 2)
        {
            Debug.LogWarning("Importer does not support gltf version " + version);
            binChunkStart = 0;
            return null;
        }
        // Chunk 0 (json)
        // 0-4  - chunkLength = total length of the chunkData
        // 4-8  - chunkType = "JSON"
        // 8-[chunkLength+8] - chunkData = json data.
        stream.Read(buffer, 0, 8);
        uint chunkLength = System.BitConverter.ToUInt32(buffer, 0);
        TextReader reader = new StreamReader(stream);
        char[] jsonChars = new char[chunkLength];
        reader.Read(jsonChars, 0, (int)chunkLength);
        string json = new string(jsonChars);

        // Chunk
        binChunkStart = chunkLength + 20;
        stream.Close();

        // Return json
        return json;
    }

}
