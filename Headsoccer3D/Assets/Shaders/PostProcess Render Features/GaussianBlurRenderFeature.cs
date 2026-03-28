using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

public class CapturedOpaquesPassData : ContextItem
{
    public TextureHandle _capturedOpaques = TextureHandle.nullHandle;
    //public TextureHandle _capturedDepth = TextureHandle.nullHandle;

    // Reset function required by ContextItem. It should reset all variables not carried
    // over to next frame.
    public override void Reset()
    {
        //only reset the parts we want
        _capturedOpaques = TextureHandle.nullHandle;
        //_capturedDepth = TextureHandle.nullHandle;
    }
}

public class CaptureOpaquesPass : ScriptableRenderPass
{
    private Material m_blitMaterial;

    ProfilingSampler m_ProfilingSampler;
    class PassData
    {
        public Material materialToUse { get; set; }
        public TextureHandle currentSourceForBlit;
    }

    public CaptureOpaquesPass(Material _blitMaterial, RenderPassEvent _renderPassEvent, string _profileName)
    {
        m_blitMaterial = _blitMaterial;
        renderPassEvent = _renderPassEvent;
        m_ProfilingSampler = new ProfilingSampler(_profileName);
    }

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
        var capturedData = frameData.GetOrCreate<CapturedOpaquesPassData>();

        var source = resourceData.activeColorTexture;
        var destDesc = renderGraph.GetTextureDesc(source);
        destDesc.name = "_CapturedOpaquesColor";
        destDesc.clearBuffer = false;
        TextureHandle capturedColor = renderGraph.CreateTexture(destDesc);
        RenderGraphUtils.BlitMaterialParameters para1 = new(source, capturedColor, m_blitMaterial, 0);
        renderGraph.AddBlitPass(para1, "Capture Opaques Color");
        capturedData._capturedOpaques = capturedColor;


        //source = resourceData.activeDepthTexture;
        //var destDescdepth = renderGraph.GetTextureDesc(source);
        //destDescdepth.name = "_CapturedOpaquesDepth";
        //destDescdepth.clearBuffer = false;
        //TextureHandle capturedDepth = renderGraph.CreateTexture(destDescdepth);
        //RenderGraphUtils.BlitMaterialParameters para2 = new(source, capturedDepth, m_blitMaterial, 0);
        //renderGraph.AddBlitPass(para2, "Capture Opaques Depth");
        //capturedData._capturedDepth = capturedDepth;
    }
}

public class GaussianBlurPostFXPass : ScriptableRenderPass
{
    private Material m_compositeMaterial;

    ProfilingSampler m_ProfilingSampler;

    class PassData
    {
        public Material materialToUse { get; set; }
        public int materialPassToUse;

        //public TextureHandle _ColorTexture;
        //public TextureHandle _DepthTexture;
        public TextureHandle source;
    }

    public GaussianBlurPostFXPass(Material _compositeMat, RenderPassEvent _renderPassEvent, string _profileName)
    {
        m_compositeMaterial = _compositeMat;
        renderPassEvent = _renderPassEvent;
        m_ProfilingSampler = new ProfilingSampler(_profileName);
    }

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
        UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
        //CapturedOpaquesPassData capturedData = frameData.Get<CapturedOpaquesPassData>();
   
        //camera to camera
        var source = resourceData.activeColorTexture;
        //first dest texture for horizontal
        var destHorizontalDesc = renderGraph.GetTextureDesc(source);
        destHorizontalDesc.name = "_GaussianBlurredHorizontal";
        destHorizontalDesc.clearBuffer = false;
        TextureHandle destination_horizontalBlur = renderGraph.CreateTexture(destHorizontalDesc);

        using (var builder = renderGraph.AddRasterRenderPass<PassData>("Horizontal Gaussian Blur", out var passData))
        {
            passData.materialToUse = m_compositeMaterial;
            passData.source = source;
            passData.materialPassToUse = 0;

            builder.UseTexture(passData.source);

            // Output
            builder.SetRenderAttachment(destination_horizontalBlur, 0);
            builder.SetRenderFunc((PassData data, RasterGraphContext rgContext) => ExecutePass(data, rgContext));
        }

        //final dest texture for full with vertical
        var destDesc = renderGraph.GetTextureDesc(source);
        destDesc.name = "_GaussianBlurredHorizontal";
        destDesc.clearBuffer = false;
        TextureHandle destination = renderGraph.CreateTexture(destDesc);
        using (var builder = renderGraph.AddRasterRenderPass<PassData>("Vertical Gaussian Blur", out var passData))
        {
            passData.materialToUse = m_compositeMaterial;
            passData.source = destination_horizontalBlur;
            passData.materialPassToUse = 1;

            builder.UseTexture(passData.source);

            // Output
            builder.SetRenderAttachment(destination, 0);
            builder.SetRenderFunc((PassData data, RasterGraphContext rgContext) => ExecutePass(data, rgContext));
        }
        resourceData.cameraColor = destination;
    }
    static void ExecutePass(PassData data, RasterGraphContext rgContext)
    {
        Blitter.BlitTexture(rgContext.cmd, data.source, new Vector4(1, 1, 0, 0), data.materialToUse, data.materialPassToUse);
    }
}

public class GaussianBlurRenderFeature : ScriptableRendererFeature
{
    [SerializeField]
    private Material compositeMaterial;
    [SerializeField]
    private Shader blitShader;

    //private RenderPassEvent sampleRenderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
    private RenderPassEvent targeteRenderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
    public string profilerName = "Gaussian Blur";

    private Material blitMaterial;

    //CaptureOpaquesPass m_FirstPass;
    GaussianBlurPostFXPass m_SecondPass;

    // Here you can create passes and do the initialization of them. This is called everytime serialization happens.
    public override void Create()
    {
        blitMaterial = CoreUtils.CreateEngineMaterial(blitShader);
        //m_FirstPass = new CaptureOpaquesPass(blitMaterial, sampleRenderPassEvent, profilerName);
        m_SecondPass = new GaussianBlurPostFXPass(compositeMaterial, targeteRenderPassEvent, profilerName);

        // Configures where the render pass should be injected.
        //m_FirstPass.renderPassEvent = sampleRenderPassEvent;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        //renderer.EnqueuePass(m_FirstPass);
        renderer.EnqueuePass(m_SecondPass);
    }
    protected override void Dispose(bool disposing)
    {
        CoreUtils.Destroy(blitMaterial);
    }
}

