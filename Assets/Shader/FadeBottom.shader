Shader "Unlit/FadeBottom"
{
    Properties
    {
        _MainTex   ("Texture", 2D) = "white" {}
        _Threshold ("Threshold", Range(0,1)) = 1
        _WaveFrequency ("Wave Frequency", Float) = 10
        _WaveAmplitude ("Wave Amplitude", Range(0,0.2)) = 0.05
        _WaveSpeed     ("Wave Speed", Float) = 1
    }
    SubShader
    {
        // 투명 오브젝트로 처리
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f    { float2 uv : TEXCOORD0; float4 pos : SV_POSITION; };

            sampler2D _MainTex;
            float4   _MainTex_ST;
            float    _Threshold;
            float    _WaveFrequency;
            float    _WaveAmplitude;
            float    _WaveSpeed;  

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv  = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                float time = _Time.y * _WaveSpeed;  
                float wave = sin(i.uv.x * _WaveFrequency + time) * _WaveAmplitude;

                // 밑쪽(_Threshold 아래)는 완전 투명
                if (i.uv.y < _Threshold + wave) 
                    col.a = 0;

                return col;
            }
            ENDCG
        }
    }
}
