Shader "Custom/CRTOverlay"
{
    Properties
    {
        [Header(Scanline Effects)]
        _ScanlineDarkness ("Scanline Darkness", Range(0, 1)) = 0.3
        _ScanlineCount ("Scanline Count", Float) = 150
        _ScanlineSpeed ("Scanline Speed", Float) = 2
        
        [Header(Glitch Effect)]
        _GlitchIntensity ("Glitch Intensity", Range(0, 1)) = 0.1
        _GlitchSpeed ("Glitch Speed", Float) = 2
        
        [Header(Distortion)]
        _DistortionAmount ("Distortion Amount", Range(0, 1)) = 0.05
        _DistortionSpeed ("Distortion Speed", Float) = 1.5
        
        [Header(Screen Effects)]
        _VignetteIntensity ("Vignette Intensity", Range(0, 1)) = 0.2
        _FlickerIntensity ("Flicker Intensity", Range(0, 0.1)) = 0.02
        
        [Header(Synchronization)]
        _TimeSync ("Time Sync", Float) = 0
    }
    
    SubShader
    {
        Tags 
        { 
            "Queue"="Transparent+100" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent"
            "RenderPipeline"="UniversalPipeline"
        }
        
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        ZTest LEqual
        Cull Off

        Pass
        {
            Name "CRTOverlay"
            
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
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
            
            CBUFFER_START(UnityPerMaterial)
                float _ScanlineDarkness;
                float _ScanlineCount;
                float _ScanlineSpeed;
                float _GlitchIntensity;
                float _GlitchSpeed;
                float _DistortionAmount;
                float _DistortionSpeed;
                float _VignetteIntensity;
                float _FlickerIntensity;
                float _TimeSync;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            float random(float2 st)
            {
                return frac(sin(dot(st.xy, float2(12.9898, 78.233))) * 43758.5453123);
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;
                
                // Animated Scanlines - scroll downward
                float scanlinePos = uv.y * _ScanlineCount + _TimeSync * _ScanlineSpeed;
                float scanline = sin(scanlinePos * 3.14159);
                scanline = scanline * 0.5 + 0.5; // Remap to 0-1
                
                // Scanline darkness - 0 = invisible, 1 = black
                float scanlineAlpha = _ScanlineDarkness * (1.0 - scanline);
                
                // Add scanline flicker
                float flicker = sin(_TimeSync * 10) * _FlickerIntensity;
                scanlineAlpha += flicker;
                
                // Glitch lines - random horizontal bars
                float glitchLine = floor(uv.y * 30);
                float glitchNoise = random(float2(glitchLine, floor(_TimeSync * _GlitchSpeed)));
                
                float glitchAlpha = 0;
                if (glitchNoise > 0.97)
                {
                    glitchAlpha = _GlitchIntensity * 0.5;
                }
                
                // Horizontal distortion visualization (subtle darkening where distorted)
                float distortionWave = sin(uv.y * 15 + _TimeSync * _DistortionSpeed * 2) * _DistortionAmount;
                float distortionAlpha = abs(distortionWave) * 0.1;
                
                // Vignette effect - darker at edges
                float2 vignetteUV = uv * 2.0 - 1.0;
                float vignette = length(vignetteUV);
                float vignetteAlpha = vignette * _VignetteIntensity * 0.5;
                
                // Phosphor decay - subtle overall brightness variation
                float phosphorDecay = sin(_TimeSync * 0.5) * _FlickerIntensity;
                
                // Combine all effects
                float totalAlpha = scanlineAlpha + glitchAlpha + distortionAlpha + vignetteAlpha + phosphorDecay;
                totalAlpha = saturate(totalAlpha);
                
                // Return black with varying alpha for overlay
                return half4(0, 0, 0, totalAlpha);
            }
            ENDHLSL
        }
    }
    
    Fallback Off
}
