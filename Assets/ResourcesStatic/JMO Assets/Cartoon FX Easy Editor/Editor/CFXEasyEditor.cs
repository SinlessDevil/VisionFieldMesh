using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using CartoonFX;

// Cartoon FX Easy Editor
// (c) 2013-2020 - Jean Moreno

public class CFXEasyEditor : EditorWindow
{
	static private CFXEasyEditor SingleWindow;
	
	[MenuItem("Tools/Cartoon FX Easy Editor")]
	static void ShowWindow()
	{
		CFXEasyEditor window = EditorWindow.GetWindow<CFXEasyEditor>(EditorPrefs.GetBool("CFX_ShowAsToolbox", true), "Easy Editor", true);
		window.minSize = new Vector2(300, 8);
		window.maxSize = new Vector2(300, 8);
	}

	private int SelectedParticleSystemsCount;

	//Change Start Color
	private bool AffectAlpha = true;
	private Color ColorValue = Color.white;
	private Color ColorValue2 = Color.white;
	
	//Scale
	private float ScalingValue = 2.0f;
	private float LTScalingValue = 1.0f;
	
	//Delay
	private float DelayValue = 1.0f;
	
	//Duration
	private float DurationValue = 5.0f;
	
	//Tint
	private bool TintStartColor = true;
	private bool TintColorModule = true;
	private bool TintColorSpeedModule = true;
	private bool TintTrailsModule = true;
	private bool TintCustomData1 = true;
	private bool TintCustomData2 = true;
	private bool TintLights = true;
	private Color TintColorValue = Color.white;

	//Change Lightness
	private int LightnessStep = 10;
	
	//Module copying system
	private ParticleSystem copyModulesSourceParticleSystem;

	private class ParticleSystemModule
	{
		public string name;
		//public bool selected;
		//public bool enabledInSource;
		public string serializedPropertyPath;

		public ParticleSystemModule(string name, string serializedPropertyPath)
		{
			this.name = " " + name + " ";
			this.serializedPropertyPath = serializedPropertyPath;
		}

		public bool CheckIfEnabledInSource(SerializedObject serializedSource)
		{
			bool enabled = true;
			var sp = serializedSource.FindProperty(this.serializedPropertyPath);
			if(sp != null)
			{
				var enabledSp = sp.FindPropertyRelative("enabled");
				if(enabledSp != null)
				{
					enabled = enabledSp.boolValue;
				}
			}
			return enabled;
		}
	}

	private ParticleSystemModule[] modulesToCopy = null;
	private List<string> initialModuleExtraProperties = null;
	private GUIContent[] modulesLabels = null;

	private ParticleSystem selectedParticleSystem;
	private bool[] selectedModules;
	private bool[] enabledModules;
	/*
	private bool[] selectedModulesMutate;
	private bool[] enabledModulesMutate;

	private float mutatePercentage = 0.1f;
	*/

	//Foldouts
	bool basicFoldout = false;
	bool colorFoldout = false;
	bool copyFoldout = false;
	//bool mutateFoldout = false;
	
	//Editor Prefs
	private bool pref_ShowAsToolbox;
	private bool pref_IncludeChildren;
	private bool pref_HideDisabledModulesCopy;

	void OnEnable()
	{
		//Load Settings
		pref_ShowAsToolbox = EditorPrefs.GetBool("CFX_ShowAsToolbox", true);
		pref_IncludeChildren = EditorPrefs.GetBool("CFX_IncludeChildren", true);
		pref_HideDisabledModulesCopy = EditorPrefs.GetBool("CFX_HideDisabledModulesCopy", true);
		basicFoldout = EditorPrefs.GetBool("CFX_BasicFoldout", false);
		colorFoldout = EditorPrefs.GetBool("CFX_ColorFoldout", false);
		copyFoldout = EditorPrefs.GetBool("CFX_CopyFoldout", false);
		//mutateFoldout = EditorPrefs.GetBool("CFX_MutateFoldout", false);

		ParticleSystem ps = null;
		if (Selection.activeGameObject != null)
		{
			ps = Selection.activeGameObject.GetComponent<ParticleSystem>();
		}
		if (ps == null && copyModulesSourceParticleSystem != null)
		{
			ps = copyModulesSourceParticleSystem.GetComponent<ParticleSystem>();
		}
		FetchParticleSystemModules(ps);
		//RefreshCurrentlyEnabledModules(ps, ref enabledModulesMutate);

		OnSelectionChange();
	}

	void OnSelectionChange()
	{
		UpdateSelectionCount();
		this.Repaint();

		if (Selection.activeGameObject != null)
		{
			selectedParticleSystem = Selection.activeGameObject.GetComponent<ParticleSystem>();
			FetchParticleSystemModules(selectedParticleSystem);
		}
	}

	void FetchParticleSystemModules(ParticleSystem ps)
	{
		// Fetch the modules list from a Particle System
		if (modulesToCopy == null && ps != null)
		{
			initialModuleExtraProperties = new List<string>();
			var modulesList = new List<ParticleSystemModule>();
			var modulesLabelsList = new List<GUIContent>();
			var so = new SerializedObject(ps);
			var sp = so.GetIterator();
			sp.Next(true);
			while (sp.NextVisible(false))
			{
				if (sp.propertyPath.EndsWith("Module"))
				{
					var module = new ParticleSystemModule(sp.propertyPath.Replace("Module", ""), sp.propertyPath);
					modulesList.Add(module);
					modulesLabelsList.Add(new GUIContent(sp.propertyPath.Replace("Module", "")));
				}
				else
				{
					initialModuleExtraProperties.Add(sp.propertyPath);
				}
			}
			modulesToCopy = modulesList.ToArray();

			// add one entry for renderer (not a module but a component)
			modulesLabelsList.Add(new GUIContent("Renderer"));
			modulesLabels = modulesLabelsList.ToArray();
		}
	}

	void OnDisable()
	{
		//Save Settings
		EditorPrefs.SetBool("CFX_BasicFoldout", basicFoldout);
		EditorPrefs.SetBool("CFX_ColorFoldout", colorFoldout);
		EditorPrefs.SetBool("CFX_CopyFoldout", copyFoldout);
		//EditorPrefs.SetBool("CFX_MutateFoldout", mutateFoldout);
	}

	void UpdateSelectionCount()
	{
		SelectedParticleSystemsCount = 0;
		foreach(var go in Selection.gameObjects)
		{
			ParticleSystem[] systems;
			if(pref_IncludeChildren)
				systems = go.GetComponentsInChildren<ParticleSystem>(true);
			else
				systems = go.GetComponents<ParticleSystem>();

			SelectedParticleSystemsCount += systems.Length;
		}
	}
	
	void OnGUI()
	{
		GUILayout.Space(4);
		
		GUILayout.BeginHorizontal();
		var rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight, EditorStyles.label);
		GUI.Label(rect, "Cartoon FX Easy Editor", EditorStyles.boldLabel);
		var guiColor = GUI.color;
		GUI.color *= new Color(.8f,.8f,.8f,1f);
		pref_ShowAsToolbox = GUILayout.Toggle(pref_ShowAsToolbox, new GUIContent("Toolbox", "If enabled, the window will be displayed as an external toolbox.\nElse it will act as a dockable Unity window."), EditorStyles.miniButton, GUILayout.Width(65));
		GUI.color = guiColor;
		if(GUI.changed)
		{
			EditorPrefs.SetBool("CFX_ShowAsToolbox", pref_ShowAsToolbox);
			this.Close();
			CFXEasyEditor.ShowWindow();
		}
		GUILayout.EndHorizontal();
		GUILayout.Label("Easily change properties of any Particle System!", EditorStyles.miniLabel);
		
	//----------------------------------------------------------------
		
		pref_IncludeChildren = GUILayout.Toggle(pref_IncludeChildren, new GUIContent("Include Children", "If checked, changes will affect every Particle Systems from each child of the selected GameObject(s)"));
		if(GUI.changed)
		{
			EditorPrefs.SetBool("CFX_IncludeChildren", pref_IncludeChildren);
			UpdateSelectionCount();
		}

