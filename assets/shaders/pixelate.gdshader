shader_type canvas_item;

uniform float pixel_size : hint_range(1.0, 32.0) = 8.0;
uniform float jitter_amount : hint_range(0.0, 2.0) = 0.5;

void fragment() {
	vec2 uv = UV;

	float time = TIME * 0.2;
	vec2 jitter = vec2(
		sin(time * 1.3 + uv.y * 50.0) * jitter_amount / 200.0,
		cos(time * 1.7 + uv.x * 50.0) * jitter_amount / 200.0
	);

	vec2 size = vec2(1.0 / pixel_size);
	vec2 pixel_uv = floor((uv + jitter) / size) * size;
	vec4 color = texture(TEXTURE, pixel_uv);
	COLOR = color;
}