sampler2D input : register(s0);
float factor : register(c0);

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float2 xOffset = float2(factor / 500.0, 0.0);
    float2 yOffset = float2(0.0, factor / 500.0);

    int sampleSteps = 6;
    float4 result = 0;
    
    for(int i = 0; i < sampleSteps; ++i)
    {
        result += tex2D(input, uv + xOffset * i);
        result += tex2D(input, uv - xOffset * i);
        result += tex2D(input, uv + yOffset * i);
        result += tex2D(input, uv - yOffset * i);
    }
    
    return result / (sampleSteps *4);
}