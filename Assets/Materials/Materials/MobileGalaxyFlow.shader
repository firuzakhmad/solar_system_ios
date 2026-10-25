// Built-in Render Pipeline. A square Quad with standard 0..1 UVs.
Shader "Unlit/MobileGalaxyFlow"
{
 Properties
 {
  _MainTex ("Galaxy Color (RGBA)", 2D) = "white" {}
  _FlowMap ("Flow Map (Linear RG)", 2D) = "gray" {}
  [HDR] _Tint ("Tint", Color) = (1,1,1,1)
  _Brightness ("Brightness", Range(0,4)) = 1.15
  _FlowStrength ("Flow Distance (UV)", Range(0,0.12)) = 0.035
  _Speed ("Cycles Per Second (0 = Still)", Range(0,0.5)) = 0.065
  _Direction ("Direction (+1 or -1)", Range(-1,1)) = 1
  _Padding ("Texture Padding / Zoom Out", Range(1,1.5)) = 1.15
  _Occlusion ("Background Occlusion (0 = Additive)", Range(0,1)) = 0.8
  _Opacity ("Overall Fade", Range(0,1)) = 1
 }
 SubShader
 {
  Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
  Cull Off
  ZWrite Off
  ZTest LEqual
  Blend One OneMinusSrcAlpha
  Pass
  {
   CGPROGRAM
   #pragma vertex vert
   #pragma fragment frag
   #pragma target 3.0
   #include "UnityCG.cginc"
   sampler2D _MainTex, _FlowMap;
   half4 _Tint;
   float _FlowStrength, _Speed, _Direction, _Padding;
   half _Brightness, _Opacity, _Occlusion;
   struct appdata { float4 vertex:POSITION; float2 uv:TEXCOORD0; };
   struct v2f { float4 vertex:SV_POSITION; float2 uv:TEXCOORD0; };
   v2f vert(appdata v)
   {
    v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.uv = (v.uv - 0.5) * _Padding + 0.5;
    return o;
   }
   half4 frag(v2f i):SV_Target
   {
    // Maps are numeric data, sRGB MUST be OFF. Byte 128 is exactly zero.
    float2 flow = (tex2D(_FlowMap, i.uv).rg * 255.0 - 128.0) / 127.0;
    flow *= _FlowStrength * _Direction;
    float phase0 = frac(_Time.y * _Speed);
    float phase1 = frac(_Time.y * _Speed + 0.5);
    float weight0 = 1.0 - abs(phase0 * 2.0 - 1.0);
    float2 uv0 = i.uv - flow * (phase0 - 0.5);
    float2 uv1 = i.uv - flow * (phase1 - 0.5);
    half4 a = tex2D(_MainTex, uv0);
    half4 b = tex2D(_MainTex, uv1);
    // Mask outside the source canvas so Clamp never stretches the outer pixels.
    float2 bound0 = min(uv0, 1.0 - uv0);
    float2 bound1 = min(uv1, 1.0 - uv1);
    a.a *= saturate(min(bound0.x, bound0.y) * 100.0);
    b.a *= saturate(min(bound1.x, bound1.y) * 100.0);
    // Premultiply BEFORE interpolating the phases, avoiding colored alpha fringes.
    a.rgb *= a.a;
    b.rgb *= b.a;
    half4 c = lerp(b, a, weight0);
    half fade = _Opacity * _Tint.a;
    return half4(c.rgb * _Tint.rgb * _Brightness * fade,
                 c.a * fade * _Occlusion);
   }
   ENDCG
  }
 }
 Fallback Off
}
