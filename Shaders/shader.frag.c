/*vec3 L = vec3(0,1,1);
float s = dot(color_sample.xyz, L );
color_sample.rgb = s * color_sample.rgb + 0.1*color_sample.rgb;
color_sample.rgb *= color_sample.a;
*/

uniform	sampler2D backBuffer;
uniform sampler2D frontBuffer;
uniform sampler1D classificationTexture;
uniform sampler3D volume;

vec3 vLightAmbient          = vec3(0.0,0.0,0.0);
vec3 vLightDiffuse          = vec3(0.87,0.0,0.87);
vec3 vLightSpecular         = vec3(0.0,0.87,0.87);
vec3 LightDir               = vec3(0.2,0.5,0.5);

// used to step over empty areas
float AlphaSkipStep = 1;

//float delta = 0.001 * 1; // 0.005;
uniform float delta; // 0.005;

varying vec2 TexCoord0;
varying vec2 TexCoord1;

vec3 start = texture2D( frontBuffer,TexCoord1).xyz;
vec3 stop  = texture2D( backBuffer ,TexCoord0).xyz;
//vec3 stop = gl_Color.xyz;

float asdf = 1;
float AlphaThreshold = 0.002;

//vec3 dir   = ( texture2D( backBuffer,TexCoord0) - texture2D( frontBuffer,TexCoord1)).xyz;
vec3 dir   =  stop - start;
vec3  norm_dir = normalize(dir);

vec3 vec = start;

float len = length(dir.xyz);

vec3  delta_dir = norm_dir * delta;

//float delta_dir_len = length(delta_dir);

vec4 color_sample = vec4(0,0,0,0);
vec3 col_acc = vec3(0,0,0);

float length_acc   = 0.0;
float alpha_sample = 0.0;
float alpha_acc    = 0;

vec3 maximum = vec3(0,0,0);

float alpha_old = 0, alpha_new = 0;

float threshold = 0.10;
float alpha = 0.0;

float depthT = -1.0;

float distanceUntilHit = 0;

float kx = 256.0 ;
float ky = 256.0 ;
float kz = 144.0 ;

vec3 vVoxelStepsize = vec3(1.0/kx,1.0/ky,1.0/kz);

vec3 Lighting (
	vec3 vPosition,
	vec3 vNormal,
	vec3 vLightAmbient,
	vec3 vLightDiffuse,
	vec3 vLightSpecular,
	vec3 voxelColor
	)
{
	vec3 vViewDir    = normalize(vec3(0.0,0.0,0.0)-vPosition);
	vec3 vReflection = normalize(reflect(vViewDir, vNormal));

	vec3 vLightDir = vec3(0.0,0.5,0.5);

	return
		1.0 * vLightAmbient +
		0.6 * vLightDiffuse  * max( dot( vNormal, -vLightDir),0.0) +
		0.8 * vLightSpecular * pow( max ( dot( vReflection, vLightDir),0.0),5.00);
}

vec3 ComputeNormal (
	vec3 vHitPosTex
	)
{
	// TODO: check the blue channel when volume data structure changed

	float fVolumValXp = texture3D(volume, vHitPosTex+vec3(+vVoxelStepsize.x,0,0)).b;
	float fVolumValXm = texture3D(volume, vHitPosTex+vec3(-vVoxelStepsize.x,0,0)).b;

	//if ((vHitPosTex.x - vVoxelStepsize.x ) < 0.0)
	//	fVolumValXm = fVolumValXp;
	//if ((vHitPosTex.x + vVoxelStepsize.x ) > 1.0)
	//	fVolumValXp = fVolumValXm;

	float fVolumValYp = texture3D(volume, vHitPosTex+vec3(0,-vVoxelStepsize.y,0)).b;
	float fVolumValYm = texture3D(volume, vHitPosTex+vec3(0,+vVoxelStepsize.y,0)).b;

	/*if ((vHitPosTex.y - vVoxelStepsize.y ) < 0.0)
		fVolumValYm = fVolumValYp;
	if ((vHitPosTex.y + vVoxelStepsize.y ) > 1.0)
		fVolumValYm = fVolumValYp;*/

	float fVolumValZp = texture3D(volume, vHitPosTex+vec3(0,0,+vVoxelStepsize.z)).b;
	float fVolumValZm = texture3D(volume, vHitPosTex+vec3(0,0,-vVoxelStepsize.z)).b;

	//if ((vHitPosTex.z - vVoxelStepsize.z ) < 0.0)
	//	fVolumValZm = fVolumValZp;
	//if ((vHitPosTex.z + vVoxelStepsize.z ) > 1.0)
	//	fVolumValZm = fVolumValZp;

	vec3  vGradient  = vec3((fVolumValXm-fVolumValXp), (fVolumValYp-fVolumValYm), (fVolumValZm-fVolumValZp));

	vec3 vNormal     = gl_NormalMatrix * vGradient;

	float l = length(vNormal);
		if (l>0.0)
			vNormal /= l; // secure normalization

	return vNormal;
}

