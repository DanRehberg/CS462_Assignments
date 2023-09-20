Shader "Unlit/Point"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _LightPos("Light Position", Vector) = (0.5, -1.0, 0.0, 0.0)
        _LightColor("Light Color", Vector) = (1.0, 1.0, 1.0, 0.0)
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 100

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

                struct appdata
                {
                    float4 vertex : POSITION;
                    float3 normal : NORMAL;
                    float4 tangent : TANGENT;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                    float3 vertGlobal : COLOR0;
                    float3 norm : COLOR1;
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;
                float4 _LightPos;
                float4 _LightColor;

                v2f vert(appdata v)
                {
                    v2f o;
                    float4x4 model = UNITY_MATRIX_M;
                    float4x4 view = UNITY_MATRIX_V;
                    float4x4 projection = UNITY_MATRIX_P;

                    //TASK 1: transform the vertex to clip space using
                    //  the model view and projection matrices
                    o.vertex = ;


                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    o.vertGlobal =
                        mul(UNITY_MATRIX_M,
                            float4(v.vertex.x, v.vertex.y, v.vertex.z, 1.0)).xyz;
                    o.norm =
                        mul(UNITY_MATRIX_M,
                            float4(v.normal.x, v.normal.y, v.normal.z, 0.0)).xyz;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    //TASK 2: Compute the vector between _LightPos.xyz and i.vertGlobal
                    //  Make sure the correct direction is computed such that the vector
                    //      represents the path from i.vertGlobal to _LightPos.xyz
                    float3 lightDir = ;


                    float sqrMag = dot(lightDir, lightDir);
                    float dist = sqrt(sqrMag);
                    lightDir /= dist;
                    float attenuation = 1.0 / (1.0 + 0.1 * dist + 0.1 * sqrMag);
                    float ambient = 0.1;
                    // Ensure the 4th component of the light color is always 1.0
                    float4 lightColor = _LightColor;
                    lightColor.w = 1.0;
                    // Sample the albedo texture
                    float4 sampled = tex2D(_MainTex, i.uv) * lightColor;
                    float3 norms = normalize(i.norm);

                    //TASK 3: Compute the Lambertian Reflectance coefficient 
                    //  using the lightDir and norms vectors.
                    //  Be sure to clamp the result between [0.0, 1.0]
                    float lambert = ;


                    fixed4 col = attenuation * (sampled * ambient + (sampled * lambert));
                    return col;
                }
                ENDCG
            }
        }
}
