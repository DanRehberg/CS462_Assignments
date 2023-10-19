// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/DirTBN"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _NormTex("Normal Texture", 2D) = "green" {}
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
                    float4 tangent : TANGENT;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                    float3 T : TEXCOORD1;
                    float3 B : TEXCOORD2;
                    float3 N : TEXCOORD3;
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;
                sampler2D _NormTex;
                float4 _LightDir;
                float4 _LightColor;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                    float3x3 rotMat = (float3x3)unity_ObjectToWorld;

                    //NEW TASK 1: Compute the Bitangent (b) vector and rotate it
                    //  and compute the rotated Tangent (t) vector
                    float3 b = ;
                    float3 t = ;
                    float3 n = mul(rotMat, v.normal);

                    o.T = t;
                    o.B = b;
                    o.N = n;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed3 lightDir = normalize(-_LightDir);
                    float ambient = 0.1;
                    // Ensure the 4th component of the light color is 1.0
                    float4 lightColor = _LightColor;
                    lightColor.w = 1.0;
                    // sample the texture
                    float4 sampled = tex2D(_MainTex, i.uv) * lightColor;
                    float3 norms =
                        normalize(2.0 * tex2D(_NormTex, i.uv).rgb - 1.0);
                    
                    float3x3 TBN = 
                        float3x3(normalize(i.T),
                            normalize(i.B),
                            normalize(i.N));

                    //NEW TASK 2: Transform your normals (norms vector) by the TBN matrix
                    norms = ;
                    
                    //OLD TASK 2: Compute the Lambertian Reflectance coefficient 
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
