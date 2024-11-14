//Controls shader for screen melting; starts effect by setting timer
using UnityEngine;

public class ScreenMelt : MonoBehaviour
{
    public Material mat;
    public bool effectOn = false;

    private void Awake()
    {
        mat.SetFloat("_Timer", 0);
    }

    void Start()
    {
        mat.SetFloat("_Timer", 0);

        Vector4[] vectorArray = new Vector4[257];
        for (int count = 0; count <= 256; count++)
        {
            vectorArray[count] = new Vector4(Random.Range(0.8f, 1.3f), 0, 0, 0);
        }
        mat.SetVectorArray("_Offset", vectorArray);
    }

    void OnApplicationQuit()
    {
        mat.SetFloat("_Timer", 0);
    }

    void Update()
    {
        if (effectOn)
        {
            float currentTime = mat.GetFloat("_Timer") + mat.GetFloat("_MeltSpeed") * Time.deltaTime;
            mat.SetFloat("_Timer", Mathf.Clamp(currentTime, 0.0f, 1.0f)); // Timer 클램프 적용
        }
    }
}
