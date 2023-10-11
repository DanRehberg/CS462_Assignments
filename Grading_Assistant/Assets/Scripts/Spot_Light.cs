using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spot_Light : MonoBehaviour
{
    public Material spotLightMaterial = null;
    public Vector4 color;

    private int lightPosID = -1;
    private int lightDirID = -1;
    private int lightColorID = -1;
    private Material lightMat = null;
    private int myColorID = -1;
    // Start is called before the first frame update
    void Start()
    {
        color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
        lightMat = this.GetComponent<Renderer>().material;
        myColorID = lightMat.shader.FindPropertyIndex("_Color");
        myColorID = lightMat.shader.GetPropertyNameId(myColorID);
        if (spotLightMaterial != null)
        {
            lightPosID = spotLightMaterial.shader.FindPropertyIndex("_LightPos");
            lightPosID = spotLightMaterial.shader.GetPropertyNameId(lightPosID);
            lightDirID = spotLightMaterial.shader.FindPropertyIndex("_LightDir");
            lightDirID = spotLightMaterial.shader.GetPropertyNameId(lightDirID);
            lightColorID = spotLightMaterial.shader.FindPropertyIndex("_LightColor");
            lightColorID = spotLightMaterial.shader.GetPropertyNameId(lightColorID);
        }
    }

    // Update is called once per frame
    void Update()
    {
        color.w = 1.0f;
        if (myColorID != -1)
        {
            lightMat.SetVector(myColorID, color);
        }
        if (lightPosID != -1 && lightDirID != -1)
        {
            spotLightMaterial.SetVector(lightPosID, new Vector4(transform.position.x,
                transform.position.y, transform.position.z, 0.0f));
            spotLightMaterial.SetVector(lightDirID, new Vector4(transform.forward.x,
                transform.forward.y, transform.forward.z, 0.0f));
        }
        if (lightColorID != -1)
        {
            spotLightMaterial.SetVector(lightColorID, color);
        }
    }
}
