Shader "XDreamer/Standard Triplanar with Cut"
{
	Properties
	{
		[Enum(UnityEngine.Rendering.CullMode)] _Cull("Cull mode", Float) = 0
		_BlendFactor("Blend Factor", Range(0.01, 10)) = 1
		[KeywordEnum(Default, Rotate90)] _Y90("Y plane Orientation", Float) = 0

		[Space(20)]
		[NoScaleOffset]
		_Color("Albedo Color", Color) = (1, 1, 1, 1)
		[NoScaleOffset]
		_MainTex("Albedo Map (RGB=Base A=smoothness)", 2D) = "white" {}
		[NoScaleOffset]
		_MetallicMap("Metallic Map (R=Metallic G=smoothness)", 2D) = "white" {}
		_MainMetalTiling("Base and Metallic Tiling", Float) = 1
		_Metallic("Metallic", Range(0,1)) = 1
		[Space(20)]
		[KeywordEnum(DiffuseAlpha, MetallicGreen)] _SmoothnessSource("Smoothness Source", Float) = 0
		_Glossy("Smoothness", Range(0,1)) = 0.5
		[Space(20)]
		[NoScaleOffset]
		_BumpMap("Normalmap", 2D) = "bump" {}
		_NormalTiling("Normal Map Tiling", Float) = 1
		_NormalGain("Normal Gain", Range(0,2)) = 1.0
		
		
	}

		#LINE 16


			SubShader{
				Tags { "RenderType" = "Opaque" "Queue" = "Geometry" }
				Cull [_Cull]
				LOD 200


			// ------------------------------------------------------------
			// Surface shader code generated out of a CGPROGRAM block:


			// ---- forward rendering base pass:
			Pass {
				Name "FORWARD"
				Tags { "LightMode" = "ForwardBase" }

		CGPROGRAM
			// compile directives
			#pragma multi_compile _SMOOTHNESSSOURCE_DIFFUSEALPHA _SMOOTHNESSSOURCE_METALLICGREEN
			#pragma multi_compile _Y90_DEFAULT _Y90_ROTATE90
			#pragma vertex vert_surf
			#pragma fragment frag_surf
			#pragma target 3.0
			#pragma multi_compile_instancing
			#pragma multi_compile_fog
			#pragma multi_compile_fwdbase
			#include "HLSLSupport.cginc"
			#define UNITY_INSTANCED_LOD_FADE
			#define UNITY_INSTANCED_SH
			#define UNITY_INSTANCED_LIGHTMAPSTS
			#include "UnityShaderVariables.cginc"
			#include "UnityShaderUtilities.cginc"
			// -------- variant for: <when no other keywords are defined>
			#if !defined(INSTANCING_ON)
			// Surface shader code generated based on:
			// vertex modifier: 'vert'
			// writes to per-pixel normal: YES
			// writes to emission: YES
			// writes to occlusion: no
			// needs world space reflection vector: no
			// needs world space normal vector: no
			// needs screen space position: no
			// needs world space position: YES
			// needs view direction: no
			// needs world space view direction: no
			// needs world space position for lighting: YES
			// needs world space view direction for lighting: YES
			// needs world space view direction for lightmaps: no
			// needs vertex color: no
			// needs VFACE: no
			// passes tangent-to-world matrix to pixel shader: YES
			// reads from normal: no
			// 0 texcoords actually used
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			#include "AutoLight.cginc"

			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) fixed3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))

			// Original surface shader snippet:
			#line 15

						#define UNITY_SETUP_BRDF_INPUT MetallicSetup
					#line 20 ""
			#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
			#endif
			/* UNITY: Original start of shader */
					//#pragma surface surf Standard vertex:vert fullforwardshadows
					//#pragma target 3.0
					#include "UnityStandardUtils.cginc"

					uniform float4 _planeEquation;
					uniform int _cuttingOn;

					fixed4 _Color;
					sampler2D _MainTex, _BumpMap;
					half _MainMetalTiling;
					half _NormalTiling;
					half _Glossy;
					half _NormalGain;
					half _Metallic;
					half _BlendFactor;

					sampler2D _MetallicMap;

					struct Input
					{
						float3 localCoord;
						float3 localNormal;
						float3 worldPos;
						float3 worldNormal;
						INTERNAL_DATA
					};

					void vert(inout appdata_full v, out Input o)
					{
						UNITY_INITIALIZE_OUTPUT(Input, o);
						o.localCoord = v.vertex.xyz;
						o.localNormal = v.normal.xyz;
					}


					void surf(Input IN, inout SurfaceOutputStandard o)
					{
						if (_cuttingOn)
							clip(dot(IN.worldPos, _planeEquation.xyz) - _planeEquation.w);

						float3 bf = normalize(abs(IN.localNormal));
						bf = pow(bf, _BlendFactor);
						bf /= max(dot(bf, (float3)1), 0.0001);
						

						// Triplanar mapping for base and metallic
						float2 mmtx = IN.localCoord.zy * _MainMetalTiling;
						#if _Y90_DEFAULT
							float2 mmty = IN.localCoord.zx * _MainMetalTiling;
						#else
							float2 mmty = IN.localCoord.xz * _MainMetalTiling;
						#endif
						float2 mmtz = IN.localCoord.xy * _MainMetalTiling;

						//base
						fixed4 cz = tex2D(_MainTex, mmtx) * bf.x;
						fixed4 cy = tex2D(_MainTex, mmty) * bf.y;
						fixed4 cx = tex2D(_MainTex, mmtz) * bf.z;
						fixed4 col = (cz + cy + cx) * _Color;
						o.Albedo = col.rgb;

						//metallic
						fixed4 mz = tex2D(_MetallicMap, mmtx) * bf.x;
						fixed4 my = tex2D(_MetallicMap, mmty) * bf.y;
						fixed4 mx = tex2D(_MetallicMap, mmtz) * bf.z;
						fixed4 met = (mz + my + mx) * _Metallic;
						o.Metallic = met.r;

						#if _SMOOTHNESSSOURCE_DIFFUSEALPHA
							o.Smoothness = col.a * _Glossy;
						#elif _SMOOTHNESSSOURCE_METALLICGREEN
							o.Smoothness = met.g * _Glossy;
						#else
							o.Smoothness =  _Glossy;
						#endif

						o.Alpha = col.a;

						// Triplanar mapping for normal
						float2 ntx = IN.localCoord.zy * _NormalTiling;
						#if _Y90_DEFAULT
							float2 nty = IN.localCoord.zx * _NormalTiling;
						#else
							float2 nty = IN.localCoord.xz * _NormalTiling;
						#endif
						float2 ntz = IN.localCoord.xy * _NormalTiling;

						fixed3 tnormalX = UnpackScaleNormal(tex2D(_BumpMap, ntx), _NormalGain) * bf.x;
						fixed3 tnormalY = UnpackScaleNormal(tex2D(_BumpMap, nty), _NormalGain) * bf.y;
						fixed3 tnormalZ = UnpackScaleNormal(tex2D(_BumpMap, ntz), _NormalGain) * bf.z;

						o.Normal = tnormalX + tnormalY + tnormalZ;

						if (_cuttingOn) {
							if (dot(IN.worldPos, _planeEquation.xyz) - _planeEquation.w < 0.004) {
								o.Albedo = fixed3(1, 0, 0);
								o.Emission = fixed3(1, 0, 0);
							}
						}
					}


					// vertex-to-fragment interpolation data
					// no lightmaps:
					#ifndef LIGHTMAP_ON
					// half-precision fragment shader registers:
					#ifdef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
					#define FOG_COMBINED_WITH_TSPACE
					struct v2f_surf {
					  UNITY_POSITION(pos);
					  float4 tSpace0 : TEXCOORD0;
					  float4 tSpace1 : TEXCOORD1;
					  float4 tSpace2 : TEXCOORD2;
					  float3 custompack0 : TEXCOORD3; // localCoord
					  float3 custompack1 : TEXCOORD4; // localNormal
					  #if UNITY_SHOULD_SAMPLE_SH
					  half3 sh : TEXCOORD5; // SH
					  #endif
					  UNITY_LIGHTING_COORDS(6,7)
					  #if SHADER_TARGET >= 30
					  float4 lmap : TEXCOORD8;
					  #endif
					  UNITY_VERTEX_INPUT_INSTANCE_ID
					  UNITY_VERTEX_OUTPUT_STEREO
					};
					#endif
					// high-precision fragment shader registers:
					#ifndef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
					struct v2f_surf {
					  UNITY_POSITION(pos);
					  float4 tSpace0 : TEXCOORD0;
					  float4 tSpace1 : TEXCOORD1;
					  float4 tSpace2 : TEXCOORD2;
					  float3 custompack0 : TEXCOORD3; // localCoord
					  float3 custompack1 : TEXCOORD4; // localNormal
					  #if UNITY_SHOULD_SAMPLE_SH
					  half3 sh : TEXCOORD5; // SH
					  #endif
					  UNITY_FOG_COORDS(6)
					  UNITY_SHADOW_COORDS(7)
					  #if SHADER_TARGET >= 30
					  float4 lmap : TEXCOORD8;
					  #endif
					  UNITY_VERTEX_INPUT_INSTANCE_ID
					  UNITY_VERTEX_OUTPUT_STEREO
					};
					#endif
					#endif
					// with lightmaps:
					#ifdef LIGHTMAP_ON
					// half-precision fragment shader registers:
					#ifdef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
					#define FOG_COMBINED_WITH_TSPACE
					struct v2f_surf {
					  UNITY_POSITION(pos);
					  float4 tSpace0 : TEXCOORD0;
					  float4 tSpace1 : TEXCOORD1;
					  float4 tSpace2 : TEXCOORD2;
					  float3 custompack0 : TEXCOORD3; // localCoord
					  float3 custompack1 : TEXCOORD4; // localNormal
					  float4 lmap : TEXCOORD5;
					  UNITY_LIGHTING_COORDS(6,7)
					  UNITY_VERTEX_INPUT_INSTANCE_ID
					  UNITY_VERTEX_OUTPUT_STEREO
					};
					#endif
					// high-precision fragment shader registers:
					#ifndef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
					struct v2f_surf {
					  UNITY_POSITION(pos);
					  float4 tSpace0 : TEXCOORD0;
					  float4 tSpace1 : TEXCOORD1;
					  float4 tSpace2 : TEXCOORD2;
					  float3 custompack0 : TEXCOORD3; // localCoord
					  float3 custompack1 : TEXCOORD4; // localNormal
					  float4 lmap : TEXCOORD5;
					  UNITY_FOG_COORDS(6)
					  UNITY_SHADOW_COORDS(7)
					  UNITY_VERTEX_INPUT_INSTANCE_ID
					  UNITY_VERTEX_OUTPUT_STEREO
					};
					#endif
					#endif

					// vertex shader
					v2f_surf vert_surf(appdata_full v) {
					  UNITY_SETUP_INSTANCE_ID(v);
					  v2f_surf o;
					  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
					  UNITY_TRANSFER_INSTANCE_ID(v,o);
					  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
					  Input customInputData;
					  vert(v, customInputData);
					  o.custompack0.xyz = customInputData.localCoord;
					  o.custompack1.xyz = customInputData.localNormal;
					  o.pos = UnityObjectToClipPos(v.vertex);
					  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
					  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
					  //fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
					  //fixed tangentSign =  v.tangent.w * unity_WorldTransformParams.w;
					  //fixed3 worldBinormal = cross(worldNormal, worldTangent) *tangentSign;

					  //fake tangents
					  fixed3 worldTangent = cross(worldNormal, fixed3(0,0,1));
					  float val = length(worldTangent);
					  if (val <0.0001)
					  {
						  worldTangent = cross(worldNormal, fixed3(1, 0, 0));
					  }
					  fixed3 worldBinormal = cross(worldNormal, worldTangent);
					  //end fake tangents

					  o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
					  o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
					  o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
					  #ifdef DYNAMICLIGHTMAP_ON
					  o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
					  #endif
					  #ifdef LIGHTMAP_ON
					  o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
					  #endif

					  // SH/ambient and vertex lights
					  #ifndef LIGHTMAP_ON
						#if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
						  o.sh = 0;
						  // Approximated illumination from non-important point lights
						  #ifdef VERTEXLIGHT_ON
							o.sh += Shade4PointLights(
							  unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
							  unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
							  unity_4LightAtten0, worldPos, worldNormal);
						  #endif
						  o.sh = ShadeSHPerVertex(worldNormal, o.sh);
						#endif
					  #endif // !LIGHTMAP_ON

					  UNITY_TRANSFER_LIGHTING(o,v.texcoord1.xy); // pass shadow and, possibly, light cookie coordinates to pixel shader
					  #ifdef FOG_COMBINED_WITH_TSPACE
						UNITY_TRANSFER_FOG_COMBINED_WITH_TSPACE(o,o.pos); // pass fog coordinates to pixel shader
					  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
						UNITY_TRANSFER_FOG_COMBINED_WITH_WORLD_POS(o,o.pos); // pass fog coordinates to pixel shader
					  #else
						UNITY_TRANSFER_FOG(o,o.pos); // pass fog coordinates to pixel shader
					  #endif
					  return o;
					}

					// fragment shader
					fixed4 frag_surf(v2f_surf IN) : SV_Target {
					  UNITY_SETUP_INSTANCE_ID(IN);
					// prepare and unpack data
					Input surfIN;
					#ifdef FOG_COMBINED_WITH_TSPACE
					  UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
					#elif defined (FOG_COMBINED_WITH_WORLD_POS)
					  UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
					#else
					  UNITY_EXTRACT_FOG(IN);
					#endif
					#ifdef FOG_COMBINED_WITH_TSPACE
					  UNITY_RECONSTRUCT_TBN(IN);
					#else
					  UNITY_EXTRACT_TBN(IN);
					#endif
					UNITY_INITIALIZE_OUTPUT(Input,surfIN);
					surfIN.localCoord.x = 1.0;
					surfIN.localNormal.x = 1.0;
					surfIN.worldPos.x = 1.0;
					surfIN.worldNormal.x = 1.0;
					surfIN.localCoord = IN.custompack0.xyz;
					surfIN.localNormal = IN.custompack1.xyz;
					float3 worldPos = float3(IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w);
					#ifndef USING_DIRECTIONAL_LIGHT
					  fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
					#else
					  fixed3 lightDir = _WorldSpaceLightPos0.xyz;
					#endif
					float3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));
					surfIN.worldPos = worldPos;
					#ifdef UNITY_COMPILER_HLSL
					SurfaceOutputStandard o = (SurfaceOutputStandard)0;
					#else
					SurfaceOutputStandard o;
					#endif
					o.Albedo = 0.0;
					o.Emission = 0.0;
					o.Alpha = 0.0;
					o.Occlusion = 1.0;
					fixed3 normalWorldVertex = fixed3(0,0,1);
					o.Normal = fixed3(0,0,1);

					// call surface function
					surf(surfIN, o);

					// compute lighting & shadowing factor
					UNITY_LIGHT_ATTENUATION(atten, IN, worldPos)
					fixed4 c = 0;
					float3 worldN;
					worldN.x = dot(_unity_tbn_0, o.Normal);
					worldN.y = dot(_unity_tbn_1, o.Normal);
					worldN.z = dot(_unity_tbn_2, o.Normal);
					worldN = normalize(worldN);
					o.Normal = worldN;

					// Setup lighting environment
					UnityGI gi;
					UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
					gi.indirect.diffuse = 0;
					gi.indirect.specular = 0;
					gi.light.color = _LightColor0.rgb;
					gi.light.dir = lightDir;
					// Call GI (lightmaps/SH/reflections) lighting function
					UnityGIInput giInput;
					UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
					giInput.light = gi.light;
					giInput.worldPos = worldPos;
					giInput.worldViewDir = worldViewDir;
					giInput.atten = atten;
					#if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
					  giInput.lightmapUV = IN.lmap;
					#else
					  giInput.lightmapUV = 0.0;
					#endif
					#if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
					  giInput.ambient = IN.sh;
					#else
					  giInput.ambient.rgb = 0.0;
					#endif
					giInput.probeHDR[0] = unity_SpecCube0_HDR;
					giInput.probeHDR[1] = unity_SpecCube1_HDR;
					#if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
					  giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
					#endif
					#ifdef UNITY_SPECCUBE_BOX_PROJECTION
					  giInput.boxMax[0] = unity_SpecCube0_BoxMax;
					  giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
					  giInput.boxMax[1] = unity_SpecCube1_BoxMax;
					  giInput.boxMin[1] = unity_SpecCube1_BoxMin;
					  giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
					#endif
					LightingStandard_GI(o, giInput, gi);

					// realtime lighting: call lighting function
					c += LightingStandard(o, worldViewDir, gi);
					c.rgb += o.Emission;
					UNITY_APPLY_FOG(_unity_fogCoord, c); // apply fog
					UNITY_OPAQUE_ALPHA(c.a);
					return c;
				  }


				  #endif

						// -------- variant for: INSTANCING_ON 
						#if defined(INSTANCING_ON)
						// Surface shader code generated based on:
						// vertex modifier: 'vert'
						// writes to per-pixel normal: YES
						// writes to emission: YES
						// writes to occlusion: no
						// needs world space reflection vector: no
						// needs world space normal vector: no
						// needs screen space position: no
						// needs world space position: YES
						// needs view direction: no
						// needs world space view direction: no
						// needs world space position for lighting: YES
						// needs world space view direction for lighting: YES
						// needs world space view direction for lightmaps: no
						// needs vertex color: no
						// needs VFACE: no
						// passes tangent-to-world matrix to pixel shader: YES
						// reads from normal: no
						// 0 texcoords actually used
						#include "UnityCG.cginc"
						#include "Lighting.cginc"
						#include "UnityPBSLighting.cginc"
						#include "AutoLight.cginc"

						#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
						#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
						#define WorldNormalVector(data,normal) fixed3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))

						// Original surface shader snippet:
						#line 15

									#define UNITY_SETUP_BRDF_INPUT MetallicSetup
								#line 20 ""
						#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
						#endif
						/* UNITY: Original start of shader */
								//#pragma surface surf Standard vertex:vert fullforwardshadows
								//#pragma target 3.0
								#include "UnityStandardUtils.cginc"

								uniform float4 _planeEquation;
								uniform int _cuttingOn;

								fixed4 _Color;
								sampler2D _MainTex, _BumpMap;
								half _MainMetalTiling;
								half _NormalTiling;
								half _Glossy;
								half _NormalGain;
								half _Metallic;
								half _BlendFactor;

								sampler2D _MetallicMap;

								struct Input
								{
									float3 localCoord;
									float3 localNormal;
									float3 worldPos;
									float3 worldNormal;
									INTERNAL_DATA
								};

								void vert(inout appdata_full v, out Input o)
								{
									UNITY_INITIALIZE_OUTPUT(Input, o);
									o.localCoord = v.vertex.xyz;
									o.localNormal = v.normal.xyz;
								}

								
								void surf(Input IN, inout SurfaceOutputStandard o)
								{
									if (_cuttingOn)
										clip(dot(IN.worldPos, _planeEquation.xyz) - _planeEquation.w);

									float3 bf = normalize(abs(IN.localNormal));
									bf = pow(bf, _BlendFactor);
									bf /= max(dot(bf, (float3)1), 0.0001);


									// Triplanar mapping for base and metallic
									float2 mmtx = IN.localCoord.zy * _MainMetalTiling;
#if _Y90_DEFAULT
									float2 mmty = IN.localCoord.zx * _MainMetalTiling;
#else
									float2 mmty = IN.localCoord.xz * _MainMetalTiling;
#endif
									float2 mmtz = IN.localCoord.xy * _MainMetalTiling;

									//base
									fixed4 cz = tex2D(_MainTex, mmtx) * bf.x;
									fixed4 cy = tex2D(_MainTex, mmty) * bf.y;
									fixed4 cx = tex2D(_MainTex, mmtz) * bf.z;
									fixed4 col = (cz + cy + cx) * _Color;
									o.Albedo = col.rgb;

									//metallic
									fixed4 mz = tex2D(_MetallicMap, mmtx) * bf.x;
									fixed4 my = tex2D(_MetallicMap, mmty) * bf.y;
									fixed4 mx = tex2D(_MetallicMap, mmtz) * bf.z;
									fixed4 met = (mz + my + mx) * _Metallic;
									o.Metallic = met.r;

#if _SMOOTHNESSSOURCE_DIFFUSEALPHA
									o.Smoothness = col.a * _Glossy;
#elif _SMOOTHNESSSOURCE_METALLICGREEN
									o.Smoothness = met.g * _Glossy;
#else
									o.Smoothness = _Glossy;
#endif

									o.Alpha = col.a;

									// Triplanar mapping for normal
									float2 ntx = IN.localCoord.zy * _NormalTiling;
#if _Y90_DEFAULT
									float2 nty = IN.localCoord.zx * _NormalTiling;
#else
									float2 nty = IN.localCoord.xz * _NormalTiling;
#endif
									float2 ntz = IN.localCoord.xy * _NormalTiling;

									fixed3 tnormalX = UnpackScaleNormal(tex2D(_BumpMap, ntx), _NormalGain) * bf.x;
									fixed3 tnormalY = UnpackScaleNormal(tex2D(_BumpMap, nty), _NormalGain) * bf.y;
									fixed3 tnormalZ = UnpackScaleNormal(tex2D(_BumpMap, ntz), _NormalGain) * bf.z;

									o.Normal = tnormalX + tnormalY + tnormalZ;

									if (_cuttingOn) {
										if (dot(IN.worldPos, _planeEquation.xyz) - _planeEquation.w < 0.004) {
											o.Albedo = fixed3(1, 0, 0);
											o.Emission = fixed3(1, 0, 0);
										}
									}
								}


								// vertex-to-fragment interpolation data
								// no lightmaps:
								#ifndef LIGHTMAP_ON
								// half-precision fragment shader registers:
								#ifdef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
								#define FOG_COMBINED_WITH_TSPACE
								struct v2f_surf {
								  UNITY_POSITION(pos);
								  float4 tSpace0 : TEXCOORD0;
								  float4 tSpace1 : TEXCOORD1;
								  float4 tSpace2 : TEXCOORD2;
								  float3 custompack0 : TEXCOORD3; // localCoord
								  float3 custompack1 : TEXCOORD4; // localNormal
								  #if UNITY_SHOULD_SAMPLE_SH
								  half3 sh : TEXCOORD5; // SH
								  #endif
								  UNITY_LIGHTING_COORDS(6,7)
								  #if SHADER_TARGET >= 30
								  float4 lmap : TEXCOORD8;
								  #endif
								  UNITY_VERTEX_INPUT_INSTANCE_ID
								  UNITY_VERTEX_OUTPUT_STEREO
								};
								#endif
								// high-precision fragment shader registers:
								#ifndef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
								struct v2f_surf {
								  UNITY_POSITION(pos);
								  float4 tSpace0 : TEXCOORD0;
								  float4 tSpace1 : TEXCOORD1;
								  float4 tSpace2 : TEXCOORD2;
								  float3 custompack0 : TEXCOORD3; // localCoord
								  float3 custompack1 : TEXCOORD4; // localNormal
								  #if UNITY_SHOULD_SAMPLE_SH
								  half3 sh : TEXCOORD5; // SH
								  #endif
								  UNITY_FOG_COORDS(6)
								  UNITY_SHADOW_COORDS(7)
								  #if SHADER_TARGET >= 30
								  float4 lmap : TEXCOORD8;
								  #endif
								  UNITY_VERTEX_INPUT_INSTANCE_ID
								  UNITY_VERTEX_OUTPUT_STEREO
								};
								#endif
								#endif
								// with lightmaps:
								#ifdef LIGHTMAP_ON
								// half-precision fragment shader registers:
								#ifdef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
								#define FOG_COMBINED_WITH_TSPACE
								struct v2f_surf {
								  UNITY_POSITION(pos);
								  float4 tSpace0 : TEXCOORD0;
								  float4 tSpace1 : TEXCOORD1;
								  float4 tSpace2 : TEXCOORD2;
								  float3 custompack0 : TEXCOORD3; // localCoord
								  float3 custompack1 : TEXCOORD4; // localNormal
								  float4 lmap : TEXCOORD5;
								  UNITY_LIGHTING_COORDS(6,7)
								  UNITY_VERTEX_INPUT_INSTANCE_ID
								  UNITY_VERTEX_OUTPUT_STEREO
								};
								#endif
								// high-precision fragment shader registers:
								#ifndef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
								struct v2f_surf {
								  UNITY_POSITION(pos);
								  float4 tSpace0 : TEXCOORD0;
								  float4 tSpace1 : TEXCOORD1;
								  float4 tSpace2 : TEXCOORD2;
								  float3 custompack0 : TEXCOORD3; // localCoord
								  float3 custompack1 : TEXCOORD4; // localNormal
								  float4 lmap : TEXCOORD5;
								  UNITY_FOG_COORDS(6)
								  UNITY_SHADOW_COORDS(7)
								  UNITY_VERTEX_INPUT_INSTANCE_ID
								  UNITY_VERTEX_OUTPUT_STEREO
								};
								#endif
								#endif

								// vertex shader
								v2f_surf vert_surf(appdata_full v) {
								  UNITY_SETUP_INSTANCE_ID(v);
								  v2f_surf o;
								  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
								  UNITY_TRANSFER_INSTANCE_ID(v,o);
								  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
								  Input customInputData;
								  vert(v, customInputData);
								  o.custompack0.xyz = customInputData.localCoord;
								  o.custompack1.xyz = customInputData.localNormal;
								  o.pos = UnityObjectToClipPos(v.vertex);
								  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
								  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
								  //fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
								  //fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
								 // fixed3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
								  //fake tangents
								  fixed3 worldTangent = cross(worldNormal, fixed3(0, 0, 1));
								  float val = length(worldTangent);
								  if (val < 0.0001)
								  {
									  worldTangent = cross(worldNormal, fixed3(1, 0, 0));
								  }
								  fixed3 worldBinormal = cross(worldNormal, worldTangent);
								  //end fake tangents
								  o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
								  o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
								  o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
								  #ifdef DYNAMICLIGHTMAP_ON
								  o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
								  #endif
								  #ifdef LIGHTMAP_ON
								  o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
								  #endif

								  // SH/ambient and vertex lights
								  #ifndef LIGHTMAP_ON
									#if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
									  o.sh = 0;
									  // Approximated illumination from non-important point lights
									  #ifdef VERTEXLIGHT_ON
										o.sh += Shade4PointLights(
										  unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
										  unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
										  unity_4LightAtten0, worldPos, worldNormal);
									  #endif
									  o.sh = ShadeSHPerVertex(worldNormal, o.sh);
									#endif
								  #endif // !LIGHTMAP_ON

								  UNITY_TRANSFER_LIGHTING(o,v.texcoord1.xy); // pass shadow and, possibly, light cookie coordinates to pixel shader
								  #ifdef FOG_COMBINED_WITH_TSPACE
									UNITY_TRANSFER_FOG_COMBINED_WITH_TSPACE(o,o.pos); // pass fog coordinates to pixel shader
								  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
									UNITY_TRANSFER_FOG_COMBINED_WITH_WORLD_POS(o,o.pos); // pass fog coordinates to pixel shader
								  #else
									UNITY_TRANSFER_FOG(o,o.pos); // pass fog coordinates to pixel shader
								  #endif
								  return o;
								}

								// fragment shader
								fixed4 frag_surf(v2f_surf IN) : SV_Target {
								  UNITY_SETUP_INSTANCE_ID(IN);
								// prepare and unpack data
								Input surfIN;
								#ifdef FOG_COMBINED_WITH_TSPACE
								  UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
								#elif defined (FOG_COMBINED_WITH_WORLD_POS)
								  UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
								#else
								  UNITY_EXTRACT_FOG(IN);
								#endif
								#ifdef FOG_COMBINED_WITH_TSPACE
								  UNITY_RECONSTRUCT_TBN(IN);
								#else
								  UNITY_EXTRACT_TBN(IN);
								#endif
								UNITY_INITIALIZE_OUTPUT(Input,surfIN);
								surfIN.localCoord.x = 1.0;
								surfIN.localNormal.x = 1.0;
								surfIN.worldPos.x = 1.0;
								surfIN.worldNormal.x = 1.0;
								surfIN.localCoord = IN.custompack0.xyz;
								surfIN.localNormal = IN.custompack1.xyz;
								float3 worldPos = float3(IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w);
								#ifndef USING_DIRECTIONAL_LIGHT
								  fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
								#else
								  fixed3 lightDir = _WorldSpaceLightPos0.xyz;
								#endif
								float3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));
								surfIN.worldPos = worldPos;
								#ifdef UNITY_COMPILER_HLSL
								SurfaceOutputStandard o = (SurfaceOutputStandard)0;
								#else
								SurfaceOutputStandard o;
								#endif
								o.Albedo = 0.0;
								o.Emission = 0.0;
								o.Alpha = 0.0;
								o.Occlusion = 1.0;
								fixed3 normalWorldVertex = fixed3(0,0,1);
								o.Normal = fixed3(0,0,1);

								// call surface function
								surf(surfIN, o);

								// compute lighting & shadowing factor
								UNITY_LIGHT_ATTENUATION(atten, IN, worldPos)
								fixed4 c = 0;
								float3 worldN;
								worldN.x = dot(_unity_tbn_0, o.Normal);
								worldN.y = dot(_unity_tbn_1, o.Normal);
								worldN.z = dot(_unity_tbn_2, o.Normal);
								worldN = normalize(worldN);
								o.Normal = worldN;

								// Setup lighting environment
								UnityGI gi;
								UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
								gi.indirect.diffuse = 0;
								gi.indirect.specular = 0;
								gi.light.color = _LightColor0.rgb;
								gi.light.dir = lightDir;
								// Call GI (lightmaps/SH/reflections) lighting function
								UnityGIInput giInput;
								UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
								giInput.light = gi.light;
								giInput.worldPos = worldPos;
								giInput.worldViewDir = worldViewDir;
								giInput.atten = atten;
								#if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
								  giInput.lightmapUV = IN.lmap;
								#else
								  giInput.lightmapUV = 0.0;
								#endif
								#if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
								  giInput.ambient = IN.sh;
								#else
								  giInput.ambient.rgb = 0.0;
								#endif
								giInput.probeHDR[0] = unity_SpecCube0_HDR;
								giInput.probeHDR[1] = unity_SpecCube1_HDR;
								#if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
								  giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
								#endif
								#ifdef UNITY_SPECCUBE_BOX_PROJECTION
								  giInput.boxMax[0] = unity_SpecCube0_BoxMax;
								  giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
								  giInput.boxMax[1] = unity_SpecCube1_BoxMax;
								  giInput.boxMin[1] = unity_SpecCube1_BoxMin;
								  giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
								#endif
								LightingStandard_GI(o, giInput, gi);

								// realtime lighting: call lighting function
								c += LightingStandard(o, worldViewDir, gi);
								c.rgb += o.Emission;
								UNITY_APPLY_FOG(_unity_fogCoord, c); // apply fog
								UNITY_OPAQUE_ALPHA(c.a);
								return c;
							  }


							  #endif


							  ENDCG

							  }

			// ---- forward rendering additive lights pass:
			Pass {
				Name "FORWARD"
				Tags { "LightMode" = "ForwardAdd" }
				ZWrite Off Blend One One

		CGPROGRAM
								  // compile directives
								  #pragma vertex vert_surf
								  #pragma fragment frag_surf
								  #pragma target 3.0
								  #pragma multi_compile_instancing
								  #pragma multi_compile_fog
								  #pragma skip_variants INSTANCING_ON
								  #pragma multi_compile_fwdadd_fullshadows
								  #include "HLSLSupport.cginc"
								  #define UNITY_INSTANCED_LOD_FADE
								  #define UNITY_INSTANCED_SH
								  #define UNITY_INSTANCED_LIGHTMAPSTS
								  #include "UnityShaderVariables.cginc"
								  #include "UnityShaderUtilities.cginc"
								  // -------- variant for: <when no other keywords are defined>
								  #if !defined(INSTANCING_ON)
								  // Surface shader code generated based on:
								  // vertex modifier: 'vert'
								  // writes to per-pixel normal: YES
								  // writes to emission: YES
								  // writes to occlusion: no
								  // needs world space reflection vector: no
								  // needs world space normal vector: no
								  // needs screen space position: no
								  // needs world space position: YES
								  // needs view direction: no
								  // needs world space view direction: no
								  // needs world space position for lighting: YES
								  // needs world space view direction for lighting: YES
								  // needs world space view direction for lightmaps: no
								  // needs vertex color: no
								  // needs VFACE: no
								  // passes tangent-to-world matrix to pixel shader: YES
								  // reads from normal: no
								  // 0 texcoords actually used
								  #include "UnityCG.cginc"
								  #include "Lighting.cginc"
								  #include "UnityPBSLighting.cginc"
								  #include "AutoLight.cginc"

								  #define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
								  #define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
								  #define WorldNormalVector(data,normal) fixed3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))

								  // Original surface shader snippet:
								  #line 15

											  #define UNITY_SETUP_BRDF_INPUT MetallicSetup
										  #line 20 ""
								  #ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
								  #endif
								  /* UNITY: Original start of shader */
										  //#pragma surface surf Standard vertex:vert fullforwardshadows
										  //#pragma target 3.0
										  #include "UnityStandardUtils.cginc"

										  uniform float4 _planeEquation;
										  uniform int _cuttingOn;

										  fixed4 _Color;
										  sampler2D _MainTex, _BumpMap;
										  half _MainMetalTiling;
										  half _NormalTiling;
										  half _Glossy;
										  half _NormalGain;
										  half _Metallic;
										  half _BlendFactor;

										  sampler2D _MetallicMap;

										  struct Input
										  {
											  float3 localCoord;
											  float3 localNormal;
											  float3 worldPos;
											  float3 worldNormal;
											  INTERNAL_DATA
										  };

										  void vert(inout appdata_full v, out Input o)
										  {
											  UNITY_INITIALIZE_OUTPUT(Input, o);
											  o.localCoord = v.vertex.xyz;
											  o.localNormal = v.normal.xyz;
										  }

										  
										  void surf(Input IN, inout SurfaceOutputStandard o)
										  {
											  if (_cuttingOn)
												  clip(dot(IN.worldPos, _planeEquation.xyz) - _planeEquation.w);

											  float3 bf = normalize(abs(IN.localNormal));
											  bf = pow(bf, _BlendFactor);
											  bf /= max(dot(bf, (float3)1), 0.0001);


											  // Triplanar mapping for base and metallic
											  float2 mmtx = IN.localCoord.zy * _MainMetalTiling;
#if _Y90_DEFAULT
											  float2 mmty = IN.localCoord.zx * _MainMetalTiling;
#else
											  float2 mmty = IN.localCoord.xz * _MainMetalTiling;
#endif
											  float2 mmtz = IN.localCoord.xy * _MainMetalTiling;

											  //base
											  fixed4 cz = tex2D(_MainTex, mmtx) * bf.x;
											  fixed4 cy = tex2D(_MainTex, mmty) * bf.y;
											  fixed4 cx = tex2D(_MainTex, mmtz) * bf.z;
											  fixed4 col = (cz + cy + cx) * _Color;
											  o.Albedo = col.rgb;

											  //metallic
											  fixed4 mz = tex2D(_MetallicMap, mmtx) * bf.x;
											  fixed4 my = tex2D(_MetallicMap, mmty) * bf.y;
											  fixed4 mx = tex2D(_MetallicMap, mmtz) * bf.z;
											  fixed4 met = (mz + my + mx) * _Metallic;
											  o.Metallic = met.r;

#if _SMOOTHNESSSOURCE_DIFFUSEALPHA
											  o.Smoothness = col.a * _Glossy;
#elif _SMOOTHNESSSOURCE_METALLICGREEN
											  o.Smoothness = met.g * _Glossy;
#else
											  o.Smoothness = _Glossy;
#endif

											  o.Alpha = col.a;

											  // Triplanar mapping for normal
											  float2 ntx = IN.localCoord.zy * _NormalTiling;
#if _Y90_DEFAULT
											  float2 nty = IN.localCoord.zx * _NormalTiling;
#else
											  float2 nty = IN.localCoord.xz * _NormalTiling;
#endif
											  float2 ntz = IN.localCoord.xy * _NormalTiling;

											  fixed3 tnormalX = UnpackScaleNormal(tex2D(_BumpMap, ntx), _NormalGain) * bf.x;
											  fixed3 tnormalY = UnpackScaleNormal(tex2D(_BumpMap, nty), _NormalGain) * bf.y;
											  fixed3 tnormalZ = UnpackScaleNormal(tex2D(_BumpMap, ntz), _NormalGain) * bf.z;

											  o.Normal = tnormalX + tnormalY + tnormalZ;

											  if (_cuttingOn) {
												  if (dot(IN.worldPos, _planeEquation.xyz) - _planeEquation.w < 0.004) {
													  o.Albedo = fixed3(1, 0, 0);
													  o.Emission = fixed3(1, 0, 0);
												  }
											  }
										  }


										  // vertex-to-fragment interpolation data
										  struct v2f_surf {
											UNITY_POSITION(pos);
											float3 tSpace0 : TEXCOORD0;
											float3 tSpace1 : TEXCOORD1;
											float3 tSpace2 : TEXCOORD2;
											float3 worldPos : TEXCOORD3;
											float3 custompack0 : TEXCOORD4; // localCoord
											float3 custompack1 : TEXCOORD5; // localNormal
											UNITY_LIGHTING_COORDS(6,7)
											UNITY_FOG_COORDS(8)
											UNITY_VERTEX_INPUT_INSTANCE_ID
											UNITY_VERTEX_OUTPUT_STEREO
										  };

										  // vertex shader
										  v2f_surf vert_surf(appdata_full v) {
											UNITY_SETUP_INSTANCE_ID(v);
											v2f_surf o;
											UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
											UNITY_TRANSFER_INSTANCE_ID(v,o);
											UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
											Input customInputData;
											vert(v, customInputData);
											o.custompack0.xyz = customInputData.localCoord;
											o.custompack1.xyz = customInputData.localNormal;
											o.pos = UnityObjectToClipPos(v.vertex);
											float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
											float3 worldNormal = UnityObjectToWorldNormal(v.normal);
											//fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
											//fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
											//fixed3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
											//fake tangents
											fixed3 worldTangent = cross(worldNormal, fixed3(0, 0, 1));
											float val = length(worldTangent);
											if (val < 0.0001)
											{
												worldTangent = cross(worldNormal, fixed3(1, 0, 0));
											}
											fixed3 worldBinormal = cross(worldNormal, worldTangent);
											//end fake tangents
											o.tSpace0 = float3(worldTangent.x, worldBinormal.x, worldNormal.x);
											o.tSpace1 = float3(worldTangent.y, worldBinormal.y, worldNormal.y);
											o.tSpace2 = float3(worldTangent.z, worldBinormal.z, worldNormal.z);
											o.worldPos.xyz = worldPos;

											UNITY_TRANSFER_LIGHTING(o,v.texcoord1.xy); // pass shadow and, possibly, light cookie coordinates to pixel shader
											UNITY_TRANSFER_FOG(o,o.pos); // pass fog coordinates to pixel shader
											return o;
										  }

										  // fragment shader
										  fixed4 frag_surf(v2f_surf IN) : SV_Target {
											UNITY_SETUP_INSTANCE_ID(IN);
										  // prepare and unpack data
										  Input surfIN;
										  #ifdef FOG_COMBINED_WITH_TSPACE
											UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
										  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
											UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
										  #else
											UNITY_EXTRACT_FOG(IN);
										  #endif
										  #ifdef FOG_COMBINED_WITH_TSPACE
											UNITY_RECONSTRUCT_TBN(IN);
										  #else
											UNITY_EXTRACT_TBN(IN);
										  #endif
										  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
										  surfIN.localCoord.x = 1.0;
										  surfIN.localNormal.x = 1.0;
										  surfIN.worldPos.x = 1.0;
										  surfIN.worldNormal.x = 1.0;
										  surfIN.localCoord = IN.custompack0.xyz;
										  surfIN.localNormal = IN.custompack1.xyz;
										  float3 worldPos = IN.worldPos.xyz;
										  #ifndef USING_DIRECTIONAL_LIGHT
											fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
										  #else
											fixed3 lightDir = _WorldSpaceLightPos0.xyz;
										  #endif
										  float3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));
										  surfIN.worldPos = worldPos;
										  #ifdef UNITY_COMPILER_HLSL
										  SurfaceOutputStandard o = (SurfaceOutputStandard)0;
										  #else
										  SurfaceOutputStandard o;
										  #endif
										  o.Albedo = 0.0;
										  o.Emission = 0.0;
										  o.Alpha = 0.0;
										  o.Occlusion = 1.0;
										  fixed3 normalWorldVertex = fixed3(0,0,1);
										  o.Normal = fixed3(0,0,1);

										  // call surface function
										  surf(surfIN, o);
										  UNITY_LIGHT_ATTENUATION(atten, IN, worldPos)
										  fixed4 c = 0;
										  float3 worldN;
										  worldN.x = dot(_unity_tbn_0, o.Normal);
										  worldN.y = dot(_unity_tbn_1, o.Normal);
										  worldN.z = dot(_unity_tbn_2, o.Normal);
										  worldN = normalize(worldN);
										  o.Normal = worldN;

										  // Setup lighting environment
										  UnityGI gi;
										  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
										  gi.indirect.diffuse = 0;
										  gi.indirect.specular = 0;
										  gi.light.color = _LightColor0.rgb;
										  gi.light.dir = lightDir;
										  gi.light.color *= atten;
										  c += LightingStandard(o, worldViewDir, gi);
										  c.a = 0.0;
										  UNITY_APPLY_FOG(_unity_fogCoord, c); // apply fog
										  UNITY_OPAQUE_ALPHA(c.a);
										  return c;
										}


										#endif


										ENDCG

										}

								  // ---- deferred shading pass:
								  Pass {
									  Name "DEFERRED"
									  Tags { "LightMode" = "Deferred" }

							  CGPROGRAM
											// compile directives
										    #pragma multi_compile _SMOOTHNESSSOURCE_DIFFUSEALPHA _SMOOTHNESSSOURCE_METALLICGREEN
											#pragma multi_compile _Y90_DEFAULT _Y90_ROTATE90
											#pragma vertex vert_surf
											#pragma fragment frag_surf
											#pragma target 3.0
											#pragma multi_compile_instancing
											#pragma exclude_renderers nomrt
											#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
											#pragma multi_compile_prepassfinal
											#include "HLSLSupport.cginc"
											#define UNITY_INSTANCED_LOD_FADE
											#define UNITY_INSTANCED_SH
											#define UNITY_INSTANCED_LIGHTMAPSTS
											#include "UnityShaderVariables.cginc"
											#include "UnityShaderUtilities.cginc"
											// -------- variant for: <when no other keywords are defined>
											#if !defined(INSTANCING_ON)
											// Surface shader code generated based on:
											// vertex modifier: 'vert'
											// writes to per-pixel normal: YES
											// writes to emission: YES
											// writes to occlusion: no
											// needs world space reflection vector: no
											// needs world space normal vector: no
											// needs screen space position: no
											// needs world space position: YES
											// needs view direction: no
											// needs world space view direction: no
											// needs world space position for lighting: YES
											// needs world space view direction for lighting: YES
											// needs world space view direction for lightmaps: no
											// needs vertex color: no
											// needs VFACE: no
											// passes tangent-to-world matrix to pixel shader: YES
											// reads from normal: no
											// 0 texcoords actually used
											#include "UnityCG.cginc"
											#include "Lighting.cginc"
											#include "UnityPBSLighting.cginc"

											#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
											#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
											#define WorldNormalVector(data,normal) fixed3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))

											// Original surface shader snippet:
											#line 15

														#define UNITY_SETUP_BRDF_INPUT MetallicSetup
													#line 20 ""
											#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
											#endif
											/* UNITY: Original start of shader */
													//#pragma surface surf Standard vertex:vert fullforwardshadows
													//#pragma target 3.0
													#include "UnityStandardUtils.cginc"

													uniform float4 _planeEquation;
													uniform int _cuttingOn;

													fixed4 _Color;
													sampler2D _MainTex, _BumpMap;
													half _MainMetalTiling;
													half _NormalTiling;
													half _Glossy;
													half _NormalGain;
													half _Metallic;
													half _BlendFactor;

													sampler2D _MetallicMap;

													struct Input
													{
														float3 localCoord;
														float3 localNormal;
														float3 worldPos;
														float3 worldNormal;
														INTERNAL_DATA
													};

													void vert(inout appdata_full v, out Input o)
													{
														UNITY_INITIALIZE_OUTPUT(Input, o);
														o.localCoord = v.vertex.xyz;
														o.localNormal = v.normal.xyz;
													}

													
													void surf(Input IN, inout SurfaceOutputStandard o)
													{
														if (_cuttingOn)
															clip(dot(IN.worldPos, _planeEquation.xyz) - _planeEquation.w);

														float3 bf = normalize(abs(IN.localNormal));
														bf = pow(bf, _BlendFactor);
														bf /= max(dot(bf, (float3)1), 0.0001);


														// Triplanar mapping for base and metallic
														float2 mmtx = IN.localCoord.zy * _MainMetalTiling;
#if _Y90_DEFAULT
														float2 mmty = IN.localCoord.zx * _MainMetalTiling;
#else
														float2 mmty = IN.localCoord.xz * _MainMetalTiling;
#endif
														float2 mmtz = IN.localCoord.xy * _MainMetalTiling;

														//base
														fixed4 cz = tex2D(_MainTex, mmtx) * bf.x;
														fixed4 cy = tex2D(_MainTex, mmty) * bf.y;
														fixed4 cx = tex2D(_MainTex, mmtz) * bf.z;
														fixed4 col = (cz + cy + cx) * _Color;
														o.Albedo = col.rgb;

														//metallic
														fixed4 mz = tex2D(_MetallicMap, mmtx) * bf.x;
														fixed4 my = tex2D(_MetallicMap, mmty) * bf.y;
														fixed4 mx = tex2D(_MetallicMap, mmtz) * bf.z;
														fixed4 met = (mz + my + mx) * _Metallic;
														o.Metallic = met.r;

#if _SMOOTHNESSSOURCE_DIFFUSEALPHA
														o.Smoothness = col.a * _Glossy;
#elif _SMOOTHNESSSOURCE_METALLICGREEN
														o.Smoothness = met.g * _Glossy;
#else
														o.Smoothness = _Glossy;
#endif

														o.Alpha = col.a;

														// Triplanar mapping for normal
														float2 ntx = IN.localCoord.zy * _NormalTiling;
#if _Y90_DEFAULT
														float2 nty = IN.localCoord.zx * _NormalTiling;
#else
														float2 nty = IN.localCoord.xz * _NormalTiling;
#endif
														float2 ntz = IN.localCoord.xy * _NormalTiling;

														fixed3 tnormalX = UnpackScaleNormal(tex2D(_BumpMap, ntx), _NormalGain) * bf.x;
														fixed3 tnormalY = UnpackScaleNormal(tex2D(_BumpMap, nty), _NormalGain) * bf.y;
														fixed3 tnormalZ = UnpackScaleNormal(tex2D(_BumpMap, ntz), _NormalGain) * bf.z;

														o.Normal = tnormalX + tnormalY + tnormalZ;

														if (_cuttingOn) {
															if (dot(IN.worldPos, _planeEquation.xyz) - _planeEquation.w < 0.004) {
																o.Albedo = fixed3(1, 0, 0);
																o.Emission = fixed3(1, 0, 0);
															}
														}
													}


													// vertex-to-fragment interpolation data
													struct v2f_surf {
													  UNITY_POSITION(pos);
													  float4 tSpace0 : TEXCOORD0;
													  float4 tSpace1 : TEXCOORD1;
													  float4 tSpace2 : TEXCOORD2;
													  float3 custompack0 : TEXCOORD3; // localCoord
													  float3 custompack1 : TEXCOORD4; // localNormal
													#ifndef DIRLIGHTMAP_OFF
													  half3 viewDir : TEXCOORD5;
													#endif
													  float4 lmap : TEXCOORD6;
													#ifndef LIGHTMAP_ON
													  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
														half3 sh : TEXCOORD7; // SH
													  #endif
													#else
													  #ifdef DIRLIGHTMAP_OFF
														float4 lmapFadePos : TEXCOORD7;
													  #endif
													#endif
													  UNITY_VERTEX_INPUT_INSTANCE_ID
													  UNITY_VERTEX_OUTPUT_STEREO
													};

													// vertex shader
													v2f_surf vert_surf(appdata_full v) {
													  UNITY_SETUP_INSTANCE_ID(v);
													  v2f_surf o;
													  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
													  UNITY_TRANSFER_INSTANCE_ID(v,o);
													  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
													  Input customInputData;
													  vert(v, customInputData);
													  o.custompack0.xyz = customInputData.localCoord;
													  o.custompack1.xyz = customInputData.localNormal;
													  o.pos = UnityObjectToClipPos(v.vertex);
													  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
													  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
													 // fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
													 // fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
													 // fixed3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
													  //fake tangents
													  fixed3 worldTangent = cross(worldNormal, fixed3(0, 0, 1));
													  float val = length(worldTangent);
													  if (val < 0.0001)
													  {
														  worldTangent = cross(worldNormal, fixed3(1, 0, 0));
													  }
													  fixed3 worldBinormal = cross(worldNormal, worldTangent);
													  //end fake tangents
													  o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
													  o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
													  o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
													  float3 viewDirForLight = UnityWorldSpaceViewDir(worldPos);
													  #ifndef DIRLIGHTMAP_OFF
													  o.viewDir.x = dot(viewDirForLight, worldTangent);
													  o.viewDir.y = dot(viewDirForLight, worldBinormal);
													  o.viewDir.z = dot(viewDirForLight, worldNormal);
													  #endif
													#ifdef DYNAMICLIGHTMAP_ON
													  o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
													#else
													  o.lmap.zw = 0;
													#endif
													#ifdef LIGHTMAP_ON
													  o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
													  #ifdef DIRLIGHTMAP_OFF
														o.lmapFadePos.xyz = (mul(unity_ObjectToWorld, v.vertex).xyz - unity_ShadowFadeCenterAndType.xyz) * unity_ShadowFadeCenterAndType.w;
														o.lmapFadePos.w = (-UnityObjectToViewPos(v.vertex).z) * (1.0 - unity_ShadowFadeCenterAndType.w);
													  #endif
													#else
													  o.lmap.xy = 0;
														#if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
														  o.sh = 0;
														  o.sh = ShadeSHPerVertex(worldNormal, o.sh);
														#endif
													#endif
													  return o;
													}
													#ifdef LIGHTMAP_ON
													float4 unity_LightmapFade;
													#endif
													fixed4 unity_Ambient;

													// fragment shader
													void frag_surf(v2f_surf IN,
														out half4 outGBuffer0 : SV_Target0,
														out half4 outGBuffer1 : SV_Target1,
														out half4 outGBuffer2 : SV_Target2,
														out half4 outEmission : SV_Target3
													#if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
														, out half4 outShadowMask : SV_Target4
													#endif
													) {
													  UNITY_SETUP_INSTANCE_ID(IN);
													  // prepare and unpack data
													  Input surfIN;
													  #ifdef FOG_COMBINED_WITH_TSPACE
														UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
													  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
														UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
													  #else
														UNITY_EXTRACT_FOG(IN);
													  #endif
													  #ifdef FOG_COMBINED_WITH_TSPACE
														UNITY_RECONSTRUCT_TBN(IN);
													  #else
														UNITY_EXTRACT_TBN(IN);
													  #endif
													  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
													  surfIN.localCoord.x = 1.0;
													  surfIN.localNormal.x = 1.0;
													  surfIN.worldPos.x = 1.0;
													  surfIN.worldNormal.x = 1.0;
													  surfIN.localCoord = IN.custompack0.xyz;
													  surfIN.localNormal = IN.custompack1.xyz;
													  float3 worldPos = float3(IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w);
													  #ifndef USING_DIRECTIONAL_LIGHT
														fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
													  #else
														fixed3 lightDir = _WorldSpaceLightPos0.xyz;
													  #endif
													  float3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));
													  surfIN.worldPos = worldPos;
													  #ifdef UNITY_COMPILER_HLSL
													  SurfaceOutputStandard o = (SurfaceOutputStandard)0;
													  #else
													  SurfaceOutputStandard o;
													  #endif
													  o.Albedo = 0.0;
													  o.Emission = 0.0;
													  o.Alpha = 0.0;
													  o.Occlusion = 1.0;
													  fixed3 normalWorldVertex = fixed3(0,0,1);
													  o.Normal = fixed3(0,0,1);

													  // call surface function
													  surf(surfIN, o);
													fixed3 originalNormal = o.Normal;
													  float3 worldN;
													  worldN.x = dot(_unity_tbn_0, o.Normal);
													  worldN.y = dot(_unity_tbn_1, o.Normal);
													  worldN.z = dot(_unity_tbn_2, o.Normal);
													  worldN = normalize(worldN);
													  o.Normal = worldN;
													  half atten = 1;

													  // Setup lighting environment
													  UnityGI gi;
													  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
													  gi.indirect.diffuse = 0;
													  gi.indirect.specular = 0;
													  gi.light.color = 0;
													  gi.light.dir = half3(0,1,0);
													  // Call GI (lightmaps/SH/reflections) lighting function
													  UnityGIInput giInput;
													  UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
													  giInput.light = gi.light;
													  giInput.worldPos = worldPos;
													  giInput.worldViewDir = worldViewDir;
													  giInput.atten = atten;
													  #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
														giInput.lightmapUV = IN.lmap;
													  #else
														giInput.lightmapUV = 0.0;
													  #endif
													  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
														giInput.ambient = IN.sh;
													  #else
														giInput.ambient.rgb = 0.0;
													  #endif
													  giInput.probeHDR[0] = unity_SpecCube0_HDR;
													  giInput.probeHDR[1] = unity_SpecCube1_HDR;
													  #if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
														giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
													  #endif
													  #ifdef UNITY_SPECCUBE_BOX_PROJECTION
														giInput.boxMax[0] = unity_SpecCube0_BoxMax;
														giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
														giInput.boxMax[1] = unity_SpecCube1_BoxMax;
														giInput.boxMin[1] = unity_SpecCube1_BoxMin;
														giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
													  #endif
													  LightingStandard_GI(o, giInput, gi);

													  // call lighting function to output g-buffer
													  outEmission = LightingStandard_Deferred(o, worldViewDir, gi, outGBuffer0, outGBuffer1, outGBuffer2);
													  #if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
														outShadowMask = UnityGetRawBakedOcclusions(IN.lmap.xy, worldPos);
													  #endif
													  #ifndef UNITY_HDR_ON
													  outEmission.rgb = exp2(-outEmission.rgb);
													  #endif
													}


													#endif

													// -------- variant for: INSTANCING_ON 
													#if defined(INSTANCING_ON)
													// Surface shader code generated based on:
													// vertex modifier: 'vert'
													// writes to per-pixel normal: YES
													// writes to emission: YES
													// writes to occlusion: no
													// needs world space reflection vector: no
													// needs world space normal vector: no
													// needs screen space position: no
													// needs world space position: YES
													// needs view direction: no
													// needs world space view direction: no
													// needs world space position for lighting: YES
													// needs world space view direction for lighting: YES
													// needs world space view direction for lightmaps: no
													// needs vertex color: no
													// needs VFACE: no
													// passes tangent-to-world matrix to pixel shader: YES
													// reads from normal: no
													// 0 texcoords actually used
													#include "UnityCG.cginc"
													#include "Lighting.cginc"
													#include "UnityPBSLighting.cginc"

													#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
													#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
													#define WorldNormalVector(data,normal) fixed3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))

													// Original surface shader snippet:
													#line 15

																#define UNITY_SETUP_BRDF_INPUT MetallicSetup
															#line 20 ""
													#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
													#endif
													/* UNITY: Original start of shader */
															//#pragma surface surf Standard vertex:vert fullforwardshadows
															//#pragma target 3.0
															#include "UnityStandardUtils.cginc"

															uniform float4 _planeEquation;
															uniform int _cuttingOn;

															fixed4 _Color;
															sampler2D _MainTex, _BumpMap;
															half _MainMetalTiling;
															half _NormalTiling;
															half _Glossy;
															half _NormalGain;
															half _Metallic;
															half _BlendFactor;

															sampler2D _MetallicMap;

															struct Input
															{
																float3 localCoord;
																float3 localNormal;
																float3 worldPos;
																float3 worldNormal;
																INTERNAL_DATA
															};

															void vert(inout appdata_full v, out Input o)
															{
																UNITY_INITIALIZE_OUTPUT(Input, o);
																o.localCoord = v.vertex.xyz;
																o.localNormal = v.normal.xyz;
															}

															
															void surf(Input IN, inout SurfaceOutputStandard o)
															{
																if (_cuttingOn)
																	clip(dot(IN.worldPos, _planeEquation.xyz) - _planeEquation.w);

																float3 bf = normalize(abs(IN.localNormal));
																bf = pow(bf, _BlendFactor);
																bf /= max(dot(bf, (float3)1), 0.0001);


																// Triplanar mapping for base and metallic
																float2 mmtx = IN.localCoord.zy * _MainMetalTiling;
#if _Y90_DEFAULT
																float2 mmty = IN.localCoord.zx * _MainMetalTiling;
#else
																float2 mmty = IN.localCoord.xz * _MainMetalTiling;
#endif
																float2 mmtz = IN.localCoord.xy * _MainMetalTiling;

																//base
																fixed4 cz = tex2D(_MainTex, mmtx) * bf.x;
																fixed4 cy = tex2D(_MainTex, mmty) * bf.y;
																fixed4 cx = tex2D(_MainTex, mmtz) * bf.z;
																fixed4 col = (cz + cy + cx) * _Color;
																o.Albedo = col.rgb;

																//metallic
																fixed4 mz = tex2D(_MetallicMap, mmtx) * bf.x;
																fixed4 my = tex2D(_MetallicMap, mmty) * bf.y;
																fixed4 mx = tex2D(_MetallicMap, mmtz) * bf.z;
																fixed4 met = (mz + my + mx) * _Metallic;
																o.Metallic = met.r;

#if _SMOOTHNESSSOURCE_DIFFUSEALPHA
																o.Smoothness = col.a * _Glossy;
#elif _SMOOTHNESSSOURCE_METALLICGREEN
																o.Smoothness = met.g * _Glossy;
#else
																o.Smoothness = _Glossy;
#endif

																o.Alpha = col.a;

																// Triplanar mapping for normal
																float2 ntx = IN.localCoord.zy * _NormalTiling;
#if _Y90_DEFAULT
																float2 nty = IN.localCoord.zx * _NormalTiling;
#else
																float2 nty = IN.localCoord.xz * _NormalTiling;
#endif
																float2 ntz = IN.localCoord.xy * _NormalTiling;

																fixed3 tnormalX = UnpackScaleNormal(tex2D(_BumpMap, ntx), _NormalGain) * bf.x;
																fixed3 tnormalY = UnpackScaleNormal(tex2D(_BumpMap, nty), _NormalGain) * bf.y;
																fixed3 tnormalZ = UnpackScaleNormal(tex2D(_BumpMap, ntz), _NormalGain) * bf.z;

																o.Normal = tnormalX + tnormalY + tnormalZ;

																if (_cuttingOn) {
																	if (dot(IN.worldPos, _planeEquation.xyz) - _planeEquation.w < 0.004) {
																		o.Albedo = fixed3(1, 0, 0);
																		o.Emission = fixed3(1, 0, 0);
																	}
																}
															}


															// vertex-to-fragment interpolation data
															struct v2f_surf {
															  UNITY_POSITION(pos);
															  float4 tSpace0 : TEXCOORD0;
															  float4 tSpace1 : TEXCOORD1;
															  float4 tSpace2 : TEXCOORD2;
															  float3 custompack0 : TEXCOORD3; // localCoord
															  float3 custompack1 : TEXCOORD4; // localNormal
															#ifndef DIRLIGHTMAP_OFF
															  half3 viewDir : TEXCOORD5;
															#endif
															  float4 lmap : TEXCOORD6;
															#ifndef LIGHTMAP_ON
															  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
																half3 sh : TEXCOORD7; // SH
															  #endif
															#else
															  #ifdef DIRLIGHTMAP_OFF
																float4 lmapFadePos : TEXCOORD7;
															  #endif
															#endif
															  UNITY_VERTEX_INPUT_INSTANCE_ID
															  UNITY_VERTEX_OUTPUT_STEREO
															};

															// vertex shader
															v2f_surf vert_surf(appdata_full v) {
															  UNITY_SETUP_INSTANCE_ID(v);
															  v2f_surf o;
															  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
															  UNITY_TRANSFER_INSTANCE_ID(v,o);
															  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
															  Input customInputData;
															  vert(v, customInputData);
															  o.custompack0.xyz = customInputData.localCoord;
															  o.custompack1.xyz = customInputData.localNormal;
															  o.pos = UnityObjectToClipPos(v.vertex);
															  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
															  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
															  //fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
															  //fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
															  //fixed3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
															  //fake tangents
															  fixed3 worldTangent = cross(worldNormal, fixed3(0, 0, 1));
															  float val = length(worldTangent);
															  if (val < 0.0001)
															  {
																  worldTangent = cross(worldNormal, fixed3(1, 0, 0));
															  }
															  fixed3 worldBinormal = cross(worldNormal, worldTangent);
															  //end fake tangents
															  o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
															  o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
															  o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
															  float3 viewDirForLight = UnityWorldSpaceViewDir(worldPos);
															  #ifndef DIRLIGHTMAP_OFF
															  o.viewDir.x = dot(viewDirForLight, worldTangent);
															  o.viewDir.y = dot(viewDirForLight, worldBinormal);
															  o.viewDir.z = dot(viewDirForLight, worldNormal);
															  #endif
															#ifdef DYNAMICLIGHTMAP_ON
															  o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
															#else
															  o.lmap.zw = 0;
															#endif
															#ifdef LIGHTMAP_ON
															  o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
															  #ifdef DIRLIGHTMAP_OFF
																o.lmapFadePos.xyz = (mul(unity_ObjectToWorld, v.vertex).xyz - unity_ShadowFadeCenterAndType.xyz) * unity_ShadowFadeCenterAndType.w;
																o.lmapFadePos.w = (-UnityObjectToViewPos(v.vertex).z) * (1.0 - unity_ShadowFadeCenterAndType.w);
															  #endif
															#else
															  o.lmap.xy = 0;
																#if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
																  o.sh = 0;
																  o.sh = ShadeSHPerVertex(worldNormal, o.sh);
																#endif
															#endif
															  return o;
															}
															#ifdef LIGHTMAP_ON
															float4 unity_LightmapFade;
															#endif
															fixed4 unity_Ambient;

															// fragment shader
															void frag_surf(v2f_surf IN,
																out half4 outGBuffer0 : SV_Target0,
																out half4 outGBuffer1 : SV_Target1,
																out half4 outGBuffer2 : SV_Target2,
																out half4 outEmission : SV_Target3
															#if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
																, out half4 outShadowMask : SV_Target4
															#endif
															) {
															  UNITY_SETUP_INSTANCE_ID(IN);
															  // prepare and unpack data
															  Input surfIN;
															  #ifdef FOG_COMBINED_WITH_TSPACE
																UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
															  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
																UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
															  #else
																UNITY_EXTRACT_FOG(IN);
															  #endif
															  #ifdef FOG_COMBINED_WITH_TSPACE
																UNITY_RECONSTRUCT_TBN(IN);
															  #else
																UNITY_EXTRACT_TBN(IN);
															  #endif
															  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
															  surfIN.localCoord.x = 1.0;
															  surfIN.localNormal.x = 1.0;
															  surfIN.worldPos.x = 1.0;
															  surfIN.worldNormal.x = 1.0;
															  surfIN.localCoord = IN.custompack0.xyz;
															  surfIN.localNormal = IN.custompack1.xyz;
															  float3 worldPos = float3(IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w);
															  #ifndef USING_DIRECTIONAL_LIGHT
																fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
															  #else
																fixed3 lightDir = _WorldSpaceLightPos0.xyz;
															  #endif
															  float3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));
															  surfIN.worldPos = worldPos;
															  #ifdef UNITY_COMPILER_HLSL
															  SurfaceOutputStandard o = (SurfaceOutputStandard)0;
															  #else
															  SurfaceOutputStandard o;
															  #endif
															  o.Albedo = 0.0;
															  o.Emission = 0.0;
															  o.Alpha = 0.0;
															  o.Occlusion = 1.0;
															  fixed3 normalWorldVertex = fixed3(0,0,1);
															  o.Normal = fixed3(0,0,1);

															  // call surface function
															  surf(surfIN, o);
															fixed3 originalNormal = o.Normal;
															  float3 worldN;
															  worldN.x = dot(_unity_tbn_0, o.Normal);
															  worldN.y = dot(_unity_tbn_1, o.Normal);
															  worldN.z = dot(_unity_tbn_2, o.Normal);
															  worldN = normalize(worldN);
															  o.Normal = worldN;
															  half atten = 1;

															  // Setup lighting environment
															  UnityGI gi;
															  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
															  gi.indirect.diffuse = 0;
															  gi.indirect.specular = 0;
															  gi.light.color = 0;
															  gi.light.dir = half3(0,1,0);
															  // Call GI (lightmaps/SH/reflections) lighting function
															  UnityGIInput giInput;
															  UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
															  giInput.light = gi.light;
															  giInput.worldPos = worldPos;
															  giInput.worldViewDir = worldViewDir;
															  giInput.atten = atten;
															  #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
																giInput.lightmapUV = IN.lmap;
															  #else
																giInput.lightmapUV = 0.0;
															  #endif
															  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
																giInput.ambient = IN.sh;
															  #else
																giInput.ambient.rgb = 0.0;
															  #endif
															  giInput.probeHDR[0] = unity_SpecCube0_HDR;
															  giInput.probeHDR[1] = unity_SpecCube1_HDR;
															  #if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
																giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
															  #endif
															  #ifdef UNITY_SPECCUBE_BOX_PROJECTION
																giInput.boxMax[0] = unity_SpecCube0_BoxMax;
																giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
																giInput.boxMax[1] = unity_SpecCube1_BoxMax;
																giInput.boxMin[1] = unity_SpecCube1_BoxMin;
																giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
															  #endif
															  LightingStandard_GI(o, giInput, gi);

															  // call lighting function to output g-buffer
															  outEmission = LightingStandard_Deferred(o, worldViewDir, gi, outGBuffer0, outGBuffer1, outGBuffer2);
															  #if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
																outShadowMask = UnityGetRawBakedOcclusions(IN.lmap.xy, worldPos);
															  #endif
															  #ifndef UNITY_HDR_ON
															  outEmission.rgb = exp2(-outEmission.rgb);
															  #endif
															}


															#endif


															ENDCG

															}

											// ---- meta information extraction pass:
											Pass {
												Name "Meta"
												Tags { "LightMode" = "Meta" }
												Cull Off

										CGPROGRAM
																// compile directives
																#pragma vertex vert_surf
																#pragma fragment frag_surf
																#pragma target 3.0
																#pragma multi_compile_instancing
																#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
																#pragma skip_variants INSTANCING_ON
																#pragma shader_feature EDITOR_VISUALIZATION

																#include "HLSLSupport.cginc"
																#define UNITY_INSTANCED_LOD_FADE
																#define UNITY_INSTANCED_SH
																#define UNITY_INSTANCED_LIGHTMAPSTS
																#include "UnityShaderVariables.cginc"
																#include "UnityShaderUtilities.cginc"
																// -------- variant for: <when no other keywords are defined>
																#if !defined(INSTANCING_ON)
																// Surface shader code generated based on:
																// vertex modifier: 'vert'
																// writes to per-pixel normal: YES
																// writes to emission: YES
																// writes to occlusion: no
																// needs world space reflection vector: no
																// needs world space normal vector: no
																// needs screen space position: no
																// needs world space position: YES
																// needs view direction: no
																// needs world space view direction: no
																// needs world space position for lighting: YES
																// needs world space view direction for lighting: YES
																// needs world space view direction for lightmaps: no
																// needs vertex color: no
																// needs VFACE: no
																// passes tangent-to-world matrix to pixel shader: YES
																// reads from normal: no
																// 0 texcoords actually used
																#include "UnityCG.cginc"
																#include "Lighting.cginc"
																#include "UnityPBSLighting.cginc"

																#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
																#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
																#define WorldNormalVector(data,normal) fixed3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))

																// Original surface shader snippet:
																#line 15

																			#define UNITY_SETUP_BRDF_INPUT MetallicSetup
																		#line 20 ""
																#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
																#endif
																/* UNITY: Original start of shader */
																		//#pragma surface surf Standard vertex:vert fullforwardshadows
																		//#pragma target 3.0
																		#include "UnityStandardUtils.cginc"

																		uniform float4 _planeEquation;
																		uniform int _cuttingOn;

																		fixed4 _Color;
																		sampler2D _MainTex, _BumpMap;
																		half _MainMetalTiling;
																		half _NormalTiling;
																		half _Glossy;
																		half _NormalGain;
																		half _Metallic;
																		half _BlendFactor;

																		sampler2D _MetallicMap;

																		struct Input
																		{
																			float3 localCoord;
																			float3 localNormal;
																			float3 worldPos;
																			float3 worldNormal;
																			INTERNAL_DATA
																		};

																		void vert(inout appdata_full v, out Input o)
																		{
																			UNITY_INITIALIZE_OUTPUT(Input, o);
																			o.localCoord = v.vertex.xyz;
																			o.localNormal = v.normal.xyz;
																		}

																		

																		void surf(Input IN, inout SurfaceOutputStandard o)
																		{
																			if (_cuttingOn)
																				clip(dot(IN.worldPos, _planeEquation.xyz) - _planeEquation.w);

																			float3 bf = normalize(abs(IN.localNormal));
																			bf = pow(bf, _BlendFactor);
																			bf /= max(dot(bf, (float3)1), 0.0001);


																			// Triplanar mapping for base and metallic
																			float2 mmtx = IN.localCoord.zy * _MainMetalTiling;
#if _Y90_DEFAULT
																			float2 mmty = IN.localCoord.zx * _MainMetalTiling;
#else
																			float2 mmty = IN.localCoord.xz * _MainMetalTiling;
#endif
																			float2 mmtz = IN.localCoord.xy * _MainMetalTiling;

																			//base
																			fixed4 cz = tex2D(_MainTex, mmtx) * bf.x;
																			fixed4 cy = tex2D(_MainTex, mmty) * bf.y;
																			fixed4 cx = tex2D(_MainTex, mmtz) * bf.z;
																			fixed4 col = (cz + cy + cx) * _Color;
																			o.Albedo = col.rgb;

																			//metallic
																			fixed4 mz = tex2D(_MetallicMap, mmtx) * bf.x;
																			fixed4 my = tex2D(_MetallicMap, mmty) * bf.y;
																			fixed4 mx = tex2D(_MetallicMap, mmtz) * bf.z;
																			fixed4 met = (mz + my + mx) * _Metallic;
																			o.Metallic = met.r;

#if _SMOOTHNESSSOURCE_DIFFUSEALPHA
																			o.Smoothness = col.a * _Glossy;
#elif _SMOOTHNESSSOURCE_METALLICGREEN
																			o.Smoothness = met.g * _Glossy;
#else
																			o.Smoothness = _Glossy;
#endif

																			o.Alpha = col.a;

																			// Triplanar mapping for normal
																			float2 ntx = IN.localCoord.zy * _NormalTiling;
#if _Y90_DEFAULT
																			float2 nty = IN.localCoord.zx * _NormalTiling;
#else
																			float2 nty = IN.localCoord.xz * _NormalTiling;
#endif
																			float2 ntz = IN.localCoord.xy * _NormalTiling;

																			fixed3 tnormalX = UnpackScaleNormal(tex2D(_BumpMap, ntx), _NormalGain) * bf.x;
																			fixed3 tnormalY = UnpackScaleNormal(tex2D(_BumpMap, nty), _NormalGain) * bf.y;
																			fixed3 tnormalZ = UnpackScaleNormal(tex2D(_BumpMap, ntz), _NormalGain) * bf.z;

																			o.Normal = tnormalX + tnormalY + tnormalZ;

																			if (_cuttingOn) {
																				if (dot(IN.worldPos, _planeEquation.xyz) - _planeEquation.w < 0.004) {
																					o.Albedo = fixed3(1, 0, 0);
																					o.Emission = fixed3(1, 0, 0);
																				}
																			}
																		}

																#include "UnityMetaPass.cginc"

																		// vertex-to-fragment interpolation data
																		struct v2f_surf {
																		  UNITY_POSITION(pos);
																		  float4 tSpace0 : TEXCOORD0;
																		  float4 tSpace1 : TEXCOORD1;
																		  float4 tSpace2 : TEXCOORD2;
																		  float3 custompack0 : TEXCOORD3; // localCoord
																		  float3 custompack1 : TEXCOORD4; // localNormal
																		  UNITY_VERTEX_INPUT_INSTANCE_ID
																		  UNITY_VERTEX_OUTPUT_STEREO
																		};

																		// vertex shader
																		v2f_surf vert_surf(appdata_full v) {
																		  UNITY_SETUP_INSTANCE_ID(v);
																		  v2f_surf o;
																		  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
																		  UNITY_TRANSFER_INSTANCE_ID(v,o);
																		  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
																		  Input customInputData;
																		  vert(v, customInputData);
																		  o.custompack0.xyz = customInputData.localCoord;
																		  o.custompack1.xyz = customInputData.localNormal;
																		  o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST);
																		  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
																		  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
																		  //fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
																		  //fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
																		  //fixed3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
																		  //fake tangents
																		  fixed3 worldTangent = cross(worldNormal, fixed3(0, 0, 1));
																		  float val = length(worldTangent);
																		  if (val < 0.0001)
																		  {
																			  worldTangent = cross(worldNormal, fixed3(1, 0, 0));
																		  }
																		  fixed3 worldBinormal = cross(worldNormal, worldTangent);
																		  //end fake tangents
																		  o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
																		  o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
																		  o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
																		  return o;
																		}

																		// fragment shader
																		fixed4 frag_surf(v2f_surf IN) : SV_Target {
																		  UNITY_SETUP_INSTANCE_ID(IN);
																		// prepare and unpack data
																		Input surfIN;
																		#ifdef FOG_COMBINED_WITH_TSPACE
																		  UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
																		#elif defined (FOG_COMBINED_WITH_WORLD_POS)
																		  UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
																		#else
																		  UNITY_EXTRACT_FOG(IN);
																		#endif
																		#ifdef FOG_COMBINED_WITH_TSPACE
																		  UNITY_RECONSTRUCT_TBN(IN);
																		#else
																		  UNITY_EXTRACT_TBN(IN);
																		#endif
																		UNITY_INITIALIZE_OUTPUT(Input,surfIN);
																		surfIN.localCoord.x = 1.0;
																		surfIN.localNormal.x = 1.0;
																		surfIN.worldPos.x = 1.0;
																		surfIN.worldNormal.x = 1.0;
																		surfIN.localCoord = IN.custompack0.xyz;
																		surfIN.localNormal = IN.custompack1.xyz;
																		float3 worldPos = float3(IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w);
																		#ifndef USING_DIRECTIONAL_LIGHT
																		  fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
																		#else
																		  fixed3 lightDir = _WorldSpaceLightPos0.xyz;
																		#endif
																		surfIN.worldPos = worldPos;
																		#ifdef UNITY_COMPILER_HLSL
																		SurfaceOutputStandard o = (SurfaceOutputStandard)0;
																		#else
																		SurfaceOutputStandard o;
																		#endif
																		o.Albedo = 0.0;
																		o.Emission = 0.0;
																		o.Alpha = 0.0;
																		o.Occlusion = 1.0;
																		fixed3 normalWorldVertex = fixed3(0,0,1);

																		// call surface function
																		surf(surfIN, o);
																		UnityMetaInput metaIN;
																		UNITY_INITIALIZE_OUTPUT(UnityMetaInput, metaIN);
																		metaIN.Albedo = o.Albedo;
																		metaIN.Emission = o.Emission;
																		return UnityMetaFragment(metaIN);
																	  }


																	  #endif


																	  ENDCG

																	  }

																// ---- end of surface shader generated code

															#LINE 113

		}

			FallBack "Legacy Shaders/Diffuse"
}