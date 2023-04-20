Shader "Lpk/LightModel/ToonLightBase"
{
    Properties
    {
        _BaseMap            ("Texture", 2D)                       = "white" {}
        _BaseColor          ("Color", Color)                      = (0.5,0.5,0.5,1)
        
        [Space]
        _ShadowStep         ("ShadowStep", Range(0, 1))           = 0.5
        _ShadowStepSmooth   ("ShadowStepSmooth", Range(0, 1))     = 0.04
        
        [Space] 
       /* _SpecularStep       ("SpecularStep", Range(0, 1))         = 0.6
        _SpecularStepSmooth ("SpecularStepSmooth", Range(0, 1))   = 0.05
        [HDR]_SpecularColor ("SpecularColor", Color)              = (1,1,1,1)*/
        
        [Space]
        _RimStep            ("RimStep", Range(0, 1))              = 0.65
        _RimStepSmooth      ("RimStepSmooth",Range(0,1))          = 0.4
        _RimColor           ("RimColor", Color)                   = (1,1,1,1)
        
        [Space]   
        //_OutlineWidth      ("OutlineWidth", Range(0.0, 1.0))      = 0.15
         _OutlineColor      ("OutlineColor", Color)                = (0.0, 0.0, 0.0, 1)

        [Space]
        _NoiseMap("Wind Texture", 2D) = "white" {}
        _NoiseControl ("xy tile / zw speed", vector) = (1,1,0,0)
        _DirectionDeformation ("Direction Deformation", vector) = (1,0,0,0)
        _OffsetVertex ("Offset Vertex", float) = 0
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

		///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		Pass
					{
						Name "DepthNormals"
						Tags
						{
							"LightMode" = "DepthNormals"
						}

			// Render State
			Cull Back
			ZTest LEqual
			ZWrite On

			// Debug
			// <None>

			// --------------------------------------------------
			// Pass

			HLSLPROGRAM

			// Pragmas
			#pragma target 4.5
			#pragma exclude_renderers gles gles3 glcore
			#pragma multi_compile_instancing
			#pragma multi_compile _ DOTS_INSTANCING_ON
			#pragma vertex vert
			#pragma fragment frag

			// DotsInstancingOptions: <None>
			// HybridV1InjectedBuiltinProperties: <None>

			// Keywords
			// PassKeywords: <None>
			// GraphKeywords: <None>

			// Defines

			#define _NORMALMAP 1
			#define _NORMAL_DROPOFF_TS 1
			#define ATTRIBUTES_NEED_NORMAL
			#define ATTRIBUTES_NEED_TANGENT
			#define ATTRIBUTES_NEED_TEXCOORD1
			#define VARYINGS_NEED_NORMAL_WS
			#define VARYINGS_NEED_TANGENT_WS
			#define FEATURES_GRAPH_VERTEX
			/* WARNING: $splice Could not find named fragment 'PassInstancing' */
			#define SHADERPASS SHADERPASS_DEPTHNORMALS
			/* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */


			// custom interpolator pre-include
			/* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

			// Includes
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			// --------------------------------------------------
			// Structs and Packing

			// custom interpolators pre packing
			/* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

			struct Attributes
			{
				 float3 positionOS : POSITION;
				 float3 normalOS : NORMAL;
				 float4 tangentOS : TANGENT;
				 float4 uv1 : TEXCOORD1;
				#if UNITY_ANY_INSTANCING_ENABLED
				 uint instanceID : INSTANCEID_SEMANTIC;
				#endif
			};
			struct Varyings
			{
				 float4 positionCS : SV_POSITION;
				 float3 normalWS;
				 float4 tangentWS;
				#if UNITY_ANY_INSTANCING_ENABLED
				 uint instanceID : CUSTOM_INSTANCE_ID;
				#endif
				#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
				 uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
				#endif
				#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
				 uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
				#endif
				#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
				 FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
				#endif
			};
			struct SurfaceDescriptionInputs
			{
				 float3 TangentSpaceNormal;
			};
			struct VertexDescriptionInputs
			{
				 float3 ObjectSpaceNormal;
				 float3 ObjectSpaceTangent;
				 float3 ObjectSpacePosition;
			};
			struct PackedVaryings
			{
				 float4 positionCS : SV_POSITION;
				 float3 interp0 : INTERP0;
				 float4 interp1 : INTERP1;
				#if UNITY_ANY_INSTANCING_ENABLED
				 uint instanceID : CUSTOM_INSTANCE_ID;
				#endif
				#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
				 uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
				#endif
				#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
				 uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
				#endif
				#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
				 FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
				#endif
			};

			PackedVaryings PackVaryings(Varyings input)
			{
				PackedVaryings output;
				ZERO_INITIALIZE(PackedVaryings, output);
				output.positionCS = input.positionCS;
				output.interp0.xyz = input.normalWS;
				output.interp1.xyzw = input.tangentWS;
				#if UNITY_ANY_INSTANCING_ENABLED
				output.instanceID = input.instanceID;
				#endif
				#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
				output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
				#endif
				#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
				output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
				#endif
				#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
				output.cullFace = input.cullFace;
				#endif
				return output;
			}

			Varyings UnpackVaryings(PackedVaryings input)
			{
				Varyings output;
				output.positionCS = input.positionCS;
				output.normalWS = input.interp0.xyz;
				output.tangentWS = input.interp1.xyzw;
				#if UNITY_ANY_INSTANCING_ENABLED
				output.instanceID = input.instanceID;
				#endif
				#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
				output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
				#endif
				#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
				output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
				#endif
				#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
				output.cullFace = input.cullFace;
				#endif
				return output;
			}


			// --------------------------------------------------
			// Graph

			// Graph Properties
			CBUFFER_START(UnityPerMaterial)
			CBUFFER_END

				// Object and Global properties

				// Graph Includes
				// GraphIncludes: <None>

				// -- Property used by ScenePickingPass
				#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
				#endif

			// -- Properties used by SceneSelectionPass
			#ifdef SCENESELECTIONPASS
			int _ObjectId;
			int _PassValue;
			#endif

			// Graph Functions
			// GraphFunctions: <None>

			// Custom interpolators pre vertex
			/* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

			// Graph Vertex
			struct VertexDescription
			{
				float3 Position;
				float3 Normal;
				float3 Tangent;
			};

			VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
			{
				VertexDescription description = (VertexDescription)0;
				description.Position = IN.ObjectSpacePosition;
				description.Normal = IN.ObjectSpaceNormal;
				description.Tangent = IN.ObjectSpaceTangent;
				return description;
			}

			// Custom interpolators, pre surface
			#ifdef FEATURES_GRAPH_VERTEX
			Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
			{
			return output;
			}
			#define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
			#endif

			// Graph Pixel
			struct SurfaceDescription
			{
				float3 NormalTS;
			};

			SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
			{
				SurfaceDescription surface = (SurfaceDescription)0;
				surface.NormalTS = IN.TangentSpaceNormal;
				return surface;
			}

			// --------------------------------------------------
			// Build Graph Inputs
			#ifdef HAVE_VFX_MODIFICATION
			#define VFX_SRP_ATTRIBUTES Attributes
			#define VFX_SRP_VARYINGS Varyings
			#define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
			#endif
			VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
			{
				VertexDescriptionInputs output;
				ZERO_INITIALIZE(VertexDescriptionInputs, output);

				output.ObjectSpaceNormal = input.normalOS;
				output.ObjectSpaceTangent = input.tangentOS.xyz;
				output.ObjectSpacePosition = input.positionOS;

				return output;
			}
			SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
			{
				SurfaceDescriptionInputs output;
				ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

			#ifdef HAVE_VFX_MODIFICATION
				// FragInputs from VFX come from two places: Interpolator or CBuffer.
				/* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

			#endif





				output.TangentSpaceNormal = float3(0.0f, 0.0f, 1.0f);


			#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
			#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
			#else
			#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
			#endif
			#undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

					return output;
			}

			// --------------------------------------------------
			// Main

			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthNormalsOnlyPass.hlsl"

			// --------------------------------------------------
			// Visual Effect Vertex Invocations
			#ifdef HAVE_VFX_MODIFICATION
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
			#endif

			ENDHLSL
			}

		///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		Pass
				{
					Name "DepthOnly"
					Tags
					{
						"LightMode" = "DepthOnly"
					}

			// Render State
			Cull Back
			ZTest LEqual
			ZWrite On
			ColorMask 0

			// Debug
			// <None>

			// --------------------------------------------------
			// Pass

			HLSLPROGRAM

			// Pragmas
			#pragma target 4.5
			#pragma exclude_renderers gles gles3 glcore
			#pragma multi_compile_instancing
			#pragma multi_compile _ DOTS_INSTANCING_ON
			#pragma vertex vert
			#pragma fragment frag

			// DotsInstancingOptions: <None>
			// HybridV1InjectedBuiltinProperties: <None>

			// Keywords
			// PassKeywords: <None>
			// GraphKeywords: <None>

			// Defines

			#define _NORMALMAP 1
			#define _NORMAL_DROPOFF_TS 1
			#define ATTRIBUTES_NEED_NORMAL
			#define ATTRIBUTES_NEED_TANGENT
			#define FEATURES_GRAPH_VERTEX
			/* WARNING: $splice Could not find named fragment 'PassInstancing' */
			#define SHADERPASS SHADERPASS_DEPTHONLY
			/* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */


			// custom interpolator pre-include
			/* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

			// Includes
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			// --------------------------------------------------
			// Structs and Packing

			// custom interpolators pre packing
			/* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

			struct Attributes
			{
				 float3 positionOS : POSITION;
				 float3 normalOS : NORMAL;
				 float4 tangentOS : TANGENT;
				#if UNITY_ANY_INSTANCING_ENABLED
				 uint instanceID : INSTANCEID_SEMANTIC;
				#endif
			};
			struct Varyings
			{
				 float4 positionCS : SV_POSITION;
				#if UNITY_ANY_INSTANCING_ENABLED
				 uint instanceID : CUSTOM_INSTANCE_ID;
				#endif
				#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
				 uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
				#endif
				#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
				 uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
				#endif
				#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
				 FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
				#endif
			};
			struct SurfaceDescriptionInputs
			{
			};
			struct VertexDescriptionInputs
			{
				 float3 ObjectSpaceNormal;
				 float3 ObjectSpaceTangent;
				 float3 ObjectSpacePosition;
			};
			struct PackedVaryings
			{
				 float4 positionCS : SV_POSITION;
				#if UNITY_ANY_INSTANCING_ENABLED
				 uint instanceID : CUSTOM_INSTANCE_ID;
				#endif
				#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
				 uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
				#endif
				#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
				 uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
				#endif
				#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
				 FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
				#endif
			};

			PackedVaryings PackVaryings(Varyings input)
			{
				PackedVaryings output;
				ZERO_INITIALIZE(PackedVaryings, output);
				output.positionCS = input.positionCS;
				#if UNITY_ANY_INSTANCING_ENABLED
				output.instanceID = input.instanceID;
				#endif
				#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
				output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
				#endif
				#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
				output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
				#endif
				#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
				output.cullFace = input.cullFace;
				#endif
				return output;
			}

			Varyings UnpackVaryings(PackedVaryings input)
			{
				Varyings output;
				output.positionCS = input.positionCS;
				#if UNITY_ANY_INSTANCING_ENABLED
				output.instanceID = input.instanceID;
				#endif
				#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
				output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
				#endif
				#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
				output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
				#endif
				#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
				output.cullFace = input.cullFace;
				#endif
				return output;
			}


			// --------------------------------------------------
			// Graph

			// Graph Properties
			CBUFFER_START(UnityPerMaterial)
			CBUFFER_END

				// Object and Global properties

				// Graph Includes
				// GraphIncludes: <None>

				// -- Property used by ScenePickingPass
				#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
				#endif

			// -- Properties used by SceneSelectionPass
			#ifdef SCENESELECTIONPASS
			int _ObjectId;
			int _PassValue;
			#endif

			// Graph Functions
			// GraphFunctions: <None>

			// Custom interpolators pre vertex
			/* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

			// Graph Vertex
			struct VertexDescription
			{
				float3 Position;
				float3 Normal;
				float3 Tangent;
			};

			VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
			{
				VertexDescription description = (VertexDescription)0;
				description.Position = IN.ObjectSpacePosition;
				description.Normal = IN.ObjectSpaceNormal;
				description.Tangent = IN.ObjectSpaceTangent;
				return description;
			}

			// Custom interpolators, pre surface
			#ifdef FEATURES_GRAPH_VERTEX
			Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
			{
			return output;
			}
			#define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
			#endif

			// Graph Pixel
			struct SurfaceDescription
			{
			};

			SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
			{
				SurfaceDescription surface = (SurfaceDescription)0;
				return surface;
			}

			// --------------------------------------------------
			// Build Graph Inputs
			#ifdef HAVE_VFX_MODIFICATION
			#define VFX_SRP_ATTRIBUTES Attributes
			#define VFX_SRP_VARYINGS Varyings
			#define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
			#endif
			VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
			{
				VertexDescriptionInputs output;
				ZERO_INITIALIZE(VertexDescriptionInputs, output);

				output.ObjectSpaceNormal = input.normalOS;
				output.ObjectSpaceTangent = input.tangentOS.xyz;
				output.ObjectSpacePosition = input.positionOS;

				return output;
			}
			SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
			{
				SurfaceDescriptionInputs output;
				ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

			#ifdef HAVE_VFX_MODIFICATION
				// FragInputs from VFX come from two places: Interpolator or CBuffer.
				/* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

			#endif







			#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
			#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
			#else
			#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
			#endif
			#undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

					return output;
			}

			// --------------------------------------------------
			// Main

			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthOnlyPass.hlsl"

			// --------------------------------------------------
			// Visual Effect Vertex Invocations
			#ifdef HAVE_VFX_MODIFICATION
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
			#endif

			ENDHLSL
			}

			///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        Pass
        {
            Name "UniversalForward"
            Tags
            {
                "LightMode" = "UniversalForward"
            }
            HLSLPROGRAM
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x

            #pragma vertex vert
            #pragma fragment frag
            // #pragma shader_feature _ALPHATEST_ON
            // #pragma shader_feature _ALPHAPREMULTIPLY_ON
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            // -------------------------------------
            // Unity defined keywords
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
             
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

            TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);
            TEXTURE2D(_NoiseMap); SAMPLER(sampler_NoiseMap);

            float4 _NoiseControl;
            float4 _DirectionDeformation;
            float _OffsetVertex;

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float _ShadowStep;
                float _ShadowStepSmooth;
                float _SpecularStep;
                float _SpecularStepSmooth;
                float4 _SpecularColor;
                float _RimStepSmooth;
                float _RimStep;
                float4 _RimColor;
            CBUFFER_END

            struct Attributes
            {     
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
                float4 tangentOS    : TANGENT;
                float2 uv           : TEXCOORD0;
                float4 color        : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            }; 

            struct Varyings
            {
                float2 uv            : TEXCOORD0;
                float4 normalWS      : TEXCOORD1;    // xyz: normal, w: viewDir.x
                float4 tangentWS     : TEXCOORD2;    // xyz: tangent, w: viewDir.y
                float4 bitangentWS   : TEXCOORD3;    // xyz: bitangent, w: viewDir.z
                float3 viewDirWS     : TEXCOORD4;
				float4 shadowCoord	 : TEXCOORD5;	// shadow receive 
				float4 fogCoord	     : TEXCOORD6;	
				float3 positionWS	 : TEXCOORD7;	
                float4 positionCS    : SV_POSITION;                
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                    
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);

                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
                float3 viewDirWS = GetCameraPositionWS() - vertexInput.positionWS;
                float3 vertexLight = VertexLighting(vertexInput.positionWS, normalInput.normalWS);
                
                ////////////
                // Use world pos xz to project texture
                float2 world_uv = vertexInput.positionWS.xz;
                // Scale texture
                world_uv.x *= _NoiseControl.x;
                world_uv.y *= _NoiseControl.y;
                // Offset texture
                world_uv.x += _NoiseControl.z * _Time;
                world_uv.y += _NoiseControl.w * _Time;
                // Unwrap texture : We need to use the SAMPLE_TEXTURE_2D_LOD in vert pass
                float noise = (SAMPLE_TEXTURE2D_LOD(_NoiseMap, sampler_NoiseMap, world_uv, 0).r * 2) - 1;

                // Multiply offset for confort
                _OffsetVertex *= .01;

                float4 pos = vertexInput.positionCS;
                // Add offset to the position based on noise, vertex color R for masking, and we normalize the direction
                pos += noise * _OffsetVertex * input.color.r * normalize(_DirectionDeformation);
                /////////////

                output.positionCS = pos;
                output.positionWS = vertexInput.positionWS;
                output.uv = input.uv;
                output.normalWS = float4(normalInput.normalWS, viewDirWS.x);
                output.tangentWS = float4(normalInput.tangentWS, viewDirWS.y);
                output.bitangentWS = float4(normalInput.bitangentWS, viewDirWS.z);
                output.viewDirWS = viewDirWS;
                output.fogCoord = ComputeFogFactor(output.positionCS.z);
                return output;
            }
            
            half remap(half x, half t1, half t2, half s1, half s2)
            {
                return (x - t1) / (t2 - t1) * (s2 - s1) + s1;
            }
            
            float4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);

                float2 uv = input.uv;
                float3 N = normalize(input.normalWS.xyz);
                float3 T = normalize(input.tangentWS.xyz);
                float3 B = normalize(input.bitangentWS.xyz);
                float3 V = normalize(input.viewDirWS.xyz);
                float3 L = normalize(_MainLightPosition.xyz);
                float3 H = normalize(V+L);
                
                float NV = dot(N,V);
                float NH = dot(N,H);
                float NL = dot(N,L);
                
                NL = NL * 0.5 + 0.5;

                float4 baseMap = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv);

                // return NH;
               float specularNH = smoothstep((1-_SpecularStep * 0.05)  - _SpecularStepSmooth * 0.05, (1-_SpecularStep* 0.05)  + _SpecularStepSmooth * 0.05, NH) ;
               float shadowNL = smoothstep(_ShadowStep - _ShadowStepSmooth, _ShadowStep + _ShadowStepSmooth, NL);

				input.shadowCoord = TransformWorldToShadowCoord(input.positionWS);
                
                //shadow
                float shadow = MainLightRealtimeShadow(input.shadowCoord);
                
                //rim
                float rim = smoothstep((1-_RimStep) - _RimStepSmooth * 0.5, (1-_RimStep) + _RimStepSmooth * 0.5, 0.5 - NV);
                
                //diffuse
                float3 diffuse = _MainLightColor.rgb * baseMap * _BaseColor * shadowNL * shadow;
                
                //specular
                float3 specular = _SpecularColor * shadow * shadowNL *  specularNH;
                
                //ambient
                float3 ambient =  rim * _RimColor + SampleSH(N) * _BaseColor * baseMap;
            
                float3 finalColor = diffuse + ambient + specular;
                finalColor = MixFog(finalColor, input.fogCoord);
                return float4(finalColor , 1.0);
            }
            ENDHLSL
        }
        
        //Outline
       /* Pass
        {
            Name "Outline"
            Cull Front
            Tags
            {
                "LightMode" = "SRPDefaultUnlit"
            }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
            };

            struct v2f
            {
                float4 pos      : SV_POSITION;
                float4 fogCoord	: TEXCOORD0;	
            };
            
            float _OutlineWidth;
            float4 _OutlineColor;
            
            v2f vert(appdata v)
            {
                v2f o;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(v.vertex.xyz);
                o.pos = TransformObjectToHClip(float4(v.vertex.xyz + v.normal * _OutlineWidth * 0.1 ,1));
                o.fogCoord = ComputeFogFactor(vertexInput.positionCS.z);

                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float3 finalColor = MixFog(_OutlineColor, i.fogCoord);
                return float4(finalColor,1.0);
            }*/
            
          /*  ENDHLSL
        }*/
        UsePass "Universal Render Pipeline/Lit/ShadowCaster"
    }
}
