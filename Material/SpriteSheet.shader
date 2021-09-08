 Shader "Instanced/SpriteSheet" {
    Properties {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
    }
    
    SubShader {
        Tags{
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
        }
        Cull Off
        Lighting Off
        ZWrite On
        Blend One OneMinusSrcAlpha
        Pass {
            //CGPROGRAM
            HLSLPROGRAM
            // Upgrade NOTE: excluded shader from OpenGL ES 2.0 because it uses non-square matrices
            #pragma exclude_renderers gles

            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.5
            #include "UnityCG.cginc"

            sampler2D _MainTex;

            StructuredBuffer<float4x4> matrixBuffer;
            StructuredBuffer<int> indexBuffer;
            StructuredBuffer<float4> uvBuffer;
			StructuredBuffer<float4> colorsBuffer;

			RWStructuredBuffer<float4x4> _DebugBuffer : register(u1);

            struct v2f{
                float4 pos : SV_POSITION;
                float2 uv: TEXCOORD0;
				fixed4 color : COLOR0;
            };

            float4x4 rotationZMatrix(float rZ){
                float angleZ = radians(rZ);
                float c = cos(angleZ);
                float s = sin(angleZ);
                float4x4 ZMatrix  = 
                    float4x4( 
                       c,  -s, 0,  0,
                       s,  c,  0,  0,
                       0,  0,  1,  0,
                       0,  0,  0,  1);
                return ZMatrix;
            }

            v2f vert (appdata_full v, uint instanceID : SV_InstanceID){
                float4x4 model = matrixBuffer[instanceID];
                float4 uv = uvBuffer[indexBuffer[instanceID]];
                // todo: construct full MVP on cpu and pass to shader - using camera.worldToCameraMatrix and projectionMatrix does not seem to work
                float4 worldPosition = mul(model, v.vertex-float4(0.5,0.5,0,0));
                v2f o;
                o.pos = mul(UNITY_MATRIX_VP, worldPosition);
                o.uv =  v.texcoord * uv.xy + uv.zw;
				o.color = colorsBuffer[instanceID];

                //_DebugBuffer[0] = model;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target{
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;
				clip(col.a - 1.0 / 255.0);
                col.rgb *= col.a;

				return col;
            }

            ENDHLSL
        }
    }
}