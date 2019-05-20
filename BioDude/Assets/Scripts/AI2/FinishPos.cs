using UnityEngine;
// ReSharper disable InconsistentNaming
// ReSharper disable SuggestVarOrType_BuiltInTypes
// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable SuggestBaseTypeForParameter

public class FinishPos : MonoBehaviour
{
    public string playerTag = "Agent";

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == playerTag)
        {
            col.gameObject.SetActive(false);
            //process finish
        }
    }
}
