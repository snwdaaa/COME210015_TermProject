using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "KeyData", menuName = "UI/KeyData")]
public class KeyData : ScriptableObject
{
    public string keyName; // 키 이름 (예: "E", "F")
    public Sprite keyImage; // 키 이미지
}