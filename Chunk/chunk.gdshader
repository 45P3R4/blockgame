shader_type spatial;
//render_mode unshaded;

uniform sampler2D tex: filter_nearest;

void fragment() {
	ALBEDO = texture(tex, UV).xyz;
	ROUGHNESS = 1.0;
	SPECULAR = 0.0;
}

void light() {
	DIFFUSE_LIGHT += clamp(dot(NORMAL, LIGHT), 0.1, 1.0) * ATTENUATION * LIGHT_COLOR;
}
