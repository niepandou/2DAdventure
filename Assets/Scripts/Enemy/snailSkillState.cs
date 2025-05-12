
using UnityEngine;

public class snailSkillState : BaseState
{
    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.currentSpeed = currentEnemy.chaseSpeed;
        currentEnemy.anim.SetBool("walk",false);
        currentEnemy.anim.SetBool("hide",true);
        currentEnemy.anim.SetTrigger("skill");

        currentEnemy.GetComponent<Character>().wudi = true;
        currentEnemy.lostTimeCounter = currentEnemy.lostTime;
        currentEnemy.GetComponent<Character>().wudiCounter = currentEnemy.lostTimeCounter;
    }

    public override void LogicUpdate()
    {
        if (currentEnemy.lostTimeCounter <= 0)
        {
            currentEnemy.SwitchState(NpcState.Patrol);
        }

        currentEnemy.GetComponent<Character>().wudiCounter = currentEnemy.lostTimeCounter;
    }

    public override void PhysicsUpdate()
    {
        
    }

    public override void OnExit()
    {
        //currentEnemy.anim.SetBool("walk",true);
        currentEnemy.anim.SetBool("hide",false);
    }
}
