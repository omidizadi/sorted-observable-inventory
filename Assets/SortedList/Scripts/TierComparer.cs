using System.Collections.Generic;

class TierComparer : IComparer<SphereCollectible.Tier>
{
    public int Compare(SphereCollectible.Tier x, SphereCollectible.Tier y)
    {
        var res = -x.CompareTo(y);

        return res == 0 ? 1 : res;
    }
}