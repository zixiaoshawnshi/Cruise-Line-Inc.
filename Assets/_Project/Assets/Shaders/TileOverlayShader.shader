Shader "CruiseLineInc/TileOverlay"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _ZoneColor ("Zone Color", Color) = (0.78, 0.78, 0.78, 1)
        _OverlayColor ("Overlay Color", Color) = (0, 0, 0, 0)
        _OverlayIntensity ("Overlay Intensity", Range(0,1)) = 0
        _OverlayBlend ("Overlay Blend", Range(0,1)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 100
        Cull Off
        Lighting Off
        ZWrite On
        ZTest LEqual

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _ZoneColor;
            float4 _OverlayColor;
            float _OverlayIntensity;
            float _OverlayBlend;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv  = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 baseCol = tex2D(_MainTex, i.uv);
                baseCol.rgb *= _ZoneColor.rgb;
                fixed4 overlay = fixed4(_OverlayColor.rgb, _OverlayColor.a * _OverlayIntensity);
                baseCol = lerp(baseCol, overlay, overlay.a * _OverlayBlend);
                baseCol.a = 1.0;
                return baseCol;
            }
            ENDCG
        }
    }
}
