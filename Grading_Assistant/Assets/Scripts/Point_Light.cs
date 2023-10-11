using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point_Light : MonoBehaviour
{
    public Material pointLightMaterial = null;
    public Vector4 color;

    private int lightPosID = -1;
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
        if (pointLightMaterial != null)
        {
            lightPosID = pointLightMaterial.shader.FindPropertyIndex("_LightPos");
            lightPosID = pointLightMaterial.shader.GetPropertyNameId(lightPosID);
            lightColorID = pointLightMaterial.shader.FindPropertyIndex("_LightColor");
            lightColorID = pointLightMaterial.shader.GetPropertyNameId(lightColorID);
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
        if (lightPosID != -1)
        {
            pointLightMaterial.SetVector(lightPosID, new Vector4(transform.position.x,
                transform.position.y, transform.position.z, 0.0f));
        }
        if (lightColorID != -1)
        {
            pointLightMaterial.SetVector(lightColorID, color);
        }
    }
}
