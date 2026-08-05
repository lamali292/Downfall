shader_type canvas_item;
render_mode blend_mix; 

// SET GLOBAL SPIN HERE!!
const float global_spin = 1.0; 

// SIZE AND ROTATION
const float zoom = 2.0;
uniform float overall_alpha : hint_range(0.0, 1.0) = 1.0; 
const float y_rotation = 15.0;

// BASE COLOR CHANG
uniform vec4 custom_base_tint : source_color = vec4(1.0, 1.0, 1.0, 1.0);

// TEXTURES
uniform sampler2D base_texture : source_color, filter_linear_mipmap; 

uniform float texture_scale : hint_range(0.1, 5.0) = 1.0; 
uniform float spin_speed : hint_range(-5.0, 5.0) = 1.0; 
uniform float base_radius : hint_range(0.0, 1.0) = 0.3; 
const float edge_softness = .01;

// DISTORTION
uniform sampler2D inner_noise_tex : repeat_enable, filter_linear_mipmap; 
const float texture_warp_amount = .05;
const vec2 texture_noise_scale = vec2(0.25, 0.25); 
const float texture_noise_speed = 0.5;

// WOBBLE
const float core_wobble_amount = 0.015;
const int core_wobble_frequency = 1;
const float core_wobble_speed = 1.0; 

// FLAME TURBULENCE
uniform float flame_spread : hint_range(0.0, 0.5) = 0.12; 
uniform float turbulence_speed : hint_range(0.0, 10.0) = 1.5; 
uniform vec2 noise_scale = vec2(1.0, 1.0); 

uniform sampler2D noise_tex : repeat_enable, filter_linear_mipmap; 

// VERTEX SHADER
void vertex() {
    float rad = radians(y_rotation);
    float c = cos(rad);

    VERTEX.x = VERTEX.x * c;
}

// FRAGMENT SHADER
void fragment() {

    vec2 center_uv = (UV - vec2(0.5)) * zoom;
    float radius = length(center_uv);

    // ANGLE AND SPIN HERE
    float angle = atan(center_uv.y, center_uv.x);
    float current_spin = TIME * (spin_speed * global_spin);

    angle += current_spin; 
    float normalized_angle = (angle / (2.0 * PI)) + 0.5;

    vec2 wobble_uv = vec2(
        normalized_angle * float(core_wobble_frequency), 
        TIME * core_wobble_speed * 0.1
    );
    float raw_wobble = textureLod(noise_tex, wobble_uv, 0.0).r;
    float wobble_offset = (raw_wobble - 0.5) * 2.0; 
    float wobbly_radius = radius + (wobble_offset * core_wobble_amount);

    vec2 flame_uv = vec2(
        normalized_angle * noise_scale.x, 
        (wobbly_radius * noise_scale.y) - (TIME * turbulence_speed * (global_spin * 1.5)* 0.1)
    );
    float raw_flame_noise = textureLod(noise_tex, flame_uv, 0.0).r;

    float final_distorted_radius = wobbly_radius - (raw_flame_noise * flame_spread);
    float final_alpha = smoothstep(base_radius + edge_softness, base_radius - edge_softness, final_distorted_radius);

    vec2 inner_noise_uv = (center_uv * texture_noise_scale) - vec2(TIME * texture_noise_speed * 0.1);
    
    float noise_x = textureLod(inner_noise_tex, inner_noise_uv, 0.0).r;
    float noise_y = textureLod(inner_noise_tex, inner_noise_uv + vec2(0.5), 0.0).r;

    vec2 noise_displacement = (vec2(noise_x, noise_y) - 0.5) * texture_warp_amount;

    float c = cos(-current_spin);
    float s = sin(-current_spin);
    mat2 rotation_matrix = mat2(vec2(c, -s), vec2(s, c));
    
    vec2 warped_rotated_uv = rotation_matrix * (center_uv - noise_displacement);
    vec2 tex_uv = ((warped_rotated_uv / zoom) / texture_scale) + vec2(0.5);
    vec4 tex_color = texture(base_texture, tex_uv);

    tex_color *= COLOR;
    tex_color.rgb *= custom_base_tint.rgb;
    tex_color.rgb = mix(tex_color.rgb, custom_base_tint.rgb, 0.0);

    COLOR.rgb = tex_color.rgb;
    COLOR.a = tex_color.a * final_alpha * overall_alpha;
}