using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class 藤蔓茎 : MonoBehaviour
{
    [ShowInInspector, HideLabel]
    public 藤蔓.生长 生长 => 玩家配置.main.机制.藤蔓茎生长;
    
    private Rigidbody2D _rb;
    private Vector3 _startPos;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _startPos = transform.position;
    }

    public IEnumerator MoveUp()
    {
        yield return MoveTo(_startPos + Vector3.up * 生长.距离);
    }

    public IEnumerator MoveDown()
    {
        yield return MoveTo(_startPos);
    }

    private IEnumerator MoveTo(Vector3 target)
    {
        while (Mathf.Abs(transform.position.y - target.y) > 0.01f)
        {
            var step = 生长.速度 * Time.fixedDeltaTime;
            if (Mathf.Abs(transform.position.y - target.y) <= step)
            {
                _rb.MovePosition(new Vector2(_rb.position.x, target.y));
                yield break;
            }
            var dir = (target - transform.position).normalized;
            _rb.MovePosition(_rb.position + new Vector2(0, dir.y) * step);
            yield return new WaitForFixedUpdate();
        }
    }
}
