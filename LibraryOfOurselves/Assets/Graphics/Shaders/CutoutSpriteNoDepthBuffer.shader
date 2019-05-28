Shader "Unlit/CutoutSpriteNoDepthBuffer" {
	Properties{
		_MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
		_Cutoff("Alpha cutoff", Range(0,1)) = 0.5

		_Color("Tint", Color) = (1,1,1,1)

		_StencilComp("Stencil Comparison", Float) = 8
		_Stencil("Stencil ID", Float) = 0
		_StencilOp("Stencil Operation", Float) = 0
		_StencilWriteMask("Stencil Write Mask", Float) = 255
		_StencilReadMask("Stencil Read Mask", Float) = 255

		_ColorMask("Color Mask", Float) = 15

		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0
	}
		SubShader{
			Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
			LOD 100

			Lighting Off
			Cull Off
			ZWrite Off
			ZTest Always
			Blend SrcAlpha OneMinusSrcAlpha

			Pass {
				CGPROGRAM
					#pragma vertex vert
					#pragma fragment frag
					#pragma target 2.0

					#include "UnityCG.cginc"

					struct appdata_t {
						float4 vertex : POSITION;
						float2 texcoord : TEXCOORD0;
						float4 color    : COLOR;
						UNITY_VERTEX_INPUT_INSTANCE_ID
					};

					struct v2f {
						float4 vertex : SV_POSITION;
						float2 texcoord : TEXCOORD0;
						fixed4 color : COLOR;
						UNITY_VERTEX_OUTPUT_STEREO
					};

					sampler2D _MainTex;
					float4 _MainTex_ST;
					fixed _Cutoff;
					fixed4 _Color;
					fixed4 _TextureSampleAdd;
					float4 _ClipRect;

					v2f vert(appdata_t v) {
						v2f o;
						UNITY_SETUP_INSTANCE_ID(v);
						UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
						o.vertex = UnityObjectToClipPos(v.vertex);
						o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

						o.color = v.color * _Color;
						return o;
					}

					fixed4 frag(v2f i) : SV_Target
					{
						float alpha = i.color.a;
						i.color.a = 1;
						half4 color = (tex2D(_MainTex, i.texcoord) + _TextureSampleAdd) * i.color;


#ifdef UNITY_UI_CLIP_RECT
						color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
#endif

#ifdef UNITY_UI_ALPHACLIP
						clip(color.a - 0.001);
#endif

						clip(color.a - _Cutoff);

						color.a = alpha;
						return color;
					}
				ENDCG
			}
		}

}
