Shader "Custom/UI/ykHud_Lerp_Add_+100" {
Properties {
 _StOneCol ("Start WhiteColor", Color) = (1,1,1,1)
 _StZeroCol ("Start BlackColor", Color) = (0,0,0,1)
 _EdOneCol ("End WhiteColor", Color) = (1,1,1,1)
 _EdZeroCol ("End BlackColor", Color) = (0,0,0,1)
 _MainTex ("Base (RGB) Transparency (A)", 2D) = "" {}
 _ScrollingSpeed ("UVScrollSpeed", Vector) = (0,0,0,0)
}
SubShader { 
 LOD 100
 Tags { "QUEUE"="Transparent+100" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent+100" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
  ZTest Always
  ZWrite Off
  Blend SrcAlpha One

  CGPROGRAM
    #pragma vertex vert
    #pragma fragment frag

    float4 _StOneCol;
    float4 _EdOneCol;
    float4 _StZeroCol;
    float4 _EdZeroCol;
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
      o.texcoord = (v.texcoord.xy + frac((_ScrollingSpeed.xy * _Time.y)));

      return o;
    }

    fixed4 frag(v2f f) : COLOR
    {
      fixed4 tmpvar_2 = tex2D(_MainTex, f.texcoord);
      fixed4 tmpvar_3 = (((tmpvar_2.x + tmpvar_2.y) + tmpvar_2.z) / 3.0);
      float4 tmpvar = float4(lerp(lerp(_EdZeroCol, _StZeroCol, f.color.xxxx), lerp(_EdOneCol, _StOneCol, f.color.yyyy), tmpvar_3).xyz, (f.color.w * tmpvar_2.w));
      return tmpvar;
    }
  ENDCG
 }
}
}