		GUILayout.Label(string.Format("{0} selected Particle System{1}", SelectedParticleSystemsCount, SelectedParticleSystemsCount > 1 ? "s" : ""), EditorStyles.helpBox);
		
		EditorGUILayout.BeginHorizontal();
		
		GUILayout.Label("Test effect(s):");
		using(new EditorGUI.DisabledScope(SelectedParticleSystemsCount <= 0))
		{

			if(GUILayout.Button("Play", EditorStyles.miniButtonLeft, GUILayout.Width(50f)))
			{
				foreach(GameObject go in Selection.gameObjects)
				{
					ParticleSystem[] systems = go.GetComponents<ParticleSystem>();
					if(systems.Length == 0) continue;
					foreach(ParticleSystem system in systems)
						system.Play(pref_IncludeChildren);
				}
			}
			if(GUILayout.Button("Pause", EditorStyles.miniButtonMid, GUILayout.Width(50f)))
			{
				foreach(GameObject go in Selection.gameObjects)
				{
					ParticleSystem[] systems = go.GetComponents<ParticleSystem>();
					if(systems.Length == 0) continue;
					foreach(ParticleSystem system in systems)
						system.Pause(pref_IncludeChildren);
				}
			}
			if(GUILayout.Button("Stop", EditorStyles.miniButtonMid, GUILayout.Width(50f)))
			{
				foreach(GameObject go in Selection.gameObjects)
				{
					ParticleSystem[] systems = go.GetComponents<ParticleSystem>();
					if(systems.Length == 0) continue;
					foreach(ParticleSystem system in systems)
						system.Stop(pref_IncludeChildren);
				}
			}
			if(GUILayout.Button("Clear", EditorStyles.miniButtonRight, GUILayout.Width(50f)))
			{
				foreach(GameObject go in Selection.gameObjects)
				{
					ParticleSystem[] systems = go.GetComponents<ParticleSystem>();
					if(systems.Length == 0) continue;
					foreach(ParticleSystem system in systems)
					{
						system.Stop(pref_IncludeChildren);
						system.Clear(pref_IncludeChildren);
					}
				}
			}
		}
		
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		//----------------------------------------------------------------

		//Separator
		GUISeparator();
		//GUILayout.Box("",GUILayout.Width(this.position.width - 12), GUILayout.Height(3));
		
		basicFoldout = EditorGUILayout.Foldout(basicFoldout, "QUICK EDIT", true);
		if(basicFoldout)
		{
			GUILayout.Space(4);

			//----------------------------------------------------------------

			GUILayout.BeginHorizontal();
			if(GUILayout.Button(new GUIContent("Scale Size", "Changes the size of the Particle System(s) and other values accordingly (speed, gravity, etc.)"), GUILayout.Width(120)))
			{
				applyScale();
			}
			GUILayout.Label("Multiplier:",GUILayout.Width(110));
			ScalingValue = EditorGUILayout.FloatField(ScalingValue,GUILayout.Width(50));
			if(ScalingValue <= 0) ScalingValue = 0.1f;
			GUILayout.EndHorizontal();
			
		//----------------------------------------------------------------
			
			GUILayout.BeginHorizontal();
			if(GUILayout.Button(new GUIContent("Set Speed", "Changes the simulation speed of the Particle System(s)\n1 = default speed"), GUILayout.Width(120)))
			{
				applySpeed();
			}
			GUILayout.Label("Speed:",GUILayout.Width(110));
			LTScalingValue = EditorGUILayout.FloatField(LTScalingValue,GUILayout.Width(50));
			if(LTScalingValue < 0.0f) LTScalingValue = 0.0f;
			else if(LTScalingValue > 9999) LTScalingValue = 9999;
			GUILayout.EndHorizontal();
			
		//----------------------------------------------------------------
			
			GUILayout.BeginHorizontal();
			if(GUILayout.Button(new GUIContent("Set Duration", "Changes the duration of the Particle System(s)"), GUILayout.Width(120)))
			{
				applyDuration();
			}
			GUILayout.Label("Duration (sec):",GUILayout.Width(110));
			DurationValue = EditorGUILayout.FloatField(DurationValue,GUILayout.Width(50));
			if(DurationValue < 0.1f) DurationValue = 0.1f;
			else if(DurationValue > 9999) DurationValue = 9999;
			GUILayout.EndHorizontal();
			
		//----------------------------------------------------------------
			
			GUILayout.BeginHorizontal();
			if(GUILayout.Button(new GUIContent("Set Delay", "Changes the delay of the Particle System(s)"), GUILayout.Width(120)))
			{
				applyDelay();
			}
			GUILayout.Label("Delay :",GUILayout.Width(110));
			DelayValue = EditorGUILayout.FloatField(DelayValue,GUILayout.Width(50));
			if(DelayValue < 0.0f) DelayValue = 0.0f;
			else if(DelayValue > 9999f) DelayValue = 9999f;
			GUILayout.EndHorizontal();
			
		//----------------------------------------------------------------
			
			GUILayout.Space(2);
			
			GUILayout.BeginHorizontal();
			if(GUILayout.Button(new GUIContent("Loop", "Loop the effect (might not work properly on some effects such as explosions)"), EditorStyles.miniButtonLeft))
			{
				loopEffect(true);
			}
			if(GUILayout.Button(new GUIContent("Unloop", "Remove looping from the effect"), EditorStyles.miniButtonRight))
			{
				loopEffect(false);
			}
			if(GUILayout.Button(new GUIContent("Prewarm On", "Prewarm the effect (if looped)"), EditorStyles.miniButtonLeft))
			{
				prewarmEffect(true);
			}
			if(GUILayout.Button(new GUIContent("Prewarm Off", "Don't prewarm the effect (if looped)"), EditorStyles.miniButtonRight))
			{
				prewarmEffect(false);
			}
			GUILayout.EndHorizontal();
			
			GUILayout.Space(2);
		
	//----------------------------------------------------------------
		
		}

		//Separator
		GUISeparator();
		//GUILayout.Box("",GUILayout.Width(this.position.width - 12), GUILayout.Height(3));
		
