sampler samplerState;

const float2 offsets[12] = 
{
	-0.32, -0.405,
	-0.84, -0.735,
	-0.69, -0.45,
	-0.2, 0.62,
	0.96, -0.19,
	0.47, -0.48,
	0.519, 0.767,
	0.185, -0.89,
	0.507, 0.06,
	0.896, 0.412,
	-0.32, -0.932,
	-0.79, -0.59,

};

struct PS_INPUT
{
	float2 TexCoord : TEXCOORD0;
};

float4 Blur(PS_INPUT input) : COLOR0
{
	float4 col = 0;
	for (int i = 0; i < 12; i++)
	{
		col += tex2D(samplerState, input.TexCoord + 0.005*offsets[i]);
	}
	float a = (col.r + col.g + col.b)/3.0f;
	
	a /= 12.0f;
	col.rgb = a;
	
	return col;
}

technique BlurTechnique
{
	pass P0
	{
		PixelShader = compile ps_2_0 Blur();
	}	
}