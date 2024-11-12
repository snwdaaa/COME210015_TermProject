//Controls shader for screen melting; starts effect by setting timer
using UnityEngine;
using System.Collections;

public class ScreenMelt : MonoBehaviour {

	public Material mat;
	public bool effectOn = false;

    //initializes array in shader to melt sprite
    void Start()
    {
        Vector4[] vectorArray = new Vector4[257];
        for (int count = 0; count <= 256; count++)
        {
            vectorArray[count] = new Vector4(Random.Range(1f, 1.25f), 0, 0, 0);
        }

        mat.SetVectorArray("_Offset", vectorArray);
    }

    //resets timer on mat to 0
    void OnApplicationQuit()
	{
		mat.SetFloat("_Timer", 0);
	}


	//starts the timer to the sprite starts melting
	void Update () 
	{
		if (effectOn)
		{
			mat.SetFloat("_Timer", mat.GetFloat("_Timer") + mat.GetFloat("_MeltSpeed") * Time.deltaTime);
		}

        
    }
}
