﻿Shader "Unlit/Raymarch"
{
	Properties
	{
		_Cube("Cubemap", CUBE) = "" {}
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

			#define MAX_STEPS 150
			#define MAX_DIST 500
			#define SURF_DIST 1e-2

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

			samplerCUBE _Cube;
			uniform RWStructuredBuffer<float3> raymarchObjectData : register(u1);

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.ro = _WorldSpaceCameraPos;
				o.hitPos = mul(unity_ObjectToWorld, v.vertex);
				return o;
			}



			/*float DE(float3 z)
			{
				float r;
				int n = 0;
				while (n < Iterations) {
					if (z.x + z.y < 0) z.xy = -z.yx; // fold 1
					if (z.x + z.z < 0) z.xz = -z.zx; // fold 2
					if (z.y + z.z < 0) z.zy = -z.yz; // fold 3
					z = z * Scale - Offset * (Scale - 1.0);
					n++;
				}
				return (length(z)) * pow(Scale, -float(n));
			}*/

			// Return distance from p to nearest point on object
			float GetDist(in float3 p) {

				float3 infp = p;
				boxFold(infp, 5.+p.x);

				//return smin(smin(sdBox(p, float3(1.0, 1.0, 1.0)), sdTorus(p, float2(2.0, 0.5)), 1),sdPlane(p + float3(0,3,0)),1);
				//float geom = sdPlane(p + float3(0, 3, 0));
				//geom = opUnion(geom, opSubtraction(sdSphere(p + float3(2, 0, 0), 1.0), sdOctahedron(p, 2)));
				//geom = opUnion(geom, sdSphere(p + float3(4,0,0), 1.0));
				float geom = sdSphere(infp + float3(1.0,0,0), 1.0);

				return geom;
			}

			// 
			float3 GetNormal(in float3 p) {
				float2 e = float2(1e-2, 0);
				float3 n = GetDist(p) - float3(
					GetDist(p - e.xyy),
					GetDist(p - e.yxy),
					GetDist(p - e.yyx)
					);

				return normalize(n);
			}

			// Return distance from ro to object in ray direction rd
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
				//float d = Raymarch(p + n * SURF_DIST * 2., l);
				/*if (d < length(lightPos - p)) {
					dif *= .1;
				}*/

				return dif;
			}

			float ref(float3 i, float3 n) {
				return i - 2.0 * dot(n, i) * n;
			}

			float GetReflection(in float3 p, in float oldrd) {
				float3 n = GetNormal(p);

				float3 rd = reflect(oldrd, n);
				//float3 ro = p + rd * 0.02;

				return 0.0;
				/*float d = Raymarch(ro, rd);

				float3 p2 = ro + rd * d;
				//return p2/6;
				float dif = GetLight(p2);

				return dif;*/
			}

			float shadow(float3 p)
			{
				float3 l = -normalize(_WorldSpaceLightPos0.xyz);
				float3 ro = p;
				float3 rd = -l;

				for (float t = 0.04; t < 30.0; )
				{
					float h = GetDist(ro + rd * t);
					if (h < 0.001)
						return 0.0;
					t += h;
				}
				return 1.0;
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

					//float ref = GetReflection(p, rd);

					float s = clamp(shadow(p) + 0.3,0.0,1.0);
					dif *= s;

					//dif += ref * 0.5;
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
