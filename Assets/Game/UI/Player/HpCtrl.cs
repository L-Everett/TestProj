using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpCtrl : MonoBehaviour
{
    public GameObject mHeart;
    
    /// <summary>
    /// 该位置是否有血量
    /// </summary>
    /// <param name="exist"></param>
    public void SetHp(bool exist)
    {
        mHeart.SetActive(exist);
    }
}
