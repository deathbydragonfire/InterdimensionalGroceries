#ifndef BEAUTIFY_AA_FX
#define BEAUTIFY_AA_FX

float4 _AntialiasData;
#define ANTIALIAS_STRENGTH _AntialiasData.x
#define ANTIALIAS_THRESHOLD _AntialiasData.y
#define ANTIALIAS_DEPTH_ATTEN _AntialiasData.z
#define ANTIALIAS_CLAMP _AntialiasData.w

float3 ApplyEdgeAA(float2 uv, float3 rgbM, float dDepth, float depthCenter, float lumaN, float lumaS, float lumaW, float lumaE, float minLuma, float maxLuma, float2 texelSize) {
        float2 gradient = float2(lumaN - lumaS, lumaW - lumaE);
        float2 absGradient = abs(gradient);
        float gradientAmp = max(absGradient.x, absGradient.y) + 1e-5;
        float2 dir = gradient / gradientAmp;
        float sampleRadius = min(gradientAmp * ANTIALIAS_STRENGTH, ANTIALIAS_CLAMP);
        float2 n = dir * sampleRadius;
        float antialiasDepthAtten = 1.0 - saturate(depthCenter * ANTIALIAS_DEPTH_ATTEN);
        n *= texelSize * antialiasDepthAtten;

        float3 rgbA = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv - n * 0.166667).rgb + SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv + n * 0.166667).rgb;
        float3 rgbB = 0.25 * (rgbA + SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv - n * 0.5).rgb + SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv + n * 0.5).rgb);
        float lumaB = getLuma(rgbB);
        if (lumaB < minLuma || lumaB > maxLuma) rgbB = rgbA * 0.5;

        return rgbB;
}

#endif

