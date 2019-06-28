#ifdef GL_ES
precision highp float;
#endif

#extension GL_OES_standard_derivatives : enable

uniform float time;
uniform vec2 resolution;

void main()
{
	vec2 pixel = (gl_FragCoord.xy * 2.0 - resolution) / min(resolution.x, resolution.y);
	vec3 color1 = vec3(0.6, 0.3, 0.2);
	vec3 color2 = vec3(0.5, 0.5, 0.0);

	float c1 = 0.0;
	float c2 = 0.0;
	float h = 0.0;
	float a = 1.0;
	const float RADIUS = 0.01;
	const float PI = 3.14159265;

	for (float i = 0.0; i < 20.0; ++i) {
		float s = sin(0.5 * time + i * PI / 10.0) * 0.4;
		float c = cos(0.5 * time + i * PI / 10.0) * 0.9;
		float d = abs(pixel.x + c);
		float e = abs(pixel.y + s);
		a = 1.0 - (d * d + e * e) / RADIUS;
		if (a > 0.0 && 0.00003 / (d * e) > 0.15) {
			h += 0.00003 / (d * e);
			c1 += 0.005 / d;
			c2 += 0.005 / e;
		}
		//d = d < 0.5 ? d : 0.0;
		//e = e < 0.5 ? e : 0.0;
	}

	gl_FragColor = vec4(c1 * color1 + c2 * color2 + vec3(h), 0.6);
}