Shader "Custom/CRTTextOverlay"
{
    Properties
    {
        _FaceTex ("Font Atlas", 2D) = "white" {}
        _FaceColor ("Text Color", Color) = (1,1,1,1)
        
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
        Tags 
        { 
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent"
            "RenderPipeline"="UniversalPipeline"
        }
        
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            Name "CRTTextEffect"
            Tags { "LightMode"="UniversalForward" }
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            TEXTURE2D(_FaceTex);
            SAMPLER(sampler_FaceTex);
            
            CBUFFER_START(UnityPerMaterial)
                float4 _FaceTex_ST;
                float4 _FaceColor;
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
                OUT.uv = TRANSFORM_TEX(IN.uv, _FaceTex);
                OUT.color = IN.color;
                return OUT;
            }

            float random(float2 st)
            {
                return frac(sin(dot(st.xy, float2(12.9898, 78.233))) * 43758.5453123);
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;
                
                // Pixelation Effect (subtle on text)
                if (_PixelSize > 1)
                {
                    float2 pixelatedUV = floor(uv * _PixelSize * 100) / (_PixelSize * 100);
                    uv = lerp(uv, pixelatedUV, 0.3);
                }
                
                // Distortion Waves
                float distortionWave = sin(uv.y * 10 + _TimeSync * _DistortionSpeed) * _DistortionAmount;
                uv.x += distortionWave * 0.01;
                
                // Glitch Effect - Random horizontal displacement
                float glitchLine = floor(IN.uv.y * 20);
                float glitchNoise = random(float2(glitchLine, floor(_TimeSync * _GlitchSpeed)));
                if (glitchNoise > 0.95)
                {
                    uv.x += (random(float2(glitchLine, _TimeSync)) - 0.5) * _GlitchIntensity * 0.1;
                }
                
                // Sample the font atlas
                half4 col = SAMPLE_TEXTURE2D(_FaceTex, sampler_FaceTex, uv);
                col *= _FaceColor * IN.color;
                
                // Scanlines (applied to the text brightness)
                float scanline = sin(IN.uv.y * _ScanlineCount * 3.14159);
                scanline = scanline * 0.5 + 0.5;
                col.rgb *= 1 - (_ScanlineIntensity * (1 - scanline));
                
                // Add emission to make text glow
                half3 emission = _EmissionColor.rgb * _EmissionIntensity * col.a;
                col.rgb += emission;
                
                return col;
            }
            ENDHLSL
        }
    }
    
    Fallback "TextMeshPro/Mobile/Distance Field"
}
