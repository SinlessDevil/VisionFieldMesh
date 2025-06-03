Cartoon FX Remaster, version R 1.2
2023/01/25
© 2012-2023 - Jean Moreno
==================================

! NOTE ABOUT URP !
------------------
If almost all effects don't show in URP, it probably is because the Depth Texture is disabled.
This texture is needed for soft particles to work, and they are enabled by default.
You can either:
- enable the Depth Texture in the URP asset
- disable soft particles globally by uncommenting the relevant line in the CFXR_Settings.cginc file


ABOUT
-----
"Cartoon FX Remaster" is an update to the old "Cartoon FX Pack 1" asset.
All effects have been remade from scratch using:
- new shaders with special effects such as dissolve animation, UV distortion, edge fading, HDR colors, single channel textures for reduced memory usage, shadow casting and dithering
- optimized meshes where relevant to reduce overdraw/fill-rate issues
- high-resolution hand-drawn textures
- specialized shader to draw perfect circles and rings while reducing overdraw (using a ring mesh instead of a quad)

"Cartoon FX Remaster" supports the built-in render pipeline and URP.


PREFABS
-------
Particle Systems prefabs are located in "Cartoon FX Remaster/CFXR Prefabs" folder.
All prefabs and their assets have a CFXR_ prefix to easily recognize them.


LEGACY EFFECTS
--------------
All the old effects from "Cartoon FX Pack 1" are still available in the "Cartoon FX (legacy)" folder.


MOBILE OPTIMIZED PREFABS?
-------------------------
Unlike the Legacy effects, "Cartoon FX Remaster" doesn't include mobile-specific prefabs.
This is because:
- mobile devices are much more powerful compared to Cartoon FX Pack 1's initial release in 2012
- prefabs are all already optimized and use a relatively small number of particles each


CARTOON FX EASY EDITOR
----------------------
You can find this tool in the menu:
| Tools > Cartoon FX > Cartoon FX Easy Editor
It allows you to easily change one or several Particle Systems properties:

"Scale Size" to change the size of your Particle Systems (changing speed, velocity, gravity, etc. values to get an accurate scaled up version of the system).
It will also scale lights' intensity accordingly if any are found.
Tip: If you don't want to scale a particular module, disable it before scaling the system and re-enable it afterwards!

"Set Speed" to change the playback speed of your Particle Systems in percentage according to the base effect speed. 100% = normal speed.

"Tint Colors" to change the hue only of the colors of your Particle Systems (including gradients).

The "Copy Modules" section allows you to copy all values/curves/gradients/etc. from one or several Shuriken modules to one or several other Particle Systems.
Just select which modules you want to copy, choose the source Particle System to copy values from, select the GameObjects you want to change, and click on "Copy properties to selected GameObject(s)".

Note: "Include Children" works for both Properties and Copy Modules sections!


TROUBLESHOOTING
---------------

* Almost all prefabs have the CFXR_Effect script attached: it handles auto-destruction or deactivation of the GameObject once an effect has finished playing, as well as camera shake and light animation where relevant
* If you don't want the camera shake and/or the lights, you can globally disable them in the CFXR_Effect.cs script: look for the global defines at the top of the file and uncomment them.
* Effects were authored using Linear Color Space; use that for the best results (in Player settings).
* If you have problems with z-sorting (transparent objects appearing in front of other when their position is actually behind), try changing the values in the Particle System -> Renderer -> Sorting Fudge; as long as the relative order is respected between the different particle systems of a same prefab, it should work ok.
* You can change the global HDR scale in the "CFXR_SETTINGS.cginc" file, if you need to adjust the effects for your bloom parameters for example.
* You can entirely disable Soft Particles in "CFXR_SETTINGS.cginc" too by uncommenting the '#define GLOBAL_DISABLE_SOFT_PARTICLES' line at the top.
* URP: If your particles don't render in the Game View or in a build, it could be because "Depth Texture" is disabled: either enable it in the URP asset for Soft Particles to work, or disable Soft Particles entirely (see above).


