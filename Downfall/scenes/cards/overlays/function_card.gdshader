shader_type canvas_item;

uniform float glitch_intensity : hint_range(0.0, 1.0) = 0.5;
uniform float block_glitch : hint_range(0.0, 1.0) = 0.1;
uniform float time_scale : hint_range(0.0, 5.0) = 0.5;
uniform float aberration : hint_range(0.0, 0.05) = 0.01;

// How often block glitches fire (higher = more frequent)
uniform float block_frequency : hint_range(0.0, 20.0) = 6.0;
// How wide block slices can be
uniform float block_size : hint_range(0.0, 0.5) = 0.1;

// Scanlines
uniform float scanline_strength : hint_range(0.0, 1.0) = 0.1;
uniform float scanline_density : hint_range(0.0, 10.0) = 3.0;

// Rolling desync band
uniform float desync_strength : hint_range(0.0, 1.0) = 0.08;
uniform float desync_speed : hint_range(0.0, 2.0) = 0.3;

// HSV
uniform float h : hint_range(0, 1) = 1;
uniform float s : hint_range(0, 5) = 1;
uniform float v = 1;

uniform sampler2D screen_tex : hint_screen_texture, filter_linear;

varying vec4 modulate_color;
varying vec2 vertex_offset;

float rand(vec2 co) {
    return fract(sin(dot(co, vec2(12.9898, 78.233))) * 43758.5453);
}
float rand1(float x) {
    return fract(sin(x * 127.1) * 43758.5453);
}

void vertex() {
    modulate_color = COLOR;
    float t = TIME * time_scale;

    float slice_y = rand1(floor(t * block_frequency));
    float slice_size = 0.02 + rand1(floor(t * block_frequency * 1.3)) * block_size;
    float in_slice = step(slice_y, UV.y) * step(UV.y, slice_y + slice_size);
    float offset = (rand1(floor(t * block_frequency * 1.7)) * 2.0 - 1.0) * block_glitch * 8.0 * in_slice;

    VERTEX.x += offset;
    vertex_offset = vec2(offset, 0.0);
}

vec4 apply_hsv(vec4 col) {
    mat3 RGB_to_YIQ = mat3(
        vec3(0.2989,  0.5959,  0.2115),
        vec3(0.5870, -0.2774, -0.5229),
        vec3(0.1140, -0.3216,  0.3114));
    col.rgb = RGB_to_YIQ * col.rgb;
    float hue = mix(0.0, 6.283185, 1.0 - h);
    mat3 hue_shift = mat3(
        vec3(1.0, 0, 0),
        vec3(0, cos(hue), -sin(hue)),
        vec3(0, sin(hue),  cos(hue)));
    col.rgb *= hue_shift;
    col.rgb = mat3(vec3(1,0,0), vec3(0,s,0), vec3(0,0,s)) * col.rgb;
    col.rgb = mix(vec3(0.0), col.rgb, v);
    col.rgb = inverse(RGB_to_YIQ) * col.rgb;
    return col;
}

void fragment() {
    vec2 uv = UV;
    float t = TIME * time_scale;

    // Row jitter
    float row = floor(uv.y * 20.0);
	float jitter = (rand(vec2(row, floor(t * 0.5))) * 2.0 - 1.0) * glitch_intensity * 0.025;
	uv.x += jitter * step(0.75, rand(vec2(row * 0.1, floor(t * 0.5))));

    // Aberration
    float ab = aberration * (0.5 + 0.5 * sin(t * 2.3)) + aberration * rand1(floor(t * 4.0));

    vec4 col_r = apply_hsv(texture(TEXTURE, uv));
    vec4 col_g = apply_hsv(texture(TEXTURE, uv - vec2(ab, 0.0)));
    vec4 col_b = apply_hsv(texture(TEXTURE, uv + vec2(ab, 0.0)));

    vec3 col = vec3(col_r.r, col_g.g, col_b.b);

    // Scanlines
    col *= 1.0 - sin(uv.y * scanline_density * 3.14159 * 100.0) * 0.5 * scanline_strength;



    // Desync band
    float band_pos = fract(t * desync_speed);
    float band = smoothstep(0.0, 0.02, abs(uv.y - band_pos)) * smoothstep(0.1, 0.05, abs(uv.y - band_pos));
    col *= 1.0 - band * desync_strength;

    float a = texture(TEXTURE, UV).a;
    COLOR = vec4(col, a) * modulate_color;
}