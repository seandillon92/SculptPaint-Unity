

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

            #include "UnityCG.cginc"

            uniform float3 position;
            uniform float radius;
            uniform sampler2D _MainTex;
            uniform float3 tangent;
            uniform float3 bitangent;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 worldPosition : POSITION1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = float4(v.uv.x *2.0f - 1.0f, (1 - v.uv.y) * 2.0f - 1.0f, 0.0f, 1.0f);
                o.worldPosition = mul(unity_ObjectToWorld, v.vertex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            float3 project(float3 a, float3 b){
                return b * (dot(a, b) / dot(b,b));
            }

            fixed frag (v2f i) : SV_Target
            {
                float3 diff = (position - i.worldPosition) * radius;
                float3 tangentProj = project(diff, tangent);
                float3 bitangentProj = project(diff, bitangent);
                float2 uv = float2(-dot(tangent, tangentProj), -dot(bitangent, bitangentProj)) + float2(0.5,0.5);
                if( uv.x > 1 || uv.y > 1 || uv.x < 0 || uv.y < 0)
                { 
                    return 0.0f;
                }
                return tex2D(_MainTex, uv).a;
            }
            ENDCG
        }
    }
}
