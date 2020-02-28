sampler2D input : register(s0);
float factor : register(c0);

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float4 clr = tex2D(input, uv.xy);
    float opacityLevel = 0.5;

    clr.r = ((clr.r * opacityLevel) * factor) + (clr.r * (1.0 - factor));
    clr.g = ((clr.g * opacityLevel) * factor) + (clr.g * (1.0 - factor));
    clr.b = ((clr.b * opacityLevel) * factor) + (clr.b * (1.0 - factor));

    return clr;
}