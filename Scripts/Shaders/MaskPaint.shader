

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

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 worldPosition : POSITION1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = float4(v.uv.x *2.0f - 1.0f, (1 - v.uv.y) * 2.0f - 1.0f, 0.0f, 1.0f);
                o.uv = v.uv;
                o.worldPosition = mul(unity_ObjectToWorld, v.vertex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed frag (v2f i) : SV_Target
            {
                float dist = distance(position, i.worldPosition) * radius;
                return saturate(1 - dist);
            }
            ENDCG
        }
    }
}
