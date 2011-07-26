uniform	sampler2D backBuffer;
uniform sampler2D frontBuffer;
uniform sampler3D volume;
uniform sampler1D color_text;
uniform sampler1D z_distance_texture;


varying vec2 TexCoord0;
varying vec2 TexCoord1;

vec3 start = texture2D(frontBuffer,TexCoord1).xyz;
vec3 stop = texture2D(backBuffer,TexCoord0).xyz;
vec3 dir = stop - start;
vec3 norm_dir = normalize(dir);
vec3 vec = start;

float len = length(dir.xyz);
float delta = 0.0005*10;

vec3  delta_dir = norm_dir * delta;
float delta_dir_len = length(delta_dir);

vec4 color_sample = vec4(0,0,0,0);
vec3 col_acc = vec3(0,0,0);

float length_acc = 0.0;
float alpha_sample = 0.0;
float alpha_acc = 0.0;

vec4 dst = vec4(0.0, 0.0, 0.0, 0.05);

float kx = 64.0 ;
float ky = 64.0 ;
float kz = 64.0 ;

vec3 vVoxelStepsize = vec3(1.0/kx,1.0/ky,1.0/kz);

 vec3 AmbientLightColor = vec3(0.8,0.7,0.78);
 vec3 vLightDiffuse = vec3(0.6,0.0,0.0);
 vec3 vLightSpecular = vec3(0.0,0.9,0.9);
    
vec3 Lighting (
    vec3 vPosition,
    vec3 vNormal,
    vec3 AmbientLightColor,
    vec3 DiffuseLightColor,
    vec3 SpecularLightColor, float length_acc )
{
    vec3 vLightDir   = vec3(1.0,1.0,1.0);
    vec3 vViewDir    = normalize(vec3(1.0,1.0,1.0)-vPosition);
    vec3 vReflection = normalize(reflect(vViewDir, vNormal));

    // Beckmann distribution
    float m = 0.5;
    float alpha = dot( vReflection, vLightDir);
    float C = exp(-pow(tan(alpha),2)/pow(m,2));
    float M = 4*m*m*pow(cos(alpha),4);
    vec3 BeckmannColor = vec3(0.8,0.8,0.8);

    /*
    return
        AmbientLightCoeff  * AmbientLightColor +
        DiffuseLightCoeff  * DiffuseLightColor  * max( dot( vNormal, -vLightDir),0.0) +
        //DiffuseLightCoeff  * vec3(0.8,0.8,0)  * max( dot( vNormal, +vLightDir),0.0) +
       SpecularLightCoeff/5 * SpecularLightColor * pow( max ( dot( vReflection, vLightDir),0.0),SpecularPowerFactor)

       + SpecularLightCoeff/2 * BeckmannColor * C / M; // Beckmann distribution
       */

     vec3 lightPosition = vec3(0.1,0.1,0.1);
     vec3 lightDir = lightPosition - vPosition;
     float distance = length(lightDir);
     distance = (distance * distance);

     lightDir = normalize(lightDir);
		 
     float i = clamp(dot(lightDir,vNormal), 0.0, 1.0);

     vec3 h = normalize(lightDir - vViewDir);
     float j = pow( clamp( dot( vNormal,h),0.0,1.0), 8.0);
	 
	 if (length_acc < 0.25)
		return clamp((i*vec3(0.7,0.7,0.7)+j*vec3(0.0,0.5,0.5)+length_acc*BeckmannColor*M),0.0,1.0);
	 else 
		return clamp((i*vec3(0.8,0.7,0.7)+j*vec3(0.0,0.5,0.5)),0.0,1.0);
	 
     return
          clamp (
          (
          0.0  * AmbientLightColor +
          j * DiffuseLightColor    +
          j * SpecularLightColor + BeckmannColor
          ), 0.0, 1.0);
}

vec3 ComputeNormal (vec3 vHitPosTex)
{
    // TODO: check the blue channel when volume data structure changed

    float fVolumValXp = texture3D(volume, vHitPosTex+vec3(+vVoxelStepsize.x,0,0)).b;
    float fVolumValXm = texture3D(volume, vHitPosTex+vec3(-vVoxelStepsize.x,0,0)).b;

    float fVolumValYp = texture3D(volume, vHitPosTex+vec3(0,-vVoxelStepsize.y,0)).b;
    float fVolumValYm = texture3D(volume, vHitPosTex+vec3(0,+vVoxelStepsize.y,0)).b;

    float fVolumValZp = texture3D(volume, vHitPosTex+vec3(0,0,+vVoxelStepsize.z)).b;
    float fVolumValZm = texture3D(volume, vHitPosTex+vec3(0,0,-vVoxelStepsize.z)).b;

    vec3  vGradient  = vec3((fVolumValXm-fVolumValXp), (fVolumValYp-fVolumValYm), (fVolumValZm-fVolumValZp));

    vec3 vNormal     = gl_NormalMatrix * vGradient;

    float l = length(vNormal);
        if (l>0.0)
            vNormal /= l; // secure normalization

    return vNormal;
}
 
