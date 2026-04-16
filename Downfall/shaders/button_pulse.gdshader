shader_type canvas_item;

uniform float pulse_strength;

void fragment() {
	COLOR.rgb += abs(sin(TIME * 2.0)) * 0.2 * pulse_strength;
}