Shader "Custom/CelTextureAnimatedOutlineShader"
{
	Properties
	{
		[MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
		[MainTexture] _BaseMap("Base Map", 2D) = "white" {}
		_ShadowColor("Shadow Color", Color) = (1, 1, 1, 1)
		_ShadowColorMap("Shadow Map", 2D) = "white" {}
		_ShadowAngle("Shadow Angle", Float) = 0.0
		_Positions("Positions", 2D) = "" {}
		_Normals("Normals", 2D) = "" {}
	}

	SubShader
	{
		Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

		Pass
		{
			ZWrite Off
			Cull Front

			HLSLPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl" 

			struct Attributes
			{
				float4 positionOS : POSITION;
				float2 uv : TEXCOORD0;
				uint vertId : SV_VertexID;
				uint instanceId : SV_InstanceID;
			};

			struct Varyings
			{
				float4 positionHCS : SV_POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			TEXTURE2D(_BaseMap);
			SAMPLER(sampler_BaseMap);
			TEXTURE2D(_ShadowColorMap);
			SAMPLER(sampler_ShadowColorMap);
			TEXTURE2D(_Positions);
			SAMPLER(sampler_Positions);
			float4 _Positions_TexelSize;
			TEXTURE2D(_Normals);
			SAMPLER(sampler_Normals);

			CBUFFER_START(UnityPerMaterial)
				half4 _BaseColor;
				half4 _ShadowColor;
				float _ShadowAngle;
				float4 _BaseMap_ST;
			CBUFFER_END

			StructuredBuffer<float4x4> _LocalToWorld;
			StructuredBuffer<float4> _AnimTime;
			float3 _CamPos;
			float3 _CamForward;

			Varyings vert(Attributes IN)
			{
				float4 animTimes = _AnimTime[IN.instanceId];
				float animTime1 = animTimes.x;
				float animTime2 = animTimes.y;
				float animBlendFactor = animTimes.z;
				float2 vertPos1 = float2(0.5 + animTime1, _Positions_TexelSize.w - 1.0 - float(IN.vertId) + 0.5) / _Positions_TexelSize.zw;
				float4 posSample1 = SAMPLE_TEXTURE2D_LOD(_Positions, sampler_Positions, vertPos1, 0.0) * 15.0;
				float4 normSample1 = SAMPLE_TEXTURE2D_LOD(_Normals, sampler_Normals, vertPos1, 0.0);
				float2 vertPos2 = float2(0.5 + animTime2, _Positions_TexelSize.w - 1.0 - float(IN.vertId) + 0.5) / _Positions_TexelSize.zw;
				float4 posSample2 = SAMPLE_TEXTURE2D_LOD(_Positions, sampler_Positions, vertPos2, 0.0) * 15.0;
				float4 normSample2 = SAMPLE_TEXTURE2D_LOD(_Normals, sampler_Normals, vertPos2, 0.0);
				float4x4 localToWorld = _LocalToWorld[IN.instanceId];
				float3 worldPos = mul(localToWorld, float4(lerp(posSample1.xyz, posSample2.xyz, animBlendFactor), 1.0)).xyz;
				float3 worldNorm = normalize(lerp(normalize(normSample1.xyz), normalize(normSample2.xyz), animBlendFactor));
				float viewZ = dot(_CamForward, worldPos - _CamPos);
				worldPos += worldNorm * 0.0025 * viewZ;
				Varyings OUT;
				OUT.positionHCS = TransformObjectToHClip(worldPos);
				OUT.normal = worldNorm;
				OUT.uv = float2(posSample1.w, normSample1.w);
				return OUT;
			}

			half4 frag(Varyings IN) : SV_Target
			{
				return half4(0.0, 0.0, 0.0, 1.0);
			}
			ENDHLSL
		}
	}
}
