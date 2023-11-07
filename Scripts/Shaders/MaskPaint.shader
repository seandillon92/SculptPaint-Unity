

Shader "Unlit/MaskPaint"
{
   Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        ZTest Off
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            #pragma enable_d3d11_debug_symbols

            #define SQRT_1_DIV_2 0.70710678118
            #define DEG2RAD 0.0174533

            #include "UnityCG.cginc"

            uniform float3 position;
            uniform float radius;
            uniform sampler2D _MainTex;
            uniform float aspect;
            uniform float3 scale;

            uniform float3 tangent;
            uniform float3 bitangent;
            uniform float3 normal;

            uniform float3 forward;

            uniform float rotation;

            // 0 - tangent local
            // 1 - tangent global
            uniform int space;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal: NORMAL;
            };

            struct v2f
            {
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 position : POSITION1;
                float3x3 transform : POSITION2;
            };

            float3 projectPlane(float3 v, float3 n){
                return v - n * dot(v, n) / dot(n, n);
            }

            float3x3 rotateMatrix(float angle, float3 axis) {
                float c, s;
                sincos(angle, s, c);

                float t = 1 - c;
                float x = axis.x;
                float y = axis.y;
                float z = axis.z;

                return float3x3(
                    t * x * x + c, t * x * y - s * z, t * x * z + s * y,
                    t * x * y + s * z, t * y * y + c, t * y * z - s * x,
                    t * x * z - s * y, t * y * z + s * x, t * z * z + c
                );
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = float4(v.uv.x *2.0f - 1.0f, (1 - v.uv.y) * 2.0f - 1.0f, 0.0f, 1.0f);
                o.position = v.vertex;

                if (space == 0) {
                    
                    float3 tangent = normalize(projectPlane(forward, normal));
                    tangent = mul(rotateMatrix(rotation * DEG2RAD, normal), tangent);
                    float3 bitangent = normalize(cross(v.normal, tangent));
                    
                    o.transform = float3x3(tangent, bitangent, v.normal);
                }
                else if (space == 1) 
                { 
                    o.transform = float3x3(tangent, bitangent, normal);
                }

                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed frag (v2f i) : SV_Target
            {

                float3 difference = (position - i.position) * scale.x * radius;

                if ( length(difference) > SQRT_1_DIV_2){
                    return 0.0f;
                }

                float3 projected = mul(i.transform, difference);
                float2 uv = projected.xy;
                uv.y *= aspect;
                uv += float2(0.5, 0.5);

                if ( uv.x > 1 || uv.y > 1 || uv.x < 0 || uv.y < 0)
                { 
                    return 0.0f;
                }

                float4 texel = tex2D(_MainTex, uv);
                return texel.a;
            }
            ENDCG
        }
    }
}
