uniform sampler2D texture;
uniform float opacity;

void main()
{
  vec4 color = texture2D(texture, gl_TexCoord[0].st);
  color.a *= opacity;
  gl_FragColor = color;
}
