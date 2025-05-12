
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class BeePatrolState : BaseState
{
    private Vector3 target;
    private Vector3 moveDir;
    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.currentSpeed = currentEnemy.normalSpeed;
        target = enemy.GetNewPoint();
    }

    public override void LogicUpdate()
    {
        if (currentEnemy.FoundPlayer())
        {
            currentEnemy.SwitchState(NpcState.Chase);
        }

        if (Mathf.Abs(target.x - currentEnemy.transform.position.x) <= 0.1
            && Mathf.Abs((target.y - currentEnemy.transform.position.y)) <= 0.1)
        {
            //到达预定坐标
            currentEnemy.wait = true;
            target = currentEnemy.GetNewPoint();
        }

        moveDir = (target - currentEnemy.transform.position).normalized;
        //朝向展示
        if (moveDir.x >= 0)
        {
            currentEnemy.transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            currentEnemy.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public override void PhysicsUpdate()
    {
        if (!currentEnemy.wait && !currentEnemy.isHurt && !currentEnemy.isDead)
        {
            currentEnemy.rb.velocity = moveDir * currentEnemy.currentSpeed * Time.deltaTime;
        }
        else
        {
            currentEnemy.rb.velocity = Vector2.zero;  
        }
    }

    public override void OnExit()
    {
    }
}
