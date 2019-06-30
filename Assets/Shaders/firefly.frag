#ifdef GL_ES
precision highp float;
#endif

uniform float time;
uniform vec2 resolution;

float ball(vec2 p, float fx, float fy, float ax, float ay)
{
	vec2 r = vec2(p.x + sin(time * fx) * ax, p.y + cos(time * fy) * ay);	
	return 0.09 / length(r);
}

void main(void) {
	vec2 pixel = (2.0 * gl_FragCoord.xy - resolution) / resolution.xy;	
	pixel.x *= resolution.x / resolution.y;

	float color = 0.0;
	color += ball(pixel, 1.0, 2.0, 0.1, 0.2);
	color += ball(pixel, 1.5, 2.5, 0.2, 0.3);
	color += ball(pixel, 2.0, 3.0, 0.3, 0.4);
	color += ball(pixel, 2.5, 3.5, 0.4, 0.5);
	color += ball(pixel, 3.0, 4.0, 0.5, 0.6);	
	color += ball(pixel, 1.5, 0.5, 0.6, 0.7);
	color += ball(pixel, 0.1, 0.5, 0.6, 0.7);
	color = max(mod(color, 0.4), min(color, 2.0));
	
	gl_FragColor = vec4(color, color * 0.3, 0.3, 1.0);

}