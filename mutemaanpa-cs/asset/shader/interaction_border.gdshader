shader_type spatial;
render_mode cull_front;

uniform float outline_thickness = 0.02;

void vertex() {
    VERTEX += NORMAL * outline_thickness;
}

void fragment() {
    ALBEDO = vec3(0.0);
}

void light() {
    DIFFUSE_LIGHT = vec3(0.0);
}
