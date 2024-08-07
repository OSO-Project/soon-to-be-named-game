Shader "Custom/DecalFadeShader"
{
    Properties
    {
        _BaseMap("Base Map", 2D) = "white" {}
        _FadeDistance("Fade Distance", Float) = 10.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _BaseMap;
            float _FadeDistance;
            float4 _BaseMap_ST;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float dist : TEXCOORD1;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _BaseMap);
                o.dist = length(_WorldSpaceCameraPos - v.vertex.xyz);
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                half4 col = tex2D(_BaseMap, i.uv);
                col.a *= saturate(1.0 - i.dist / _FadeDistance);
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}