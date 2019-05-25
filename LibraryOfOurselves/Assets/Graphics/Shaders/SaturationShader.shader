// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "UI/Saturation"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)

		_StencilComp("Stencil Comparison", Float) = 8
		_Stencil("Stencil ID", Float) = 0
		_StencilOp("Stencil Operation", Float) = 0
		_StencilWriteMask("Stencil Write Mask", Float) = 255
		_StencilReadMask("Stencil Read Mask", Float) = 255

		_ColorMask("Color Mask", Float) = 15

		_Saturation("Saturation", Range(0,1)) = 1

		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0
	}

		SubShader
		{
			Tags
			{
				"Queue" = "Transparent"
				"IgnoreProjector" = "True"
				"RenderType" = "Transparent"
				"PreviewType" = "Plane"
				"CanUseSpriteAtlas" = "True"
			}

			Stencil
			{
				Ref[_Stencil]
				Comp[_StencilComp]
				Pass[_StencilOp]
				ReadMask[_StencilReadMask]
				WriteMask[_StencilWriteMask]
			}

			Cull Off
			Lighting Off
			ZWrite Off
			ZTest[unity_GUIZTestMode]
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMask[_ColorMask]

			Pass
			{
				Name "Default"
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 2.0

				#include "UnityCG.cginc"
				#include "UnityUI.cginc"

				#pragma multi_compile __ UNITY_UI_CLIP_RECT
				#pragma multi_compile __ UNITY_UI_ALPHACLIP

				struct appdata_t {
					float4 vertex   : POSITION;
					float4 color    : COLOR;
					float2 texcoord : TEXCOORD0;
					UNITY_VERTEX_INPUT_INSTANCE_ID
				};

				struct v2f {
					float4 vertex   : SV_POSITION;
					fixed4 color : COLOR;
					float2 texcoord  : TEXCOORD0;
					float4 worldPosition : TEXCOORD1;
					UNITY_VERTEX_OUTPUT_STEREO
				};

				fixed4 _Color;
				fixed4 _TextureSampleAdd;
				float4 _ClipRect;
				float _Saturation;

				v2f vert(appdata_t v) {
					v2f OUT;
					UNITY_SETUP_INSTANCE_ID(v);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
					OUT.worldPosition = v.vertex;
					OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

					OUT.texcoord = v.texcoord;

					OUT.color = v.color * _Color;
					return OUT;
				}

				sampler2D _MainTex;

				float3 rgb_to_hsv_no_clip(float3 RGB) {
					float3 HSV;

					float minChannel, maxChannel;
					if (RGB.x > RGB.y) {
						maxChannel = RGB.x;
						minChannel = RGB.y;
					} else {
						maxChannel = RGB.y;
						minChannel = RGB.x;
					}

					if (RGB.z > maxChannel) maxChannel = RGB.z;
					if (RGB.z < minChannel) minChannel = RGB.z;

					HSV.xy = 0;
					HSV.z = maxChannel;
					float delta = maxChannel - minChannel;             //Delta RGB value
					if (delta != 0) {                    // If gray, leave H  S at zero
						HSV.y = delta / HSV.z;
						float3 delRGB;
						delRGB = (HSV.zzz - RGB + 3 * delta) / (6.0*delta);
						if (RGB.x == HSV.z) HSV.x = delRGB.z - delRGB.y;
						else if (RGB.y == HSV.z) HSV.x = (1.0 / 3.0) + delRGB.x - delRGB.z;
						else if (RGB.z == HSV.z) HSV.x = (2.0 / 3.0) + delRGB.y - delRGB.x;
					}
					return (HSV);
				}

				float3 hsv_to_rgb(float3 HSV) {
					float3 RGB = HSV.z;

					float var_h = HSV.x * 6;
					float var_i = floor(var_h);   // Or ... var_i = floor( var_h )
					float var_1 = HSV.z * (1.0 - HSV.y);
					float var_2 = HSV.z * (1.0 - HSV.y * (var_h - var_i));
					float var_3 = HSV.z * (1.0 - HSV.y * (1 - (var_h - var_i)));
					if (var_i == 0) { RGB = float3(HSV.z, var_3, var_1); } else if (var_i == 1) { RGB = float3(var_2, HSV.z, var_1); } else if (var_i == 2) { RGB = float3(var_1, HSV.z, var_3); } else if (var_i == 3) { RGB = float3(var_1, var_2, HSV.z); } else if (var_i == 4) { RGB = float3(var_3, var_1, HSV.z); } else { RGB = float3(HSV.z, var_1, var_2); }

					return (RGB);
				}


				fixed4 frag(v2f IN) : SV_Target
				{
					float4 colMult = IN.color;
					colMult.a = 1;
					half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * colMult;

					#ifdef UNITY_UI_CLIP_RECT
					color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
					#endif

					#ifdef UNITY_UI_ALPHACLIP
					clip(color.a - 0.001);
					#endif

					float saturation = _Saturation;
					float3 hsv = rgb_to_hsv_no_clip(color.rgb);
					hsv.g *= saturation;
					color.rgb = hsv_to_rgb(hsv);


					return color;
				}
			ENDCG
			}
		}
}
