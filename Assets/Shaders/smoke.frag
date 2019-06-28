#ifdef GL_ES
precision highp float;
#endif

#extension GL_OES_standard_derivatives : enable

uniform float time;
uniform vec2 resolution;//The width and height of our screen
uniform vec2 mouse;//The x,y are the posiiton. The z is the power/density
uniform sampler2D texture;//Our input texture

void main()
{
  vec2 pixel = gl_FragCoord.xy / resolution.xy;

  //Get the color of the current pixel
  gl_FragColor = texture2D(texture, pixel);

  //Get the distance of the current pixel from the smoke source
  float dist = distance(mouse, gl_FragCoord.xy);
  
  //Generate smoke when mouse is pressed
  //gl_FragColor.rgb += abs(sin(time)) * 1280.0 * max(15.0 - dist, 0.0);
  
  //Generate fixed smoke (this is the little point moving around in the center)
  vec2 smokePoint = vec2(resolution.x / 2.0 + 100.0 * sin(time), resolution.y / 2.0 + cos(time * 3.5) * 20.0);
  dist = distance(smokePoint, gl_FragCoord.xy);
  gl_FragColor.rgb += 0.01 * max(15.0-dist, 0.0);
  
  //Smoke diffuse
  float xPixel = 1.0 / resolution.x; //The size of a single pixel
  float yPixel = 1.0 / resolution.y;
  vec4 rColor = texture2D(texture, vec2(pixel.x + xPixel, pixel.y));
  vec4 lColor = texture2D(texture, vec2(pixel.x - xPixel, pixel.y));
  vec4 uColor = texture2D(texture, vec2(pixel.x, pixel.y + yPixel));
  vec4 dColor = texture2D(texture, vec2(pixel.x, pixel.y - yPixel));
  
  //Handle the bottom boundary
  if (pixel.y <= yPixel){
    dColor.rgb = vec3(0.0);
  }

  //Diffuse equation
  float factor = 8.0 * 0.016 * (lColor.r + rColor.r + dColor.r * 3.0 + uColor.r - 6.0 * gl_FragColor.r);
  
  //Account for low precision of texels
  float minimum = 0.003;
  if (factor >= -minimum && factor < 0.0) factor = -minimum;
  
  gl_FragColor.rgb += factor;
 }