using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGame : UIItems
{
    // Start is called before the first frame update
    public void MoveUp() => PlayerMove.Instance.StartMove(Vector2.up);
    public void MoveDown() => PlayerMove.Instance.StartMove(Vector2.down);
    public void MoveLeft() => PlayerMove.Instance.StartMove(Vector2.left);
    public void MoveRight() => PlayerMove.Instance.StartMove(Vector2.right);
    public void StopMove() => PlayerMove.Instance.StopMove();
}
