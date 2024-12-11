using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;

public class ScoreManager : NetworkBehaviour
{
    public TextMeshProUGUI textPrefab;
    private TextMeshProUGUI[] rec=new TextMeshProUGUI [10];
    private float[] score=new float [10];
    private float curx=650,cury=-400;
    private float yinterval=30;
    private int total=0;
    private float initialscore=0f;
    public TextMeshProUGUI GenerateNewScore()
    {
        total++;
        score[total]=initialscore;
        TextMeshProUGUI cur=Instantiate(textPrefab,transform);
        cur.transform.localPosition=new Vector3(curx,cury,0);
        cury+=yinterval;
        cur.text="P"+total.ToString()+": "+((int)(initialscore)).ToString();
        rec[total]=cur;
        return cur;
    }
    public void ModifyByIndex(int index,float inc)
    {
        if(index<0) return;
        if(!IsSpawned) return;
        // Debug.Log("spawnle");
        ModifyByIndexServerRpc(index,inc);
    }
    [ServerRpc(RequireOwnership = false)]
    private void ModifyByIndexServerRpc(int index,float inc)
    {
        ModifyByIndexClientRpc(index,inc);
    }
    [ClientRpc]
    private void ModifyByIndexClientRpc(int index,float inc)
    {
        score[index]+=inc;
        rec[index].text="P"+index.ToString()+": "+((int)(score[index])).ToString();
    }
    public void trygameover()
    {
        int index=-1;
        for(int i=1,len=total;i<=len;i++)
        if(rec[i].color==Color.green) index=i;
        if(index==-1)
        {
            Debug.Log("zhaobudaoren");
        }
        int ran=1;
        for(int i=1,len=total;i<=len;i++)
        {
            if(i==index) continue;
            if(score[i]>score[index]) ran++;
        }
        // Debug.Log("jinlaile");
        GameObject tmp=GameObject.Find("FinalRank");
        tmp.GetComponent<TextMeshProUGUI>().text="Your rank: "+ran.ToString();
        tmp.GetComponent<TextMeshProUGUI>().color=Color.green;
    }
}
