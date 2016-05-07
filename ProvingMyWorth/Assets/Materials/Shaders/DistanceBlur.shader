Shader "Custom/DistanceBlur" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Dist("Distance", Float) = 1
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0

		_EmissionColor("Color", Color) = (0,0,0)
		_EmissionMap("Emission", 2D) = "white" {}
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
#pragma surface surf Standard fullforwardshadows


		// Use shader model 3.0 target, to get nicer looking lighting
#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _EmissionMap;

	struct Input {
		float2 uv_MainTex, uv_EmissionMap;
		float3 viewDir;
		fixed4 pos : SV_POSITION;
	};

	half _Dist;
	half _Glossiness;
	half _Metallic;
	fixed4 _Color;
	float4 vertex : POSITION;

	void surf(Input IN, inout SurfaceOutputStandard o) {
		float dist = length(mul(_World2Object, float4 (IN.viewDir.xy, IN.viewDir.z + _Dist, 0)));
		// Albedo comes from a texture tinted by color
		//fixed4 distVector = IN.pos;
		//float3 test = (0, 0, 0);
		float distVec = distance(_WorldSpaceCameraPos, vertex);
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
		o.Albedo = c.rgb;
		// Metallic and smoothness come from slider variables
		o.Metallic = _Metallic;
		o.Smoothness = saturate(_Glossiness * dist);// / distVec * _Dist);//dist);
		o.Emission = tex2D (_EmissionMap, IN.uv_EmissionMap) * _Color;
		o.Alpha = c.a;
	}
	ENDCG
	}
		FallBack "Diffuse"
}