Shader "Unlit/Dir"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _LightDir("Light Direction", Vector) = (0.5, -1.0, 0.0, 0.0)
        _LightColor("Light Color", Vector) = (1.0, 1.0, 1.0, 1.0)
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
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                    float4 norm : COLOR0;
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;
                float4 _LightDir;
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
                    o.norm =
                        mul(UNITY_MATRIX_M,
                            float4(v.normal.x, v.normal.y, v.normal.z, 0.0));
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed3 lightDir = normalize(-_LightDir.xyz);
                    float ambient = 0.1;
                    // Ensure the 4th component of the light color is always 1.0
                    float4 lightColor = _LightColor;
                    lightColor.w = 1.0;
                    // Sample the albedo texture
                    float4 sampled = tex2D(_MainTex, i.uv) * lightColor;
                    float3 norms = i.norm;

                    //TASK 2: Compute the Lambertian Reflectance coefficient 
                    //  using the lightDir and norms vectors.
                    //  Be sure to clamp the result between [0.0, 1.0]
                    float lambert = ;


                    fixed4 col = sampled * ambient + sampled * lambert;
                    return col;
                }
                ENDCG
            }
        }
}
