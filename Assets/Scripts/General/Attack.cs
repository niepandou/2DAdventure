using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [Header("基本属性")] 
    public int damage;//攻击伤害
    public float attackRange;//攻击范围
    public float attackRate;//攻击频率

    private void OnTriggerStay2D(Collider2D other)
    {
        //获取other对象的Character组件,调用里面的TakeDamage方法,让other自行进行伤害计算
        //加一个?,保证对方拥有Character这个组件
        //深层次的,?是判断获取的这个Character对象是否为空
        other.GetComponent<Character>()?.TakeDamage(this);//进行伤害
    }
}
