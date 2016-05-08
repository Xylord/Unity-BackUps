Shader "Unlit/AlphaFade"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_MinVisDistance ("Min", Float) = 0
		_MaxVisDistance ("Max", Float) = 100
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				half4 pos       : POSITION;
				fixed4 color : COLOR0;
				half2 uv        : TEXCOORD0;
			};



			sampler2D   _MainTex;
			half        _MinVisDistance = 0;
			half        _MaxVisDistance = 100;
			half		swag = 0;



			v2f vert(appdata_full v)
			{
				v2f o;

				o.pos = mul((half4x4)UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.texcoord.xy;
				o.color = v.color;

				//distance falloff
				half3 viewDirW = _WorldSpaceCameraPos - mul((half4x4)_Object2World, v.vertex);
				half viewDist = length(viewDirW);
				half falloff = saturate((viewDist - _MinVisDistance) / (_MaxVisDistance - _MinVisDistance));
				swag = falloff;
				o.color.a *= (1.0f - falloff);
				return o;
			}


			fixed4 frag(v2f i) : COLOR
			{
				fixed4 color = tex2D(_MainTex, i.uv) * i.color * (1.0f - swag);
			return color;
			}
			ENDCG
		}
	}
}
