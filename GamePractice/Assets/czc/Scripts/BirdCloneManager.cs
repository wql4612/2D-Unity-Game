using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdCloneManager : MonoBehaviour
{
    private List<BirdClone> clones = new List<BirdClone>();

    public void RegisterClone(BirdClone clone)
    {
        clones.Add(clone);
    }

    public bool AllClonesGrounded()
    {
        foreach (var clone in clones)
        {
            if (!clone.IsGrounded()) return false;
        }
        return true;
    }

    public void FuseAllClones()
    {
        foreach (var clone in clones)
        {
            Destroy(clone.gameObject);
        }
        clones.Clear();
    }
    public void RemoveClone(BirdClone clone)
{
    clones.Remove(clone);

    if (clones.Count == 0)
    {
        // 所有分身都消失了
        FuseAllClones();
    }
}
}
