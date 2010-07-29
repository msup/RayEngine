/*vec3 L = vec3(0,1,1);
		float s = dot(color_sample.xyz, L );
		color_sample.rgb = s * color_sample.rgb + 0.1*color_sample.rgb;
		color_sample.rgb *= color_sample.a;
*/	

uniform	sampler2D backBuffer;
uniform sampler2D frontBuffer;
uniform sampler3D volume;

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
float delta = 0.0005 * 1; // 0.005;


vec3  delta_dir = norm_dir * delta;

//float delta_dir_len = length(delta_dir);

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

float distanceUntilHit = 0;

float k = 64.0 ;
vec3 vVoxelStepsize = vec3(1.0/k,1.0/k,1.0/k);

vec3 Lighting(vec3 vPosition, vec3 vNormal, vec3 vLightAmbient, vec3 vLightDiffuse, vec3 vLightSpecular, vec3 voxelColor) {
	vec3 vViewDir    = normalize(vec3(0.0,0.0,0.0)-vPosition);
	vec3 vReflection = normalize(reflect(vViewDir, vNormal));
	
	vec3 vLightDir = vec3(1.0,1.0,1.0);
	
	return 1*vLightAmbient + 
	//1*vLightDiffuse*max(dot(vNormal, -vLightDir),0.0)*voxelColor.rgb+
	0.0*vLightDiffuse  * max(dot(vNormal, -vLightDir),0.0) +  
	0.0*vLightSpecular * pow(max(dot(vReflection, vLightDir),0.0),3.00);
}

vec3 ComputeNormal(vec3 vHitPosTex) { 

	float fVolumValXp = texture3D(volume, vHitPosTex+vec3(+vVoxelStepsize.x,0,0)).x;
	float fVolumValXm = texture3D(volume, vHitPosTex+vec3(-vVoxelStepsize.x,0,0)).x;
	if ((vHitPosTex.x - vVoxelStepsize.x ) < 0.0)
		fVolumValXm = fVolumValXp;
	if ((vHitPosTex.x + vVoxelStepsize.x ) > 1.0)
		fVolumValXp = fVolumValXm;
	
	
	float fVolumValYp = texture3D(volume, vHitPosTex+vec3(0,-vVoxelStepsize.y,0)).x;
	float fVolumValYm = texture3D(volume, vHitPosTex+vec3(0,+vVoxelStepsize.y,0)).x;
	if ((vHitPosTex.y - vVoxelStepsize.y ) < 0.0)
		fVolumValYm = fVolumValYp;
	if ((vHitPosTex.y + vVoxelStepsize.y ) > 1.0)
		fVolumValYm = fVolumValYp;
	
	float fVolumValZp = texture3D(volume, vHitPosTex+vec3(0,0,+vVoxelStepsize.z)).x;
	float fVolumValZm = texture3D(volume, vHitPosTex+vec3(0,0,-vVoxelStepsize.z)).x;
	if ((vHitPosTex.z - vVoxelStepsize.z ) < 0.0)
		fVolumValZm = fVolumValZp;
	if ((vHitPosTex.z + vVoxelStepsize.z ) > 1.0)
		fVolumValZm = fVolumValZp;
	
	
	//vec3  vGradient  = vec3(fVolumValXm-fVolumValXp, fVolumValYp-fVolumValYm, fVolumValZm-fVolumValZp); 
	vec3  vGradient  = vec3((fVolumValXm-fVolumValXp), (fVolumValYp-fVolumValYm), (fVolumValZm-fVolumValZp)); 
	
	vec3 vNormal     = gl_NormalMatrix * vGradient;
	
	float l = length(vNormal); 
	if (l>0.0) vNormal /= l; // secure normalization
	
	return vNormal;
}



