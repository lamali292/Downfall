shader_type canvas_item;

uniform float speed : hint_range(0.1, 5.0) = 1.0;
uniform float strength : hint_range(0.0, 0.1) = 0.02;
uniform float transparency : hint_range(0.0, 1.0) = 0.7;
uniform float y_rot : hint_range(-180, 180) = 0.0;
uniform float x_rot : hint_range(-180, 180) = 0.0;
uniform float seed = 0.0;
uniform float intensity : hint_range(0, 1, 0.01) = 1.0;

void fragment() {
    float t = TIME * speed + seed;
    float tilt_shift = (x_rot * 0.02) + (y_rot * 0.03);

    // Check original alpha to preserve card shape
    vec4 orig = texture(TEXTURE, UV);
    if (orig.a < 0.01) discard;

    // Wobble UV
    vec2 uv = UV;
    uv.x += sin(UV.y * 12.0 + t * 2.1) * strength;
    uv.x += sin(UV.y * 5.0  + t * 1.3) * strength * 0.5;
    uv.y += cos(UV.x * 9.0  + t * 1.7) * strength;
    uv.y += cos(UV.x * 4.0  + t * 0.9) * strength * 0.5;
    uv = clamp(uv, 0.0, 1.0);

    vec4 tex = texture(TEXTURE, uv);

    vec3 ghost_color = vec3(0.4, 0.75, 1.0);

    float shimmer = 0.5 + 0.5 * sin(UV.y * 6.0 + t * 1.5 + tilt_shift * 3.0);
    shimmer += 0.3 * sin(UV.x * 8.0 + t * 0.8 - tilt_shift * 2.0);
    shimmer = clamp(shimmer, 0.0, 1.0);

    float pulse = 0.85 + 0.15 * sin(t * 1.2 + seed);

    tex.rgb = mix(tex.rgb, ghost_color, 0.25 + 0.1 * shimmer);
    tex.rgb += ghost_color * shimmer * 0.1;
    tex.a = orig.a * transparency * pulse;

    COLOR = mix(orig, tex, intensity);
}