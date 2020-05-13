Shader "Unlit/Raymarch"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "Primitives.hlsl"
			#include "Utils.hlsl"

			#define MAX_STEPS 100
			#define MAX_DIST 100
			#define SURF_DIST 1e-3

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 ro : TEXCOORD1;
				float3 hitPos : TEXCOORD2;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.ro = _WorldSpaceCameraPos;
				o.hitPos = mul(unity_ObjectToWorld, v.vertex);
				return o;
			}

			float GetDist(in float3 p) {

				float3 infp = infinite(p, 5);

				//return smin(smin(sdBox(p, float3(1.0, 1.0, 1.0)), sdTorus(p, float2(2.0, 0.5)), 1),sdPlane(p + float3(0,3,0)),1);
				return min(sdOctahedron(p, 2), sdPlane(p + float3(0, 3, 0)));

				float d = length(p) - 1.;
				return d;
			}


			float3 GetNormal(in float3 p) {
				float2 e = float2(1e-2, 0);
				float3 n = GetDist(p) - float3(
					GetDist(p - e.xyy),
					GetDist(p - e.yxy),
					GetDist(p - e.yyx)
					);

				return normalize(n);
			}

			

			float Raymarch(in float3 ro, in float3 rd) {
				float dO = 0;
				float dS;

				for (int i = 0; i < MAX_STEPS; i++) {
					float3 p = ro + dO * rd;
					dS = GetDist(p);
					dO += dS;
					if (dS < SURF_DIST || dO > MAX_DIST) {
						break;
					}
				}

				return dO;
			}

			float GetLight(in float3 p) {

				//float3 lightPos = _WorldSpaceLightPos0.xyz; //float3(0, 5, 6);
				//lightPos.xz += vec2(sin(iTime), cos(iTime)) * 2.;
				float3 l = normalize(_WorldSpaceLightPos0.xyz);// normalize(lightPos - p);
				float3 n = GetNormal(p);

				float dif = clamp(dot(n, l), 0., 1.);
				float d = Raymarch(p + n * SURF_DIST * 2., l);
				/*if (d < length(lightPos - p)) {
					dif *= .1;
				}*/

				return dif;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float2 uv = i.uv - .5;
				float3 ro = i.ro;
				float3 rd = normalize(i.hitPos - ro);

				float d = Raymarch(ro, rd);

				fixed4 col = 0;

				if (d < MAX_DIST) {
					float3 p = ro + rd * d;
					//float3 n = GetNormal(p);
					float dif = GetLight(p);
					float shadow = Softshadow(p, normalize(_WorldSpaceLightPos0.xyz), 0.01, 3.0, 1.0 / 8.0);
					dif *= shadow;
					col.rgb = float3(dif, dif, dif);
				}
				else {
					discard;
				}

				// sample the texture
				//col.rgb = rd;
				return col;
			}
		ENDCG
	}
	}
}
