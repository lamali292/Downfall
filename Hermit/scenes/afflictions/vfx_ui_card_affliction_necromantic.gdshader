shader_type canvas_item;
render_mode blend_mix;

#include "res://shaders/vfx/_util/erosion_from_factors.gdshaderinc"
#include "res://shaders/vfx/_util/polar_coordinates.gdshaderinc"
#include "res://shaders/vfx/_util/tiling_and_offset.gdshaderinc"

uniform vec4 main_st;
uniform vec2 mask_erosion;
uniform sampler2D lut : repeat_disable;
uniform sampler2D mask : repeat_disable;

// Necromantic tuning
uniform float scroll_speed = 0.15;      // how fast the wisps rise
uniform float swirl_amount = 0.35;      // polar swirl strength
uniform float flicker_speed = 4.0;      // pulse rate
uniform float flicker_depth = 0.25;     // 0 = no flicker, 1 = full
uniform float distortion = 0.04;        // texture-driven warp
uniform float lut_strength = 1.0;       // 0 = raw tex, 1 = fully LUT-graded

varying vec4 vertex_color;

void vertex() {
    vertex_color = COLOR;
}

void fragment() {
    // Polar mapping with a slow rotational swirl over time
    vec2 polar_uv = polar_coordinates(UV, vec2(0.5, 0.5), 1.0, 1.0);
    polar_uv.x += TIME * swirl_amount * 0.1; // rotate angle
    polar_uv.y -= TIME * scroll_speed;       // rise

    polar_uv = tiling_and_offset(polar_uv, main_st, vec2(0.0));

    // Self-distortion: sample once to warp the second sample (smoky turbulence)
    vec4 warp = texture(TEXTURE, polar_uv);
    vec2 warped_uv = polar_uv + (warp.rg - 0.5) * distortion;
    vec4 main_tex = texture(TEXTURE, warped_uv);

    // LUT grade: use the texture's luminance as the lookup coordinate
    float luma = dot(main_tex.rgb, vec3(0.299, 0.587, 0.114));
    vec3 graded = texture(lut, vec2(luma, 0.5)).rgb;
    vec3 rgb = mix(main_tex.rgb, graded, lut_strength);

    // Mask + erosion (unchanged logic from your original)
    vec4 mask_tex = texture(mask, UV);
    float alpha = vertex_color.a * mask_tex.a;
    alpha = erosion_from_factors(vec2(mask_erosion.x, mask_erosion.y), alpha);

    // Flicker: a subtle pulse so the glow breathes like candlelight/necrotic energy
    float flicker = 1.0 - flicker_depth * (0.5 + 0.5 * sin(TIME * flicker_speed));
    alpha *= flicker;

    COLOR = vec4(rgb * vertex_color.rgb, alpha);
}