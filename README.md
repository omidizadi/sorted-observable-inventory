
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

![](https://cdn-images-1.medium.com/max/2304/1*2YgMAAIBAb0SIJ-6Xrldtg.png)

![](https://cdn-images-1.medium.com/max/2304/1*4wVO1LWMqEeer58ZciBV6g.png)

First I implemented an abstract inventory to be able to retrieve its instance in the scene:

![](https://cdn-images-1.medium.com/max/2264/1*LWmhC986J3hMuBP7R_bj_g.png)

I used the Singleton pattern here for ease of use, but I will be cautious about not letting my dependencies hide between codes because of that.

Then I implemented a concrete version of ICollectible for the balls and defined three tiers (T1 for grey, T2 for purple, and T3 for purple).

![](https://cdn-images-1.medium.com/max/2264/1*-OLF_Ns66HcyQEQCdKutWA.png)

Now by each click on the balls, they add themselves to the inventory and disappear.

The sorted inventory would be like this:

![](https://cdn-images-1.medium.com/max/3156/1*3gDRa-Zn1iIGo_X2sQTfcw.png)

As you can see I used a SortedList to store the items. I forgot to mention that all sorted collections receive a key-value pair as an element and sort them by the key. If you give an int, float, enum, string, or char as a key, by default it will sort them in ascending order. But there is a problem! SortedList doesn’t accept duplicated keys! We know that in some circumstances, we must store duplicated items in a sorted collection, like this example that I want to store different items with the same tier. The solution is to give the SortedList your own implementation of IComparer!

![](https://cdn-images-1.medium.com/max/2624/1*GgiHOfL82zah3rYQkXroyg.png)

With this, my SortedList will sort items in descending order (because T3 items are more important thus should be seen first) and treat the same tiers as greater.

Note that *IComparer *interface is generic, therefore you can implement the comparison for anything you desire such as *GameObjects*, *Vectors*, *Raycasts*, absolutely anything.

in the *SortedInventory *class, I stored the *OnItemCollected *event for any class that wants to get notified when something is being collected.

So I created a UI class to show my inventory:

![](https://cdn-images-1.medium.com/max/2860/1*nJP387TsnCuUqvd3IQAvww.png)

Now everything is set and ready:

![](https://cdn-images-1.medium.com/max/2000/1*6P5w_egveU8fCH0QF8Lprw.gif)

## ObservableCollection

In the previous section, you saw that I used the *OnItemCollected* to let other classes subscribe to my inventory events. But now I want to use a collection that has the same built-in functionality. **ObservableCollection **gives us the ability to subscribe to its changes.

Same as the previous example, I made a scene with a bunch of *GameObjects *(this time cubes in two colors)

![](https://cdn-images-1.medium.com/max/2000/1*Xcwun-J9XVauQxvSK2KdqQ.png)

I want to collect cubes with a single click and add them to my collection. I also need to update my UI whenever something is collected but this time I won’t use a C# event. First my *CubeCollectible *class:

![](https://cdn-images-1.medium.com/max/2328/1*FsvN_kmQ4zsL1TGv_EwARw.png)

It’s almost the same as the *SphereCollectible *with different *Type *enum.

Now for the inventory, I use an encapsulated *ObservableCollection*:

![](https://cdn-images-1.medium.com/max/2452/1*ZXsAt6Pq2f9oD5rOPLHD4Q.png)

Other classes can subscribe to this collection by adding an event to its *CollectionChanged*. like this:

![](https://cdn-images-1.medium.com/max/2768/1*lPeeqVdixmVNQddb_aRpwA.png)

This script adds the *UpdateInventoryUI *to our *ObservableCollection *to show equivalent UI items when an item is collected. I attach this script to two components for each color inside my canvas:

![](https://cdn-images-1.medium.com/max/2000/1*1pnLw-ybB5MgOx-gXHPO-A.png)

![](https://cdn-images-1.medium.com/max/2000/1*GSQ2PYnM8w8gisVheNfmCQ.png)

Now we play the scene:

![](https://cdn-images-1.medium.com/max/2000/1*_Pu5YmkoK-P7lbTJHR9ItA.gif)

As you can see those two UI components are notified by the *ObservableCollection *and update the UI if the collected cube color is the one they’re looking for.
