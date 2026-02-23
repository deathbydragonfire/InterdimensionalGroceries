Shader "Custom/VHSEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        
        _WarpIntensity ("Warp Intensity", Range(0, 0.1)) = 0.02
        _WarpSpeed ("Warp Speed", Range(0, 5)) = 1.0
        
        _TrackingBarIntensity ("Tracking Bar Intensity", Range(0, 1)) = 0.3
        _TrackingBarSpeed ("Tracking Bar Speed", Range(0, 2)) = 0.5
        _TrackingBarThickness ("Tracking Bar Thickness", Range(0, 0.2)) = 0.05
        
        _ColorBleed ("Color Bleed", Range(0, 0.05)) = 0.015
        _NoiseIntensity ("Noise Intensity", Range(0, 1)) = 0.2
        _NoiseSpeed ("Noise Speed", Range(0, 5)) = 2.0
        
        _ScanlineIntensity ("Scanline Intensity", Range(0, 1)) = 0.3
        _ScanlineCount ("Scanline Count", Range(100, 1000)) = 500
        _ScanlineFlicker ("Scanline Flicker", Range(0, 0.5)) = 0.1
        
        _VignetteStrength ("Vignette Strength", Range(0, 1)) = 0.3
        _ColorShift ("Color Shift", Range(0, 1)) = 0.2
        _Desaturation ("Desaturation", Range(0, 1)) = 0.3
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        LOD 100
        
        Pass
        {
            Name "VHS Effect"
            
            ZTest Always
            ZWrite Off
            Cull Off
            
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            
            TEXTURE2D(_NoiseTex);
            SAMPLER(sampler_NoiseTex);
            
            CBUFFER_START(UnityPerMaterial)
                float4 _BlitTexture_ST;
                float4 _NoiseTex_ST;
                float _WarpIntensity;
                float _WarpSpeed;
                float _TrackingBarIntensity;
                float _TrackingBarSpeed;
                float _TrackingBarThickness;
                float _ColorBleed;
                float _NoiseIntensity;
                float _NoiseSpeed;
                float _ScanlineIntensity;
                float _ScanlineCount;
                float _ScanlineFlicker;
                float _VignetteStrength;
                float _ColorShift;
                float _Desaturation;
            CBUFFER_END
            
            float rand(float2 co)
            {
                return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);
            }
            
            half4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                
                float2 uv = input.texcoord;
                float time = _Time.y;
                
                float2 centerOffset = uv - 0.5;
                float distFromCenter = length(centerOffset);
                
                float warpOffset = sin(uv.y * 10.0 + time * _WarpSpeed + rand(floor(time * _WarpSpeed))) * _WarpIntensity;
                warpOffset += sin(uv.y * 20.0 + time * _WarpSpeed * 0.7) * _WarpIntensity * 0.3;
                
                float2 warpedUV = uv;
                warpedUV.x += warpOffset;
                
                half3 color = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, warpedUV).rgb;
                
                float trackingPos = frac(uv.y + time * _TrackingBarSpeed);
                float trackingBar = smoothstep(_TrackingBarThickness, 0.0, abs(trackingPos - 0.5) - 0.45);
                trackingBar *= rand(floor(time * _TrackingBarSpeed + uv.y * 5.0));
                color = lerp(color, color * 0.5, trackingBar * _TrackingBarIntensity);
                
                float2 noiseUV = uv * 2.0 + float2(time * _NoiseSpeed * 0.1, time * _NoiseSpeed * 0.05);
                float noise = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, noiseUV).r;
                noise = (noise - 0.5) * _NoiseIntensity;
                color += noise;
                
                float scanline = sin(uv.y * _ScanlineCount + time * 10.0 * _ScanlineFlicker) * 0.5 + 0.5;
                scanline = pow(scanline, 3.0);
                color -= scanline * _ScanlineIntensity * 0.1;
                
                float vignette = pow(1.0 - distFromCenter, 2.0);
                vignette = lerp(1.0, vignette, _VignetteStrength);
                color *= vignette;
                
                float luma = dot(color, half3(0.299, 0.587, 0.114));
                color = lerp(color, half3(luma, luma, luma), _Desaturation);
                
                color.r += _ColorShift * 0.05;
                color.g += _ColorShift * 0.02;
                color.b -= _ColorShift * 0.03;
                
                color = saturate(color);
                
                return half4(color, 1.0);
            }
            ENDHLSL
        }
    }
}
