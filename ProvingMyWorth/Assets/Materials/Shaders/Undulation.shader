Shader "Custom/Undulation" {
	Properties{
		_MainColor("Color", Color) = (1,1,1,1)
		_MinY("Minimum Y Value", float) = 0.0

		_xScale("X Amount", Range(-1,1)) = 0.5
		_yScale("Z Amount", Range(-1,1)) = 0.5

		_Scale("Effect Scale", float) = 1.0
		_Speed("Effect Speed", float) = 1.0

		_WorldScale("World Scale", float) = 1.0
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }

		CGPROGRAM
#pragma surface surf Standard fullforwardshadows vertex:vert           
#pragma target 3.0
#include "UnityCG.cginc"

		float4 _MainColor;
	float _MinY;
	float _xScale;
	float _yScale;
	float _Scale;
	float _WorldScale;
	float _Speed;

	struct Input {
		float2 uv_MainTex;
	};

	float _Amount;

	void vert(inout appdata_full v) {
		float num = v.vertex.z;

		if ((num - _MinY) > 0.0) {
			float3 worldPos = mul(_Object2World, v.vertex).xyz;
			float x = sin(worldPos.x / _WorldScale + (_Time.y*_Speed)) * (num - _MinY) * _Scale * 0.01;
			float y = sin(worldPos.y / _WorldScale + (_Time.y*_Speed)) * (num - _MinY) * _Scale * 0.01;

			v.vertex.x += x * _xScale;
			v.vertex.y += y * _yScale;
		}
	}

	sampler2D _MainTex;

	void surf(Input IN, inout SurfaceOutputStandard o) {
		o.Albedo = _MainColor;
		// Metallic and smoothness come from slider variables
		o.Metallic = 0.0f;
		o.Smoothness = 0.0f;
		o.Alpha = 1.0f;
	}

	ENDCG
	}
		Fallback "Diffuse"
}