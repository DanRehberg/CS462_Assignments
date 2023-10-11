using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Direction_Light : MonoBehaviour
{
    public Material directionLightMaterial = null;
    public Vector4 color;

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
        if (directionLightMaterial != null)
        {
            lightDirID = directionLightMaterial.shader.FindPropertyIndex("_LightDir");
            lightDirID = directionLightMaterial.shader.GetPropertyNameId(lightDirID);
            lightColorID = directionLightMaterial.shader.FindPropertyIndex("_LightColor");
            lightColorID = directionLightMaterial.shader.GetPropertyNameId(lightColorID);
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
        if (lightDirID != -1)
        {
            directionLightMaterial.SetVector(lightDirID, new Vector4(transform.forward.x,
                transform.forward.y, transform.forward.z, 0.0f));
        }
        if (lightColorID != -1)
        {
            directionLightMaterial.SetVector(lightColorID, color);
        }
    }
}
