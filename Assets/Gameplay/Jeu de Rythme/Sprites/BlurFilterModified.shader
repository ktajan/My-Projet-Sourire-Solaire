Shader "Custom/Lisa_Naninovel_Final"
{
    Properties
    {
        [HideInInspector] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Intensity ("Blur Intensity", Range(0, 10)) = 1.0
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        
        // Indispensable pour l'UI
        ZWrite Off Cull Off Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes { float4 positionOS : POSITION; };
            struct Varyings { float4 positionCS : SV_POSITION; float4 screenPos : TEXCOORD0; };

            // On utilise la texture de capture d'URP
            TEXTURE2D(_CameraOpaqueTexture);
            SAMPLER(sampler_CameraOpaqueTexture);
            float4 _CameraOpaqueTexture_TexelSize;
            float _Intensity;

            Varyings vert (Attributes input) {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.screenPos = ComputeScreenPos(output.positionCS);
                return output;
            }

            // --- REPRISE DE TES CONSTANTES NANINOVEL [cite: 6, 7] ---
            half4 frag (Varyings input) : SV_Target {
                float2 uv = input.screenPos.xy / input.screenPos.w;
                
                const float devStep1 = 1.3846153846; [cite: 5]
                const float devStep2 = 3.2307692308; [cite: 6]
                const float bStep1 = 0.2270270270; [cite: 6]
                const float bStep2 = 0.3162162162; [cite: 6]
                const float bStep3 = 0.0702702703; [cite: 6]

                float2 stride = _CameraOpaqueTexture_TexelSize.xy * _Intensity; [cite: 7]
                
                // On fusionne les passes Horizontale et Verticale [cite: 11, 12]
                half4 color = SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, uv) * bStep1; [cite: 8]
                
                float2 d1 = stride * devStep1;
                float2 d2 = stride * devStep2;

                // Mix Horizontal + Vertical en une seule passe pour l'UI
                color += SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, uv + float2(d1.x, 0)) * bStep2; [cite: 8]
                color += SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, uv - float2(d1.x, 0)) * bStep2; [cite: 9]
                color += SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, uv + float2(0, d1.y)) * bStep2;
                color += SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, uv - float2(0, d1.y)) * bStep2;

                color += SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, uv + float2(d2.x, 0)) * bStep3; [cite: 9]
                color += SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, uv - float2(d2.x, 0)) * bStep3; [cite: 10]
                color += SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, uv + float2(0, d2.y)) * bStep3;
                color += SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, uv - float2(0, d2.y)) * bStep3;

                return color / 2.0; 
            }
            ENDHLSL
        }
    }
}