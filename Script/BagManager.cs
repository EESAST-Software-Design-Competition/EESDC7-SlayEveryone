using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class BagManager : MonoBehaviour
{
    private float itemwidth=50,itemheight=50;
    private Vector2 PictureOffset=new Vector2(-15,15);
    private Vector3 NumberOffset=new Vector3(123,-37);
    private const int FontSize=24;
    public Image[] ImagePrefabs;                //物品图像，需要手动放进去
    private Image[] Bagpositions=new Image[5];      //背包物品
    private TextMeshProUGUI[] NumbersOfItems=new TextMeshProUGUI[5];    //物品数量
    private string prestring="pos";
    ItemList[] upg=new ItemList[5];
    int[] num=new int[5];
    private ItemManager itemManager;
    public void Start()
    {
        //*这仅为示例，还需要后期解决网络功能后继续完善
        itemManager = ItemManager.GetItemsManagerForPlayer("1");
        for(int i=0;i<5;i++)
        {
            upg[i]=ScriptableObject.CreateInstance<ItemList>();
        }
    }
    public void PictureRender(int BagPos,int PrefabId)
    {
        string posname=prestring+(BagPos+1).ToString();
        GameObject cur=GameObject.Find(posname);
        Bagpositions[BagPos]=Instantiate(ImagePrefabs[PrefabId],cur.transform);
        Bagpositions[BagPos].rectTransform.sizeDelta=new Vector2(itemwidth,itemheight);
        Bagpositions[BagPos].rectTransform.localPosition=PictureOffset;     //生成物品图片和位置
        NumbersOfItems[BagPos] = new GameObject(posname+"text").AddComponent<TextMeshProUGUI>();
        NumbersOfItems[BagPos].transform.SetParent(cur.transform, false);
        NumbersOfItems[BagPos].fontSize=FontSize;
        NumbersOfItems[BagPos].rectTransform.localPosition=NumberOffset;    //显示物品数量
    }
    public void NumberReset(int id,int NewNumber)    //id是背包位置编号，不是武器种类编号
    {
        // NumbersOfItems[id].text=num[id].ToString();
        NumbersOfItems[id].text=NewNumber.ToString();
    }
    // public void PickUp<T> (T cur) where T:upgrades  //捡道具，任何种类都可以直接调用，用这个函数捡东西可以自动把UI处理好
    // public void PickUp(PropertiesForItems cur)
    // {
    //     for(int i=0;i<5;i++)
    //     {
    //        if(upg[i].itemNumber==0||upg[i].itemName==cur.nameOfItems)
    //        {
    //            if(num[i]==0)
    //            {
    //                 ItemList newItem = new ItemList
    //                 {
    //                     itemName = cur.nameOfItems,
    //                     itemNumber = 1,
    //                     maxItemNumber = cur.maxItemNumber
    //                 };
    //                upg[i]=newItem;
    //                PictureRender(i,cur.id);
    //            }
    //            num[i]++;
    //            NumberReset(i);
    //            break;
    //        }
    //     }
    // }
    public void destroyGUI(int id)
    {
        Destroy(Bagpositions[id].gameObject);
        Destroy(NumbersOfItems[id].gameObject);
    }
    // public void Useitem(int curid)//curid是背包位置，不是道具种类，使用背包中某个位置的道具。
    // {
    //     if(num[curid]==0) return;
    //     num[curid]--;
    //     NumberReset(curid,num);
    //     //-------------------------
    //     //用于测试功能
    //     // if(upg[curid].id==1)
    //     // {
    //     //     BloodBottle now=(BloodBottle) upg[curid];
    //     //     bloodtable.BloodDecline(-now.cure);
    //     // }
    //     // if(upg[curid].id==2)
    //     // {
    //     //     BigBloodBottle now=(BigBloodBottle) upg[curid];
    //     //     bloodtable.BloodDecline(-now.cure);
    //     // }
    //     //--------------------------
    //     if(num[curid]==0)
    //     {
    //         destroyitem(curid);
    //         // upg[curid].id=0;
    //     }
    // }
    // public void Update()
    // {
    //    //---------------------
    //    //用于测试功能
    //    if(Input.GetKeyDown(KeyCode.O)) PickUp(ScriptableObject.CreateInstance<BloodBottle>());
    //    if(Input.GetKeyDown(KeyCode.P)) PickUp(ScriptableObject.CreateInstance<BigBloodBottle>());
    //    for(int i=0,j=1;j<=5&&i<4;i++,j++)
    //    if (Input.GetKeyDown(KeyCode.Alpha0 + i) || Input.GetKeyDown(KeyCode.Keypad0 + j))
    //    Useitem(i);
    //    //---------------------
    // }
    
}