vec4 Classify (vec4 color_sample)
{	
    float sample = color_sample.b;
    vec4 color   = vec4(0);
	
	if ((sample >= 0.4) && (sample <= 0.5))
    {
        float D = 200/255.0;
        float H = 255/255.0;
        float q = D / (D-H);
        float k = -q / D;

        float alpha =  k * color_sample.b + q;
        color = vec4(0.1,0.1,0.97, alpha/100.0 );		
    }
	if ((sample >= 0.5) && (sample < 0.6)) 
	{
		color = vec4(0.0,0.2,0.7,0.8);
	}
	else if ((sample >= 0.2) && (sample <= 0.24)) 
	{
		color = vec4(1.0,0.2,0.,0.1);
	}
	else
	{
	color = vec4(0.0,0.0,0.0,0.00);
	}

    color = texture1D( color_text, sample);	
 	
	color = vec4(0);
	if (sample >= 0.7 && sample <= 1.0)
		color = vec4(0.95,0.8,0,0.8);
	
	return color;
}

float skip_delta_low_alpha = delta;
bool skipping_empty = false;
vec4 target_color = vec4(0.0);

void main()
{	
	float k_alpha = 10;

	vec4 color_sample_00 = texture3D(volume,start + 0.0*delta_dir*k_alpha);
		 color_sample_00 = Classify (color_sample_00);
	vec4 color_sample_05 = texture3D(volume,start + 0.05*delta_dir*k_alpha);
		 color_sample_05 = Classify (color_sample_05);
	vec4 color_sample_15 = texture3D(volume,start + 0.15*delta_dir*k_alpha);
	     color_sample_15 = Classify (color_sample_15);
	vec4 color_sample_20 = texture3D(volume,start + 0.20*delta_dir*k_alpha);
		 color_sample_20 = Classify (color_sample_20);
	vec4 color_sample_35 = texture3D(volume,start + 0.35*delta_dir*k_alpha);
	  	 color_sample_35 = Classify (color_sample_35);
		
	float t_alpha = 0.1;
	
	float k_limit = 1.00;
	
	if ((k_limit*color_sample_15.a >= color_sample_05.a))
		{
		target_color = vec4(1.00,0.0,0.0,0.5);
		vec = start + k_alpha * delta * 0.05 / 2;
		}
	else if ((k_limit*color_sample_20.a >= color_sample_15.a))
		{
		target_color = vec4(0.0,1.0,0.0,0.5);
		//vec = start + k_alpha * delta * 0.15 / 2;		
		}
	else if ((k_limit*color_sample_35.a >= color_sample_20.a))
		{
		target_color = vec4(0.0,0.0,1.0,0.5);
		vec = start + k_alpha * delta * 0.20 / 2;
		}
		
	if (start == stop)
		{
		gl_FragColor = vec4(1.0,0.0,0.0,1.0);
		discard;
		}
	
	for (int i = 0; i < 1.0/delta + 1; i++)
	{
		color_sample = texture3D(volume,vec);
		color_sample = Classify (color_sample);
		
		//color_sample.rgb = texture1D( z_distance_texture, 1-vec.z ).rgb;
	
		vec += delta_dir;
		length_acc += delta_dir_len;
		
		//color_sample.rgb += 0.1*Lighting (vec, ComputeNormal(vec), AmbientLightColor, vLightDiffuse, vLightSpecular );
		//color_sample.rgb = clamp(color_sample.rgb,0.0,1.0);
			
 		//color_sample.a *=  0.89;				
		
		if (color_sample.a > 0.5)
			color_sample.a = 1.00 - pow(1.0 - color_sample.a, delta * 5000);
					
		dst.rgb = dst.rgb + (1.0 - dst.a) * color_sample.a * color_sample.rgb;
		dst.a   = dst.a   + (1.0 - dst.a) * color_sample.a;
		
		if (color_sample.a > 0.8)
			dst.a *= 1.00;
		
		//if (dst.a > 0.1)
		//    delta_dir = norm_dir * delta/2;
				
		// vzdialenostnu transformaciu k prvemu okraju / hrane pre potreby akceleracie
		 // ----------------- early stopping criteria ------------
  		if ( dst.a >= 0.99)
		break;

		if (vec.x > 1.0f || vec.y > 1.0f || vec.z > 1.0f )
            break;
	}	
		
	//normalize(ComputeNormal(vec));
		
	if (dst.a > 0.5 )
	{
	  dst.rgb += 0.7*Lighting (vec, ComputeNormal(vec), AmbientLightColor, vLightDiffuse, vLightSpecular, length_acc);
	  dst.rgb = clamp(dst.rgb,0.0,1.0);
	}
		
	if (length(dst.rgb) < 0.5 && dst.a < 0.2)
		dst.a = 0;
	
	gl_FragColor = dst;
	
	vec4 frontColor = vec4( dst.r,dst.g,dst.b,dst.a);
    vec4 texColor = vec4(0.0,0.0,0.35,0.8);
	//texColor = target_color;
	
    gl_FragColor.rgb = frontColor.a * frontColor.rgb + (1.0 - frontColor.a) * texColor.rgb;
    gl_FragColor.a = frontColor.a + (1.0 - frontColor.a) * texColor.a;
	
	//gl_FragColor += target_color;
	
		
	//gl_FragDepth = 10*length_acc;
	 
}
