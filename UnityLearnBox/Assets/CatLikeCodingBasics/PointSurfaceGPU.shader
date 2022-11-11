Shader "Graph/PointSurfaceGPU"
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
        #pragma surface ConfigureSurface Standard fullforwardshadows addshadow
        #pragma instancing_options assumeuniformscaling procedural:ConfigureProcedural
        #pragma editor_sync_compilation //this is to avoid Unity to use the dummy shader and to possibly crash when rendering lots of shaders
        #pragma target 4.5
        #include "PointGPU.hlsl"
        
        struct Input{
            float3 worldPos;
        };

        float _Smoothness;

        //#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
        //    StructuredBuffer<float3> _Positions;
        //#endif

        //float _Step;

        //void ConfigureProcedural(){
        //    #if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
        //        float3 position = _Positions[unity_InstanceID];
        //        unity_ObjectToWorld = 0.0;
        //        unity_ObjectToWorld._m03_m13_m23_m33 = float4(position, 1.0);
        //        unity_ObjectToWorld._m00_m11_m22 = _Step;
        //    #endif
        //}

        void ConfigureSurface (Input input, inout SurfaceOutputStandard surface){
            surface.Smoothness = _Smoothness;   
            surface.Albedo = saturate(input.worldPos * 0.5 + 0.5);
        }

        ENDCG
    }
    FallBack "Diffuse"
}