vec4 Classify (
	vec4 color_sample
	)
{
	float sample = color_sample.b;

	float bulhar = 255.0;
	float prah   =  50 / bulhar;  //40
	float add    =  60.0 / bulhar;
	vec4 color   = vec4(0);

	if ((sample >= 0.20) && (sample <= 0.25))
	{
		float D = 0.20 * 255;
		float H = 0.25 * 255;
		float q = D / (D-H);
		float k = -q / D;

		float alpha =  k * color_sample.b*255 + q;
		color = vec4(0.99,0.99,0.99, 0.0  );
	}
	else if (sample < 0.2)
		color = vec4(0,0,0,0);

	else if ((sample >= 0.9) && (sample <= 1.0))
	{
		float D = 0.95*255;
		float H = 1.0*255;
		float q = D / (D-H);
		float k = -q / D;

		float alpha =  k * color_sample.b*255 + q;
		color = vec4(0.77,0.72,0.87, alpha );
	}
	else
		color = vec4(0.087,0.099,0.87,0.000);

	if (color.a < AlphaThreshold)
	{
		//delta_dir = norm_dir * AlphaSkipStep * delta;
		//asdf = 0;
	}
	else
	{
		//delta_dir = norm_dir * delta;
		//asdf = 0;
	}

	//delta_dir = norm_dir * delta;
	color = vec4(0);
	color = texture1D( classificationTexture, sample).rgba;
	//color = vec4(0);

	return color;
}

//vec4 color_sample_1 = vec4(0,0,0,0);
//vec4 color_sample_2 = vec4(0,0,0,0);
//vec4 color_sample_3 = vec4(0,0,0,0);
//vec4 color_sample_4 = vec4(0,0,0,0);

