Shader "WD/Outline with Pulse Option" {
	Properties {
//		_Color ("Main Color", Color) = (.5,.5,.5,1)
		_OutlineColor ("Outline Color", Color) = (0,0,0,1)
		_Outline ("Outline width", Range (0.0, 0.03)) = .005
		_MainTex ("Base (RGB)", 2D) = "white" { }
		_Speed ("Speed", Range(0.0, 300.0)) = 0.0
	}
	
CGINCLUDE 

			
			float _Outline;
			float4 _OutlineColor;
			float _Speed;
			sampler2D _MainTex; 
			
			struct Input {
				float2 uv_MainTex;
			}; 
			
ENDCG

	SubShader {
		// note that a vertex shader is specified here but its using the one above 
			Tags {  "Queue" = "Transparent"  }
		Pass {
			Name "OUTLINE"
			Tags { "LightMode" = "Always" }
			Cull Back
			ZWrite Off
			ZTest LEqual
			ColorMask RGB // alpha not used

			// you can choose what kind of blending mode you want for the outline
			Blend SrcAlpha OneMinusSrcAlpha // Normal
//			Blend One One // Additive
			//Blend One OneMinusDstColor // Soft Additive
			//Blend DstColor Zero // Multiplicative
			//Blend DstColor SrcColor // 2x Multiplicative

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			
			struct appdata {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};
			
			struct v2f {
				float4 pos : POSITION;
				float4 color : COLOR;
			};
			
			v2f vert(appdata v) {
				// just make a copy of incoming vertex data but scaled according to normal direction
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
			
				float3 norm   = mul ((float3x3)UNITY_MATRIX_IT_MV, v.normal);
				float2 offset = TransformViewToProjection(norm.xy);
			
				o.pos.xy += offset * o.pos.z * _Outline;
				o.color = _OutlineColor;
				return o;
			}
			
			half4 frag(v2f i) :COLOR {
				return  half4(i.color.rgb, (sin(_Time.x * _Speed) + 1) * 0.5);
			}
			ENDCG
		}

		Pass {
//			Tags { "Queue" = "Geometry" "RenderType"="Opaque" }	
			
			Name "BASE"
			ZWrite On
			ZTest LEqual
//			Blend One  OneMinusSrcAlpha 
			Cull Back
			Material {
				Diffuse (1,1,1,1)
				Ambient (1,1,1,1)
			}
			Lighting On
			
			SetTexture [_MainTex] {
				Combine texture * primary Quad
			}
		}
	}
	Fallback "Diffuse"
}
