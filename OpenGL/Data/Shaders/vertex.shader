varying vec2 TexCoord0,TexCoord1;

void main()
{ 
 TexCoord0 = gl_MultiTexCoord0.st;
 TexCoord1 = gl_MultiTexCoord1.st;
	
 gl_Position = ftransform(); 
}