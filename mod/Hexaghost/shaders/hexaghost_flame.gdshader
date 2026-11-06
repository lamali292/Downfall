shader_type canvas_item;
render_mode blend_mix;

uniform float HueShift : hint_range(0.0, 1.0) = 0.0;
uniform vec4 OuterColor : source_color = vec4(1.000000, 0.000000, 0.000000, 1.000000);
const vec2 InnerColorStep = vec2(0.435, 1.285);
const float Noise2Strength = 0.815;
const vec2 Noise2Scaling = vec2(0.645, 0.365);
const vec2 Noise2Panning = vec2(.2, .5);
uniform sampler2D Noise2Texture : repeat_enable;
const float Noise1Strength = 1.02;
const vec2 Noise1Scaling = vec2(1.000000, 1.000000);
const vec2 Noise1Panning = vec2(1.000000, 1.000000);
uniform sampler2D Noise1Texture : repeat_enable;
const bool InvertNoiseMask = true;
const vec2 NoiseMaskScale = vec2(1.000000, 1.000000);
const vec2 NoiseMaskOffset = vec2(0.0, 0.0);
uniform sampler2D NoiseMask : repeat_disable;
const vec2 Distortion2Scale = vec2(0.88, 0.585);
const vec2 Distortion2Panning = vec2(1.000000, 0.67);
uniform sampler2D Distortion2Texture : repeat_enable;
const float Distortion2Strength = 0.065;
const vec2 Distortion1Scale = vec2(1.000000, 1.000000);
const vec2 Distortion1Panning = vec2(1.06, 1.41);
uniform sampler2D Distortion1Texture : repeat_enable;
const float Distortion1Strength = 0.085;
const vec2 DistortionMaskScale = vec2(1.000000, 1.185);
const vec2 DistortionMaskOffset = vec2(0.0, -0.15);
uniform sampler2D DistortionMask : repeat_disable;
uniform vec4 InnerColor : source_color = vec4(1.000000, 0.850000, 0.000000, 1.000000);
const vec2 OuterStep = vec2(0.075, 0.26);

vec3 rgb2hsv(vec3 c) {
    vec4 K = vec4(0.0, -0.333333, 0.666666, -1.0);
    vec4 p = mix(vec4(c.bg, K.wz), vec4(c.gb, K.xy), step(c.b, c.g));
    vec4 q = mix(vec4(p.xyw, c.r), vec4(c.r, p.yzx), step(p.x, c.r));
    float d = q.x - min(q.w, q.y);
    float e = 1.0e-10;
    return vec3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}

vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 0.666666, 0.333333, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

