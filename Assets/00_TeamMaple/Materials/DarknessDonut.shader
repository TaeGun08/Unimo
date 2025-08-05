Shader "Custom/DarknessDonut"
{
    Properties
    {
        _Color ("Overlay Color", Color) = (0,0,0,1)
        _PlayerWorldPosition ("Player Position", Vector) = (0,0,0,0)
        _InnerRadius ("Inner Radius", Float) = 1.2
        _OuterRadius ("Outer Radius", Float) = 2.5
    }
    SubShader
    {
        Tags { "Queue"="Overlay" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 _Color;
            float4 _PlayerWorldPosition;
            float _InnerRadius;
            float _OuterRadius;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float dist = distance(i.worldPos.xz, _PlayerWorldPosition.xz);
                float fog = saturate((dist - _InnerRadius) / (_OuterRadius - _InnerRadius));
                
                // 💡 외곽 알파 강조
                float enhancedFog = pow(fog, 1.0);

                return float4(_Color.rgb, _Color.a * enhancedFog);
            }
            ENDCG
        }
    }
}
