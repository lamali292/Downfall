shader_type canvas_item;

uniform sampler2D emission_mask;
uniform sampler2D noise_texture : repeat_enable; 

uniform vec4 emission_color : source_color = vec4(0.2, 0.8, 1.0, 1.0);
uniform float emission_strength : hint_range(0.0, 10.0) = 1.0;

uniform float pulse_speed : hint_range(0.0, 10.0) = 3.0;
uniform float pulse_depth : hint_range(0.0, 1.0) = 0.5; 

uniform vec2 noise_scale = vec2(3.0, 3.0); 
uniform float phase_spread : hint_range(0.0, 15.0) = 6.28318; 

uniform float glow_radius : hint_range(0.0, 30.0) = 12.0; 
uniform float glow_intensity : hint_range(0.0, 5.0) = 2.0;
uniform float white_core_threshold : hint_range(0.0, 5.0) = 1.2;

// ALPHA
uniform float global_alpha : hint_range(0.0, 1.0) = 1.0;

void fragment() {
    vec4 base_texture = texture(TEXTURE, UV);
    float core_mask = texture(emission_mask, UV).r;
    
    float noise_val = texture(noise_texture, UV * noise_scale).r;
    
    float pulse = (sin(TIME * pulse_speed + (noise_val * phase_spread)) * 0.5) + 0.5;
    float local_multiplier = mix(1.0 - pulse_depth, 1.0, pulse);
    
    float dynamic_strength = emission_strength * local_multiplier;
    
    float blur_mask = 0.0;
    float total_weight = 0.0;
    
    for (float angle = 0.0; angle < 6.28318; angle += 0.78539) {
        vec2 direction = vec2(cos(angle), sin(angle));
        for (float step = 1.0; step <= 4.0; step++) {
            float distance = step * (glow_radius / 4.0);
            float weight = 1.0 / (step * step); 
            
            vec2 offset = direction * distance * TEXTURE_PIXEL_SIZE;
            blur_mask += texture(emission_mask, UV + offset).r * weight;
            total_weight += weight;
        }
    }
    
    blur_mask = blur_mask / total_weight;
    
    float total_glow = (core_mask * dynamic_strength) + (blur_mask * glow_intensity * dynamic_strength);
    
    vec3 color_layer = emission_color.rgb * clamp(total_glow, 0.0, 1.0);
    vec3 screen_blend = 1.0 - (1.0 - base_texture.rgb) * (1.0 - color_layer);
    
    float white_core = max(0.0, total_glow - white_core_threshold);
    vec3 final_rgb = screen_blend + vec3(white_core);
    
    // FINAL COLOR
    COLOR = vec4(final_rgb, base_texture.a * global_alpha);
}