uniform	sampler2D backBuffer;
uniform sampler2D frontBuffer;
uniform sampler3D volume;

varying vec2 TexCoord0;
varying vec2 TexCoord1;

//vec3 start = texture2D(frontBuffer,TexCoord1).xyz;
start = gl_FragColor.xyz;

vec3 dir   = ( texture2D( backBuffer,TexCoord0) - texture2D( frontBuffer,TexCoord1)).xyz;
vec3  norm_dir = normalize(dir);

vec3 vec = start;

float len = length(dir.xyz);
float delta = 0.001; // 0.005;
vec3  delta_dir = norm_dir * delta;
float delta_dir_len = length(delta_dir);

vec4 color_sample = vec4(0,0,0,0);
vec3 col_acc = vec3(0,0,0);

float length_acc = 0.0;
float alpha_sample = 0.0;
float alpha_acc = 0;

vec3 maximum = vec3(0,0,0);

float alpha_old = 0, alpha_new = 0;

float threshold = 0.10;
float alpha = 0.0;

float depthT = -1.0;

void main()
{

	//if ( texture2D(frontBuffer,TexCoord0) == texture2D(backBuffer,TextCoord0))
	//discard;

	/*
	if ( ( texture2D(frontBuffer,TexCoord0).r == 1.00) &&
		 ( texture2D(frontBuffer,TexCoord0).g == 1.00) &&
		 ( texture2D(frontBuffer,TexCoord0).b == 1.00)
		)
	discard;*/
	
		
	for (int i = 0; i < 1/delta + 1; i++)
	{

		color_sample = texture3D(volume,vec);
						
		// postclassification
		/*
		if ((color_sample.r >= 33.37/255.0) && (color_sample.r <= 43/255.0))
			color_sample = vec4(0.0,0.25,0.0,0.1);
		else if ((color_sample.r >= 92.37/255.0) && (color_sample.r <= 120/255.0))
			{
			color_sample = vec4(0.0,0.0,0.8,0.2);
			}
		else if ((color_sample.r >= 196.0/255.0) && (color_sample.r <= 243/255.0))
			color_sample = vec4(0.5,0.0,0.0,0.5);
		else color_sample = vec4(0,0,0,0);
		*/
		
		/*if ((color_sample.r >= 0.3) && (color_sample.r <= 0.34))
			color_sample = vec4(0.0,0.8,0.0,0.4);
		else color_sample = vec4(0);
		*/
		/*else if ((color_sample.r >= 0.35) && (color_sample.r <= 0.4))
			color_sample = vec4(0.0,0.5,0.2,0.2);
		else if ((color_sample.r >= 0.4) && (color_sample.r <= 0.5))
			color_sample = vec4(0.6,0.5,0.2,0.3);
		*/
		/*
				// postclassification
		if ((color_sample.r >= 0.0) && (color_sample.r <= 0.005))
			color_sample = vec4(0.0,0.25,0.8,0.00);
		else if ((color_sample.r >= 0.005) && (color_sample.r <= 0.1))
			color_sample = vec4(0.8,0.8,0.1,0.00);
		else if ((color_sample.r >= 0.1) && (color_sample.r <= 0.15))
			color_sample = vec4(0.8,0.0,0.0,0.01);
		else if ((color_sample.r >= 0.15) && (color_sample.r <= 0.2))
			color_sample = vec4(0.0,0.8,0.8,0.03);
		else if ((color_sample.r >= 0.2) && (color_sample.r <= 0.3))
			color_sample = vec4(0.0,0.9,0.8,0.02);
		else if ((color_sample.r >= 0.3) && (color_sample.r <= 0.32))
			color_sample = vec4(0.0,0.99,0.0,0.01);
		else if ((color_sample.r >= 0.32) && (color_sample.r <= 0.36))
			color_sample = vec4(0.7,0.0,0.5,0.00);
		//else if ((color_sample.r >= 0.36) && (color_sample.r <= 0.46))
		//	color_sample = vec4(0.75,0.82,0.91,0.74);
		*/
		
	/*	if ((color_sample.r >= 0.0) && (color_sample.r <= 0.1))
			color_sample = vec4(0.0,0.25,0.0,0.01);
		else if ((color_sample.r >= 0.1) && (color_sample.r <= 0.32))
			color_sample = vec4(0.0,0.25,0.8,0.05);
		else if ((color_sample.r >= 0.4) && (color_sample.r <= 0.95))
			color_sample = vec4(0.8,0.0,0.8,0.4);
*/


		// skull
		float bulhar = 255.0;
		float prah = 40.0 /bulhar;
		float add =  60.0 / bulhar;

		if ((color_sample.r >= prah) && (color_sample.r <= (prah+0.3)))
			{
			float ttt =  2.0*(0.0134*color_sample.r*250-0.3414);
			color_sample = vec4(0.9,0.9,0.8,ttt);
			}
		else if ((color_sample.r >= 80.0/bulhar) && (color_sample.r <= (100.0/bulhar)))
			color_sample = vec4(0.94234,0.98,0.890,0.9735);
		else 
			color_sample = vec4(0);
		
		
		//else
		//color_sample = vec4(1.0,1.0,1.0,0.8);
			
		
	 
			
		alpha_sample = 1.0*color_sample.a * delta;
				
		//alpha_acc += 1*alpha_sample;
		alpha_acc += ( 1.0 - alpha_acc)*alpha_sample;
		alpha_acc *=   1.0;
		
		// save first hit ray parameter for depth value calculation
			if (depthT < 0.0 && alpha_acc > 0.0)
				depthT = length_acc;
			 
				
		vec += delta_dir;
		 
				
		
		
		length_acc += delta_dir_len;

		col_acc   += (1.0 - alpha_acc) * color_sample.rgb * alpha_sample  ; // 3 * 1/2*exp(vec);
		//col_acc += color_sample.rgb*alpha_sample.rgb;
		
		alpha_old = alpha_new;
		alpha_new = alpha_acc;
		
		//if (abs(alpha_new-alpha_old) > 0.1)
		//alpha_acc *= 1.3;
		
		// doplnit ukoncovanie na zaklade pomerov v rgb.
		
		// if ((alpha_acc >= 0.1) && (length(vec) < 0.05))
		// {
		//	alpha_acc = 0;
		//	col_acc = vec3(0.9,0,0);
		//  }
		
		// toto treba
		//gl_FragDepth = 1.0 - 1.5 * depthT / len;
		
		//gl_FragDepth = 1.0 - 0.5*depthT / len;
		
		if ((length_acc >= len)  || (alpha_acc >= 1.0))
			{
			alpha_acc = 1.0;
			//gl_FragDepth = 0.0;
			break;
			}
			
		
	}
	/*
	if (length(col_acc) < 0.005)
	col_acc = vec3(1.0,0,0);
	else
	*/
	
	//if ((alpha_acc) < 0.005	)
	//	gl_FragColor = vec4(1,1,1,1);
	//else 
	
	if (alpha_acc < 0.00001);
	{
	//col_acc = vec3(0,0,0.5);
	//alpha_acc = 0.5;
	}
	
	col_acc = 50 * (col_acc);
 
	//gl_FragDepth = 1.0 - 1.0 / length_acc;
	//vec4(length_acc,length_acc,length_acc,0.8);
	
	// calculate depth value from ray parameter
	// gl_FragDepth = 1.0 - 1*depthT / len;
	
	/*if (depthT >= 0.0)
		gl_FragDepth = calculateDepthValue(depthT/tend, textureLookup2Dnormalized(entryPointsDepth_, entryParameters_, p).z,
														textureLookup2Dnormalized(exitPointsDepth_, exitParameters_, p).z);
	*/
														
	
	gl_FragColor = vec4(col_acc.r,col_acc.g,col_acc.b,alpha_acc);	 
	//gl_FragDepth *= 1.2;
	
	//gl_FragDepth = gl_FragCoord.z;
	if (gl_FragDepth > 0.0)
	{
		gl_FragColor = vec4(gl_FragDepth,0.0,0,gl_FragDepth);
		gl_FragColor = vec4(col_acc.r,col_acc.g,col_acc.b,alpha_acc);	 
	}
	else
		{
		//gl_FragColor = vec4(0.8,1,0.8,0.89);
		//gl_FragDepth = 0;
		}
		//gl_FragColor = vec4(col_acc.r,col_acc.g,col_acc.b,alpha_acc);


	//gl_FragColor = vec4(0.3,0,0.25,0.1);
	//gl_FragDepth = 0.2 - depthT/len;
	 
	
			
	//gl_FragColor = vec4(gl_FragDepth,0,0,1);
	
	//gl_DepthColor = 4;
	//gl_FragColor = vec4(col_acc.r,0,0,alpha_acc);

	gl_FragColor = vec4(0.5,0.2,0.4,0.8);
	// gl_FragColor = texture2D(backBuffer,TexCoord0);
}
	


/*uniform	sampler2D backBuffer;
uniform sampler2D frontBuffer;
uniform sampler3D volume;

varying vec2 TexCoord0;
varying vec2 TexCoord1;

vec3 start = texture2D(frontBuffer,TexCoord1).xyz;
vec3 dir   = ( texture2D( backBuffer,TexCoord0) - texture2D( frontBuffer,TexCoord1)).xyz;
vec3 norm_dir = normalize(dir);

vec3 vec = start+vec3(0.1);
vec4 color_sample = vec4(0,0,0,0);

vec4 a = vec4(0.1,0.1,0.1,0.5);
				
void main()
{
	  color_sample = texture3D(volume,vec);
	
	  gl_FragColor = 1*vec4(dir,0.5) +  0.2*color_sample;
	 //gl_FragColor = vec4(frontBuffer,1);
}
*/