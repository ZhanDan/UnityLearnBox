Shader "Graph/PointSurface"
{
    Properties
    {
        _Smoothness ("Smoothness", range(0,1)) = 0.5
        //_Color ("Color", Color) = (1,1,1,1)
        //_MainTex ("Albedo (RGB)", 2D) = "white" {}
        //_Glossiness ("Smoothness", Range(0,1)) = 0.5
        //_Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        CGPROGRAM
        #pragma surface ConfigureSurface Standard fullforwardshadows
        #pragma target 3.0
        
        struct Input{
            float3 worldPos;
        };

        float _Smoothness;
        void ConfigureSurface (Input input, inout SurfaceOutputStandard surface){
            surface.Smoothness = _Smoothness;   
            surface.Albedo = saturate(input.worldPos * 0.5 + 0.5);
        }

        ENDCG
    }
    FallBack "Diffuse"
}
