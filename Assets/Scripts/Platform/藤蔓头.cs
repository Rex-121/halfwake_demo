using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class 藤蔓头 : MonoBehaviour
{
    [ShowInInspector, HideLabel]
    public 藤蔓.生长 生长 => 玩家配置.main.机制.藤蔓头生长;
    
    private Rigidbody2D _rb;
    private Vector3 _startPos;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _startPos = transform.position;
    }

    public IEnumerator MoveLeft()
    {
        yield return MoveTo(_startPos + Vector3.left * 生长.距离);
    }

    public IEnumerator MoveRight()
    {
        yield return MoveTo(_startPos);
    }

    [Button]
    public void 向左()
    {
        StartCoroutine(MoveLeft());
    }
    
    [Button]
    public void 向右()
    {
        StartCoroutine(MoveRight());
    } 

    private IEnumerator MoveTo(Vector3 target)
    {
        while (Mathf.Abs(transform.position.x - target.x) > 0.01f)
        {
            var step = 生长.速度 * Time.fixedDeltaTime;
            if (Mathf.Abs(transform.position.x - target.x) <= step)
            {
                _rb.MovePosition(new Vector2(target.x, _rb.position.y));
                yield break;
            }
            var dir = (target - transform.position).normalized;
            _rb.MovePosition(_rb.position + new Vector2(dir.x, 0) * step);
            yield return new WaitForFixedUpdate();
        }
    }
}
