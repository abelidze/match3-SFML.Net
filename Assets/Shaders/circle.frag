#ifdef GL_ES
precision mediump float;
#endif

uniform vec2 resolution;
uniform float time;

const float PI = 3.1415926535;

void main()
{
	vec2 p = (2.0 * gl_FragCoord.xy - resolution) / min(resolution.x,resolution.y);
	vec3 color = vec3(0.0);

	const float SIZE = 0.01;
	const float COUNT = 10.0;
	const float RADIUS = 0.4;
	
	float t =  mod(time, 2.0 / (1.0 - 1.0 / COUNT));
	for (float i = 0.0; i < COUNT; ++i) {
		float a = PI * i / COUNT;
		float x = cos(t * a);
		float y = sin(t * a);
		vec2 o = RADIUS * vec2(x, y);
			
		float r = fract(t * a);
		color += SIZE / length(p - o) * vec3(r, 1.0 - r, 1.0);
	}
	gl_FragColor = vec4(color, 1.0);
}