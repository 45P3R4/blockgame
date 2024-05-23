shader_type spatial;
//render_mode unshaded;

uniform sampler2D tex;

void fragment() {
	ALBEDO = texture(tex, UV).xyz;
	ROUGHNESS = 1.0;
	SPECULAR = 0.0;
}

//void light() {
	// Called for every pixel for every light affecting the material.
	// Uncomment to replace the default light processing function with this one.
//}
