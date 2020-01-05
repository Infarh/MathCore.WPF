sampler2D input : register(s0);
float factor : register(c0);

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float4 clr = tex2D(input, uv.xy);
    float avg = (clr.r + clr.g + clr.b) / 3.0;

    clr.r = ((0.8 * avg * factor) + (clr.r * (1.0 - factor)));
    clr.g = ((0.4 * avg * factor) + (clr.g * (1.0 - factor)));
    clr.b = ((0.2 * avg * factor) + (clr.b * (1.0 - factor)));

    return clr;
}