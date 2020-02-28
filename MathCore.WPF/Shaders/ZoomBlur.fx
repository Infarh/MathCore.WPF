/// <class>ZoomBlurEffect</class>
/// <description>An effect that applies a radial blur to the input.</description>

sampler2D  inputSource : register(S0);

/// <summary>The center of the blur.</summary>
float2 Center : register(C0);

/// <summary>The amount of blur.</summary>
float BlurAmount : register(C1);


float4 main(float2 uv : TEXCOORD) : COLOR
{
    float4 c = 0;
    uv -= Center;

    for (int i = 0; i < 15; i++)  {
        float scale = 1.0 + BlurAmount * (i / 14.0);
        c += tex2D(inputSource, uv * scale + Center);
    }
   
    c /= 15;
    return c;
}