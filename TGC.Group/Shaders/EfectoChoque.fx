
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

float ChoqueAtras;
float ChoqueAdelante;

/**************************************************************************************/
/* RenderScene */
/**************************************************************************************/

//Input del Vertex Shader
struct VS_INPUT
{
	float4 Position : POSITION0;
	float4 Color : COLOR0;
	float2 Texcoord : TEXCOORD0;
};

//Output del Vertex Shader
struct VS_OUTPUT
{
	float4 Position :        POSITION0;
	float2 Texcoord :        TEXCOORD0;
	float4 Color :			COLOR0;
};

//Vertex Shader
VS_OUTPUT vs_main(VS_INPUT Input)
{
	VS_OUTPUT Output;
	//Proyectar posicion
	Output.Position = mul(Input.Position, matWorldViewProj);

	//Propago las coordenadas de textura
	Output.Texcoord = Input.Texcoord;

	//Propago el color x vertice
	Output.Color = Input.Color;

	return(Output);
}

// Ejemplo de un vertex shader que anima la posicion de los vertices
// ------------------------------------------------------------------
VS_OUTPUT vs_main2(VS_INPUT Input)
{
	VS_OUTPUT Output;
	Output = Input;

	// Animar posicion
	float Y = Input.Position.y;
	float Z = Input.Position.z;
    float v = 0;
	float tiempo = 0;

	if (v < 1) v = 1;
	if (v > 1.2) v = 1.25;
	Input.Position.y = Y/v;// *cos(v) - Z * sin(v);


	if (ChoqueAdelante >0)
	{
		if (Input.Position.z < 0.22)
		{
			Input.Position.z = Z - 10 * sin(Y * 200);

			Input.Position.x = Input.Position.x - 10 * sin(Y * 200);
		}
	}
	else {
		if (ChoqueAtras<0)
		{
			if (Input.Position.z > 0.22)
			{
				Input.Position.z = Z - 5 * sin(Y * 200);

				Input.Position.x = Input.Position.x - 5 * sin(Y * 200);
			}
		}
	}
		//Proyectar posicion
	Output.Position = mul(Input.Position, matWorldViewProj);

	//Propago las coordenadas de textura
	Output.Texcoord = Input.Texcoord;

	// Animar color
	Input.Color.r = 255;// Input.Color.r*v + 222;
	
	//Propago el color x vertice
	Output.Color = Input.Color;

	return(Output);
}

//Pixel Shader
float4 ps_main(float2 Texcoord: TEXCOORD0, float4 Color : COLOR0) : COLOR0
{
	// Obtener el texel de textura
	// diffuseMap es el sampler, Texcoord son las coordenadas interpoladas
    float4 fvBaseColor = tex2D(diffuseMap, Texcoord);

    // cambia color 
	fvBaseColor.r = fvBaseColor.r + 0.005*(-1.2);
	fvBaseColor.g = fvBaseColor.g;
    return fvBaseColor;
}

// ------------------------------------------------------------------
technique RenderScene
{
	pass Pass_0
	{
		VertexShader = compile vs_2_0 vs_main2();
		PixelShader = compile ps_2_0 ps_main();
	}
}

