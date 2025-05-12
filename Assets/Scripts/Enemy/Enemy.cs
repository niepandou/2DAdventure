using System.Collections;

using UnityEngine;

[RequireComponent(typeof(Rigidbody2D),typeof(PhySicsCheck),typeof(Animator))]


public class Enemy : MonoBehaviour
{ 
    public Rigidbody2D rb;
    [HideInInspector]public Animator anim;
    [HideInInspector]public PhySicsCheck phySicsCheck;
    [Header("基本参数")]
    public float normalSpeed;
    public float chaseSpeed;
    [HideInInspector]public float currentSpeed;
    [HideInInspector]public Vector3 faceDir;//面朝方向
    //在tranform的scale中发现,敌人面朝方向左时x为1,因此可以通过scale获取方向,通过方向来施加速度
    [HideInInspector] public float scalingFactor;//缩放系数
    public Transform attacker;
    public float hurtForce;
    public Vector3 spawnPoint;
    
    [Header("检测参数")] 
    public Vector2 centerOffset;

    public Vector2 checkSize;
    public float checkDistance;
    public LayerMask attackLayer;
    [Header("计时器")] 
    public float waitTime;
    public float waitTimeCounter;
    public bool wait;
    public float lostTime;
    public float lostTimeCounter;

    [Header("状态")] 
    public bool isHurt;
    public bool isDead;
    protected BaseState currentState;
    protected BaseState patrolState;
    protected BaseState chaseState;
    protected BaseState skillState;
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        phySicsCheck = GetComponent<PhySicsCheck>();
        
        currentSpeed = normalSpeed;
        waitTimeCounter = waitTime;
        spawnPoint = transform.position;
        phySicsCheck.isGround = true;//防止PhysicsCheck左右撞墙检测点出问题
        scalingFactor = transform.localScale.z;//记录缩放系数
    }

    private void OnEnable()
    {
        currentState = patrolState;
        currentState.OnEnter(this);
    }

    private void Update()
    {
        //-是为了让x的速度方向与面朝方向相同,x为正时方向为右
        faceDir = new Vector3(-transform.localScale.x, 0, 0).normalized;
        
        currentState.LogicUpdate();
        TimeCounter();
    }

    private void FixedUpdate()
    {
        if(!isHurt && !isDead && !wait)
            Move();
        currentState.PhysicsUpdate();   
    }

    private void OnDisable()
    {
        currentState.OnExit();
    }

    protected virtual void Move()
    {
        if(!anim.GetCurrentAnimatorStateInfo(0).IsName("snailPreMove")
           && !anim.GetCurrentAnimatorStateInfo(0).IsName("snailRecover"))//蜗牛的walk动画有前摇,这个前摇不允许移动
        //记得加时间修正
            rb.velocity = new Vector2(faceDir.x * currentSpeed * Time.deltaTime, 0);
    }

    //计时器
    private void TimeCounter()
    {
        if (wait)
        {
            waitTimeCounter -= Time.deltaTime;
            if (waitTimeCounter <= 0)
            {
                wait = false;
                waitTimeCounter = waitTime;
                //转身
                transform.localScale = new Vector3(faceDir.x, 1, 1) * scalingFactor;
            }
        }

        if (!FoundPlayer() && lostTimeCounter > 0)
        {
            lostTimeCounter -= Time.deltaTime;
        }
    }

    public virtual bool FoundPlayer()
    {
        return Physics2D.BoxCast(
            transform.position + (Vector3)centerOffset, 
            checkSize, 
            0, 
            faceDir,
            checkDistance,
            attackLayer);
    }

    public virtual Vector3 GetNewPoint()
    {
        return transform.position;
    }
    //巡逻,追击状态切换
    public void SwitchState(NpcState state)
    {
        var newState = state switch
        {
            NpcState.Patrol => patrolState,
            NpcState.Chase => chaseState,
            NpcState.Skill => skillState, 
            _ => null
        };
        
        currentState.OnExit();
        currentState = newState;
        currentState.OnEnter(this);
    }
    #region 事件方法
    public void onTakeDamage(Transform attackTrans)
    {
        attacker = attackTrans;

        //攻击者在我的右侧
        if (attacker.position.x - transform.position.x > 0)
        {
            //转向
            transform.localScale = new Vector3(-1, 1, 1) * scalingFactor;
        }

        if (attacker.position.x - transform.position.x < 0)
        {
            transform.localScale = new Vector3(1, 1, 1) * scalingFactor;
        }
        
        //受伤击退
        isHurt = true;
        anim.SetTrigger("hurt");
        Vector2 dir = new Vector2((transform.position.x - attacker.position.x), 0).normalized;
        rb.velocity = new Vector2(0, rb.velocity.y);
        //启动携程
        StartCoroutine(OnHurt(dir));
    }

    IEnumerator OnHurt(Vector2 dir)
    {
        rb.AddForce(dir * hurtForce,ForceMode2D.Impulse);
        yield return new WaitForSeconds(2f);
        isHurt = false;
    }

    public void OnDie()
    {
        gameObject.layer = 2;//设置为ignore图层,不与玩家图层互动
        anim.SetBool("dead",true);
        isDead = true;
    }

    public void DestroyAfterAnimation()
    {
        Destroy(this.gameObject);
    }
    #endregion

    public virtual void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position + (Vector3)centerOffset + new Vector3(checkDistance * faceDir.x,0,0), 0.2f);
    }
}
