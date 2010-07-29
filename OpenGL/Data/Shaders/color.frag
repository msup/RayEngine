uniform	sampler2D backBuffer;
uniform sampler2D frontBuffer;
uniform sampler3D volume;

varying vec2 TexCoord0;
varying vec2 TexCoord1;

vec3 start = texture2D(frontBuffer,TexCoord1).xyz;
vec3 dir   = ( texture2D( backBuffer,TexCoord0) - texture2D( frontBuffer,TexCoord1)).xyz;
vec3  norm_dir = normalize(dir);

vec3 vec = start;

float len = length(dir.xyz);
float delta = 0.005;
vec3  delta_dir = norm_dir * delta;
float delta_dir_len = length(delta_dir);

vec4 color_sample = vec4(0,0,0,0);
vec3 col_acc = vec3(0,0,0);

float length_acc = 0.0;
float alpha_sample = 0.0;
float alpha_acc = 0;

vec3 maximum = vec3(0,0,0);

float alpha_old = 0, alpha_new = 0;

void main()
{
	/*
    if ( ( texture2D(frontBuffer,TexCoord0).r == 1.00) &&
	     ( texture2D(frontBuffer,TexCoord0).g == 1.00) &&
		 ( texture2D(frontBuffer,TexCoord0).b == 1.00)
		)
	discard;
	*/
		
	for (int i = 0; i < 1/delta + 1; i++)
	{

		color_sample = texture3D(volume,vec);
		vec += delta_dir;

		alpha_sample = 2.0*color_sample.a * delta;
		alpha_acc += alpha_sample;

		vec += delta_dir;
		length_acc += delta_dir_len;

		col_acc   += (1.0 - alpha_acc) * color_sample.rgb * alpha_sample * 3 * 1/2*exp(vec);
		//col_acc += color_sample*alpha_sample;
		
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
		
		if (length_acc >= len || alpha_acc >= 1.0)
		break;
		
	}
	
	col_acc = 50*(col_acc);
	gl_FragColor = vec4(col_acc.r,col_acc.g,col_acc.b,alpha_acc);

	// gl_FragColor = texture2D(backBuffer,TexCoord0);
}
	
