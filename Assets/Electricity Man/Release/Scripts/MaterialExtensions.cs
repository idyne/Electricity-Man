using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MaterialExtensions
{
   
    public static void EnableEmission(this Material material)
    {
        material.EnableKeyword("_EMISSION");
    }

    public static void DisableEmission(this Material material)
    {
        material.DisableKeyword("_EMISSION");
    }
}
