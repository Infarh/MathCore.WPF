sampler2D input : register(s0);

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float4 color = tex2D(input, uv);
    float alpha = color.a;

    color = 1 - color;
    color.a = alpha;
    color.rgb *= alpha;

    return color;
}