void main()
{
	// check if raycasting is necessary - if we are in box
	if (stop == start)
	{
		gl_FragColor = vec4(0.9,0.5,0.9,0.9);
		return;
	}

	vec4 dst = vec4(0.0, 0.0, 0.0, 0);

	/*vec4 dst_1 = vec4(0, 0, 0, 0);
	vec4 dst_2 = vec4(0, 0, 0, 0);
	vec4 dst_3 = vec4(0, 0, 0, 0);
	vec4 dst_4 = vec4(0, 0, 0, 0);*/

	for (int i = 0; i < 1/delta + 1; i++)
	{
		// ---------------- classification begin ------------------

		// get sample
		color_sample = texture3D(volume,vec);

		// post classify
		color_sample = Classify(color_sample);

		// transluency correction
		color_sample.a *= (1.0 - exp( -1 / length(vec)) );

		// advance in direction
		vec += delta_dir;

		// accumulation reached the level
		if (dst.a >= 0.999f)
			break;

		//  if transluency is lower than level for fast skipping, step back and use standard integration step
		/*if (color_sample.a > AlphaThreshold && asdf == 1)
		{
			vec	-=	norm_dir * AlphaSkipStep * delta;
			vec +=  norm_dir * delta;
		}
		*/
		// ---------------- classification end ------------------

		// ---------------- color optical model -----------------
		//vec3 L = vec3(0.25,0.35,0.25);
		//float s = dot ( color_sample.rgb, L);

		//color_sample.rgb = 0.5* (s * color_sample.rgb + color_sample.rgb + 0.02);

		vec3 normal = ComputeNormal(vec);
		//float gradient = 0.8*dot(normal,normal);
		//color_sample.rgb += gradient;

		//color_sample.rgb += Lighting(vec,  normal,  vLightAmbient,  vLightDiffuse,  vLightSpecular);

		float gradient = 1.0 * ( normal.r*normal.r + normal.g*normal.g + normal.b*normal.b);
		//color_sample.rgb *= 1*normal;

		//color_sample.rgb /= 3.0;

		//color_sample.rgb *= (1-vec)*5;

		color_sample.rgb += (normal);
		color_sample.rgb += Lighting(vec,  normal,  vLightAmbient,  vLightDiffuse,  vLightSpecular, color_sample.rgb);
		color_sample.rgb = clamp(color_sample.rgb,0.0,1.0);

		//if (length(vec) < 0.8)
		//	color_sample.a *= 0;

		//color_sample.rgb = (vec3((1-normal.r,1-normal.r,1-normal.r)));

		color_sample.rgb *= color_sample.a;

		color_sample.a	 *= 1.0;

		dst.rgb  = ( 1.0 - dst.a ) * color_sample.a * color_sample.rgb + dst.rgb;
		dst.a    = ( 1.0 - dst.a ) * color_sample.a                    + dst.a;

		//dst.a *= 1.01;

		//if ((color_sample.a > 0.001) && distanceUntilHit == 0)
		//	distanceUntilHit = length(vec);

		// ----------------- early stopping criteria ------------

		if (vec.x > 1.0f || vec.y > 1.0f || vec.z > 1.0f )
			break;
	}

	//dst = 0.75*dst + 1.0/4*( dst_1 + dst_2 + dst_3 + dst_4);
	/*	float K = 0.004;
	if (dst.r < K && dst.g < K && dst.b < K)
	gl_FragColor =  vec4(0.7);
	else
	*/

	// if accumulated alpha is too low change the background color
	/*
	if ( dst.a > 0.1)
		gl_FragColor = 1 * vec4( dst.r,dst.g,dst.b,dst.a);
	else
	{
		dst.rgb = mix( vec3( 1.0,1.0,1.0),dst.rgb,0.1);
		gl_FragColor =  vec4( dst.r,dst.g,dst.b,dst.a);
	}
	*/

	gl_FragColor = 3.5 * vec4( dst.r,dst.g,dst.b,dst.a);

	//gl_FragColor = vec4(distanceUntilHit,0,0,0.5);
}

/*
color_sample_1.rgb *= color_sample_1.a;
color_sample_2.rgb *= color_sample_2.a;
color_sample_3.rgb *= color_sample_3.a;
color_sample_4.rgb *= color_sample_4.a;

dst_1.rgb  = ( 1.0 - dst_1.a ) * color_sample_1.a * color_sample_1.rgb + dst_1.rgb;
dst_1.a    = ( 1.0 - dst_1.a ) * color_sample_1.a                 	   + dst_1.a;

dst_2.rgb  = ( 1.0 - dst_2.a ) * color_sample_2.a * color_sample_2.rgb + dst_2.rgb;
dst_2.a    = ( 1.0 - dst_2.a ) * color_sample_2.a                 	   + dst_1.a;

dst_3.rgb  = ( 1.0 - dst_3.a ) * color_sample_3.a * color_sample_3.rgb + dst_3.rgb;
dst_3.a    = ( 1.0 - dst_3.a ) * color_sample_3.a                 	   + dst_3.a;

dst_4.rgb  = ( 1.0 - dst_4.a ) * color_sample_4.a * color_sample_4.rgb + dst_4.rgb;
dst_4.a    = ( 1.0 - dst_4.a ) * color_sample_4.a                 	   + dst_4.a;
*/

//if ( color_sample.r == 0.99)
//dst.a              *=   1.025;