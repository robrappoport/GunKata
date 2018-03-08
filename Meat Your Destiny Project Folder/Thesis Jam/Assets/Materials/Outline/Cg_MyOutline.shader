Shader "FX/Cg_MyOutline"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Tint("Tint", Color) = (1,1,1,1)
		_Flip("Flip", Range(0,10)) = 1
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" "Queue"="Overlay"}
		LOD 100

		GrabPass{}

		Pass
		{
			Stencil{
				ref 1
				Comp Equal
				Pass Keep
			}

			Stencil{
				ref 1
				Comp Greater
				Pass Replace
			}

			ZWrite Off
			ZTest Always
			Blend One Zero

			Name "Outline"

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float4 uvgrab : TEXCOORD1;
			};

			sampler2D _MainTex;
			sampler2D _GrabTexture;
			float4 _MainTex_ST;
			float4 _GrabTexture_ST;
			float4 _Tint;
			float _Flip;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uvgrab = ComputeGrabScreenPos(o.vertex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 substract = tex2Dproj(_GrabTexture, i.uvgrab);
				substract = clamp(1 - substract,0,1) * _Flip * _Tint;
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return substract;
			}
			ENDCG
		}
	}
}
