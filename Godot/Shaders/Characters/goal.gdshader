shader_type canvas_item;

uniform float x_mult : hint_range(0.0, 15.0, 0.01) = 1.0;
uniform float y_mult : hint_range(0.0, 15.0, 0.01) = 1.0;

void fragment() {
	COLOR = texture(TEXTURE, vec2(UV.g  * x_mult,UV.r * y_mult));

}
