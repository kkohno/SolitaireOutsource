// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/BurningPaper"
{
	Properties {
		_MainTex ("Main texture", 2D) = "white" {}
		_DissolveTex ("Dissolution texture", 2D) = "gray" {}
		_Threshold ("Threshold", Range(0, 1.1)) = 0
		_Color ("Tint", Color) = (1,1,1,1)
	}

	SubShader {

		Tags { "Queue"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite On
		Pass {
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};
			
			sampler2D _MainTex;
			float4 _MainTex_ST;

			v2f vert(appdata_base v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}
			fixed4 _Color;
			sampler2D _DissolveTex;
			float _Threshold;

			fixed4 frag(v2f i) : SV_Target {
				fixed4 c = tex2D(_MainTex, i.uv);
				fixed val = 1 - tex2D(_DissolveTex, i.uv).r;
				if(val < _Threshold - 0.04)
				{
					discard;
				}
				bool b = val < _Threshold;
				fixed4 res = lerp(c,  _Color, b);
				res.a = res.a * c.a;
				return res;
			}

			ENDCG

		}

	}
	FallBack "Diffuse"
}