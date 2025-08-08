Shader "Custom/FogSafeZoneDonut"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _SafeRadius ("Safe Radius", Float) = 5
        _Center ("Center", Vector) = (0,0,0,0)
        _Color ("Color Tint", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _SafeRadius;
            float4 _Center;
            fixed4 _Color;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 centerXZ = _Center.xz;
                float2 worldXZ = i.worldPos.xz;
                float dist = distance(worldXZ, centerXZ);

                // 0~1 마스크 생성 (SafeRadius 안이면 alpha 0, 바깥으로 갈수록 1)
                float edgeSmooth = 1.0; // 부드럽게 전환될 거리 (단위: world units)
                float alphaMask = smoothstep(_SafeRadius - edgeSmooth, _SafeRadius, dist);

                fixed4 tex = tex2D(_MainTex, i.uv) * _Color;
                tex.a *= alphaMask;

                return tex;
            }
            ENDCG
        }
    }
}
