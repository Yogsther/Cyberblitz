
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/RenderPass/CustomPass/CustomPassCommon.hlsl"


void SampleCustomColor_float(float2 UV, out float4 Color)
{


    Color = SampleCustomColor(UV);
}

void SampleCustomColor_half(half2 UV, out half4 Color)
{

    Color = SampleCustomColor(UV);

}

void SampleCustomDepth_float(float2 UV, out float Depth)
{

    Depth = LinearEyeDepth(SampleCustomDepth(UV), _ZBufferParams);

}

void SampleCustomDepth_half(half2 UV, out half Depth)
{

    Depth = LinearEyeDepth(SampleCustomDepth(UV), _ZBufferParams);

}