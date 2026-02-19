Shader "Custom/CRTScreenEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        
        [Header(Emission)]
        [HDR] _EmissionColor ("Emission Color", Color) = (0,0,0,1)
        _EmissionIntensity ("Emission Intensity", Range(0, 10)) = 1
        
        [Header(Scanline Effects)]
        _ScanlineIntensity ("Scanline Intensity", Range(0, 1)) = 0.1
        _ScanlineCount ("Scanline Count", Float) = 200
        
        [Header(Pixelation)]
        _PixelSize ("Pixel Size", Float) = 1
        
        [Header(Glitch Effect)]
        _GlitchIntensity ("Glitch Intensity", Range(0, 1)) = 0.05
        _GlitchSpeed ("Glitch Speed", Float) = 1
        
        [Header(Distortion)]
        _DistortionAmount ("Distortion Amount", Range(0, 1)) = 0.02
        _DistortionSpeed ("Distortion Speed", Float) = 1
        
        [Header(Synchronization)]
        _TimeSync ("Time Sync", Float) = 0
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" "Queue"="Geometry" }
        LOD 100

        Pass
        {
            Name "CRTEffect"
            Tags { "LightMode"="UniversalForward" }
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Color;
                float4 _EmissionColor;
                float _EmissionIntensity;
                float _ScanlineIntensity;
                float _ScanlineCount;
                float _PixelSize;
                float _GlitchIntensity;
                float _GlitchSpeed;
                float _DistortionAmount;
                float _DistortionSpeed;
                float _TimeSync;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                return OUT;
            }

            float random(float2 st)
            {
                return frac(sin(dot(st.xy, float2(12.9898, 78.233))) * 43758.5453123);
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;
                
                // Pixelation Effect
                if (_PixelSize > 1)
                {
                    uv = floor(uv * _PixelSize) / _PixelSize;
                }
                
                // Distortion Waves - make them more visible
                float distortionWave = sin(uv.y * 15 + _TimeSync * _DistortionSpeed * 2) * _DistortionAmount;
                distortionWave += sin(uv.y * 8 - _TimeSync * _DistortionSpeed) * _DistortionAmount * 0.5;
                uv.x += distortionWave;
                
                // Glitch Effect - Random horizontal displacement
                float glitchLine = floor(uv.y * 20);
                float glitchNoise = random(float2(glitchLine, floor(_TimeSync * _GlitchSpeed)));
                if (glitchNoise > 0.95)
                {
                    uv.x += (random(float2(glitchLine, _TimeSync)) - 0.5) * _GlitchIntensity;
                }
                
                // Sample texture
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
                col *= _Color;
                
                // Animated Scanlines - scroll downward
                float scanlinePos = IN.uv.y * _ScanlineCount + _TimeSync * 2;
                float scanline = sin(scanlinePos * 3.14159);
                scanline = scanline * 0.5 + 0.5;
                float scanlineEffect = 1 - (_ScanlineIntensity * (1 - scanline));
                
                // Add scanline flicker
                float flicker = sin(_TimeSync * 10) * 0.02 + 1;
                scanlineEffect *= flicker;
                
                col.rgb *= scanlineEffect;
                
                // Add emission
                half3 emission = _EmissionColor.rgb * _EmissionIntensity;
                col.rgb += emission;
                
                // Add subtle brightness variation for CRT phosphor decay
                float phosphorDecay = sin(_TimeSync * 0.5) * 0.05 + 1;
                col.rgb *= phosphorDecay;
                
                return col;
            }
            ENDHLSL
        }
    }
    
    Fallback "Universal Render Pipeline/Lit"
}