		EditorGUI.BeginChangeCheck();
		colorFoldout = EditorGUILayout.Foldout(colorFoldout, "COLOR EDIT", true);
		if(colorFoldout)
		{
			GUILayout.Space(4);

			//----------------------------------------------------------------

			GUILayout.BeginHorizontal();
			if(GUILayout.Button(new GUIContent("Set Start Color(s)", "Changes the color(s) of the Particle System(s)\nSecond Color is used when Start Color is 'Random Between Two Colors'."),GUILayout.Width(120)))
			{
				applyColor();
			}
			ColorValue = EditorGUILayout.ColorField(ColorValue);
			ColorValue2 = EditorGUILayout.ColorField(ColorValue2);
			AffectAlpha = GUILayout.Toggle(AffectAlpha, new GUIContent("Alpha", "If checked, the alpha value will also be changed"));
			GUILayout.EndHorizontal();

			//----------------------------------------------------------------

			GUILayout.Space(8);

			using(new EditorGUI.DisabledScope(!TintStartColor && !TintColorModule && !TintColorSpeedModule && !TintTrailsModule && !TintCustomData1 && !TintCustomData2))
			{
				GUILayout.BeginHorizontal();
				if(GUILayout.Button(new GUIContent("Tint Colors", "Colorize the Particle System(s) to a specific color, including gradients!\n(preserving their saturation and value)"), GUILayout.Width(120)))
				{
					tintColor();
				}
				TintColorValue = EditorGUILayout.ColorField(TintColorValue);
				TintColorValue = HSLColor.FromRGBA(TintColorValue).VividColor();
				GUILayout.EndHorizontal();

				//----------------------------------------------------------------

				GUILayout.BeginHorizontal();

				if (GUILayout.Button("-10", EditorStyles.miniButtonLeft)) hueShift(-10);
				if (GUILayout.Button("-5", EditorStyles.miniButtonMid)) hueShift(-5);
				if (GUILayout.Button("-1", EditorStyles.miniButtonMid)) hueShift(-1);
				GUILayout.Label(new GUIContent("Hue Shift", "Tints the colors of the Particle System(s) by shifting the original hues by a value\n(preserving differences between colors)"), GUILayout.Width(120));
				if (GUILayout.Button("+10", EditorStyles.miniButtonMid)) hueShift(10);
				if (GUILayout.Button("+5", EditorStyles.miniButtonMid)) hueShift(5);
				if (GUILayout.Button("+1", EditorStyles.miniButtonRight)) hueShift(1);

				GUILayout.EndHorizontal();

				//----------------------------------------------------------------

				GUILayout.BeginHorizontal();

				if (GUILayout.Button("-10", EditorStyles.miniButtonLeft)) satShift(-10);
				if (GUILayout.Button("-5", EditorStyles.miniButtonMid)) satShift(-5);
				if (GUILayout.Button("-1", EditorStyles.miniButtonMid)) satShift(-1);
				GUILayout.Label(new GUIContent("Saturation Shift", "Change the saturation of the colors of the Particle System(s)\n(preserving their hue and value)"), GUILayout.Width(120));
				if (GUILayout.Button("+10", EditorStyles.miniButtonMid)) satShift(10);
				if (GUILayout.Button("+5", EditorStyles.miniButtonMid)) satShift(5);
				if (GUILayout.Button("+1", EditorStyles.miniButtonRight)) satShift(1);

				GUILayout.EndHorizontal();

				//----------------------------------------------------------------

				GUILayout.BeginHorizontal();

				if (GUILayout.Button("-10", EditorStyles.miniButtonLeft)) valShift(-10);
				if (GUILayout.Button("-5", EditorStyles.miniButtonMid)) valShift(-5);
				if (GUILayout.Button("-1", EditorStyles.miniButtonMid)) valShift(-1);
				GUILayout.Label(new GUIContent("Value Shift", "Change the value of the colors of the Particle System(s)\n(preserving their hue and saturation)"), GUILayout.Width(120));
				if (GUILayout.Button("+10", EditorStyles.miniButtonMid)) valShift(10);
				if (GUILayout.Button("+5", EditorStyles.miniButtonMid)) valShift(5);
				if (GUILayout.Button("+1", EditorStyles.miniButtonRight)) valShift(1);

				GUILayout.EndHorizontal();

				//----------------------------------------------------------------

				GUILayout.BeginHorizontal();
				if (GUILayout.Button(new GUIContent("Gamma -> Linear", "Converts the colors from Gamma to Linear color space")))
				{
					gammaToLinear();
				}
				if (GUILayout.Button(new GUIContent("Linear -> Gamma", "Converts the colors from Linear to Gamma color space")))
				{
					linearToGamma();
				}
				GUILayout.EndHorizontal();
			}

			GUILayout.Space(4);
		
		//----------------------------------------------------------------
			
			/*
			GUILayout.BeginHorizontal();
			GUILayout.Label("Add/Substract Lightness:");
			
			LightnessStep = EditorGUILayout.IntField(LightnessStep, GUILayout.Width(30));
			if(LightnessStep > 99) LightnessStep = 99;
			else if(LightnessStep < 1) LightnessStep = 1;
			GUILayout.Label("%");
			
			if(GUILayout.Button("-", EditorStyles.miniButtonLeft, GUILayout.Width(22)))
			{
				addLightness(true);
			}
			if(GUILayout.Button("+", EditorStyles.miniButtonRight, GUILayout.Width(22)))
			{
				addLightness(false);
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			*/
			
		//----------------------------------------------------------------
			
			GUILayout.Label("Color Modules to affect:");
			
			GUILayout.BeginHorizontal();
			GUILayout.Space(4);
			TintStartColor = GUILayout.Toggle(TintStartColor, new GUIContent("Start Color", "If checked, the \"Start Color\" value(s) will be affected."), EditorStyles.toolbarButton);
			TintColorModule = GUILayout.Toggle(TintColorModule, new GUIContent("Color over Lifetime", "If checked, the \"Color over Lifetime\" value(s) will be affected."), EditorStyles.toolbarButton);
			TintColorSpeedModule = GUILayout.Toggle(TintColorSpeedModule, new GUIContent("Color by Speed", "If checked, the \"Color by Speed\" value(s) will be affected."), EditorStyles.toolbarButton);
			GUILayout.Space(4);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Space(4);
			TintTrailsModule = GUILayout.Toggle(TintTrailsModule, new GUIContent("Trails", "If checked, the \"Trails\" value(s) will be affected."), EditorStyles.toolbarButton);
			TintCustomData1 = GUILayout.Toggle(TintCustomData1, new GUIContent("Custom Data 1", "If checked, the \"Custom Data 1\" value(s) will be affected if they have been set to a color value."), EditorStyles.toolbarButton);
			TintCustomData2 = GUILayout.Toggle(TintCustomData2, new GUIContent("Custom Data 2", "If checked, the \"Custom Data 2\" value(s) will be affected if they have been set to a color value."), EditorStyles.toolbarButton);
			GUILayout.Space(4);
			GUILayout.EndHorizontal();

			TintLights = GUILayout.Toggle(TintLights, new GUIContent(" Tint Lights found in the effect's Hierarchy", "Will search for and tint any Lights found in the effect hierarchy."));

			GUILayout.Space(4);
			
		//----------------------------------------------------------------
			
		}

		GUISeparator();
		
	//----------------------------------------------------------------
		
