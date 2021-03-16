<p align="center">
<img src="../Shared/Logo.png" width="450"/>
</p>

# Animation Toolset

**Bitmoji for Games Animation Toolset**

Copyright © 2019-2020 Snap Inc. All rights reserved. All information subject to change without notice.

[Snap Developer Terms of Service](https://kit.snapchat.com/portal/eula?viewOnly=true)

[Snap Inc. Terms of Service](https://www.bitmoji.com/support/terms.html)

# About this guide

You’ll find information about how to use the Bitmoji low-res Mobile Rig & hi-res Console Rig, including some control functions and best practices. At the bottom of this document is a list of some files provided relating to animation, and some basic instructions on how to get your animation from Maya into game engines.

# Using the Bitmoji Rig

**Changing Expressions in Mobile Rig:**

1. Select the `C_head_CON` controller on the rig

1. Scroll through the "Face Shapes" attribute in the Channel Box

1. Set keyframes for your animation
<p align="center">
<img src="images/lod3_v06_face.gif" />
</p>

**Face Controls in Console Rig:**

1. The face rig has a user-friendly setup, with mostly intuitive functions

1. The triangles on the left/right side of the mouth can round off or pinch the mouth corners

1. The big, yellow oval has an option in the channel box to turn on/off teeth and tongue controls

<p align="center">
<img src="images/lod0_face.gif" />
</p>


**Changing Characters in Mobile Rig:**

1. Select the `C_visibility_CON` controller on the rig

1. Select the preferred character in the `Character` attribute found in the Channel Box

<p align="center">
<img src="images/lod3_v06_character_swap.gif" />
</p>

**Using the Proxy Geo:**

1. Select the `C_visibility_CON` controller on the rig

1. Adjust the `Proxy Transparency` attribute found in the Channel Box

    - This is a visual representation of the different volumes Bitmoji styles can include. Being aware of the proxy volume while animating will help to avoid interpenetration issues when switching different Bitmoji styles and body customizations.

<table align="center">
    <tr>
        <td>
            <img src="images/lod3_v06_proxy_on_off.gif" width="400"/>
        </td>
        <td>
            <img src="images/lod0_proxy.gif" width="400"/>
        </td>
    </tr>
</table>


**Best Practices:**

Posing around the heavy proxy will make animations compatible with all body types, with minimal interpenetrations.  If animations will be viewed from a locked camera they can be cheated to look good from that specific angle.  This can also allow "faked" contact points on the body that otherwise won’t work due to differing body sizes.

<table align="center">
    <tr>
        <th>POSE ON PROXY</th>
        <th>CHEAT TO LOCKED CAMERA!</th>
    </tr>
    <tr>
        <td>
            <img src="images/lod3_v06_proxy_posing.gif" width="400"/>
        </td>
        <td>
            <img src="images/lod3_v06_proxy_cheating.gif" width="400"/>
        </td>
    </tr>
</table>

**Using the Skirt Proxy:**

1. Select the `C_visibility_CON` controller on the rig

1. Turn the skirt proxy off/on with the Skirt attribute found in the Channel Box

    - This is a visual representation of a skirt that a player’s Bitmoji could be wearing.  Skirts are closed on the bottom so nothing can be seen underneath, but using this proxy can show what animations could look like in skirts or dresses.

<table align="center">
    <tr>
        <td>
            <img src="images/lod3_v06_skirt_on_off.gif" width="400"/>
        </td>
        <td>
            <img src="images/lod0_skirt.gif" width="400"/>
        </td>
    </tr>
</table>

**Best Practices:**

Use the Skirt proxy to get an idea of how poses will look in skirts (the bottom of all avatar skirts are closed off).  Briefly seeing the underside of the skirt is fine, but avoid prolonged views/poses when possible.

<table align="center">
    <tr>
        <th>FINE</th>
        <th>AVOID</th>
    </tr>
    <tr>
        <td>
            <img src="images/lod3_v06_skirt_fine.gif" width="400"/>
        </td>
        <td>
            <img src="images/lod3_v06_skirt_avoid.gif" width="400"/>
        </td>
    </tr>
</table>

# Files Provided

**avatar**

- snap_mobile_rig.ma

- snap_console_rig.ma

- snap_console_bind_skeleton.fbx

- snap_mobile_bind_skeleton.fbx

**scripts for Maya**

- snap_bake.py 

**scripts for Unity**

- GLBLoader.cs

- MobileFacialAnimationEvent.cs

- FacialAnimConverter.cs

**third party scripts for Unity**

- GLTFUtility-master.zip

- Newtonsoft.Json-for-Unity-master.zip

**converters**

- FBX2glTF-windows-x64.exe

- FBX2glTF-darwin-x64

# Bitmoji Rig Animation into Unity

1. Use the latest **snap_mobile_rig.ma**​ or **snap_console_rig.ma** for animation in Maya.  Or use our **snap_mobile_bind_skeleton.fbx** or **snap_console_bind_skeleton.fbx** for custom rigging (but the following scripts may not be compatible)

1. Create animation for body and face

1. In Maya run the **snap_bake.py** script to bake the animation and export an `.fbx` with body animation and a `.json` with face animation data. After the baking process some group nodes will be deleted but the joint hierarchy will stay the same.

1. Bring `.fbx` and `.json` into your Unity Project.

1. Make sure **FacialAnimConverter.cs** is loaded in Unity, then use **Bitmoji For Games: Facial Animation Converter** to add face animation data. (Go to **Add Facial Animation** section of [Avatar Setup in Unity](https://github.com/Bitmoji/Bitmoji-3D/) doc for more details)

1. Connect the animation to your Animator Controller, then apply it to the Animator that is added to the `AVATAR` node of the loaded glb.

# Providing Feedback			

We would love to hear your thoughts on our product! Please visit this form so that you can submit your suggestions and let us know how we can better improve your Bitmoji Gaming Experience! [Feedback Form](https://forms.gle/48xjwZPUazYGrBZu5) 
