![image alt text](image_0.png)

Animation Toolset 

* * *


Document Version 0.5

Last Published : June 23rd, 2020

# About this guide

You’ll find information about how to use the Bitmoji low-res Mobile Rig, including some control functions and best practices.  At the bottom of this doc is a list of some files provided relating to animation,  and some basic instructions on how to get your animation from Maya into game engines.

# Using the Bitmoji Mobile Rig

**Changing Expressions:**

1. Select the "C_head_CON" controller on the rig

2. Scroll through the "Face Shapes" attribute in the Channel Box

3. Set keyframes for your animation

	

![image alt text](image_1.gif)

**Changing Characters:**

1. Select the "C_visibility_CON" controller on the rig

2. Select the preferred character in the "Character" attribute found in the Channel Box

![image alt text](image_2.gif)

**Using the Proxy Geo:**

1. Select the "C_visibility_CON" controller on the rig

2. Adjust the "Proxy Transparency" attribute found in the Channel Box

    1. This is a visual representation of the different volumes Bitmoji styles can include. Being aware of the proxy volume while animating will help to avoid interpenetration issues when switching different Bitmoji styles and body customizations.

![image alt text](image_3.gif)

**Best Practices:**

Posing around the heavy proxy will make animations compatible with all body types, with minimal interpenetrations.  If animations will be viewed from a locked camera they can be cheated to look good from that specific angle.  This can also allow "faked" contact points on the body that otherwise won’t work due to differing body sizes

 POSE ON PROXY                                           CHEAT TO LOCKED CAMERA![image alt text](image_4.gif)                      ![image alt text](image_5.gif)

**Using the Skirt Proxy:**

1. Select the "C_visibility_CON" controller on the rig

2. Turn the skirt proxy off/on with the Skirt attribute found in the Channel Box

    1. This is a visual representation of a skirt that a player’s Bitmoji could be wearing.  Skirts are closed on the bottom so nothing can be seen underneath, but using this proxy can show what animations could look like in skirts or dresses.

![image alt text](image_6.gif)

**Best Practices:**

Use the Skirt proxy to get an idea of how poses will look in skirts (the bottom of all avatar skirts are closed off).  Briefly seeing the underside of the skirt is fine, but avoid prolonged views/poses when possible.

 FINE                                                                                   AVOID                       ![image alt text](image_7.gif)              ![image alt text](image_8.gif)

# Files Provided

**avatar**

-snap_mobile_rig.ma 

-snap_mobile_bind_skeleton_v01.fbx

**scripts for Maya**

-mobile_bake.py 

**scripts for Unity**

-MobileGLBLoader.cs

-MobileFacialAnimationEvent.cs

-MobileFacialAnim2FBX.cs

-GLTFUtility-master.zip

-Newtonsoft.Json-for-Unity-master.zip

**converters**

-FBX2glTF-windows-x64.exe

-FBX2glTF-darwin-x64

# Mobile Rig Animation into Unity

**1)  **Use latest **snap_mobile_rig.ma**​ for animation in Maya.  Or use our **snap_mobile_bind_skeleton.fbx** for custom rigging (but following scripts may not be compatible)

**2) ** Create animation for body and face

**3)**  In Maya run the **mobile_bake.py** script to bake the animation and export an .fbx with body animation and a .json with face texture animation data.  After the baking process some group nodes will be deleted but the joint hierarchy will stay the same

**4)  **Bring .fbx and .json into your Unity Project

**5)  **Make sure "**MobileFacialAnim2FBX.cs**" is loaded in Unity, then use “**Mobile: Facial Animation Generator**” to add face animation data as animation events in fbx. [(go to Add Facial Animation section of the Avatar Setup in Unity doc for more details)](https://docs.google.com/document/d/1BcUCn2HWBcxOG-wtaF9on-INb-v4GDdTSoUv0X4cGfQ/edit#)

**6)**  Connect the animation to your Animator Controller, then apply it to the Animator that is added to the "AVATAR" node of the loaded glb

# ______________________________________________

# Providing Feedback			

We would love to hear your thoughts on our product! Please visit this form so that you can submit your suggestions and let us know how we can better improve your Bitmoji Gaming Experience! [Feedback Form](https://forms.gle/48xjwZPUazYGrBZu5) 

	

