Shader "Custom/UI/ykHud_dau_+100" {
Properties {
 _MainTex ("Base (RGB) Transparency (A)", 2D) = "" {}
 _ScrollingSpeed ("UVScrollSpeed", Vector) = (0,0,0,0)
}
SubShader { 
 LOD 100
 Tags { "QUEUE"="Transparent+1000" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent+1000" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
  ZTest Always
  ZWrite Off
  Blend SrcAlpha OneMinusSrcAlpha

  CGPROGRAM
    #pragma vertex vert
    #pragma fragment frag

    sampler2D _MainTex;
    float4 _ScrollingSpeed;

    struct appdata_t
    {
      float4 vertex : POSITION;
      float4 color : COLOR;
      half2 texcoord : TEXCOORD0;
    };

    struct v2f
    {
      float4 vertex : POSITION;
      float4 color : COLOR;
      half2 texcoord : TEXCOORD0;
    };

    v2f vert(appdata_t v)
    {
      v2f o;

      o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
      o.color = v.color;
      o.texcoord = (v.texcoord.xy + frac((_ScrollingSpeed * _Time.y)).xy);

      return o;
    }

    fixed4 frag(v2f f) : COLOR
    {
      fixed4 c;
      fixed4 tmpvar_2;
      tmpvar_2 = tex2D(_MainTex, f.texcoord);
      float4 tmpvar_3;
      tmpvar_3 = (tmpvar_2 * f.color);
      c = tmpvar_3;
      return c;
    }
  ENDCG
 }
}
}
