using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowGroupController : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 0, 0);

    void LateUpdate() {
        // 只继承XY轴移动（根据需求调整）
        transform.position = new Vector3(
            target.position.x + offset.x,
            target.position.y + offset.y,
            transform.position.z  // 保持原有Z
        );
    }
}
