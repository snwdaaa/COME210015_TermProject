using UnityEngine;

public class ScreenMelt : MonoBehaviour
{
    public Material mat;
    public bool effectOn = false;
    public int offsetResolution = 256; // 오프셋 텍스처 해상도

    private void Awake()
    {
        mat.SetFloat("_Timer", 0);
        CreateOffsetTexture();
    }

    void Start()
    {
        mat.SetFloat("_Timer", 0);
    }

    void Update()
    {
        if (effectOn)
        {
            float currentTime = mat.GetFloat("_Timer") + mat.GetFloat("_MeltSpeed") * Time.deltaTime;
            mat.SetFloat("_Timer", Mathf.Clamp(currentTime, 0.0f, 1.0f)); // Timer 클램프
        }
    }

    private void CreateOffsetTexture()
    {
        // 1D 텍스처 생성
        Texture2D offsetTexture = new Texture2D(offsetResolution, 1, TextureFormat.RFloat, false);
        for (int i = 0; i < offsetResolution; i++)
        {
            float offsetValue = Random.Range(1f, 1.25f); // 랜덤 오프셋 값
            offsetTexture.SetPixel(i, 0, new Color(offsetValue, 0, 0, 0));
        }
        offsetTexture.Apply();
        offsetTexture.wrapMode = TextureWrapMode.Clamp; // UV 값 외삽 방지
        mat.SetTexture("_OffsetTex", offsetTexture);
    }
}
