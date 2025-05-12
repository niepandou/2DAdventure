using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    [Header("监听事件")]
    public SceneLoadEventSO loadSceneEvent;

    public VoidEventSO afterSceneLoadedEvent;
    public VoidEventSO loadDataEventSO;
    public VoidEventSO backToMenuEventSO;
    [Header("组件")]
    public PlayerInputControl inputControl;
    public CapsuleCollider2D coll;
    public AudioDefination audioDef;
    private Character character;
    //刚体
    public Rigidbody2D rb;
    private PhySicsCheck phySicsCheck;
    private PlayerAnimation playerAnimation;
    
    [Header("基本参数")]    
    public Vector2 inputDirection;
    private float lastNonZeroInput = 1; // 初始化玩家默认向右
    public float speed;
    private float runSpeed;
    private float walkSpeed => speed/ 4f;
    
    public float jumpForce;
    public float wallJumpForce;
    public float hurtForce;
    public float slideDistance;
    public float slideSpeed;
    public int slidePowerCost;
    
    // private Vector2 originalOffset;
    // private Vector2 originalSize;
    
    [Header("状态")]
    public bool isGuard;
    public bool isHurt;
    public bool couldAntiAttack;
    
    public bool isDead;
    public bool isAttack;
    public bool wallJump;
    public bool isSlide;
    public int jumpCount;//跳跃次数
    public int maxJumpCount;//最大跳跃次数
    [Header("物理材质")] 
    public PhysicsMaterial2D wall;
    public PhysicsMaterial2D normal;

    public static event Action<PlayerController> OnPlayerSpawned;

    //系统启动Awake,激活物件OnEnable

    //Awake适合初始化一些变量
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        phySicsCheck = GetComponent<PhySicsCheck>();
        inputControl = new PlayerInputControl();
        coll = GetComponent<CapsuleCollider2D>();
        character = GetComponent<Character>();
        playerAnimation = GetComponent<PlayerAnimation>();
        // originalOffset = coll.offset;
        // originalSize = coll.size;
        
        //跳跃
        inputControl.Gameplay.Jump.started += Jump;

        #region 强制走路
        
        runSpeed = speed;
        inputControl.Gameplay.WalkButton.performed += ctx =>
        {
            if (phySicsCheck.isGround)
            {
                speed = walkSpeed;
            }
        };
        inputControl.Gameplay.WalkButton.canceled += ctx =>
        {
            if (phySicsCheck.isGround)
            {
                speed = runSpeed;
            }
        };
        inputControl.Gameplay.Slide.started += Slide;

        #endregion
        //攻击
        inputControl.Gameplay.Attack.started += PlayerAttack;
    }


    private void OnEnable()
    {
        OnPlayerSpawned?.Invoke(this);
        inputControl.Enable();
        loadSceneEvent.LoadRequestEvent += OnLoadSceneEvent;
        afterSceneLoadedEvent.OnEventRaised += OnAfterSceneLoadedEvent;
        loadDataEventSO.OnEventRaised += OnLoadDataEvent;
        //借用一下loadDataEvent来恢复玩家死亡状态为false
        backToMenuEventSO.OnEventRaised += OnLoadDataEvent;
    }

    

    private void OnDisable()
    {
        inputControl.Disable();
        loadSceneEvent.LoadRequestEvent -= OnLoadSceneEvent;
        afterSceneLoadedEvent.OnEventRaised -= OnAfterSceneLoadedEvent;
        loadDataEventSO.OnEventRaised -= OnLoadDataEvent;
        backToMenuEventSO.OnEventRaised -= OnLoadDataEvent;
    }

    


    //每帧执行一次,不稳定
    private void Update()
    {
        //每帧读取一次左右方向的输入
        inputDirection = inputControl.Gameplay.Move.ReadValue<Vector2>();
        checkState();
    }

    //0.02秒执行一次
    private void FixedUpdate()
    {
        //TODO:此处把isHurt删除了,不知道有没有什么影响
        if(!isAttack && ! isSlide&& !wallJump)
            Move();
    }

    #region SOEvent

    //场景加载时停止玩家移动操作
    private void OnLoadSceneEvent(GameSceneSO arg0, Vector3 arg1, bool arg2)
    {
        inputControl.Gameplay.Disable();
    }
    //场景加载完成时恢复玩家的移动
    private void OnAfterSceneLoadedEvent()
    {
        inputControl.Gameplay.Enable();
    }
    private void OnLoadDataEvent()
    {
        isDead = false;
    }

    #endregion
    
    
    //控制玩家移动和一些运动参数
    private void Move()
    {
        //Time.deltaTime 可以修正不同帧下的速度,使其保持一致
        if(!isGuard)
            rb.velocity = new Vector2(inputDirection.x * speed * Time.deltaTime, rb.velocity.y);

        // 记录最后有效输入方向
        if (inputDirection.x != 0)
        {
            lastNonZeroInput = Mathf.Sign(inputDirection.x);
        }
        //玩家翻转
        transform.localScale = new Vector3(lastNonZeroInput, 1, 1);
        
        //防御
        isGuard = inputDirection.y < -0.5f;
        if (isGuard && !isAttack)
        {
            //TODO:进行防御免伤,判断防御反击
            //防御免伤,加个受击伤害系数应该没问题,但是,这东西应该由Character来操作
            character.currentHurtRate = character.hurtRate;
            //受击判定如何做,做这个是为了做出防御反击
            //关键在于我要如何我知道我在防御期间受击了
            //通过SO传递信号还是其他的更合适,或许不用考虑,因为我本身就有已经有一个takeDamage用来传递信号了,接收一下即可
            //也不用,在GetHurt方法中,我受伤时就会修改isHurt,这个isHurt就可以当做我的受击判断
            //此时我只需要保存这个受击状态,即我在防御期间,只要受击,即可在防御期间的任意时刻进行反击
            if (isHurt && !couldAntiAttack)
            {
                couldAntiAttack = true;
            }
            //反击动作就交给Attack方法了,其实到这步就什么也不用修改了
        }
        else
        {
            couldAntiAttack = false;
            character.currentHurtRate = character.defaultHurtRate;
        }
    }
    
    void RepeatJump()
    {
        jumpCount++;
        rb.velocity = new Vector2(0, 0);
        rb.AddForce(transform.up * jumpForce,ForceMode2D.Impulse);
        isSlide = false;
        gameObject.layer = LayerMask.NameToLayer("Player");
        audioDef.playAudioClip();
        //这个是做什么的来着
        //中止所有携程,在这个脚本中只有Slide这一个携程,也就是提前终止Slide动作
        StopAllCoroutines();
    }
    //跳跃
    private void Jump(InputAction.CallbackContext obj)
    {
        if (phySicsCheck.isGround)
        {
            rb.AddForce(transform.up * jumpForce,ForceMode2D.Impulse); 
            isSlide = false;
            jumpCount = 1;
            gameObject.layer = LayerMask.NameToLayer("Player");
            audioDef.playAudioClip();
            StopAllCoroutines();
        }

        else if (phySicsCheck.onWall)
        {
            rb.velocity = Vector2.zero;
            rb.AddForce(new Vector2(-inputDirection.x,2f) * wallJumpForce ,ForceMode2D.Impulse);
            wallJump = true;
            jumpCount = 1;
            audioDef.playAudioClip();
        }
        else if (jumpCount >= 1 && jumpCount < maxJumpCount)
        {
            RepeatJump();
        }
    }

    private void PlayerAttack(InputAction.CallbackContext obj)
    {
        playerAnimation.PlayAttack();
        isAttack = true;
    }
    private void Slide(InputAction.CallbackContext obj)
    {
        if (!isSlide && phySicsCheck.isGround && character.currentPower >= slidePowerCost)
        {
            isSlide = true;
            gameObject.layer = LayerMask.NameToLayer("Enemy");//滑铲无敌帧
            var targetPos = new Vector3(transform.position.x + transform.localScale.x * slideDistance,
                transform.position.y);

            StartCoroutine(TriggerSlide(targetPos));//启动携程
            character.OnSlide(slidePowerCost);
        }
        
        
    }

    private IEnumerator TriggerSlide(Vector3 target)
    {
        do
        {
            yield return null;//每帧检测
            
            if(!phySicsCheck.isGround) break;

            if ((phySicsCheck.touchLeftWall && transform.localScale.x < 0)
                || (phySicsCheck.touchRightWall && transform.localScale.x > 0))
            {
                break;
            }
            
            rb.MovePosition(new Vector2(transform.position.x + transform.localScale.x * slideSpeed,transform.position.y));
        } while (Mathf.Abs(target.x - transform.position.x) > 0.1f);

        isSlide = false;
        gameObject.layer = LayerMask.NameToLayer("Player");//退出滑铲无敌帧 
    }
    #region UnityEvent
    public void GetHurt(Transform attacker) 
    {
        isHurt = true;
        rb.velocity = Vector2.zero;
        //normalized将数值归1化,即将一个数的正负方向表示出来.0.5为1,-0.5为-1,0为0
        Vector2 dir = new Vector2((transform.position.x - attacker.position.x), 0).normalized;//得到玩家面对怪物的方向
        
        //impulse 瞬时力
        rb.AddForce(dir*hurtForce,ForceMode2D.Impulse);
    }

    public void PlayerDead()
    {
        isDead = true;
        inputControl.Gameplay.Disable();//关闭玩家游玩操作,Ui操作没有关闭
    }
    
    #endregion
    
    private void checkState()
    {
        coll.sharedMaterial = phySicsCheck.isGround ? normal : wall;

        if (phySicsCheck.onWall)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / 2f);
        }
        else
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
        }


        if (wallJump && rb.velocity.y <= 0f)
        {
            wallJump = false;
        }
    }
}