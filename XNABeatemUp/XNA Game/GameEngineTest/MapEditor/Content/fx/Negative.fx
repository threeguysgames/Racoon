
// TODO: add effect parameters here.

sampler sampleState;

struct PS_INPUT
{
	float2 TexCoord : TEXCOORD0;
}

float4 Neg(PS_INPUT input) : COLOR0
{
    float4 col = tex2D(samplerState, input.TexCoord.xy);
    col.rgb = 1 - col.rgb;

    return col;
}

technique Negative
{
    pass P0
    {
        PixelShader = compile ps_2_0 Neg();
    }
}
