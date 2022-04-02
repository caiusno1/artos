/*========================================================================
Copyright (c) 2017-2021 PTC Inc. All Rights Reserved.

Vuforia is a trademark of PTC Inc., registered in the United States and other
countries.
=========================================================================*/
Shader "Vuforia/VFX/Unlit/RadialFade" {
    Properties {
        _Color("Base Color", Color) = (1,1,1,1)
        _MainTex("Base (RGBA)", 2D) = "white" {}
        _Radius("Radius", Range(0, 1)) = 1
    }

    SubShader {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }

        Pass {
          ZWrite Off
          Cull Off
          Lighting Off
          Blend SrcAlpha OneMinusSrcAlpha

          CGPROGRAM
          
          #pragma vertex vert
          #pragma fragment frag

          #include "UnityCG.cginc"

          sampler2D _MainTex;
          float4 _Color;
          float _Radius;
          
          struct v2f {
              float4  pos : SV_POSITION;
              float2  uv : TEXCOORD0;
          };

          float4 _MainTex_ST;

          v2f vert(appdata_base v)
          {
               v2f o;
               o.pos = UnityObjectToClipPos(v.vertex);
               o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
               return o;
          }

          half4 frag(v2f i) : COLOR
          {
              float radius = 2.0 * length(i.uv - float2(0.5, 0.5));
              float ru = clamp(radius / _Radius, 0.0, 1.0);
              float alpha = smoothstep(1.0, 0.0, ru);
              half4 color = _Color * tex2D(_MainTex, i.uv);
              color.a *= alpha;
              return color;
          }

          ENDCG
        }
    }

    FallBack "Diffuse"
}

