
# Uncommon data collections in C# and Unity

In this article, I will introduce two C collections which can be quite handy for game development

![an inventory sample, from the Genshin Impact game](https://cdn-images-1.medium.com/max/2000/1*kk3aRJwwVYe8b7TELZfCCw.png)*an inventory sample, from the Genshin Impact game*

I was playing Genshin Impact the other day and noticed that my items in the inventory are always sorted by their tier, star, or level. I thought that it should be easy to implement. But I always like to know my options.

These two collections that I will talk about, has been used more often by .Net developers than Unity programmers.

## SortedList

Almost all common data structures such as list, dictionary, or hash set has an equivalent sorted type.

**SortedList, SortedDictionary, **and **SortedSet** are all built-in C# collections. As you can guess, their job is to sort the elements each time you add or remove them. But the important thing here is the way a SortedList compares the elements. What happens if we try to store GameObjects? How a GameObject comparison is handled?

Let's find out with an example.

### Sorted Inventory

I created a super simple scene with a bunch of balls as collectible items, a canvas to show the inventory UI, and a terrain to hold the balls (didn’t want it to look too simple!)

![](https://cdn-images-1.medium.com/max/2000/1*1e1j5p2xgqkaHjLzfFFeOA.png)

Then I implemented two interfaces for my collectibles and inventory:

```c#
using UnityEngine;

public interface ICollectible
{
   GameObject collectibleGameObject { get; }
}
```

```c#
public interface IInventory
{
   void Add(ICollectible collectible);
}
```

First I implemented an abstract inventory to be able to retrieve its instance in the scene:

```c#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Inventory : MonoBehaviour, IInventory
{
    public static IInventory Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public abstract void Add(ICollectible collectible);
}
```

I used the Singleton pattern here for ease of use, but I will be cautious about not letting my dependencies hide between codes because of that.

Then I implemented a concrete version of ICollectible for the balls and defined three tiers (T1 for grey, T2 for purple, and T3 for purple).

```c#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereCollectible : MonoBehaviour,ICollectible
{
    public enum Tier
    {
        T1,
        T2,
        T3
    }

    public Tier tier;
    private IInventory inventory;

    void Start()
    {
        inventory = Inventory.Instance;
    }

    private void OnMouseDown()
    {
        inventory.Add(this);
        gameObject.SetActive(false);
    }
    public GameObject collectibleGameObject => gameObject;
}
```

Now by each click on the balls, they add themselves to the inventory and disappear.

The sorted inventory would be like this:

```c#
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortedInventory : Inventory
{
    public event Action<SortedList<SphereCollectible.Tier, ICollectible>> OnItemCollected;

    private SortedList<SphereCollectible.Tier, ICollectible> sortedList =
        new SortedList<SphereCollectible.Tier, ICollectible>(new TierComparer());

    public override void Add(ICollectible collectible)
    {
        var tier = collectible.collectibleGameObject.GetComponent<SphereCollectible>().tier;
        sortedList.Add(tier, collectible);
        OnItemCollected?.Invoke(sortedList);
    }
}
```

As you can see I used a SortedList to store the items. I forgot to mention that all sorted collections receive a key-value pair as an element and sort them by the key. If you give an int, float, enum, string, or char as a key, by default it will sort them in ascending order. But there is a problem! SortedList doesn’t accept duplicated keys! We know that in some circumstances, we must store duplicated items in a sorted collection, like this example that I want to store different items with the same tier. The solution is to give the SortedList your own implementation of IComparer!

```c#
using System.Collections.Generic;

class TierComparer : IComparer<SphereCollectible.Tier>
{
    public int Compare(SphereCollectible.Tier x, SphereCollectible.Tier y)
    {
        var res = -x.CompareTo(y);

        return res == 0 ? 1 : res;
    }
}
```

With this, my SortedList will sort items in descending order (because T3 items are more important thus should be seen first) and treat the same tiers as greater.

Note that *IComparer *interface is generic, therefore you can implement the comparison for anything you desire such as *GameObjects*, *Vectors*, *Raycasts*, absolutely anything.

in the *SortedInventory *class, I stored the *OnItemCollected *event for any class that wants to get notified when something is being collected.

So I created a UI class to show my inventory:

```c#
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SortedInventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject[] items;
    [SerializeField] private Color[] tierColors;
    private SortedInventory sortedInventory;
    

    // Start is called before the first frame update
    void Start()
    {
        sortedInventory = (SortedInventory) Inventory.Instance;
        sortedInventory.OnItemCollected += UpdateInventoryUI;
    }

    private void UpdateInventoryUI(SortedList<SphereCollectible.Tier, ICollectible> sortedList)
    {
        var index = 0;
        foreach (var item in sortedList)
        {
            items[index].GetComponent<Image>().color = tierColors[(int)item.Key];
            items[index].gameObject.SetActive(true);
            index++;
        }
    }
}
```

Now everything is set and ready:

![](https://cdn-images-1.medium.com/max/2000/1*6P5w_egveU8fCH0QF8Lprw.gif)

## ObservableCollection

In the previous section, you saw that I used the *OnItemCollected* to let other classes subscribe to my inventory events. But now I want to use a collection that has the same built-in functionality. **ObservableCollection **gives us the ability to subscribe to its changes.

Same as the previous example, I made a scene with a bunch of *GameObjects *(this time cubes in two colors)

![](https://cdn-images-1.medium.com/max/2000/1*Xcwun-J9XVauQxvSK2KdqQ.png)

I want to collect cubes with a single click and add them to my collection. I also need to update my UI whenever something is collected but this time I won’t use a C# event. First my *CubeCollectible *class:

```c#
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeCollectible : MonoBehaviour,ICollectible
{
    public enum Type
    {
        RED,
        BLUE
    }

    private IInventory inventory;
    public Type type;



    private void Start()
    {
        inventory = Inventory.Instance;
    }

    private void OnMouseDown()
    {
        inventory.Add(this);
        gameObject.SetActive(false);
    }

    public GameObject collectibleGameObject => gameObject;
}
```

It’s almost the same as the *SphereCollectible *with different *Type *enum.

Now for the inventory, I use an encapsulated *ObservableCollection*:

```c#
  
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using UnityEngine;

public class ObservableInventory : Inventory
{
    public ObservableCollection<ICollectible> collectibles { get; } = new ObservableCollection<ICollectible>();
    public override void Add(ICollectible collectible)
    {
        collectibles.Add(collectible);
    }

}
```

Other classes can subscribe to this collection by adding an event to its *CollectionChanged*. like this:

```c#
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using UnityEngine;

public class ObservableInventoryUI : MonoBehaviour
{
    [SerializeField] private Transform collectiblesParent;
    [SerializeField] private CubeCollectible.Type type;
    private ObservableInventory observableInventory;

    private void Start()
    {
        observableInventory = (ObservableInventory) Inventory.Instance;
        observableInventory.collectibles.CollectionChanged += UpdateInventoryUI;
    }

    private void UpdateInventoryUI(object sender, NotifyCollectionChangedEventArgs e)
    {
        var newCollected = (CubeCollectible) e.NewItems[0];
        if (newCollected.type != type) return;
        foreach (Transform tr in collectiblesParent)
        {
            if (tr.gameObject.activeSelf) continue;
            tr.gameObject.SetActive(true);
            break;
        }
    }
}
```

This script adds the *UpdateInventoryUI *to our *ObservableCollection *to show equivalent UI items when an item is collected. I attach this script to two components for each color inside my canvas:

![](https://cdn-images-1.medium.com/max/2000/1*1pnLw-ybB5MgOx-gXHPO-A.png)

![](https://cdn-images-1.medium.com/max/2000/1*GSQ2PYnM8w8gisVheNfmCQ.png)

Now we play the scene:

![](https://cdn-images-1.medium.com/max/2000/1*_Pu5YmkoK-P7lbTJHR9ItA.gif)

As you can see those two UI components are notified by the *ObservableCollection *and update the UI if the collected cube color is the one they’re looking for.
