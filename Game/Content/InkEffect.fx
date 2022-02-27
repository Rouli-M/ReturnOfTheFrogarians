#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;
sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};
float spritesheet_offset_x;
float spritesheet_offset_y;
float spritesheet_width;
float spritesheet_height;
float frame_number_x;
float frame_number_y;
float scroll;
float4 inkable_color;

Texture2D jam_texture;
sampler2D jam_sampler = sampler_state
{
	Texture = <jam_texture>;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR
{
	float4 pixel_color = tex2D(SpriteTextureSampler, input.TextureCoordinates);
	//if (pixel_color.r * 255 != float(255))
	if (pixel_color.g < 0.1f)
		//return float4(10,10,10, 255);
		return pixel_color;
	else
	{
		float2 textureposition = input.TextureCoordinates;
		textureposition.x *= frame_number_x;
		textureposition.y *= frame_number_y;
		textureposition.x -= (spritesheet_offset_x / spritesheet_width) * frame_number_x;
		textureposition.y -= (spritesheet_offset_y / spritesheet_height) * frame_number_y;
		textureposition.y -= scroll;
		float4 jam_color = tex2D(jam_sampler, textureposition);
		if (jam_color.a == 1)
			return jam_color;
		else return pixel_color;
	}
}


technique SpriteDrawing
{
	pass
	{
		PixelShader = compile PS_SHADERMODEL PixelShaderFunction();
	}
};