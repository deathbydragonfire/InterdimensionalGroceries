Shader "Custom/ScreenGlitchEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GlitchIntensity ("Glitch Intensity", Range(0, 1)) = 0
        _ChromaticAberration ("Chromatic Aberration", Float) = 0.01
        _Displacement ("Displacement", Float) = 0.05
        _ScanLineJitter ("Scan Line Jitter", Float) = 0.1
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        
        Pass
        {
            Name "ScreenGlitch"
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
            
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            
            float _GlitchIntensity;
            float _ChromaticAberration;
            float _Displacement;
            float _ScanLineJitter;
            
            float random(float2 uv)
            {
                return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
            }
            
            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                return output;
            }
            
            half4 frag(Varyings input) : SV_Target
            {
                float2 uv = input.uv;
                float intensity = _GlitchIntensity;
                
                if (intensity <= 0.0)
                {
                    return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
                }
                
                float time = _Time.y;
                
                float blockNoise = random(floor(uv * float2(1, 30) + time));
                float displacement = (_Displacement * intensity) * (blockNoise * 2.0 - 1.0);
                
                float scanLineJitter = _ScanLineJitter * intensity * sin(uv.y * 100.0 + time * 10.0) * random(float2(time, uv.y));
                
                uv.x += displacement + scanLineJitter;
                
                float chromatic = _ChromaticAberration * intensity;
                
                half r = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(chromatic, 0)).r;
                half g = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv).g;
                half b = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv - float2(chromatic, 0)).b;
                
                half4 color = half4(r, g, b, 1.0);
                
                float noiseIntensity = random(uv + time) * 0.1 * intensity;
                color.rgb += noiseIntensity;
                
                return color;
            }
            ENDHLSL
        }
    }
}
