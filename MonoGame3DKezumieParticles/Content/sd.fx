//float4x4 World;
float4x4 View;
float4x4 Projection;
texture Texture;


sampler2D textureSampler = sampler_state {
	Texture = (Texture);
	MagFilter = Linear;
	MinFilter = Linear;
	MipFilter = Point;
	AddressU = Clamp;
	AddressV = Clamp;
};

struct VertexShaderInput
{
	float4 Position :  SV_Position0;
	float2 TextureCoordinate : TEXCOORD0; 
};

struct VertexShaderOutput
{
	float4 Position : SV_Position0;
	float2 TextureCoordinate : TEXCOORD1; 
};


// Vertex shader helper for computing the position of a particle.
float4 ComputeParticlePosition(float3 position)
{
	// Apply the camera view and projection transforms.	
	                                                                // матрица камеры
	float4x4 matRevTrans = View;                                          // здесь будет нужная матрица
	matRevTrans._41 = matRevTrans._42 = matRevTrans._43 = 0;    // смещение не трогаем
	matRevTrans =transpose(matRevTrans);
	return mul(mul(float4(position, 1), View), Projection); //
}

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;

	/*float3 pos = input.Position;
	float3 pos_offset = float3(input.TextureCoordinate, 0);
	float3 view_pos = mul(float4(pos, 1.0f), View).xyz + pos_offset;
	float3 view_norm = normalize(pos_offset) + float3(0.f, 0.f, -0.3f);
	float4 proj_pos = mul(float4(view_pos, 1.0f), Projection);
	output.Position = proj_pos;*/
	/*float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);*/	
	float3 position = input.Position;
	output.Position = mul(mul(float4(position, 1), View), Projection);
	output.TextureCoordinate = input.TextureCoordinate;
	return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : SV_Target0
{
	float4 textureColor = tex2D(textureSampler, input.TextureCoordinate);
	return textureColor;
}

technique sd
{
	pass Pass1
	{
		VertexShader = compile vs_4_0 VertexShaderFunction();
		PixelShader = compile  ps_4_0 PixelShaderFunction();
	}
}