Shader "Unlit/SpotTBN"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _NormTex("Normal Texture", 2D) = "green" {}
        _LightPos("Light Position", Vector) = (0.5, -1.0, 0.0, 0.0)
        _LightDir("Light Direction", Vector) = (0.0, -1.0, 0.0, 0.0)
        _ConeA("Cone Inner Range", Range(0.01, 0.8)) = 0.5
        _ConeB("Cone Outer Range", Range(0.01, 0.9)) = 0.7
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
                    float3 vertGlobal : TRXCOORD4;
                    float3 T : TEXCOORD1;
                    float3 B : TEXCOORD2;
                    float3 N : TEXCOORD3;
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;
                sampler2D _NormTex;
                float4 _LightPos;
                float4 _LightDir;
                float _ConeA;
                float _ConeB;
                float4 _LightColor;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    o.vertGlobal =
                        mul(UNITY_MATRIX_M,
                            float4(v.vertex.x, v.vertex.y, v.vertex.z, 1.0)).xyz;
                    
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
                    //OLD TASK 2: Compute the vector between _LightPos.xyz and i.vertGlobal
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

                    float3 spotDir = -normalize(_LightDir.xyz);

                    //OLD TASK 3: Complete the dot product belowe to determine if
                    //  the direction from the current pixel to the light is within the
                    //      cone about the spot light's direction axis.
                    //  This will use lightDir and spotDir vectors.
                    float coneTest = 1.0 - dot();


                    float intensity = 0.0;
                    if (coneTest < _ConeA)
                    {
                        float edgeRange = _ConeA - _ConeB;
                        intensity = clamp((coneTest - _ConeA) / edgeRange, 0.5, 1.0);
                    }
                    else if (coneTest < _ConeB)
                    {
                        float edgeRange = _ConeB - 1.0;
                        intensity = clamp((coneTest - _ConeB) / edgeRange, 0.0, 0.5);
                    }
                    float outterBuffer = max(_ConeA, _ConeB);

                    float3 norms = normalize(2.0 * tex2D(_NormTex, i.uv).rgb - 1.0);

                    float3x3 TBN = transpose(
                        float3x3(normalize(i.T),
                            normalize(i.B),
                            normalize(i.N)));

                    //NEW TASK 2: Transform your normals (norms vector) by the TBN matrix
                    norms = ;

                    //OLD TASK 4: Compute the Lambertian Reflectance coefficient 
                    //  using the lightDir and norms vectors.
                    //  Be sure to clamp the result between [0.0, 1.0]
                    float lambert = ;


                    fixed4 col = attenuation * (sampled * ambient + sampled * intensity * lambert);
                    return col;
                }
                ENDCG
            }
        }
}
