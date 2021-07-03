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
        Cull Back
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

            StructuredBuffer<float4> matrixBuffer;
            StructuredBuffer<int> indexBuffer;
            StructuredBuffer<float4> uvBuffer;
			StructuredBuffer<float4> colorsBuffer;

			RWStructuredBuffer<float4> _DebugBuffer : register(u1);

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
                float4 mat = matrixBuffer[instanceID];
                //float2 position = float2(mat[0], mat[1]);
                //float rotationZ = mat[2];
                //float scale = mat[3];
                //float4 uv = uvBuffer[indexBuffer[instanceID]];
                ////rotate the vertex
                //v.vertex = mul(v.vertex-float4(0.5,0.5,0,0),rotationZMatrix(rotationZ));
                ////scale it
                //float randomZ = -position.y/10;
                float4 worldPosition = v.vertex * mat * UNITY_MATRIX_VP; //float3(position,randomZ) + v.vertex.xyz * scale;
               
                v2f o;
                o.pos = UnityObjectToClipPos(worldPosition); //float4(worldPosition, 1.0f));
                o.uv =  v.texcoord * uv.xy + uv.zw;
				o.color = colorsBuffer[instanceID];
                
                _DebugBuffer[0] = float4(position, rotationZ, 42);
                _DebugBuffer[1] = float4(scale, 42, 42, 42);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target{
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;
				clip(col.a - 1.0 / 255.0);
                col.rgb *= col.a;

                //buffer[0] = float4(.3, .3, 0, 0);


				return col;
            }

            ENDHLSL
        }
    }
}