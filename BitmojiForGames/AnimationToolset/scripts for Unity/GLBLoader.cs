//GLBLoader.cs v0.2
//combine Mobile&Console GLB loaders
using System.Collections;
using System.Collections.Generic;
using Siccity.GLTFUtility;
using System;
using System.Text;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Scripting;

namespace GLBUtil
{
    public class GLBLoader : MonoBehaviour
    {
        public bool useMobile = false;//option to use mobile or console
        public string characterControlScript = "CharacterControl";
        public string avatarPath = "/RootNode/AVATAR";

        private GameObject avatar_glbObject;
        private RuntimeAnimatorController avatar_animatorController;
        private string head_path = null;

        [System.Serializable]
        public struct colliders
        {
          public float collider_center_X;
          public float collider_center_Y;
          public float collider_center_Z;
          public float collider_radius;
          public float collider_height;
        }
        public GLBLoader.colliders colliderSize = new GLBLoader.colliders
        {
          collider_center_X = 0f,
          collider_center_Y = 0.78f,
          collider_center_Z = 0f,
          collider_radius = 0.41f,
          collider_height = 1.42f
        };

        [Header("Mobile")]
        public GameObject mobileAvatarGlbObject;
        public RuntimeAnimatorController mobileAvatarAnimatorController;
        public string facilAnimEventScript = "MobileFacialAnimationEvent";

        private string jsonString;
        private float animFrameLength;
        private string splitArray;

        [Header("Console")]
        public GameObject consoleAvatarGlbObject;
        public RuntimeAnimatorController consoleAvatarAnimatorController;

        private CharacterController m_Controller; 
        private Animator animator;
        private string glbPath;

        void Start()
        {
            PlatformSwitch();
            glbPath = AssetDatabase.GetAssetPath(avatar_glbObject);
            ImportGLTF(glbPath);
            AnimationSetup();
        }
        void PlatformSwitch()
        {
          if (useMobile)
          {
            avatar_glbObject = mobileAvatarGlbObject;
            avatar_animatorController = mobileAvatarAnimatorController;
            head_path = avatarPath + "/C_head_GEO";
          }
          else
          {
            avatar_glbObject = consoleAvatarGlbObject;
            avatar_animatorController = consoleAvatarAnimatorController;
          }
        }
        void ImportGLTF(string path)
        {
            // search head geometry
            GameObject result = Importer.LoadFromFile(path);
            GameObject AVATAR_object = GameObject.Find(avatarPath);
            System.Type ControllerScript = System.Type.GetType(characterControlScript + ",Assembly-CSharp");
            AVATAR_object.AddComponent(ControllerScript);
            if (useMobile)
            {
              System.Type MyScriptType = System.Type.GetType(facilAnimEventScript + ",Assembly-CSharp");
              AVATAR_object.AddComponent(MyScriptType);
              MobileFacialAnimationEvent.glb_Object = glbPath;
              MobileFacialAnimationEvent.head_path = head_path;
            }
        }

        void AnimationSetup()
        {
            // search AVATAR node and add animator
            GameObject avatar = GameObject.Find(avatarPath);
            avatar.AddComponent<CharacterController>();
            animator = avatar.GetComponent<Animator>();
            m_Controller = avatar.GetComponent<CharacterController>();
            m_Controller.center = new Vector3(colliderSize.collider_center_X, colliderSize.collider_center_Y, colliderSize.collider_center_Z);
            m_Controller.height = colliderSize.collider_height;
            m_Controller.radius = colliderSize.collider_radius;
            // attach camera behind character
            GameObject emptyObj = new GameObject("Camera");
            emptyObj.transform.parent = avatar.gameObject.transform;
            emptyObj.AddComponent<Camera>();
            Vector3 movePosition = new Vector3(0f, 1.3f, -1.95f);
            emptyObj.transform.position = movePosition;
            animator.GetComponent<Animator>().runtimeAnimatorController = avatar_animatorController as RuntimeAnimatorController;
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
}
