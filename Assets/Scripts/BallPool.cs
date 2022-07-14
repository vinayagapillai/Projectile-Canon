using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BallPool : MonoBehaviour
{
    public GameObject BallPrefab;
    public int PoolSize;
    public List<GameObject> BallPoolList;

    private GameObject _poolParent;
    private void Awake()
    {
        BallPoolList = new List<GameObject>();
        _poolParent = new GameObject("PoolParent");
        CreateBallPool();
    }

    private void CreateBallPool()
    {
        
        for (int i = 0; i < PoolSize; i++)
        {
            GameObject ghostObj = Instantiate(BallPrefab, Vector3.zero, Quaternion.identity, _poolParent.transform);
            BallPoolList.Add(ghostObj);
            ghostObj.SetActive(false);
        }
    }

    public GameObject GetBallFromPool()
    {
        foreach(Transform i in this.transform)
        {
            if (!i.gameObject.activeSelf)
            {
                i.gameObject.SetActive(true);
                return i.gameObject;
            }
        }

        GameObject ghostObj = Instantiate(BallPrefab, Vector3.zero, Quaternion.identity, _poolParent.transform);
        BallPoolList.Add(ghostObj);

        return ghostObj;
    }

}
