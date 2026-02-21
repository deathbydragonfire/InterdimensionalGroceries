using UnityEngine;

namespace Beautify.Universal {

    public class BeautifyStripSettings : ScriptableObject {

        [Tooltip("Do not compile ACES tonemapping shader feature, reducing build time.")]
        public bool stripBeautifyTonemappingACES;

        [Tooltip("Do not compile ACES Fitted tonemapping shader feature, reducing build time.")]
        public bool stripBeautifyTonemappingACESFitted;

        [Tooltip("Do not compile AGX tonemapping shader feature, reducing build time.")]
        public bool stripBeautifyTonemappingAGX;

        [Tooltip("Do not compile sharpen shader feature, reducing build time.")]
        public bool stripBeautifySharpen;

        [Tooltip("Do not compile sharpen exclusion mask shader feature, reducing build time.")]
        public bool stripBeautifySharpenExclusionMask = true;

        [Tooltip("Do not compile dithering shader feature, reducing build time.")]
        public bool stripBeautifyDithering;

        [Tooltip("Do not compile edge antialiasing shader feature, reducing build time.")]
        public bool stripBeautifyEdgeAA;

        [Tooltip("Do not compile LUT shader feature, reducing build time.")]
        public bool stripBeautifyLUT;

        [Tooltip("Do not compile LUT 3D shader feature, reducing build time.")]
        public bool stripBeautifyLUT3D = true;

        [Tooltip("Do not compile daltonize, sepia or white balance shader feature, reducing build time.")]
        public bool stripBeautifyColorTweaks;

        [Tooltip("Do not compile Bloom, Anamorphic & Sun Flares shader features, reducing build time.")]
        public bool stripBeautifyBloom;

        [Tooltip("Do not compile Lens Dirt shader feature, reducing build time.")]
        public bool stripBeautifyLensDirt;

        [Tooltip("Do not compile Chromatic Aberration shader feature, reducing build time.")]
        public bool stripBeautifyChromaticAberration;

        [Tooltip("Do not compile Depth Of Field shader feature, reducing build time.")]
        public bool stripBeautifyDoF;

        [Tooltip("Do not compile Depth Of Field transparency support shader feature, reducing build time.")]
        public bool stripBeautifyDoFTransparentSupport = true;

        [Tooltip("Do not compile Purkinje Shift shader feature, reducing build time.")]
        public bool stripBeautifyEyeAdaptation = true;

        [Tooltip("Do not compile Purkinje Shift shader feature, reducing build time.")]
        public bool stripBeautifyPurkinje = true;

        [Tooltip("Do not compile Vignetting shader features, reducing build time.")]
        public bool stripBeautifyVignetting;

        [Tooltip("Do not compile Vignetting Mask shader feature, reducing build time.")]
        public bool stripBeautifyVignettingMask = true;

        [Tooltip("Do not compile Outline shader feature, reducing build time.")]
        public bool stripBeautifyOutline = true;

        [Tooltip("Do not compile Night Vision shader feature, reducing build time.")]
        public bool stripBeautifyNightVision = true;

        [Tooltip("Do not compile Thermal Vision shader feature, reducing build time.")]
        public bool stripBeautifyThermalVision = true;

        [Tooltip("Do not compile Frame shader features, reducing build time.")]
        public bool stripBeautifyFrame = true;

        [Tooltip("Do not compile Film Grain shader feature, reducing build time.")]
        public bool stripBeautifyFilmGrain = true;

        [Tooltip("Do not compile Unity Post Processing's Film Grain shader feature, reducing build time.")]
        public bool stripUnityFilmGrain;

        [Tooltip("Do not compile Unity Post Processing's Dithering shader feature, reducing build time.")]
        public bool stripUnityDithering;

        [Tooltip("Do not compile Unity Post Processing's Tonemapping shader feature, reducing build time.")]
        public bool stripUnityTonemapping;

        [Tooltip("Do not compile Unity Post Processing's Bloom shader feature, reducing build time.")]
        public bool stripUnityBloom;

        [Tooltip("Do not compile Unity Post Processing's Chromatic Aberration shader feature, reducing build time.")]
        public bool stripUnityChromaticAberration;

        [Tooltip("Do not compile Unity Post Processing's Screen Distortion features, reducing build time.")]
        public bool stripUnityDistortion;

        public bool stripUnityDebugVariants;
    }
}

