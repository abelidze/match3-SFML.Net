#version 120

uniform sampler2D texture;
uniform float rnd;
uniform float intensity;
uniform float colorswap;

float rand(vec2 co){
    return fract(sin(dot(co.xy ,vec2(12.9898,78.233))) * 43758.5453);
}

void main (void)
{
	//intensity = 1.0f;
	float f = max(0, rand(vec2(gl_FragCoord.y, gl_FragCoord.x+rnd)) - rand(vec2(0, gl_FragCoord.x+rnd))*intensity);

	vec4 color = vec4(0, f, 0, texture2D(texture, gl_TexCoord[0].st).w * gl_Color.w * f);
	
	vec4 c = texture2D(texture, gl_TexCoord[0].st);
	float r = c.r; float b = c.b; float g = c.g;

	float swap2 = (0.5f - abs(0.5f - colorswap))*2.0f;
	c.r = r*(1.0f-swap2) + g*swap2;
	c.b = b*(1.0f-swap2) + g*swap2;
	c.g = g*(1.0f-swap2) + b*swap2;
	
	float swap1 = colorswap * (1.0f - swap2);
	r = c.r; b = c.b;
	
	c.r = r*(1.0f-swap1) + b*swap1;
	c.b = b*(1.0f-swap1) + r*swap1;
	
	vec4 c1 = c * gl_Color;
	
	//c1.w *= 1.0f-intensity;
	//c1.w -= gl_FragCoord.y*0.1f;
	
	gl_FragColor = c1*(1-intensity*2) + color*intensity*2;
}