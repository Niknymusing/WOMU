Shader "CustomRenderTexture/GridShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _Speed("Speed", float) = 0.5
        _Frequency("Frequency", float) = 0.5
        _Width("Width", float) = 0.1
        
     }

     SubShader
     {
        Blend One Zero

        Pass
        {
            Name "GridShader"

            CGPROGRAM
            #include "UnityCustomRenderTexture.cginc"
            #pragma vertex CustomRenderTextureVertexShader
            #pragma fragment frag
            #pragma target 3.0

            float4      _Color;
            float _Speed;
            float _Frequency;
            float _Width;

            float4 frag(v2f_customrendertexture IN) : COLOR
            {
                float2 uv = IN.globalTexcoord.xy;
                float gradValue =(((_Time.x * _Speed) + uv.y) * _Frequency % 1.0f);
                float brightness = smoothstep(0.5 - _Width, 0.5, gradValue) - smoothstep(0.5, 0.5 + _Width, gradValue);
                float4 color =  brightness * _Color;

                // TODO: Replace this by actual code!
                // uint2 p = uv.xy * 256;
                // return countbits(~(p.x & p.y) + 1) % 2 * float4(uv, 1, 1) * color;

                return color;
            }
            ENDCG
        }
    }
}
