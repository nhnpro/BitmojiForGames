<p align="center">
<img src="../../Shared/Logo.png" width="450"/>
</p>

# Avatar Setup in Unity

**Bitmoji for Games Avatar Setup in Unity**

Copyright © 2019-2020 Snap Inc. All rights reserved. All information subject to change without notice.

[Snap Developer Terms of Service](https://kit.snapchat.com/portal/eula?viewOnly=true)

[Snap Inc. Terms of Service](https://www.bitmoji.com/support/terms.html)

# About this guide

Below is a recommended setup of our Bitmoji Rig avatars for both facial animation and body animation in Unity.  Information about the Animator Controller setups and use of props in the sample project are provided for demonstration purposes.  Developers are also encouraged to create their own Animator Controllers and scripts that work with their games.

# Plugins & Scripts

1. GLBLoader.cs *(to load glb avatar)* 

1. FacialAnimationConverter.cs *(to add face data)*

1. MobileFacialAnimationEvent.cs *(for face texture animation)*

1. CharacterControl.cs *(to control character animation in sample project, optional)*

1. GLTFUtility https://github.com/Siccity/GLTFUtility

1. Newtonsoft.Json-for-Unity https://github.com/jilleJr/Newtonsoft.Json-for-Unity

# Sample Project

Find the sample project that was setup with our mobile & console avatar and the scripts above. A sample animator controller which uses some animations from our animation library is also included.


<p align="center">
<img src="images/image11.png" />
</p>

# Facial Animation Preparation for Mobile

1. Select “Bitmoji For Games: Facial Animation Converter” from the “Window” menu to open the “Facial Animation Converter” window. 

    <p align="center">
    <img src="images/image9.png" />
    </p>

1. For each animation imported to Unity, add json and corresponding fbx file to input fields in the mobile section, then press “Mobile: Convert facial animation” to add face animation data as animation events in fbx. 

    <p align="center">
    <img src="images/image1.png" />
    <img src="images/image6.jpg" />
    </p>

1. After face animation data is transferred, delete json files *(optional)*

1. After following Avatar Setup steps below, facial animation and body animation should work together as expected. 


# Facial Animation Preparation for Console

1. Select “Bitmoji For Games: Facial Animation Converter” from the “Window” menu to open the “Facial Animation Converter” window.     
<p align="center">
<img src="images/image9.png" />
</p>

1. For each animation imported to Unity, add the json file and output path to input fields in the console section, then press “Console: Convert facial animation” to generate face animation clips.

    <p align="center">
    <img src="images/image1.png" />
    <img src="images/image3.png" />
    </p>

1. After face animation data is generated, delete json files *(optional)*

1. Prepare Animator Controller with body animation clips. *(you can find sample in attached project)*

    <p align="center">
    <img src="images/image12.png" />
    </p>

1. As shown above, change the name of the existing animation layer to “Body Layer” *(optional)* and then add a new layer called “Face Layer”. Press the gear icon for options, set weight value to 1 and check the “sync” option then an animation tree cloned from “Body Layer” will appear on “Face Layer”. 

1. In Face Layer, swap body animation clip with corresponding facial animation clip in motion slots. Make sure the animation tree in “Facel Layer” is identical to the one in “Body Layer”.*(If there is no blend tree in specific animation states, create a blend tree and connect facial animation clips in the same orders and values like “Body Layer”. Make sure all parameters used for blend trees in the new layer are the same.)*

    <p align="center">
    <img src="images/image5.png" />
    </p>

1. After following Avatar Setup steps below, facial animation and body animation should work together as expected. 

# Avatar Setup

1. Create GameObject node and connect “GLBLoader.cs”

1. Connect mobile character GLB file to “Mobile Avatar Glb Object”; connect console character GLB file to “Console Avatar Glb Object”

    <p align="center">
    <img src="images/image7.png" />
    </p>

1. Connect Animator Controller for mobile  to “Mobile Avatar Animator”; connect Animator Controller for console to “Console Avatar Animator”     

1. Turn on “Use Mobile” option for animation demo with mobile avatar; leave it unchecked for animation demo with console avatar.

1. As shown, press Play and “GLBLoader.cs” will:
    a. Load the avatar GLB file as “RootNode” 
    a. Connect “MobileFacialAnimationEvent.cs” to “AVATAR” under “RootNode” (only for mobile, this will allow facial texture animation in later steps)
    a. Connect “CharacterControl.cs” and add an Animator with a Character Controller to “AVATAR” under “RootNode” (This is the provided control scheme for our Sample Project.  However, developers are encouraged to create customized character control scripts, Animator Controller and other Unity components)
 
    <p align="center">
    <img src="images/image10.png" />
    </p> 
 
 
# Animator Controller Setup in Sample Project

“ConsoleAnimatorController” and “MobileAnimatorController” are sample animator controllers that work with “CharacterControl.cs” to demonstrate the animation library provided and serve as a reference for simple animation setups.

<p align="center">
<img src="images/image8.png" />
</p>

Keyboard controls for Sample Project: 

[Character movements] 

* Idle animation will be playing by default when no key is pressed;
* Press and hold “w” to move character forward and play walk cycle animation;
* Press and hold “Shift” with “w” to move character forward faster and play run cycle animation;
* Press and hold  “Tab” to play variations of animations above;
* Press and hold “Space” to make the character jump and down, jump animations will be playing.
* Additionally, hold on “a” or “d” to turn character left or right while moving;

[Other sample animations from Animation Library]

* Press and hold “1” to play winning animation;
* Press and hold “2” to play losing animation;
* Press and hold “3” to play throwing animation; *(to add prop see upcoming setups)*
* Additionally, hold on “Tab” to play variations of animations above.

# Prop Setup in Sample Project

This is one possible setup for throwing a prop. Functions used are “ThrowAction_temp()” and “ThrowReset_temp()” in “CharacterControl.cs”. 

1. Adding RigidBody and Capsule Collider to the prop. *(“ketchup” in this setup)* 

    <p align="center">
    <img src="images/image2.png" />
    </p>

1. Add 3 events to the throw_small_mobile at the beginning, middle and end of the clip, calling functions “ThrowReset_temp()”, “ThrowAction_temp()”, “ThrowReset_temp()” as shown.

    <p align="center">
    <img src="images/image14.png" />
    </p>

1. When the glb avatar is loaded, add ketchup and the right hand joint to the inputs of “CharacterControl” in AVATAR.

    <p align="center">
    <img src="images/image4.jpg" />
    </p>

1. Press key “3”, the avatar will throw out a ketchup bottle.

    <p align="center">
    <img src="images/image15.gif" />
    </p>


# Providing Feedback			

We would love to hear your thoughts on our product! Please visit this form so that you can submit your suggestions and let us know how we can better improve your Bitmoji Gaming Experience! [Feedback Form](https://forms.gle/48xjwZPUazYGrBZu5) 
