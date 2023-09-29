Shader"Unlit/Brush"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            #pragma enable_d3d11_debug_symbols

            #include "UnityCG.cginc"
            
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
                float2 distance : POSITION1;
            };

            uniform float2 mousePos;
            uniform float size;
            uniform float aspect;
            uniform sampler2D _MainTex;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                float4 pos = ComputeScreenPos(o.vertex);
                o.distance = mousePos - pos.xy/pos.w;
                o.distance.x *= aspect;
                o.uv = v.uv;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed3 frag(v2f i) : SV_Target
            {
                float2 coords = i.distance * size + float2(0.5, 0.5);
                float w = 0;
                if (coords.x > 0 && coords.x < 1 && coords.y > 0 && coords.y < 1)
                {
                    w = tex2D(_MainTex, coords).a;
                }
                float2 uv = i.uv * ceil(w);
                return fixed3(uv.xy, w);
            }
            ENDCG
        }
    }
}
