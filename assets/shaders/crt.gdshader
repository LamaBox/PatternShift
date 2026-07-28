shader_type canvas_item;

uniform float scanline_opacity : hint_range(0.0, 0.3) = 0.08;
uniform float vignette_intensity : hint_range(0.0, 0.8) = 0.3;
uniform float curvature : hint_range(0.0, 0.05) = 0.015;
uniform float glow_intensity : hint_range(0.0, 0.4) = 0.1;
uniform float zoom : hint_range(1.0, 1.1) = 1.02;
uniform float chromatic_intensity : hint_range(0.001, 1) = 0.001;
uniform float chromatic_angle : hint_range(0.0, 6.283) = 0.0;

uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear_mipmap;

void fragment() {
	vec2 uv = UV;

	vec2 curved_uv = uv * 2.0 - 1.0;
	curved_uv *= 1.0 + curvature * dot(curved_uv, curved_uv);
	curved_uv = curved_uv * 0.5 + 0.5;
	curved_uv = (curved_uv - 0.5) / zoom + 0.5;

	vec2 dir = vec2(cos(chromatic_angle), sin(chromatic_angle));
	vec4 color_r = texture(SCREEN_TEXTURE, curved_uv + dir * chromatic_intensity);
	vec4 color_g = texture(SCREEN_TEXTURE, curved_uv);
	vec4 color_b = texture(SCREEN_TEXTURE, curved_uv - dir * chromatic_intensity * 1.5);
	vec4 color = vec4(color_r.r, color_g.g, color_b.b, 1.0);

	float time = TIME * 0.02;
	float scanline = sin((curved_uv.y + time) * SCREEN_PIXEL_SIZE.y * 300.0);
	scanline = abs(scanline);
	scanline = smoothstep(0.0, 0.3, scanline);
	scanline = mix(1.0 - scanline_opacity, 1.0, scanline);
	color.rgb *= scanline;

	vec2 dist = abs(curved_uv - 0.5) * 2.0;
	float vignette = 1.0 - dot(dist, dist) * vignette_intensity;
	color.rgb *= vignette;

	float brightness = dot(color.rgb, vec3(0.2126, 0.7152, 0.0722));
	float glow = brightness * glow_intensity;
	color.rgb += glow * 0.3;
    
	COLOR = color;
}