		EditorGUI.BeginChangeCheck();
		copyFoldout = EditorGUILayout.Foldout(copyFoldout, "COPY MODULES", true);
		if(copyFoldout)
		{
			GUILayout.Space(4);

			EditorGUILayout.HelpBox("Copy selected modules from a Particle System to others!", MessageType.Info);
			
			GUILayout.Label("Source Particle System to copy from:");
			GUILayout.BeginHorizontal();
			{
				EditorGUI.BeginChangeCheck();
				copyModulesSourceParticleSystem = (ParticleSystem)EditorGUILayout.ObjectField(copyModulesSourceParticleSystem, typeof(ParticleSystem), true);
				if (GUILayout.Button("Get Selected", EditorStyles.miniButton))
				{
					if (Selection.activeGameObject != null)
					{
						var ps = Selection.activeGameObject.GetComponent<ParticleSystem>();
						if (ps != null)
						{
							copyModulesSourceParticleSystem = ps;
						}
					}
				}
				if (EditorGUI.EndChangeCheck())
				{
					if (copyModulesSourceParticleSystem != null)
					{
						FetchParticleSystemModules(copyModulesSourceParticleSystem);
						RefreshCurrentlyEnabledModules(copyModulesSourceParticleSystem, ref enabledModules);
					}
				}
			}
			GUILayout.EndHorizontal();
			
			EditorGUILayout.LabelField("Modules to Copy:");
			
			GUILayout.Space(4);

			DrawSelectableModules(ref selectedModules, ref enabledModules, copyModulesSourceParticleSystem);

			/*

			GUILayout.BeginHorizontal();
			GUILayout.Space(8f);
			GUISelectModule(modulesToCopy[0]);
			GUISelectModule(modulesToCopy[1]);
			GUISelectModule(modulesToCopy[2]);
			GUILayout.EndHorizontal();

			SelectModulesSpace();
			GUILayout.BeginHorizontal();
			GUILayout.Space(8f);
#if UNITY_5_4_OR_NEWER
			GUISelectModule(modulesToCopy[3]);
			GUISelectModule(modulesToCopy[4]);
			GUISelectModule(modulesToCopy[16]);
			GUISelectModule(modulesToCopy[5]);
#else
			GUISelectModule(modulesToCopy[3]);
			GUISelectModule(modulesToCopy[4]);
			GUISelectModule(modulesToCopy[5]);
#endif
			GUILayout.EndHorizontal();

			SelectModulesSpace();
			GUILayout.BeginHorizontal();
			GUILayout.Space(8f);
			GUISelectModule(modulesToCopy[6]);
			GUISelectModule(modulesToCopy[7]);
			GUILayout.EndHorizontal();

			SelectModulesSpace();
			GUILayout.BeginHorizontal();
			GUILayout.Space(8f);
			GUISelectModule(modulesToCopy[8]);
			GUISelectModule(modulesToCopy[9]);
			GUILayout.EndHorizontal();

			SelectModulesSpace();
			GUILayout.BeginHorizontal();
			GUILayout.Space(8f);
			GUISelectModule(modulesToCopy[10]);
			GUISelectModule(modulesToCopy[11]);
			GUILayout.EndHorizontal();

			SelectModulesSpace();
			GUILayout.BeginHorizontal();
			GUILayout.Space(8f);
			GUISelectModule(modulesToCopy[12]);
#if UNITY_5_4_OR_NEWER
			GUISelectModule(modulesToCopy[17]);
#endif
			GUISelectModule(modulesToCopy[13]);
			GUILayout.EndHorizontal();

			SelectModulesSpace();
			GUILayout.BeginHorizontal();
			GUILayout.Space(8f);
#if UNITY_5_5_OR_NEWER
			GUISelectModule(modulesToCopy[20]);
			GUISelectModule(modulesToCopy[19]);
			GUISelectModule(modulesToCopy[18]);
#endif
			GUISelectModule(modulesToCopy[14]);
			GUILayout.EndHorizontal();

			SelectModulesSpace();
			GUILayout.BeginHorizontal();
			GUILayout.Space(8f);
			GUISelectModule(modulesToCopy[15]);
			GUILayout.EndHorizontal();

			GUI.color = guiColor;

			SelectModulesSpace();

			*/

			GUILayout.BeginHorizontal();
			{
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("Copy properties to\nselected Object(s)", GUILayout.Height(Mathf.Ceil(EditorGUIUtility.singleLineHeight * 2.1f))))
				{
					bool foundPs = false;
					foreach (GameObject go in Selection.gameObjects)
					{
						var system = go.GetComponent<ParticleSystem>();
						if (system != null)
						{
							foundPs = true;
							CopyModules(copyModulesSourceParticleSystem, system);
						}
					}

					if (!foundPs)
					{
						EditorApplication.Beep();
						Debug.LogWarning("Cartoon FX Easy Editor: No Particle System found in the selected GameObject(s)!");
					}
				}
			}
			GUILayout.EndHorizontal();
		}

		/*

		GUISeparator();

		// TODO full list of the Properties with saving/loading presets

		mutateFoldout = EditorGUILayout.Foldout(mutateFoldout, "MUTATE MODULES", true);
		if (mutateFoldout)
		{
			GUILayout.Space(4);

			DrawSelectableModules(ref selectedModulesMutate, ref enabledModulesMutate, selectedParticleSystem);

			EditorGUI.BeginChangeCheck();
			float newValue = EditorGUILayout.Slider("± percent", Mathf.Floor(mutatePercentage * 100), 0, 100);
			if (EditorGUI.EndChangeCheck())
			{
				mutatePercentage = newValue / 100f;
			}

			if (GUILayout.Button("Mutate Modules", GUILayout.Height(EditorGUIUtility.singleLineHeight*2.1f)))
			{
				MutateModules();
			}
		}

		*/

		//----------------------------------------------------------------

		GUILayout.Space(8);

		//Resize window if needed
		if (Event.current.type == EventType.Repaint)
		{
			float h = GUILayoutUtility.GetLastRect().yMax;
			if (lastHeight != h)
			{
				lastHeight = h;
				this.minSize = new Vector2(300, h);
				this.maxSize = new Vector2(300, h);
			}
		}
	}

	float lastHeight = 0;

	void DrawSelectableModules(ref bool[] selected, ref bool[] enabled, ParticleSystem target)
	{
		if (modulesToCopy != null)
		{
			if (enabled == null || enabled.Length != modulesToCopy.Length)
			{
				enabled = new bool[modulesToCopy.Length];
			}


			// +1 for the Renderer component
			if (selected == null || selected.Length != (modulesToCopy.Length + 1))
			{
				selected = new bool[modulesToCopy.Length + 1];
			}
			int renderer = selected.Length - 1;

			// quick select:
			GUILayout.BeginHorizontal();
			GUILayout.Label("Select:", GUILayout.Width(60));
			if (GUILayout.Button(" All ", EditorStyles.miniButton))
			{
				for (int i = 0; i < modulesToCopy.Length; i++)
				{
					if (!pref_HideDisabledModulesCopy || enabled[i])
					{
						selected[i] = true;
					}
					selected[renderer] = true;
				}
			}

			var guiColor = GUI.color;
			GUI.color *= new Color(1.0f, 0.5f, 0.5f);
			if (GUILayout.Button(" None ", EditorStyles.miniButton))
			{
				for (int i = 0; i < modulesToCopy.Length; i++)
				{
					selected[i] = false;
				}
				selected[renderer] = false;
			}
			GUILayout.EndHorizontal();
			GUI.color = guiColor;

			GUILayout.BeginHorizontal();
			GUILayout.Label(" ", GUILayout.Width(60));
			if (GUILayout.Button(" Size ", EditorStyles.miniButton))
			{
				for (int i = 0; i < modulesToCopy.Length; i++)
				{
					switch (modulesToCopy[i].serializedPropertyPath)
					{
						case "SizeModule":
						case "SizeBySpeedModule":
							selected[i] = true;
							break;
					}
				}
			}
			if (GUILayout.Button(" Movement ", EditorStyles.miniButton))
			{
				for (int i = 0; i < modulesToCopy.Length; i++)
				{
					switch (modulesToCopy[i].serializedPropertyPath)
					{
						case "VelocityModule":
						case "InheritVelocityModule":
						case "ForceModule":
						case "ExternalForcesModule":
						case "ClampVelocityModule":
						case "NoiseModule":
						case "CollisionModule":
							selected[i] = true;
							break;
					}
				}
			}
			if (GUILayout.Button(" Color/Appearance ", EditorStyles.miniButton))
			{
				for (int i = 0; i < modulesToCopy.Length; i++)
				{
					switch (modulesToCopy[i].serializedPropertyPath)
					{
						case "ColorModule":
						case "ColorBySpeedModule":
						case "TrailModule":
						case "LightsModule":
						case "UVModule":
							selected[i] = true;
							break;
					}
					selected[renderer] = true;
				}
			}
			GUILayout.EndHorizontal();

			using (new EditorGUI.DisabledScope(target == null))
			{
				EditorGUI.BeginChangeCheck();
				pref_HideDisabledModulesCopy = GUILayout.Toggle(pref_HideDisabledModulesCopy, new GUIContent(" Hide disabled modules", "Will hide modules that are disabled on the current source Particle System"));
				if (EditorGUI.EndChangeCheck())
				{
					EditorPrefs.SetBool("CFX_HideDisabledModulesCopy", pref_HideDisabledModulesCopy);
					RefreshCurrentlyEnabledModules(target, ref enabled);
				}
			}

			const int row = 4;
			const int padding = 4;
			for (int i = 0; i < modulesToCopy.Length; i += row)
			{
				GUILayout.BeginHorizontal();
				{
					GUILayout.Space(padding);
					for (int j = 0; j < row; j++)
					{
						if (i+j < modulesLabels.Length)
						{
							var col = GUI.color;

							// special case: renderer
							/*
							if (i+j == modulesLabels.Length-1)
							{
								if (selectedModuleRenderer) GUI.color *= Color.cyan;
								selectedModuleRenderer = GUILayout.Toggle(selectedModuleRenderer, modulesLabels[i+j], EditorStyles.toolbarButton);
							}
							else
							*/
							{
								bool enbl = (i+j) >= enabled.Length ? true : enabled[i+j];
								if (pref_HideDisabledModulesCopy && !enbl)
								{
									continue;
								}
								if (selected[i+j]) GUI.color *= Color.cyan;
								selected[i+j] = GUILayout.Toggle(selected[i+j], modulesLabels[i+j], EditorStyles.toolbarButton);
							}
							GUI.color = col;
						}
					}
					GUILayout.Space(padding);
				}
				GUILayout.EndHorizontal();
			}
		}
		else
		{
			EditorGUILayout.HelpBox("Select a Particle System to initialize the module list.", MessageType.Info);
		}
	}

	void RefreshCurrentlyEnabledModules(ParticleSystem source, ref bool[] enabledModules)
	{
		if (modulesToCopy == null)
		{
			return;
		}

		if (enabledModules == null || enabledModules.Length != modulesToCopy.Length)
		{
			// note: no need for extra slot for renderer, consider it always enabled
			enabledModules = new bool[modulesToCopy.Length];
		}

		if (source != null)
		{
			var so = new SerializedObject(source);
			for (int i = 0; i < modulesToCopy.Length; i++)
			{
				enabledModules[i] = modulesToCopy[i].CheckIfEnabledInSource(so);
			}
		}
		else
		{
			for (int i = 0; i < enabledModules.Length; i++)
			{
				enabledModules[i] = true;
			}
		}
	}

	//Loop effects
	private void loopEffect(bool setLoop)
	{
		foreach(GameObject go in Selection.gameObjects)
		{
			//Scale Shuriken Particles Values
			ParticleSystem[] systems;
			if(pref_IncludeChildren)
				systems = go.GetComponentsInChildren<ParticleSystem>(true);
			else
				systems = go.GetComponents<ParticleSystem>();
			
			foreach(ParticleSystem ps in systems)
			{
				SerializedObject so = new SerializedObject(ps);
				so.FindProperty("looping").boolValue = setLoop;
				so.ApplyModifiedProperties();
			}
		}
	}
	
	//Prewarm effects
	private void prewarmEffect(bool setPrewarm)
	{
		foreach(GameObject go in Selection.gameObjects)
		{
			//Scale Shuriken Particles Values
			ParticleSystem[] systems;
			if(pref_IncludeChildren)
				systems = go.GetComponentsInChildren<ParticleSystem>(true);
			else
				systems = go.GetComponents<ParticleSystem>();
			
			foreach(ParticleSystem ps in systems)
			{
				SerializedObject so = new SerializedObject(ps);
				so.FindProperty("prewarm").boolValue = setPrewarm;
				so.ApplyModifiedProperties();
			}
		}
	}
	
	//Scale Size
	private void applyScale()
	{
		Undo.IncrementCurrentGroup();
		int groupId = Undo.GetCurrentGroup();

		foreach(GameObject go in Selection.gameObjects)
		{
			//Scale Shuriken Particles Values
			ParticleSystem[] systems;
			if(pref_IncludeChildren)
				systems = go.GetComponentsInChildren<ParticleSystem>(true);
			else
				systems = go.GetComponents<ParticleSystem>();
			
			foreach(ParticleSystem ps in systems)
			{
				Undo.RegisterCompleteObjectUndo(ps, "Apply Particle Scale");
				Undo.RegisterCompleteObjectUndo(ps.transform, "Apply Particle Scale");
				ScaleParticleValues(ps, go);
			}
			
			//Scale Lights' range
			Light[] lights = go.GetComponentsInChildren<Light>();
			foreach(Light light in lights)
			{
				Undo.RegisterCompleteObjectUndo(light, "Apply Particle Scale");
				Undo.RegisterCompleteObjectUndo(light.transform, "Apply Particle Scale");
				light.range *= ScalingValue;
				light.transform.localPosition *= ScalingValue;
			}
		}

		Undo.CollapseUndoOperations(groupId);
	}

	//Change Color
	private void applyColor()
	{
		foreach(GameObject go in Selection.gameObjects)
		{
			ParticleSystem[] systems;
			if(pref_IncludeChildren)
				systems = go.GetComponentsInChildren<ParticleSystem>(true);
			else
				systems = go.GetComponents<ParticleSystem>();
			
			foreach(ParticleSystem ps in systems)
			{
				SerializedObject psSerial = new SerializedObject(ps);
				if(!AffectAlpha)
				{
					psSerial.FindProperty("InitialModule.startColor.maxColor").colorValue = new Color(ColorValue.r, ColorValue.g, ColorValue.b, psSerial.FindProperty("InitialModule.startColor.maxColor").colorValue.a);
					psSerial.FindProperty("InitialModule.startColor.minColor").colorValue = new Color(ColorValue2.r, ColorValue2.g, ColorValue2.b, psSerial.FindProperty("InitialModule.startColor.minColor").colorValue.a);
				}
				else
				{
					psSerial.FindProperty("InitialModule.startColor.maxColor").colorValue = ColorValue;
					psSerial.FindProperty("InitialModule.startColor.minColor").colorValue = ColorValue2;
				}
				psSerial.ApplyModifiedProperties();
			}
		}
	}
	
	//TINT COLORS ================================================================================================================================
	
	private void tintColor()
	{
		float hue = HSLColor.FromRGBA(TintColorValue).h;
		GenericProcessColors(color =>
		{
			return HSLColor.FromRGBA(color).ColorWithHue(hue, false);
		});
	}

	private void hueShift(float shift)
	{
		GenericProcessColors(color =>
		{
			return HSLColor.FromRGBA(color).ColorWithHue(shift, true);
		});
	}

	private void satShift(float shift)
	{
		GenericProcessColors(color =>
		{
			return HSLColor.FromRGBA(color).ColorWithSaturationOffset(shift / 100f);
		});
	}

	private void valShift(float shift)
	{
		GenericProcessColors(color =>
		{
			return HSLColor.FromRGBA(color).ColorWithLightnessOffset(shift / 100f);
		});
	}

	private void GenericTintColorProperty(SerializedProperty colorProperty, float hue, bool shift)
	{
		GenericEditColorProperty(colorProperty, color =>
		{
			return HSLColor.FromRGBA(color).ColorWithHue(hue, shift);
		});
	}

	private void GenericEditGradient(SerializedProperty gradientProperty, System.Func<Color, Color> callback)
	{
		gradientProperty.FindPropertyRelative("key0").colorValue = callback(gradientProperty.FindPropertyRelative("key0").colorValue);
		gradientProperty.FindPropertyRelative("key1").colorValue = callback(gradientProperty.FindPropertyRelative("key1").colorValue);
		gradientProperty.FindPropertyRelative("key2").colorValue = callback(gradientProperty.FindPropertyRelative("key2").colorValue);
		gradientProperty.FindPropertyRelative("key3").colorValue = callback(gradientProperty.FindPropertyRelative("key3").colorValue);
		gradientProperty.FindPropertyRelative("key4").colorValue = callback(gradientProperty.FindPropertyRelative("key4").colorValue);
		gradientProperty.FindPropertyRelative("key5").colorValue = callback(gradientProperty.FindPropertyRelative("key5").colorValue);
		gradientProperty.FindPropertyRelative("key6").colorValue = callback(gradientProperty.FindPropertyRelative("key6").colorValue);
		gradientProperty.FindPropertyRelative("key7").colorValue = callback(gradientProperty.FindPropertyRelative("key7").colorValue);
	}

	private void gammaToLinear()
	{
		GenericProcessColors(color =>
		{
			return color.gamma;
		});
	}

	private void linearToGamma()
	{
		GenericProcessColors(color =>
		{
			return color.linear;
		});
	}

	private void GenericProcessColors(System.Func<Color, Color> callback)
	{
		foreach (GameObject go in Selection.gameObjects)
		{
			ParticleSystem[] systems;
			if (pref_IncludeChildren)
				systems = go.GetComponentsInChildren<ParticleSystem>(true);
			else
				systems = go.GetComponents<ParticleSystem>();

			foreach (ParticleSystem ps in systems)
			{
				SerializedObject psSerial = new SerializedObject(ps);

				if (TintStartColor)					GenericEditColorProperty(psSerial.FindProperty("InitialModule.startColor"), callback);
				if (TintColorModule)				GenericEditColorProperty(psSerial.FindProperty("ColorModule.gradient"), callback);
				if (TintColorSpeedModule)			GenericEditColorProperty(psSerial.FindProperty("ColorBySpeedModule.gradient"), callback);
				if (TintCustomData1)				GenericEditColorProperty(psSerial.FindProperty("CustomDataModule.color0"), callback);
				if (TintCustomData2)				GenericEditColorProperty(psSerial.FindProperty("CustomDataModule.color1"), callback);

				psSerial.ApplyModifiedProperties();
			}

			if (TintLights)
			{
				var lights = go.GetComponentsInChildren<Light>();
				foreach (var light in lights)
				{
					var psLight = new SerializedObject(light);
					var colorProperty = psLight.FindProperty("m_Color");
					colorProperty.colorValue = callback(colorProperty.colorValue);
					psLight.ApplyModifiedProperties();
				}

				var cfxrEffect = go.GetComponentInChildren<CFXR_Effect>();
				if (cfxrEffect != null)
				{
					if (System.Array.Exists(cfxrEffect.animatedLights, item => item.animateColor))
					{
						var psEffect = new SerializedObject(cfxrEffect);
						var lightsProp = psEffect.FindProperty("animatedLights");
						int iteration = lightsProp.arraySize;
						for (int i = 0; i < iteration; i++)
						{
							var prop = lightsProp.GetArrayElementAtIndex(i);
							var animateColor = prop.FindPropertyRelative("animateColor");
							if (animateColor.boolValue)
							{
								GenericEditGradient(prop.FindPropertyRelative("colorGradient"), callback);
							}
						}
						psEffect.ApplyModifiedProperties();
					}
				}
			}
		}
	}

	private void GenericEditColorProperty(SerializedProperty colorProperty, System.Func<Color, Color> callback)
	{
		int state = colorProperty.FindPropertyRelative("minMaxState").intValue;
		switch (state)
		{
			//Constant Color
			case 0:
				colorProperty.FindPropertyRelative("maxColor").colorValue = callback(colorProperty.FindPropertyRelative("maxColor").colorValue);
				break;

			//Gradient
			case 1:
				GenericEditGradient(colorProperty.FindPropertyRelative("maxGradient"), callback);
				break;

			//Random between 2 Colors
			case 2:
				colorProperty.FindPropertyRelative("minColor").colorValue = callback(colorProperty.FindPropertyRelative("minColor").colorValue);
				colorProperty.FindPropertyRelative("maxColor").colorValue = callback(colorProperty.FindPropertyRelative("maxColor").colorValue);
				break;

			//Random between 2 Gradients
			case 3:
				GenericEditGradient(colorProperty.FindPropertyRelative("maxGradient"), callback);
				GenericEditGradient(colorProperty.FindPropertyRelative("minGradient"), callback);
				break;
		}
	}
	
	//LIGHTNESS OFFSET ================================================================================================================================
	
	private void addLightness(bool substract)
	{
		if(!TintStartColor && !TintColorModule && !TintColorSpeedModule)
		{
			Debug.LogWarning("Cartoon FX Easy Editor: You must toggle at least one of the three Color Modules to be able to change lightness!");
			return;
		}
		
		float lightness = (float)(LightnessStep/100f);
		if(substract)
			lightness *= -1f;
		
		foreach(GameObject go in Selection.gameObjects)
		{
			ParticleSystem[] systems;
			if(pref_IncludeChildren)
				systems = go.GetComponentsInChildren<ParticleSystem>(true);
			else
				systems = go.GetComponents<ParticleSystem>();
			
			foreach(ParticleSystem ps in systems)
			{
				SerializedObject psSerial = new SerializedObject(ps);
				
				if(TintStartColor)
					GenericAddLightness(psSerial.FindProperty("InitialModule.startColor"), lightness);
				
				if(TintColorModule)
					GenericAddLightness(psSerial.FindProperty("ColorModule.gradient"), lightness);
				
				if(TintColorSpeedModule)
					GenericAddLightness(psSerial.FindProperty("ColorBySpeedModule.gradient"), lightness);
				
				psSerial.ApplyModifiedProperties();
				psSerial.Update();
			}
		}
	}
	
	private void GenericAddLightness(SerializedProperty colorProperty, float lightness)
	{
		int state = colorProperty.FindPropertyRelative("minMaxState").intValue;
		switch(state)
		{
			//Constant Color
		case 0:
			colorProperty.FindPropertyRelative("maxColor").colorValue = HSLColor.FromRGBA(colorProperty.FindPropertyRelative("maxColor").colorValue).ColorWithLightnessOffset(lightness);
			break;
			
			//Gradient
		case 1:
			AddLightnessGradient(colorProperty.FindPropertyRelative("maxGradient"), lightness);
			break;
			
			//Random between 2 Colors
		case 2:
			colorProperty.FindPropertyRelative("minColor").colorValue = HSLColor.FromRGBA(colorProperty.FindPropertyRelative("minColor").colorValue).ColorWithLightnessOffset(lightness);
			colorProperty.FindPropertyRelative("maxColor").colorValue = HSLColor.FromRGBA(colorProperty.FindPropertyRelative("maxColor").colorValue).ColorWithLightnessOffset(lightness);
			break;
			
			//Random between 2 Gradients
		case 3:
			AddLightnessGradient(colorProperty.FindPropertyRelative("maxGradient"), lightness);
			AddLightnessGradient(colorProperty.FindPropertyRelative("minGradient"), lightness);
			break;
		}
	}
	
	private void AddLightnessGradient(SerializedProperty gradientProperty, float lightness)
	{
		gradientProperty.FindPropertyRelative("key0").colorValue = HSLColor.FromRGBA(gradientProperty.FindPropertyRelative("key0").colorValue).ColorWithLightnessOffset(lightness);
		gradientProperty.FindPropertyRelative("key1").colorValue = HSLColor.FromRGBA(gradientProperty.FindPropertyRelative("key1").colorValue).ColorWithLightnessOffset(lightness);
		gradientProperty.FindPropertyRelative("key2").colorValue = HSLColor.FromRGBA(gradientProperty.FindPropertyRelative("key2").colorValue).ColorWithLightnessOffset(lightness);
		gradientProperty.FindPropertyRelative("key3").colorValue = HSLColor.FromRGBA(gradientProperty.FindPropertyRelative("key3").colorValue).ColorWithLightnessOffset(lightness);
		gradientProperty.FindPropertyRelative("key4").colorValue = HSLColor.FromRGBA(gradientProperty.FindPropertyRelative("key4").colorValue).ColorWithLightnessOffset(lightness);
		gradientProperty.FindPropertyRelative("key5").colorValue = HSLColor.FromRGBA(gradientProperty.FindPropertyRelative("key5").colorValue).ColorWithLightnessOffset(lightness);
		gradientProperty.FindPropertyRelative("key6").colorValue = HSLColor.FromRGBA(gradientProperty.FindPropertyRelative("key6").colorValue).ColorWithLightnessOffset(lightness);
		gradientProperty.FindPropertyRelative("key7").colorValue = HSLColor.FromRGBA(gradientProperty.FindPropertyRelative("key7").colorValue).ColorWithLightnessOffset(lightness);
	}
	
	//RGB / HSL Conversions
	private struct HSLColor
	{
		public float h;
		public float s;
		public float l;
		public float a;
		
		public HSLColor(float h, float s, float l, float a)
		{
			this.h = h;
			this.s = s;
			this.l = l;
			this.a = a;
		}
		
		public HSLColor(float h, float s, float l)
		{
			this.h = h;
			this.s = s;
			this.l = l;
			this.a = 1f;
		}
		
		public HSLColor(Color c)
		{
			HSLColor temp = FromRGBA(c);
			h = temp.h;
			s = temp.s;
			l = temp.l;
			a = temp.a;
		}
		
		public static HSLColor FromRGBA(Color c)
		{
			float h, s, l, a;
			a = c.a;
			
			float cmin = Mathf.Min(Mathf.Min(c.r, c.g), c.b);
			float cmax = Mathf.Max(Mathf.Max(c.r, c.g), c.b);
			
			l = (cmin + cmax) / 2f;
			
			if (cmin == cmax)
			{
				s = 0;
				h = 0;
			}
			else
			{
				float delta = cmax - cmin;
				
				s = (l <= .5f) ? (delta / (cmax + cmin)) : (delta / (2f - (cmax + cmin)));
				
				h = 0;
				
				if (c.r == cmax)
				{
					h = (c.g - c.b) / delta;
				}
				else if (c.g == cmax)
				{
					h = 2f + (c.b - c.r) / delta;
				}
				else if (c.b == cmax)
				{
					h = 4f + (c.r - c.g) / delta;
				}
				
				h = Mathf.Repeat(h * 60f, 360f);
			}
			
			return new HSLColor(h, s, l, a);
		}
		
		public Color ToRGBA()
		{
			float r, g, b, a;
			a = this.a;
			
			float m1, m2;
			
			m2 = (l <= .5f) ? (l * (1f + s)) : (l + s - l * s);
			m1 = 2f * l - m2;
			
			if (s == 0f)
			{
				r = g = b = l;
			}
			else
			{
				r = Value(m1, m2, h + 120f);
				g = Value(m1, m2, h);
				b = Value(m1, m2, h - 120f);
			}
			
			return new Color(r, g, b, a);
		}
		
		static float Value(float n1, float n2, float hue)
		{
			hue = Mathf.Repeat(hue, 360f);
			
			if (hue < 60f)
			{
				return n1 + (n2 - n1) * hue / 60f;
			}
			else if (hue < 180f)
			{
				return n2;
			}
			else if (hue < 240f)
			{
				return n1 + (n2 - n1) * (240f - hue) / 60f;
			}
			else
			{
				return n1;
			}
		}
		
		public Color VividColor()
		{
			this.l = 0.5f;
			this.s = 1.0f;
			return this.ToRGBA();
		}
		
		public Color ColorWithHue(float hue, bool shift)
		{
			if(shift)
			{
				this.h += hue;
				while(this.h >= 360f)
					this.h -= 360f;
				while(this.h <= -360f)
					this.h += 360f;
			}
			else
				this.h = hue;

			return this.ToRGBA();
		}

		public Color ColorWithLightnessOffset(float lightness)
		{
			this.l += lightness;
			if(this.l > 1.0f) this.l = 1.0f;
			else if(this.l < 0.0f) this.l = 0.0f;
			
			return this.ToRGBA();
		}

		public Color ColorWithSaturationOffset(float saturation)
		{
			this.s += saturation;
			if(this.s > 1.0f) this.s = 1.0f;
			else if(this.s < 0.0f) this.s = 0.0f;
			
			return this.ToRGBA();
		}
		
		public static implicit operator HSLColor(Color src)
		{
			return FromRGBA(src);
		}
		
		public static implicit operator Color(HSLColor src)
		{
			return src.ToRGBA();
		}
	}
	
	//Scale Lifetime only
	private void applySpeed()
	{
		foreach(GameObject go in Selection.gameObjects)
		{
			ParticleSystem[] systems;
			if(pref_IncludeChildren)
				systems = go.GetComponentsInChildren<ParticleSystem>(true);
			else
				systems = go.GetComponents<ParticleSystem>();
			
			//Scale Lifetime
			foreach(ParticleSystem ps in systems)
			{
#if UNITY_5_5_OR_NEWER
				var main = ps.main;
				main.simulationSpeed = LTScalingValue;
#else
				ps.playbackSpeed = LTScalingValue;
#endif
			}
		}
	}
	
	//Set Duration
	private void applyDuration()
	{
		foreach(GameObject go in Selection.gameObjects)
		{
			//Scale Shuriken Particles Values
			ParticleSystem[] systems;
			if(pref_IncludeChildren)
				systems = go.GetComponentsInChildren<ParticleSystem>(true);
			else
				systems = go.GetComponents<ParticleSystem>();
			
			foreach(ParticleSystem ps in systems)
			{
				SerializedObject so = new SerializedObject(ps);
				so.FindProperty("lengthInSec").floatValue = DurationValue;
				so.ApplyModifiedProperties();
			}
		}
	}
	
	//Change delay
	private void applyDelay()
	{
		foreach(GameObject go in Selection.gameObjects)
		{
			ParticleSystem[] systems;
			if(pref_IncludeChildren)
				systems = go.GetComponentsInChildren<ParticleSystem>(true);
			else
				systems = go.GetComponents<ParticleSystem>();
			
			//Scale Lifetime
			foreach(ParticleSystem ps in systems)
			{
#if UNITY_5_5_OR_NEWER
				var main = ps.main;
				main.startDelay = DelayValue;
#else
				ps.startDelay = DelayValue;
#endif
			}
		}
	}
	
	//Copy Selected Modules
	private void CopyModules(ParticleSystem source, ParticleSystem dest)
	{
		if(source == null)
		{
			Debug.LogWarning("Cartoon FX Easy Editor: Select a source Particle System to copy properties from first!");
			return;
		}
		
		SerializedObject psSource = new SerializedObject(source);
		SerializedObject psDest = new SerializedObject(dest);

		for (int i = 0; i < modulesToCopy.Length; i++)
		{
			var module = modulesToCopy[i];

			if (!selectedModules[i])
			{
				continue;
			}

			var sp = psSource.FindProperty(module.serializedPropertyPath);
			if (sp == null)
			{
				Debug.LogError("Couldn't find module in SerializedObject: " + module.serializedPropertyPath);
				continue;
			}

			psDest.CopyFromSerializedProperty(sp);

			// special case: extra properties for initial module
			if (i == 0)
			{
				foreach (var path in initialModuleExtraProperties)
				{
					var extraSp = psSource.FindProperty(path);
					if (extraSp != null)
					{
						psDest.CopyFromSerializedProperty(extraSp);
					}
				}
			}

			psDest.ApplyModifiedProperties();
		}

		//Renderer
		if (selectedModules[selectedModules.Length-1])
		{
			ParticleSystemRenderer rendSource = source.GetComponent<ParticleSystemRenderer>();
			ParticleSystemRenderer rendDest = dest.GetComponent<ParticleSystemRenderer>();
			UnityEditorInternal.ComponentUtility.CopyComponent(rendSource);
			UnityEditorInternal.ComponentUtility.PasteComponentValues(rendDest);
		}
	}

	/*
	private void MutateModules()
	{
		var systems = pref_IncludeChildren ? Selection.activeGameObject.GetComponentsInChildren<ParticleSystem>() : Selection.activeGameObject.GetComponents<ParticleSystem>();
		var so = new SerializedObject(systems);

		for (int i = 0; i < modulesToCopy.Length; i++)
		{
			if (!selectedModulesMutate[i])
			{
				continue;
			}

			var sp = so.FindProperty(modulesToCopy[i].serializedPropertyPath);
			if (sp != null)
			{
				if (sp.FindPropertyRelative("enabled").boolValue)
				{
					GenericMutateProperty(sp);
					sp.serializedObject.ApplyModifiedProperties();
				}
			}
		}
	}

	private void GenericMutateProperty(SerializedProperty sp, bool recursive = true)
	{
		System.Func<int, int> randomizeInt = intValue =>
		{
			return intValue + Mathf.RoundToInt(intValue * Random.Range(-mutatePercentage, +mutatePercentage));
		};
		System.Func<float, float> randomizeFloat = floatValue =>
		{
			return floatValue + (floatValue * Random.Range(-mutatePercentage, +mutatePercentage));
		};
		System.Func<float, float> randomizeHue = hueValue =>
		{
			hueValue = randomizeFloat(hueValue);
			while (hueValue > 1) hueValue -= 1;
			while (hueValue < 0) hueValue += 1;
			return hueValue;
		};

		//Debug.Log(new string('\t', sp.depth) + sp.name + ": " + sp.type + ", " + sp.propertyType);

		switch (sp.type)
		{
			case "MinMaxCurve":
				GenericMutateProperty(sp.FindPropertyRelative("scalar"), false);
				GenericMutateProperty(sp.FindPropertyRelative("minScalar"), false);
				return;

			case "MinMaxGradient":
				GenericEditColorProperty(sp, color =>
				{
					float h, s, v;
					Color.RGBToHSV(color, out h, out s, out v);
					h = randomizeHue(h);
					return Color.HSVToRGB(h, s, v);
				});
				return;
		}

		switch (sp.propertyType)
		{
			case SerializedPropertyType.Float: sp.floatValue = randomizeFloat(sp.floatValue); break;
			case SerializedPropertyType.Integer: sp.intValue = randomizeInt(sp.intValue); break;
			case SerializedPropertyType.Vector2: sp.vector2Value += new Vector2(randomizeFloat(sp.vector2Value.x), randomizeFloat(sp.vector2Value.y)); break;
			case SerializedPropertyType.Vector3: sp.vector3Value += new Vector3(randomizeFloat(sp.vector3Value.x), randomizeFloat(sp.vector3Value.y), randomizeFloat(sp.vector3Value.z)); break;
			case SerializedPropertyType.Vector4: sp.vector4Value += new Vector4(randomizeFloat(sp.vector4Value.x), randomizeFloat(sp.vector4Value.y), randomizeFloat(sp.vector4Value.z), randomizeFloat(sp.vector4Value.w)); break;
		}

		if (recursive && sp.hasVisibleChildren)
		{
			var endProp = sp.GetEndProperty();
			while (!SerializedProperty.EqualContents(endProp, sp) && sp.NextVisible(true))
			{
				GenericMutateProperty(sp, false);
			}
		}
	}
	*/

	//Scale System
	private void ScaleParticleValues(ParticleSystem ps, GameObject parent)
	{
		//Particle System
		if (ps.gameObject != parent)
		{
			ps.transform.localPosition *= ScalingValue;
		}

		SerializedObject psSerial = new SerializedObject(ps);

		foreach(var path in PropertiesToScale)
		{
			var prop = psSerial.FindProperty(path);
			if(prop != null)
			{
				if(prop.propertyType == SerializedPropertyType.Float)
				{
					prop.floatValue *= ScalingValue;
				}
				else
				{
					Debug.LogWarning("Property in ParticleSystem is not a float: " + path + "\n");
				}
			}
			else
			{
				Debug.LogWarning("Property doesn't exist in ParticleSystem: " + path + "\n");
			}
		}

		//Shape Module special case
		/*
		if(psSerial.FindProperty("ShapeModule.enabled").boolValue)
		{
			//(ShapeModule.type 6 == Mesh)
			if(psSerial.FindProperty("ShapeModule.type").intValue == 6)
			{
				//Unity 4+ : changing the Transform scale will affect the shape Mesh
				ps.transform.localScale = ps.transform.localScale * ScalingValue;
				EditorUtility.SetDirty(ps.transform);
			}
		}
		*/
		
		//Apply Modified Properties
		psSerial.ApplyModifiedPropertiesWithoutUndo();
	}

	//Properties to scale, with per-version differences
	private string[] PropertiesToScale = new string[]
	{
		//Initial
		"InitialModule.startSize.scalar",
		"InitialModule.startSpeed.scalar",
#if UNITY_2017_1_OR_NEWER
		"InitialModule.startSize.minScalar",
		"InitialModule.startSpeed.minScalar",

		"InitialModule.startSizeY.scalar",
		"InitialModule.startSizeY.minScalar",
		"InitialModule.startSizeZ.scalar",
		"InitialModule.startSizeZ.minScalar",
#endif
		//Size by Speed
		"SizeBySpeedModule.range.x",
		"SizeBySpeedModule.range.y",
		//Velocity over Lifetime
		"VelocityModule.x.scalar",
		"VelocityModule.y.scalar",
		"VelocityModule.z.scalar",
#if UNITY_2017_1_OR_NEWER
		"VelocityModule.x.minScalar",
		"VelocityModule.y.minScalar",
		"VelocityModule.z.minScalar",

		"VelocityModule.orbitalX.scalar",
		"VelocityModule.orbitalX.minScalar",
		"VelocityModule.orbitalY.scalar",
		"VelocityModule.orbitalY.minScalar",
		"VelocityModule.orbitalZ.scalar",
		"VelocityModule.orbitalZ.minScalar",

		"VelocityModule.orbitalOffsetX.scalar",
		"VelocityModule.orbitalOffsetX.minScalar",
		"VelocityModule.orbitalOffsetY.scalar",
		"VelocityModule.orbitalOffsetY.minScalar",
		"VelocityModule.orbitalOffsetZ.scalar",
		"VelocityModule.orbitalOffsetZ.minScalar",

		"VelocityModule.radial.scalar",
		"VelocityModule.radial.minScalar",

		"VelocityModule.speedModifier.scalar",
		"VelocityModule.speedModifier.minScalar",

		"InheritVelocityModule.m_Curve.scalar",
		"InheritVelocityModule.m_Curve.minScalar",

		"ExternalForcesModule.multiplier",

		"NoiseModule.strength.scalar",
		"NoiseModule.strength.minScalar",
		"NoiseModule.strengthY.scalar",
		"NoiseModule.strengthY.minScalar",
		"NoiseModule.strengthZ.scalar",
		"NoiseModule.strengthZ.minScalar",
#endif
		//Limit Velocity over Lifetime
		"ClampVelocityModule.x.scalar",
		"ClampVelocityModule.y.scalar",
		"ClampVelocityModule.z.scalar",
#if UNITY_2017_1_OR_NEWER
		"ClampVelocityModule.x.minScalar",
		"ClampVelocityModule.y.minScalar",
		"ClampVelocityModule.z.minScalar",
#endif
		"ClampVelocityModule.magnitude.scalar",
		//Force over Lifetime
		"ForceModule.x.scalar",
		"ForceModule.y.scalar",
		"ForceModule.z.scalar",
#if UNITY_2017_1_OR_NEWER
		"ForceModule.x.minScalar",
		"ForceModule.y.minScalar",
		"ForceModule.z.minScalar",
#endif
		//Special cases per Unity version
#if UNITY_2017_1_OR_NEWER
		"ShapeModule.m_Scale.x",
		"ShapeModule.m_Scale.y",
		"ShapeModule.m_Scale.z",
#else
		"ShapeModule.boxX",
		"ShapeModule.boxY",
		"ShapeModule.boxZ",
#if UNITY_5_6_OR_NEWER
		"ShapeModule.radius.value",
#else
		"ShapeModule.radius",
#endif
#endif
#if UNITY_5_5_OR_NEWER
		"EmissionModule.rateOverDistance.scalar",
		"InitialModule.gravityModifier.scalar",
	#if UNITY_2017_1_OR_NEWER
		"EmissionModule.rateOverDistance.minScalar",
		"InitialModule.gravityModifier.minScalar",
	#endif
#else
		"InitialModule.gravityModifier",
		"EmissionModule.rate.scalar",
#endif
	};

	void GUISeparator()
	{
		GUILayout.Space(4);
		if(EditorGUIUtility.isProSkin)
		{
			GUILine(new Color(.15f, .15f, .15f), 1);
			GUILine(new Color(.4f, .4f, .4f), 1);
		}
		else
		{
			GUILine(new Color(.3f, .3f, .3f), 1);
			GUILine(new Color(.9f, .9f, .9f), 1);
		}
		GUILayout.Space(4);
	}

	static public void GUILine(Color color, float height = 2f)
	{
		Rect position = GUILayoutUtility.GetRect(0f, float.MaxValue, height, height, LineStyle);

		if(Event.current.type == EventType.Repaint)
		{
			Color orgColor = GUI.color;
			GUI.color = orgColor * color;
			LineStyle.Draw(position, false, false, false, false);
			GUI.color = orgColor;
		}
	}

	static public GUIStyle _LineStyle;
	static public GUIStyle LineStyle
	{
		get
		{
			if(_LineStyle == null)
			{
				_LineStyle = new GUIStyle();
				_LineStyle.normal.background = EditorGUIUtility.whiteTexture;
				_LineStyle.stretchWidth = true;
			}

			return _LineStyle;
		}
	}
}
