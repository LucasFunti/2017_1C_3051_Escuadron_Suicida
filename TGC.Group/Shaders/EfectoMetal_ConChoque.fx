// ---------------------------------------------------------
// Enviroment Map
// ---------------------------------------------------------

/**************************************************************************************/
/* Variables comunes */
/**************************************************************************************/

//Matrices de transformacion
float4x4 matWorld; //Matriz de transformacion World
float4x4 matWorldView; //Matriz World * View
float4x4 matWorldViewProj; //Matriz World * View * Projection
float4x4 matInverseTransposeWorld; //Matriz Transpose(Invert(World))

//Textura para DiffuseMap
texture texDiffuseMap;
sampler2D diffuseMap = sampler_state
{
	Texture = (texDiffuseMap);
	ADDRESSU = WRAP;
	ADDRESSV = WRAP;
	MINFILTER = LINEAR;
	MAGFILTER = LINEAR;
	MIPFILTER = LINEAR;
};

float3 fvLightPosition = float3(-100.00, 100.00, -100.00);
float3 fvEyePosition = float3(0.00, 0.00, -100.00);
float ChoqueAtras;
float ChoqueAdelante;

float k_la = 0.6;							// luz ambiente global
float k_ld = 0.3;							// luz difusa
float k_ls = 0.8;							// luz specular
float fSpecularPower = 16;//16.84;

/**************************************************************************************/
/* RenderScene */
/**************************************************************************************/

//Output del Vertex Shader
struct VS_OUTPUT
{
	float4 Position :        POSITION0;
	float2 Texcoord :        TEXCOORD0;
	float3 Norm :			TEXCOORD1;		// Normales
	float3 Pos :   			TEXCOORD2;		// Posicion real 3d
};

//Vertex Shader
VS_OUTPUT vs_main(float4 Pos:POSITION, float3 Normal : NORMAL, float2 Texcoord : TEXCOORD0)
{
	VS_OUTPUT Output;
	
	// Animar posicion
	float Y = Pos.y;
	float Z = Pos.z;
    float v = 0;
	float tiempo = 0;

	if (v < 1) v = 1;
	if (v > 1.2) v = 1.25;
	Pos.y = Y/v;// *cos(v) - Z * sin(v);


	if (ChoqueAdelante >0)
	{
		if (Pos.z < 0.22)
		{
			Pos.z = Z - 10 * sin(Y * 200);

			Pos.x = Pos.x - 10 * sin(Y * 200);
		}
	}
	else {
		if (ChoqueAtras<0)
		{
			if (Pos.z > 0.22)
			{
				Pos.z = Z - 5 * sin(Y * 200);

				Pos.x = Pos.x - 5 * sin(Y * 200);
			}
		}
	}
	
	//Proyectar posicion
	Output.Position = mul(Pos, matWorldViewProj);
	//Propago  las coord. de textura
	Output.Texcoord = Texcoord;
	// Calculo la posicion real
	Output.Pos = mul(Pos, matWorld).xyz;
	// Transformo la normal y la normalizo
	Output.Norm = normalize(mul(Normal, matInverseTransposeWorld));
	//Output.Norm = normalize(mul(Normal,matWorld));
	return(Output);
}

//Pixel Shader
float4 ps_main(float2 Texcoord: TEXCOORD0, float3 N : TEXCOORD1,
	float3 Pos : TEXCOORD2) : COLOR0
{
	float ld = 0;		// luz difusa
	float le = 0;		// luz specular

	// corrijo la normal
	float aux = N.y;
	N.y = N.z;
	N.z = aux;
	N = normalize(N);

	// for(int =0;i<cant_ligths;++i)
	// 1- calculo la luz diffusa
	float3 LD = normalize(fvLightPosition - float3(Pos.x,Pos.y,Pos.z));
	ld += saturate(dot(N, LD))*k_ld;

	// 2- calcula la reflexion specular
	float3 D = normalize(float3(Pos.x,Pos.y,Pos.z) - fvEyePosition);
	float ks = saturate(dot(reflect(LD,N), D));
	ks = pow(ks,fSpecularPower);
	le += ks*k_ls;

	//Obtener el texel de textura
	float4 fvBaseColor = tex2D(diffuseMap, Texcoord);

	// suma luz diffusa, ambiente y especular
	float4 RGBColor = 0;
	RGBColor.rgb = saturate(fvBaseColor*(saturate(k_la + ld)) + le);
	return RGBColor;
}

technique RenderScene
{
	pass Pass_0
	{
		VertexShader = compile vs_3_0 vs_main();
		PixelShader = compile ps_3_0 ps_main();
	}
}
