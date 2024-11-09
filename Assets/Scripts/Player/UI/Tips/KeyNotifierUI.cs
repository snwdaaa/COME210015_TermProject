using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 상호작용 가능한 트리거 안에 있을 때, 상호작용 하기 위한 키를 UI에 표시
/// </summary>
public class KeyNotifierUI : MonoBehaviour
{
    [SerializeField] private Image keyImageUI;  // 키 이미지가 표시될 UI
    [SerializeField] private Text keyTextUI;    // 텍스트가 표시될 UI
    [SerializeField] private KeyData[] keyDataArray; // 모든 키 데이터를 보관할 배열

    private KeyData currentKeyData;

    /// <summary>
    /// 키 이름을 통해 KeyData 찾기
    /// </summary>
    /// <param name="keyName">찾을 키 이름</param>
    /// <returns>KeyData 객체</returns>
    private KeyData FindKeyData(string keyName)
    {
        foreach (var keyData in keyDataArray)
        {
            if (keyData.keyName == keyName)
                return keyData;
        }
        return null;
    }

    /// <summary>
    /// 키 이름을 받아서 해당하는 키를 표시
    /// </summary>
    /// <param name="keyName"></param>
    public void Show(string keyName, string keyDesc)
    {
        currentKeyData = FindKeyData(keyName);
        if (currentKeyData != null)
        {
            keyImageUI.sprite = currentKeyData.keyImage;
            keyImageUI.gameObject.SetActive(true);
            keyTextUI.gameObject.SetActive(true);
            keyTextUI.text = keyDesc;
        }
    }

    /// <summary>
    /// 표시 중인 키를 숨김
    /// </summary>
    public void Hide()
    {
        keyImageUI.gameObject.SetActive(false);
        keyTextUI.gameObject.SetActive(false);
    }
}
