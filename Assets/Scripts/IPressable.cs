using UnityEngine;

public interface IPressable
{
    public void Press(IPresser presser);
    public void Release(IPresser presser);
}
