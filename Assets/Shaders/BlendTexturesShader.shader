Shader "Custom/SpottedShader"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _SpotTex ("Spot Texture", 2D) = "white" {}
        _SpotAmount ("Spot Amount", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        sampler2D _MainTex;
        sampler2D _SpotTex;
        float _SpotAmount;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_SpotTex;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            half4 baseColor = tex2D (_MainTex, IN.uv_MainTex);
            half4 spotColor = tex2D (_SpotTex, IN.uv_SpotTex);

            // ????????? ???????? ? ??????? ?? ???????? ????????
            o.Albedo = lerp(baseColor.rgb, spotColor.rgb, _SpotAmount);
            o.Alpha = baseColor.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
