shader_type canvas_item;

uniform sampler2D noise_tex : repeat_enable;
uniform sampler2D gradient_tex;

void fragment() {
   	float scrollOffset1 = texture(noise_tex, SCREEN_UV * 0.1 + TIME * 0.0005 + 0.24).r;
	float col1 = texture(noise_tex, SCREEN_UV + TIME * 0.0005 + scrollOffset1).r;
    
   	float scrollOffset2 = texture(noise_tex, SCREEN_UV * 0.1 + TIME * 0.0005 + 0.24).r;
	float col2 = texture(noise_tex, SCREEN_UV + TIME * -0.0007 + scrollOffset2).r;
    
    float final_val = col1 * 0.75 + col2 * 0.35;
    final_val = smoothstep(0.4, 0.6, final_val);
    
    COLOR.rgb = texture(gradient_tex, vec2(final_val,0.0)).rgb;
}