vec4 Classify (vec4 color_sample) {

	float bulhar = 255.0;
	float prah = 100.0 / bulhar;  //40 
	float add =  60.0 / bulhar;
	vec4 color = vec4(0);
	
	if (color_sample.r >= prah) 
	{
		float alpha =  1.0/155.0 * color_sample.r*255 - 100.0/155.0;
		color = vec4(0.89,0.84,0.89,alpha/20);		
	}
	else if ((color_sample.r > 50/bulhar) && (color_sample.r < 60/bulhar)) 
	{
		float D = 50;
		float H = 100;
		float q = D / (D-H);
		float k = -q / D;
		
		float alpha =  k * color_sample.r*255 + q;
		
		color = vec4(0.25,0.87,0.98, alpha/10);		
	}
	else if ( (color_sample.r <= (prah)))
		color = vec4(0.087,0.099,0.87,0.000);
	
	
	if (color.a < AlphaThreshold)
	{
		delta_dir = norm_dir * 1 * delta;
		asdf = 0;
	}
	else
	{
		delta_dir = norm_dir * delta;
		asdf = 0;
	}
	
	//delta_dir = norm_dir * delta;
	
	return color;
}

vec4 color_sample_1 = vec4(0,0,0,0);
vec4 color_sample_2 = vec4(0,0,0,0);
vec4 color_sample_3 = vec4(0,0,0,0);
vec4 color_sample_4 = vec4(0,0,0,0);

void main()
{
	vec4 dst = vec4(0.0, 0.0, 0.0, 0);
	
	vec4 dst_1 = vec4(0, 0, 0, 0);
	vec4 dst_2 = vec4(0, 0, 0, 0);
	vec4 dst_3 = vec4(0, 0, 0, 0);
	vec4 dst_4 = vec4(0, 0, 0, 0);
	
	for (int i = 0; i < 1/delta + 1; i++)
	{
		
		// get sample
		color_sample = texture3D(volume,vec);		
		
		// post classify	
		
		color_sample = Classify(color_sample);
		
		color_sample.a *= exp(1/length(vec));
		
		
		vec += delta_dir;
		
		
		if (dst.a >= 0.999f)
		break;
		
	/*	if (color_sample.a > AlphaThreshold && asdf == 1)
		{
			vec	-=	norm_dir * 1 * delta;
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
		
		vec3 vLightAmbient  = vec3(0.9,0.9,0.9);
		vec3 vLightDiffuse  = vec3(0.0,0.0,1.0);
		vec3 vLightSpecular = vec3(0.0,1.0,0.0);
		
		//color_sample.rgb += Lighting(vec,  normal,  vLightAmbient,  vLightDiffuse,  vLightSpecular);
		
		float gradient = 1.0 * (normal.r*normal.r + normal.g*normal.g + normal.b*normal.b);
		//color_sample.rgb += vec3(gradient);
		//color_sample.rgb /= 3.0;
		
		vec3 LightDir = vec3(1.0,1.0,1.0);
		//color_sample.rgb += Lighting(vec,  normal,  vLightAmbient,  vLightDiffuse,  vLightSpecular, color_sample.rgb);
		//color_sample.rgb = (vec3((1-normal.r,1-normal.r,1-normal.r)));
		color_sample.rgb *= color_sample.a;
		
		color_sample.a	 *= 1.0;
		
		dst.rgb  = ( 1.0 - dst.a ) * color_sample.a * color_sample.rgb + dst.rgb;
		dst.a    = ( 1.0 - dst.a ) * color_sample.a                    + dst.a;
		
		//dst.a *= 1.01;
		
		if ((color_sample.a > 0.001) && distanceUntilHit == 0)
			distanceUntilHit = length(vec);
		
		
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
	if (dst.a > 0.0001)
		gl_FragColor = 20 * vec4(dst.r,dst.g,dst.b,dst.a);	 
	else
	{
		dst.rgb = mix(dst.rgb,vec3(0.96,0.76,0.85),0.2);
		gl_FragColor =  vec4(dst.r,dst.g,dst.b,dst.a);
	}
	
	//gl_FragColor = vec4(distanceUntilHit,0,0,0.5);
	
}

/*
"Fragment shader failed to compile with the following errors:\n
ERROR: 0:65: error(#143) Undeclared identifier float4\n
ERROR: 0:65: error(#132) Syntax error: 'dst' parse error\n
ERROR: error(#273) 2 compilation errors.  No code generated\n\n"

*/




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
//dst.a   *=   1.025;