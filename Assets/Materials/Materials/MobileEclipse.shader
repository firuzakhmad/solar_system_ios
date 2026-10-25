// Built-in Render Pipeline. Apply to a square Quad, not a sphere.
// One pass, zero textures, no scene-color/depth reads, no lighting or Bloom required.
Shader "Unlit/MobileEclipse"
{
    Properties
    {
        [HDR] _RimColor ("Rim - White Gold", Color) = (1, 0.88, 0.58, 1)
        [HDR] _CoronaColor ("Corona - Gold", Color) = (1, 0.48, 0.10, 1)
        [HDR] _HaloColor ("Halo - Warm Amber", Color) = (1, 0.36, 0.075, 1)
        _DiskColor ("Disk Color", Color) = (0, 0, 0, 1)
        _Radius ("Disk Radius", Range(0.15, 0.55)) = 0.33
        _RimWidth ("Bright Rim Width", Range(0.001, 0.025)) = 0.004
        _CoronaLength ("Corona Length", Range(0.03, 0.3)) = 0.14
        _Filaments ("Filament Contrast", Range(0, 2)) = 1.1
        _Intensity ("Brightness", Range(0, 5)) = 1.5
        _HaloStrength ("Outer Halo Strength", Range(0, 1)) = 0.18
        _Speed ("Animation Speed (0 = Still)", Range(0, 1)) = 0.12
        _Opacity ("Overall Fade", Range(0, 1)) = 1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        LOD 100
        Cull Off
        ZWrite Off
        ZTest LEqual
        // RGB emission + disk coverage: black disk still hides the AR camera.
        Blend One OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #include "UnityCG.cginc"

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float4 vertex : SV_POSITION; float2 uv : TEXCOORD0; };
            half4 _RimColor, _CoronaColor, _HaloColor, _DiskColor;
            float _Radius, _RimWidth, _CoronaLength, _Filaments;
            float _Intensity, _HaloStrength, _Speed, _Opacity;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // Full float coordinates keep the hash stable on mobile GPUs.
            float hash21(float2 p)
            {
                float3 q = frac(float3(p.x, p.y, p.x) * 0.1031);
                q += dot(q, q.yzx + 33.33);
                return frac((q.x + q.y) * q.z);
            }

            float noise21(float2 p)
            {
                float2 cell = floor(p);
                float2 f = frac(p);
                f = f * f * (3.0 - 2.0 * f);
                return lerp(lerp(hash21(cell), hash21(cell + float2(1,0)), f.x),
                            lerp(hash21(cell + float2(0,1)), hash21(cell + 1.0), f.x), f.y);
            }

            half4 frag(v2f i) : SV_Target
            {
                float2 p = (i.uv - 0.5) * 2.0;
                float r = length(p);
                float2 direction = p / max(r, 0.0001);
                float d = r - _Radius;
                float aa = max(fwidth(r), 0.0005);
                float disk = 1.0 - smoothstep(-aa, aa, d);
                float outside = 1.0 - disk;
                float x = max(d, 0.0);
                float t = _Time.y * _Speed* 50;

                // Direction-space noise has no polar UV seam or atan2 cost.
                // Slow radial variation stretches the texture into filaments.
                float broad = noise21(direction * 7.0 + float2(t * 0.13, -t * 0.1));
                float strands = noise21(direction * 35.0 + x * 5.0 + float2(t * 0.2, -t));
                float fine = noise21(direction * 95.0 + x * 11.0 + float2(-t * 0.3, t * 0.7));
                // Fade fine strands with pixel footprint to reduce distant shimmer.
                float fineVisibility = 1.0 - smoothstep(0.35, 1.5, aa * 95.0 / max(r, 0.1));
                fine = lerp(0.5, fine, fineVisibility);

                float reach = _CoronaLength * (0.38 + broad * 1.15);
                float falloff = exp2(-x * 5.0 / max(reach, 0.001));
                float threads = saturate((strands * 0.7 + fine * 0.3 - 0.22) * 2.0);
                threads = threads * threads;
                float corona = falloff * (0.12 + threads * _Filaments * 2.5);
                float rim = exp2(-x / max(_RimWidth, aa * 0.55));
                float rootGlow = exp2(-x / 0.018) * 0.32;
                float halo = exp2(-x / max(_CoronaLength * 0.8, 0.001));
                halo *= _HaloStrength * (0.7 + broad * 0.6);

                // A soft radial fade prevents the rectangular Quad edge appearing.
                float edgeFade = 1.0 - smoothstep(0.80, 0.98, r);
                half3 emission = _RimColor.rgb * (rim * 2.0 + rootGlow)
                               + _CoronaColor.rgb * corona
                               + _HaloColor.rgb * halo;
                emission *= outside * edgeFade * _Intensity;
                // Subtle warm inner edge, with a genuinely black central disk.
                float innerRim = exp2(-abs(d) / 0.008) * disk * 0.08;
                half3 rgb = _DiskColor.rgb * disk + emission + _CoronaColor.rgb * innerRim;
                return half4(rgb * _Opacity, disk * _Opacity);
            }
            ENDCG
        }
    }
    Fallback Off
}
