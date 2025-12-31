shader_type canvas_item;
uniform vec4  new_color: source_color = vec4(1.0);
uniform sampler2D color_map: filter_nearest;
uniform bool is_grey;


void fragment() {
	// The color of the current "Pixel" on our mask
	vec4 mask_color = texture(color_map, UV);
	// The color of the current "Pixel" on our sprite
	vec4 orig_color = texture(TEXTURE, UV);
	float average = (orig_color.r + orig_color.g + orig_color.b) / 3.0;
	if(mask_color.r == 1.0 && !is_grey){

		// Averaging the Tint and our grayscale, this looks a bit flat but is somewhat accurate
		COLOR.r = (new_color.r + average) /2.0;
		COLOR.g = (new_color.g + average) /2.0;
		COLOR.b = (new_color.b + average) / 2.0;
		// We want to use source alpha
		COLOR.a = orig_color.a;
	}
	else if(is_grey)
	{
		// Grayscale for the losers of the race
		COLOR.rgb = vec3(average);
	}

}