void fragment() {
// ColorParameter:18
	vec4 n_out18p0 = OuterColor;

// Vector2Parameter:21
	vec2 n_out21p0 = InnerColorStep;

// VectorDecompose:22
	float n_out22p0 = n_out21p0.x;
	float n_out22p1 = n_out21p0.y;

// FloatParameter:72
	float n_out72p0 = Noise2Strength;

// Vector2Parameter:85
	vec2 n_out85p0 = Noise2Scaling;

// UVFunc:86
	vec2 n_in86p2 = vec2(0.00000, 0.00000);
	vec2 n_out86p0 = (UV - n_in86p2) * n_out85p0 + n_in86p2;

// Vector2Parameter:48
	vec2 n_out48p0 = Noise2Panning;

// Input:50
	float n_out50p0 = TIME;

// VectorOp:49
	vec2 n_out49p0 = n_out48p0 * vec2(n_out50p0);


// UVFunc:52
	vec2 n_in52p1 = vec2(1.00000, 1.00000);
	vec2 n_out52p0 = n_out49p0 * n_in52p1 + n_out86p0;


	vec4 n_out51p0;
// Texture2D:51
	n_out51p0 = texture(Noise2Texture, n_out52p0);
	float n_out51p3 = n_out51p0.b;


// FloatOp:73
	float n_out73p0 = n_out72p0 * n_out51p3;


// FloatParameter:67
	float n_out67p0 = Noise1Strength;


// Vector2Parameter:84
	vec2 n_out84p0 = Noise1Scaling;


// UVFunc:83
	vec2 n_in83p2 = vec2(0.00000, 0.00000);
	vec2 n_out83p0 = (UV - n_in83p2) * n_out84p0 + n_in83p2;


// Vector2Parameter:43
	vec2 n_out43p0 = Noise1Panning;


// Input:46
	float n_out46p0 = TIME;


// VectorOp:45
	vec2 n_out45p0 = n_out43p0 * vec2(n_out46p0);


// UVFunc:37
	vec2 n_in37p1 = vec2(1.00000, 1.00000);
	vec2 n_out37p0 = n_out45p0 * n_in37p1 + n_out83p0;


	vec4 n_out42p0;
// Texture2D:42
	n_out42p0 = texture(Noise1Texture, n_out37p0);
	float n_out42p3 = n_out42p0.b;


// FloatOp:71
	float n_out71p0 = n_out67p0 * n_out42p3;


// FloatOp:69
	float n_out69p0 = n_out73p0 * n_out71p0;


// BooleanParameter:60
	bool n_out60p0 = InvertNoiseMask;


// Vector2Parameter:81
	vec2 n_out81p0 = NoiseMaskScale;


// UVFunc:82
	vec2 n_in82p2 = vec2(0.00000, 0.04000);
	vec2 n_out82p0 = (UV - n_in82p2) * n_out81p0 + n_in82p2;


// Vector2Parameter:57
	vec2 n_out57p0 = NoiseMaskOffset;


// UVFunc:56
	vec2 n_in56p1 = vec2(1.00000, 1.00000);
	vec2 n_out56p0 = n_out57p0 * n_in56p1 + n_out82p0;


	vec4 n_out55p0;
// Texture2D:55
	n_out55p0 = texture(NoiseMask, n_out56p0);
	float n_out55p3 = n_out55p0.b;


// FloatFunc:66
	float n_out66p0 = 1.0 - n_out55p3;


	float n_out65p0;
// Switch:65
	n_out65p0 = mix(n_out55p3, n_out66p0, float(n_out60p0));


// FloatOp:68
	float n_out68p0 = n_out69p0 + n_out65p0;


// Input:14
	vec2 n_out14p0 = UV;


// Vector2Parameter:74
	vec2 n_out74p0 = Distortion2Scale;


// UVFunc:75
	vec2 n_in75p2 = vec2(0.00000, 0.00000);
	vec2 n_out75p0 = (UV - n_in75p2) * n_out74p0 + n_in75p2;


// Vector2Parameter:35
	vec2 n_out35p0 = Distortion2Panning;


// Input:17
	float n_out17p0 = TIME;


// VectorOp:36
	vec2 n_out36p0 = n_out35p0 * vec2(n_out17p0);


// UVFunc:44
	vec2 n_in44p1 = vec2(1.00000, 1.00000);
	vec2 n_out44p0 = n_out36p0 * n_in44p1 + n_out75p0;


	vec4 n_out34p0;
// Texture2D:34
	n_out34p0 = texture(Distortion2Texture, n_out44p0);


// FloatParameter:38
	float n_out38p0 = Distortion2Strength;


// VectorOp:39
	vec2 n_out39p0 = vec2(n_out34p0.xy) * vec2(n_out38p0);


// Vector2Parameter:77
	vec2 n_out77p0 = Distortion1Scale;


// UVFunc:78
	vec2 n_in78p2 = vec2(0.00000, 0.00000);
	vec2 n_out78p0 = (UV - n_in78p2) * n_out77p0 + n_in78p2;


// Vector2Parameter:10
	vec2 n_out10p0 = Distortion1Panning;


// VectorOp:9
	vec2 n_out9p0 = vec2(n_out17p0) * n_out10p0;


// UVFunc:4
	vec2 n_in4p1 = vec2(1.00000, 1.00000);
	vec2 n_out4p0 = n_out9p0 * n_in4p1 + n_out78p0;


	vec4 n_out12p0;
// Texture2D:12
	n_out12p0 = texture(Distortion1Texture, n_out4p0);


// FloatParameter:5
	float n_out5p0 = Distortion1Strength;


// VectorOp:16
	vec2 n_out16p0 = vec2(n_out12p0.xy) * vec2(n_out5p0);


// VectorOp:40
	vec3 n_out40p0 = vec3(n_out39p0, 0.0) + vec3(n_out16p0, 0.0);


// Vector2Parameter:80
	vec2 n_out80p0 = DistortionMaskScale;


// UVFunc:79
	vec2 n_in79p2 = vec2(0.00000, 0.00000);
	vec2 n_out79p0 = (UV - n_in79p2) * n_out80p0 + n_in79p2;


// Vector2Parameter:32
	vec2 n_out32p0 = DistortionMaskOffset;


// UVFunc:31
	vec2 n_in31p1 = vec2(1.00000, 1.00000);
	vec2 n_out31p0 = n_out32p0 * n_in31p1 + n_out79p0;


	vec4 n_out29p0;
// Texture2D:29
	n_out29p0 = texture(DistortionMask, n_out31p0);


// VectorOp:30
	vec3 n_out30p0 = n_out40p0 * vec3(n_out29p0.xyz);


// VectorOp:15
	vec2 n_out15p0 = n_out14p0 + vec2(n_out30p0.xy);


// UVFunc:13
	vec2 n_in13p1 = vec2(1.00000, 1.00000);
	vec2 n_in13p2 = vec2(0.00000, 0.00000);
	vec2 n_out13p0 = n_in13p2 * n_in13p1 + n_out15p0;


	vec4 n_out3p0;
// Texture2D:3
	n_out3p0 = texture(TEXTURE, n_out13p0);
	float n_out3p3 = n_out3p0.b;


// FloatOp:70
	float n_out70p0 = n_out68p0 * n_out3p3;


// SmoothStep:20
	vec3 n_out20p0 = smoothstep(vec3(n_out22p0), vec3(n_out22p1), vec3(n_out70p0));


// ColorParameter:23
	vec4 n_out23p0 = InnerColor;


// VectorOp:24
	vec3 n_out24p0 = n_out20p0 * vec3(n_out23p0.xyz);


// VectorOp:19
	vec3 n_out19p0 = vec3(n_out18p0.xyz) + n_out24p0;


// Vector2Parameter:25
	vec2 n_out25p0 = OuterStep;


// VectorDecompose:26
	float n_out26p0 = n_out25p0.x;
	float n_out26p1 = n_out25p0.y;


// SmoothStep:27
	float n_out27p0 = smoothstep(n_out26p0, n_out26p1, n_out70p0);


// OUTPUT WITH HUE SHIFT
    vec3 hsv_color = rgb2hsv(n_out19p0);
    hsv_color.x = fract(hsv_color.x + HueShift);
    COLOR.rgb = hsv2rgb(hsv_color);
    COLOR.a = n_out27p0;


}
