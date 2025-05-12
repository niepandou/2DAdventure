using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoarPatrolState : BaseState
{
    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.currentSpeed = currentEnemy.normalSpeed;
    }

    public override void LogicUpdate()
    {
        //发现敌人切换到chase状态
        if (currentEnemy.FoundPlayer())
        {
            currentEnemy.SwitchState(NpcState.Chase);
        }
        //巡逻
        //撞墙等待
        if (!currentEnemy.phySicsCheck.isGround 
            || (currentEnemy.phySicsCheck.touchLeftWall && currentEnemy.faceDir.x < 0) 
            || (currentEnemy.phySicsCheck.touchRightWall && currentEnemy.faceDir.x > 0))
        {
            currentEnemy.anim.SetBool("walk",false);
            currentEnemy.wait = true;
        }
        else
        {
            currentEnemy.anim.SetBool("walk",true);
        }
    }

    public override void PhysicsUpdate()
    {
        
    }

    public override void OnExit()
    {
       currentEnemy.anim.SetBool("walk",false);
    }
}
