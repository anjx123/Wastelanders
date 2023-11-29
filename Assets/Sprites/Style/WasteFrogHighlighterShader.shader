Shader "Custom/OutlineShader"
{
    Properties
    {
        _OutlineColor ("Outline Color", Color) = (1,1,0,1)
        _Outline ("Outline width", Range (.002, 0.03)) = 0.005
    }

    CGINCLUDE
#include "UnityCG.cginc"
    CGINCLUDE

    OUTLINE

    SubShader
    {
        Tags
        {
            "Queue" = "Overlay"
        }

        Pass
        {
            Name "OUTLINE"

            CGPROGRAM
            #pragma exclude_renderers gles xbox360 ps3

            #pragma vertex vert
            #pragma exclude_renderers flash xbox360 xbox360_ps3 gles gles xbox360 ps3

            OUTLINE

            ENDCG
        }
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Overlay"
        }

        LOD 100

        // Use the Standard surface shader model
        CGPROGRAM
        #pragma surface surf Standard

        OUTLINE

        ENDCG
    }
}