PLEASE LEAVE A REVIEW OR RATE THE PACKAGE IF YOU FIND IT USEFUL!
It helps a lot! :)


Enjoy! :)


CONTACT
-------
Questions, suggestions, help needed?
Contact me at:

jean.moreno.public+unity@gmail.com

I'd be happy to see any effects used in your project, so feel free to drop me a line about that! :)


RELEASE NOTES
-------------

R 1.2.3
- Improved CFXR_ParticleText usability when toggling the "is Dynamic" flag on an existing text effect
- Disabled mesh GPU instancing in the shaders because it was causing too many issues (even when using Unity's default particle shader)

R 1.2.2
- Removed GPU instancing support on the "CFXR Procedural Ring" shader, because it visually breaks it
- Fixed compilation error related to 'projPos' in the "CFXR Particle Glow" shader

R 1.2.1
- Added support for GPU instancing in the shaders
- Fixed possible build erro when adding a text effect in a prefab due to HideFlags

R 1.2.0
- Merged CFXR_ParticleText and CFXR_ParticleText_Runtime: now you just have to enable "isDynamic" to include the script at runtime and be able to update the text
- CFXR_ParticleText now also allows to change the colors and lifetime multiplier
- Fixed legacy custom shaders not working in URP builds
- Added subcategory for legacy shaders ("Cartoon FX/Legacy/...")
- Fixed Camera Shake that was still applied when Time.timeScale is 0

R 1.1.3
- Updated files for compatibility with "Cartoon FX 4 Remaster"

R 1.1.2
- Added "Screen Distortion" shader, and a few effect variants using it
- Fixed fireball materials
- Fixed compilation errors on some platforms (e.g. PS4)
- Prevent possible asmdef conflict with Toony Colors Pro 2
- Strip shaders for the unused render pipeline during build, depending on if built-in or URP is used

R 1.1.1
- Fixed AABB errors spamming in the console in Unity 2020.2+ with some effects (e.g. "CFXR Fire Trail")
- Fixed shader compilation warnings when building

R 1.1.0
- Added text prefabs variants with a new font
- Fixed fire materials where flames could sometime clip out of the particle mesh
- Fixed an effect using a legacy material instead of a new one (should be safe to delete the legacy folder now if not needed)
- Fixed shader compilation on Metal platform (iOS, macOS)
- Fixed some textures not working on WebGL (they were set to "Alpha 8" instead of "R8")
- Fixed typo in "Flame" prefabs name (Flamme => Flame)

R 1.0.9
- Added Single-Pass Stereo support (VR) in the shaders

R 1.0.8
- Fixed Camera Shake for URP
- Fixed "Hide Flags" issue in CFXR Text prefabs that could prevent from building

R 1.0.7
- Updated lighting support in "CFXR Particle Ubershader"
- Added "CFXR_ParticleText_Runtime" script to allow updating a text effect at runtime with an example in the "CFXR Demo" scene

R 1.0.6
- Forced single-channel red textures to be uncompressed for WebGL target, as they don't work with Chrome otherwise
- Added lighting support in "CFXR Particle Ubershader"

R 1.0.5
- Added CFXR_SETTINGS.cginc file for global shader settings
- Added support for Orthographic cameras (enable the relevant setting in CFXR_SETTINGS)

R 1.0.4
- Disabled internal menu item ("Tools/Create font asset")

R 1.0.3
- Added URP 2D Renderer support

R 1.0.2
- Added "DISABLE_SOFT_PARTICLES" global define in CFXR.cginc: uncomment it to disable Soft Particles for all effects.
Can be useful with URP, because particles won't render at all if the "Depth Texture" isn't enabled.
- [Unity 2019.1+] Changed `shader_feature` to `shader_feature_local` to avoid global shader keyword usage

R 1.0.1
- Fixed "Soft Particles" in shaders for both pipelines (built-in & URP)
- Added "GLOBAL_CAMERA_SHAKE_MULTIPLIER" const in CFXR_Effect.cs to easily scale the camera shaking for all effects

R 1.0
- "Cartoon FX Remaster" first release, as a free update of "Cartoon FX Pack"
