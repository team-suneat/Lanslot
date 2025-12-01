using UnityEngine;
#if UNITY_PIPELINE_URP
using UnityEngine.Rendering.Universal;

namespace CRTPostprocess
{
    public class NTSCFeature : ScriptableRendererFeature
    {
        public int bufferHeight = 480;
        public int outputHeight = 240;
        public NTSCPass.CrossTalkMode crossTalkMode = NTSCPass.CrossTalkMode.Vertical;
        public float crossTalkStrength = 2f;
        public float brightness = 0.95f;
        public float blackLevel = 1.0526f;
        public float artifactStrength = 1f;
        public float fringeStrength = 0.75f;
        public float chromaModFrequencyScale = 1f;
        public float chromaPhaseShiftScale = 1f;
        public NTSCPass.GaussianBlurWidth gaussianBlurWidth = NTSCPass.GaussianBlurWidth.TAP8;
        public bool curvature = true;
        public bool cornerMask = true;
        [Tooltip("Non curvature mode uses this parameter.")] public int cornerRadius = 16;
        public float scanlineStrength = 1f;
        public float beamSpread = 0.5f;
        public float beamStrength = 1f;
        public float overscanScale = 0.985f;
        public NTSCPass.DisplayOrientation displayOrientation = NTSCPass.DisplayOrientation.None;
        
        private Material _material;
        private NTSCPass _ntscPass;
        private NTSCPass.Parameter _param;
        
        public override void Create()
        {
            _ntscPass = new NTSCPass();
            _material = new Material(Shader.Find("Hidden/NTSCPass"));
            _material.hideFlags = HideFlags.HideAndDontSave;
            _ntscPass.InitKeywords(_material.shader);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            _param.bufferHeight = bufferHeight;
            _param.outputHeight = outputHeight;
            _param.gaussianBlurWidth = gaussianBlurWidth;
            _param.crossTalkMode = crossTalkMode;
            _param.crossTalkStrength = crossTalkStrength;
            _param.brightness = brightness;
            _param.blackLevel = blackLevel;
            _param.artifactStrength = artifactStrength;
            _param.fringeStrength = fringeStrength;
            _param.chromaModFrequencyScale = chromaModFrequencyScale;
            _param.chromaPhaseShiftScale = chromaPhaseShiftScale;
            _param.useCurvature = curvature;
            _param.useCornerMask = cornerMask;
            _param.cornerRadius = cornerRadius;
            _param.scanlineStrength = scanlineStrength;
            _param.beamStrength = beamStrength;
            _param.beamSpread = beamSpread;
            _param.overscanScale = overscanScale;
            _param.displayOrientation = displayOrientation;
            _ntscPass.SetupParameters(_material, _param);
            #if !UNITY_2022_1_OR_NEWER
                RenderTargetHandle targetHandle = new RenderTargetHandle();
                targetHandle.Init("_CameraColorAttachmentA");
                _ntscPass.SetRenderingData(targetHandle.Identifier());
            #endif
            renderer.EnqueuePass(_ntscPass);
        }
        
        #if !UNITY_6000_0_OR_NEWER && UNITY_2022_1_OR_NEWER
        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
        {
            _ntscPass.SetRenderingData(renderer.cameraColorTargetHandle);
        }
        #endif
    }
}

#endif