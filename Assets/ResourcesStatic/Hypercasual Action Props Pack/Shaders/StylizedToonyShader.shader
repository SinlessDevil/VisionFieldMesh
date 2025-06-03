// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/StylizedToonyShader"
{
	Properties
	{
		_MainTex("Texture Image", 2D) = "white" {}
		_Color("Texture Color", Color) = (1,1,1,1)
		_DiffuseColor("Diffuse Color", Color) = (1,1,1,1)
		_SpecColor("Specular Material Color", Color) = (1,1,1,1)
		_Shininess("Specular Shininess", Range(0, 1)) = 0.5
		_SpecIntensity("Specular Intensity", Range(0, 1)) = 1
		_Mask("Mask", 2D) = "white" {}
		_AO("Ambient Occlusion", 2D) = "white" {}
		_ShadowTint("Shadow Tint Color", Color) = (0, 0, 0, 0)
		_RimColor("Rim Color", Color) = (1,1,1,1)
		_RimPower("Rim Power", Range(0, 10)) = 3.0
		[Toggle] _IsUseEmission("Is Use Emission", Float) = 0
		_Emission("Emission (RGB)", 2D) = "white" {}
		_IlluminPower("Illumination Power", Float) = 1
	}

		SubShader
		{
			Pass
			{
				Tags { "LightMode" = "ForwardBase" } // pass for first light source

				CGPROGRAM

				#pragma vertex vert  
				#pragma fragment frag
				#pragma multi_compile_fog
				#pragma multi_compile_fwdbase

				#include "UnityCG.cginc"
				#include "AutoLight.cginc"

				uniform sampler2D _MainTex; // Main Texture
				uniform float4 _Color; // Texture Color

				uniform float4 _LightColor0; // color of light source (from "Lighting.cginc")

				uniform float4 _DiffuseColor; // Diffuse Color

				uniform float4 _SpecColor; // Specular Color
				uniform float _Shininess; // Specular Shininess
				uniform float _SpecIntensity; // Specular Intensity

				uniform sampler2D _Mask;
				uniform sampler2D _AO;

				uniform float4 _RimColor;
				uniform float _RimPower;
				uniform float4 _ShadowTint;

				sampler2D _Emission; //Emission Texture
				float _IsUseEmission;
				float _IlluminPower;


				struct vertexInput {
					float4 vertex : POSITION;
					float3 normal : NORMAL;
					float2 uvMain : TEXCOORD0;
					float2 uvAO : TEXCOORD1;
					float2 uvIllum : TEXCOORD2;
					float2 texcoord1 : TEXCOORD3;
				};

				struct vertexOutput {
					float4 pos : SV_POSITION;
					float2 uv1 : TEXCOORD1;
					float4 posWorld : TEXCOORD2;
					float2 uvMain : TEXCOORD3;
					float2 uvAO : TEXCOORD4;
					float3 normalDir : TEXCOORD5;
					float2 uvIllum : TEXCOORD6;
					UNITY_FOG_COORDS(7)
					SHADOW_COORDS(8)
				};

					float4 _MainTex_ST;


				vertexOutput vert(vertexInput v) {
					vertexOutput output;

					float4x4 modelMatrix = unity_ObjectToWorld;
					float4x4 modelMatrixInverse = unity_WorldToObject;

					output.posWorld = mul(modelMatrix, v.vertex);
					output.normalDir = normalize(mul(float4(v.normal, 0.0), modelMatrixInverse).xyz);
					output.pos = UnityObjectToClipPos(v.vertex);

					output.uvMain = TRANSFORM_TEX(v.uvMain, _MainTex);
					output.uvAO = v.uvAO;
					output.uvIllum = v.uvIllum;
					output.uv1 = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
					UNITY_TRANSFER_FOG(output,output.pos);
					TRANSFER_SHADOW(output);

					return output;
				}

				float4 frag(vertexOutput input) : COLOR
				{
					float3 normalDirection = normalize(input.normalDir);

					float3 viewDirection = normalize(_WorldSpaceCameraPos - input.posWorld.xyz);
					float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
					float attenuation = 1.0;
					half3 lightmap = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, input.uv1));

#if LIGHTMAP_ON
					half shadow = SHADOW_ATTENUATION(input) *lightmap.r;
					//half shadow = SHADOW_ATTENUATION(input);
#else
					half shadow = SHADOW_ATTENUATION(input);
#endif

					float4 clearColor = tex2D(_MainTex, input.uvMain) * _Color * shadow;

					float3 diffuseReflection = lerp(_DiffuseColor.rgb, float3(1.0, 1.0, 1.0),
						clamp(attenuation * max(0.0, dot(normalDirection, lightDirection)), 0.0, 1.0));

					float3 specularReflection;

					if (dot(normalDirection, lightDirection) < 0.0) {
						specularReflection = float3(0.0, 0.0, 0.0);
					}
					else {
						specularReflection = attenuation
							* _SpecColor.rgb * _SpecIntensity * _LightColor0.rgb * pow(max(0.0, dot(
								reflect(-lightDirection, normalDirection),
								viewDirection)), (_Shininess * _Shininess * 100 + 1));
					}

					float4 spec = float4(specularReflection, 1.0);


					float4 diffuse = float4(diffuseReflection, 1.0);
					half3 ao = tex2D(_AO, input.uvAO);
					half3 mask = tex2D(_Mask, input.uvAO);

					float rim = 1 - saturate(dot(normalize(viewDirection), normalDirection));
					float4 rimLighting = float4(attenuation * _LightColor0.xyz * _RimColor * pow(rim, _RimPower), 1);

					clearColor.rgb *= ao.rgb;

					half3 emisColor = tex2D(_Emission, input.uvIllum).rgb;



					float4 resultColor = lerp(_ShadowTint, (clearColor * _LightColor0 + (rimLighting + spec) * mask.r) * diffuse, shadow);
					if (_IsUseEmission == 1) {
						resultColor.rgb += emisColor.rgb * _IlluminPower;
					}
					UNITY_APPLY_FOG(input.fogCoord, resultColor);

					return resultColor;
			  }

			  ENDCG
		  }

		  UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
		}

			Fallback "Diffuse"
}