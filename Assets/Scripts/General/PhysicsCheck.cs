using System;
using UnityEngine;

public class PhySicsCheck : MonoBehaviour
{
    private CapsuleCollider2D coll;
    private PlayerController playerController;
    private Rigidbody2D rb;
    [Header("检测参数")] 
    public bool manual;//是否手动调整墙体碰撞

    public bool isPlayer;
    public bool groundManual;//是否手动调整地面碰撞
    public Vector2 bottomOffset;
    public Vector2 leftOffset;
    public Vector2 rightOffset;
    public float checkRadius;
    public LayerMask groundLayer;
    public Vector3 faceDir;
    public float scalingFactor;
    [Header("状态")]
    public bool isGround;

    public bool touchLeftWall;
    public bool touchRightWall;
    public bool onWall;
    
    private void Awake()
    {
        coll = GetComponent<CapsuleCollider2D>();
        rb = GetComponent<Rigidbody2D>();

        scalingFactor = coll.transform.localScale.z;
        if (!manual)
        {
            FixDir();
        }
        if (!groundManual)
        {
            FixGroundDir();
        }

        if (isPlayer)
        {
            playerController = GetComponent<PlayerController>();
            
        }
    }

    private void Update()
    {
        if (!manual)
        {
            FixDir();
        }
        if (!groundManual)
        {
            FixGroundDir();
        }
        Check(); 
    }

    public void FixDir()//修正转向之后的错误变量计算
    {
        //修正地面计算
        //bottomOffset = new Vector2()
        //修正左右偏移量
        float horizontalEdge = coll.size.x / 2f * coll.transform.localScale.x / MathF.Abs(coll.transform.localScale.x);
        rightOffset = new Vector2(horizontalEdge + coll.offset.x,coll.bounds.size.y / scalingFactor/2f);
        leftOffset = new Vector2(-horizontalEdge + coll.offset.x,coll.bounds.size.y / scalingFactor/2f);
    }

    public void FixGroundDir()
    {
        if (!onWall)
        {
            bottomOffset = new Vector2(-coll.bounds.size.x/2f/scalingFactor + coll.offset.x, 0);
        }
        else
        {
            bottomOffset = new Vector2(-coll.bounds.size.x / 2f + coll.offset.x, bottomOffset.y);
        }
    }
    // 实际检测时转换为世界坐标
    Vector2 GetLeftEdgeWorldPos() {
        return transform.TransformPoint(leftOffset);
    }

    Vector2 GetRightEdgeWorldPos() {
        return transform.TransformPoint(rightOffset);
    }

    Vector2 GetBottomEdgeWorldPos()
    {
        return transform.TransformPoint(bottomOffset);
    }

    public void Check()
    {
        //检测地面
        isGround = Physics2D.OverlapCircle(GetBottomEdgeWorldPos() , checkRadius, groundLayer);
        //墙体判断
        touchLeftWall = Physics2D.OverlapCircle(GetLeftEdgeWorldPos(), checkRadius, groundLayer);
        touchRightWall = Physics2D.OverlapCircle(GetRightEdgeWorldPos(), checkRadius, groundLayer);

        if(isPlayer)
            onWall = ((touchLeftWall && playerController.inputDirection.x < 0f) || (touchRightWall && playerController.inputDirection.x > 0f)) && (rb.velocity.y <= 0f);
    }

    private void OnDrawGizmosSelected()
    {
        //绘制虚线球形  
        Gizmos.DrawWireSphere(GetBottomEdgeWorldPos() ,checkRadius);
        Gizmos.DrawSphere(GetLeftEdgeWorldPos(), checkRadius);
        Gizmos.DrawSphere(GetRightEdgeWorldPos(), checkRadius);
    } 
}
