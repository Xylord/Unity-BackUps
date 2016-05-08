Shader "Custom/SurfandVerttest" {
	Properties{
		_MainTex("Texture", 2D) = "white" {}
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		CGPROGRAM
#pragma surface surf Lambert vertex:vert
	struct Input {
		float2 uv_MainTex;
		half alpha;
	};

	half        _MinVisDistance;
	half        _MaxVisDistance;

	void vert(inout appdata_full v, out Input o) {
		UNITY_INITIALIZE_OUTPUT(Input, o);
		_MinVisDistance = 0;
		_MaxVisDistance = 10;
		half3 viewDirW = _WorldSpaceCameraPos - mul((half4x4)_Object2World, v.vertex);
		half viewDist = length(viewDirW);
		half falloff = saturate((viewDist - _MinVisDistance) / (_MaxVisDistance - _MinVisDistance));
		o.alpha = (1 - falloff); // viewDist / 10);
	}
	sampler2D _MainTex;
	void surf(Input IN, inout SurfaceOutput o) {
		o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
		o.Albedo *= IN.alpha;
	}
	ENDCG
	}
		Fallback "Diffuse"
}