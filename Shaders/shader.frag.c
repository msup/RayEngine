/*vec3 L = vec3(0,1,1);
float s = dot(color_sample.xyz, L );
color_sample.rgb = s * color_sample.rgb + 0.1*color_sample.rgb;
color_sample.rgb *= color_sample.a;
*/

uniform	sampler2D backBuffer;
uniform sampler2D frontBuffer;
uniform sampler1D classificationTexture;
uniform sampler3D volume;

//vec3 AmbientLightColor           = vec3(0.75,0.75,0.75);
uniform vec3  AmbientLightColor;
uniform float AmbientLightCoeff;

//vec3 vLightDiffuse          = vec3(0.0,0.9,0.0);
uniform vec3 DiffuseLightColor;
uniform float DiffuseLightCoeff;

uniform vec3 SpecularLightColor;
uniform float SpecularLightCoeff;
uniform float SpecularPowerFactor;


//vec3 vLightSpecular         = vec3(1.0,1.0,1.0);
vec3 LightDir               = vec3(0.0,0.5,0.5);

uniform vec3  FinalAlphaMixColor;
uniform float FinalAlphaMixFactor;
uniform float FinalAlphaMixThreshold;

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
    vec3 AmbientLightColor,
    vec3 DiffuseLightColor,
    vec3 SpecularLightColor,
    vec3 voxelColor
    )
{
    vec3 vLightDir   = vec3(1.0,1.0,1.0);
    vec3 vViewDir    = normalize(vec3(1.0,1.0,1.0)-vPosition);
    vec3 vReflection = normalize(reflect(vViewDir, vNormal));

    // Beckmann distribution
    float m = 0.5;
    float alpha = dot( vReflection, vLightDir);
    float C = exp(-pow(tan(alpha),2)/pow(m,2));
    float M = 4*m*m*pow(cos(alpha),4);
    vec3 BalckmanColor = vec3(0.8,0.8,0.0);

    /*
    return
        AmbientLightCoeff  * AmbientLightColor +
        DiffuseLightCoeff  * DiffuseLightColor  * max( dot( vNormal, -vLightDir),0.0) +
        //DiffuseLightCoeff  * vec3(0.8,0.8,0)  * max( dot( vNormal, +vLightDir),0.0) +
       SpecularLightCoeff/5 * SpecularLightColor * pow( max ( dot( vReflection, vLightDir),0.0),SpecularPowerFactor)

       + SpecularLightCoeff/2 * BalckmanColor * C / M; // Beckmann distribution
       */

     vec3 lightPosition = vec3(0.1,0.1,0.1);
     vec3 lightDir = lightPosition - vPosition;
     float distance = length(lightDir);
     distance = (distance * distance);

     lightDir = normalize(lightDir);

     float i = clamp(dot(lightDir,vNormal), 0.0, 1.0);

     vec3 h = normalize(lightDir+vViewDir);
     float j = pow( clamp( dot( vNormal,h),0.0,1.0), 20.0);

     return
          clamp (
          (
          1  * AmbientLightColor //+
          //i * DiffuseLightColor  * 0.8/distance   +
          //j * SpecularLightColor * 0.8/distance
          ), 0.0, 1.0);
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

  /*  if ((sample >= 0.50) && (sample <= 1.00))
    {
        float D = 0.50 * 255;
        float H = 1.00 * 255;
        float q = D / (D-H);
        float k = -q / D;

        float alpha =  k * color_sample.b*255 + q;

        alpha = clamp(alpha - 0.45f,0.0f,1.0f);

        color = vec4(0.99,0.19,0.19, alpha/1);
    }
    else if (sample > 0.40 && sample < 0.50)
        color = vec4(0,0,1,0.09);

    if ((sample >= 0.82) && (sample <= 1.0))
    {
        float D = 0.75*255;
        float H = 1.00*255;
        float q = D / (D-H);
        float k = -q / D;

        float alpha =  k * color_sample.b*255 + q;
        color = vec4(0.77,0.72,0.87, alpha );
    }

    if (sample > 0.3 && sample < 0.42)
        color = vec4(1,0.8,0,0.1);*/

   if ( sample >= 23.1/255.0 && sample <= 41.93/255)
        color = vec4(92/255.0, 230/255.0, 169/255.0, 23.84/255.0);

   // else
     //   color = vec4(0.087,0.099,0.87,0.000);

    /*
    if (color.a < AlphaThreshold)
    {
        delta_dir = norm_dir * AlphaSkipStep * delta;
        asdf = 0;
    }
    else
    {
        delta_dir = norm_dir * delta;
        asdf = 0;
    }
    */

    //delta_dir = norm_dir * delta;

    //color = vec4(0);
    //color = texture1D( classificationTexture, sample).rgba;
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
        //color_sample.a *= 0.25;

        // transluency correction
        //color_sample.a /= (exp( 0.1 / length(vec)) );
        //color_sample.a = 1.0 - pow(1.0-color_sample.a,0.05/length(vec));

        //if (color_sample.a < 0.5)
        //	color_sample.a /= 8.00;

        // advance in direction//
        vec += delta_dir;

        // accumulation reached the level
        //if (dst.a >= 0.999f)
        //    break;

       /* if (dst.a >= 0.5f && length(vec) < 0.5)
            dst.a *= 0 ;*/

        //  if transluency is lower than level for fast skipping, step back and use standard integration step
      /*  if (color_sample.a > AlphaThreshold && asdf == 1)
        {
            vec	-=	norm_dir * AlphaSkipStep * delta;
            vec +=  norm_dir * delta;
        }*/

        // ---------------- classification end ------------------

        // ---------------- color optical model -----------------
        //vec3 L = vec3(0.25,0.35,0.25);
        //float s = dot ( color_sample.rgb, L);

        //color_sample.rgb = 0.5* (s * color_sample.rgb + color_sample.rgb + 0.02);

        vec3 normal = ComputeNormal(vec);
        //float gradient = 0.8*dot(normal,normal);
        //color_sample.rgb += gradient;

        //color_sample.rgb += Lighting(vec,  normal,  AmbientLightColor,  vLightDiffuse,  vLightSpecular);

        //float gradient = 1.0 * ( normal.r*normal.r + normal.g*normal.g + normal.b*normal.b);

        // opacity modulation based on gradient

        //color_sample.rgb += 1 * normal;

        //color_sample.rgb /= 3.0;

        //color_sample.rgb *= (1-vec)*5;

        //color_sample.rgb += (normal/5);
        color_sample.rgb += Lighting(vec,  normal,  AmbientLightColor,  DiffuseLightColor,  SpecularLightColor, color_sample.rgb);

      /*  vec3 reference = vec3( 0.0,1.0,1.0);
        float L = length( abs( vec-reference));
        if (L < 0.2)
            color_sample.a *= 0;*/

        //color_sample.rgb = clamp(color_sample.rgb,0.0,1.0);

        //if (length(vec) < 0.5)
        //	color_sample.a /= 5;

        //color_sample.rgb = (vec3((1-normal.r,1-normal.r,1-normal.r)));

        color_sample.rgb *= color_sample.a;

        color_sample.a	 *= 0.05;

        dst.rgb  = ( 1.0 - dst.a ) * color_sample.a * color_sample.rgb + dst.rgb;
        dst.a    = ( 1.0 - dst.a ) * color_sample.a                    + dst.a;

        //dst.a *= 1.01;

        if ((color_sample.a > 0.01) && distanceUntilHit == 0)
        	distanceUntilHit = pow(length(vec),3.5);

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

    /*if ( dst.a > FinalAlphaMixThreshold)
    	gl_FragColor = 3 * vec4( dst.r,dst.g,dst.b,dst.a);
    else
    {
        dst.rgb = mix(dst.rgb,FinalAlphaMixColor,FinalAlphaMixFactor);
        gl_FragColor = 4 * vec4( dst.r,dst.g,dst.b,0.5);
    }
*/

    vec4 frontColor = vec4( dst.r,dst.g,dst.b,dst.a);
    vec4 texColor = vec4(0.95,0.95,0.95,0.1);

    gl_FragColor.rgb = frontColor.a * frontColor.rgb + (1.0 - frontColor.a) * texColor.rgb;
    gl_FragColor.a = frontColor.a + (1.0 - frontColor.a) * texColor.a;
    //gl_FragDepth = gl_FragCoord.z;

    //gl_FragColor = 40 * vec4( dst.r,dst.g,dst.b,dst.a);

 /*   if (length(dst.rgb) < 0.03)
        {
        dst.rgb = mix(dst.rgb,vec3(1.0,1.0,1.0),0.5);
        gl_FragColor = 1 * vec4( dst.r,dst.g,dst.b,1);
        }*/

    //gl_FragColor = 2.0 * vec4( dst.r,dst.g,dst.b,dst.a);

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