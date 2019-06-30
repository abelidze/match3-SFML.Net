#ifdef GL_ES
precision highp float;
#endif

#extension GL_OES_standard_derivatives : enable

uniform sampler2D texture;
uniform float time;
uniform vec2 resolution;
uniform vec2 position;
uniform vec2 size;

void main()
{
	vec2 pixel = gl_FragCoord.xy / min(resolution.x, resolution.y);
	vec2 origin = position / min(resolution.x, resolution.y);
	vec3 color1 = vec3(0.6, 0.3, 0.2);
	vec3 color2 = vec3(0.5, 0.5, 0.0);
	
	float c1 = 0.0;
	float c2 = 0.0;
	float h = 0.0;

	const float PI = 3.14159265;
	const float COUNT = 20.0;
	const float RADIUS = 0.0005;
	const float RQSR = RADIUS * RADIUS;
	const float PI_PART = PI * 2.0 / COUNT;

	float scale = (size / resolution).x * 0.7;
	float time = mod(time, PI_PART);

	for (float i = 0.0; i < COUNT; ++i) {
		float s = sin(time + i * PI_PART) * scale - origin.y;
		float c = cos(time + i * PI_PART) * scale - origin.x;
		float d = abs(pixel.x + c);
		float e = abs(pixel.y + s);	
		float a = 0.05 - (d * d + e * e) / RADIUS;
		if (a > 0.0 ) { // && RQSR / (d * e) > 0.001
			h += RQSR / (d * e);
			c1 += RADIUS / d;
			c2 += RADIUS / e;
		}
	}

	gl_FragColor = vec4(c1 * color1 + c2 * color2 + vec3(h), 1.0);
}