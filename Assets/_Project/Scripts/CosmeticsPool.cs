using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// Singleton.
/// </summary>
public class CosmeticsPool : MonoBehaviour
{
    public static CosmeticsPool s_Instance;

    [SerializeField, ReadOnly, Tooltip("Stack Pool")]
    public IObjectPool<Cosmetic> Pool;

    [SerializeField]
    private CosmeticPoolRandomiser _cosmeticPoolRandomiserWeights;

    [SerializeField, ReadOnly]
    private int _inactiveItems;

    private bool collectionChecks = false;
    private int maxPoolSize = 15;

    void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (s_Instance != null && s_Instance != this)
        {
            Destroy(this);
        }
        else
        {
            s_Instance = this;
        }

        _cosmeticPoolRandomiserWeights.SetTotalWeights();
        Pool = new ObjectPool<Cosmetic>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, collectionChecks, 10, maxPoolSize);
    }

    private void Update()
    {
        if (Pool != null)
        {
            _inactiveItems = Pool.CountInactive;
        }
    }

    private void OnReturnedToPool(Cosmetic system)
    {
        system.gameObject.SetActive(false);
    }

    private void OnDestroyPoolObject(Cosmetic system)
    {
        Destroy(system.gameObject);
    }

    private void OnTakeFromPool(Cosmetic system)
    {
        system.gameObject.SetActive(true);
    }

    private Cosmetic CreatePooledItem()
    {
        // Get a random item from the cosmetic randomiser
        Cosmetic tmp = Instantiate(_cosmeticPoolRandomiserWeights.GetRandomCosmetic(), this.transform);
        return tmp;
    }
}

[Serializable]
public class CosmeticPoolRandomiser
{
    public CosmeticPoolRandomiserWeight[] CosmeticWeights;

    [SerializeField, ReadOnly]
    private float _totalWeights = 0f;

    public void SetTotalWeights()
    {
        foreach (CosmeticPoolRandomiserWeight cosmetic in CosmeticWeights)
        {
            _totalWeights += cosmetic.Weight;
        }
    }

    public Cosmetic GetRandomCosmetic()
    {
        float weightedChoice = UnityEngine.Random.Range(0, _totalWeights);
        float remainingWeightValue = _totalWeights;

        foreach (CosmeticPoolRandomiserWeight cosmeticWeights in CosmeticWeights)
        {
           remainingWeightValue -= cosmeticWeights.Weight;

           if (remainingWeightValue < weightedChoice)
           {
               return cosmeticWeights.Cosmetic;
           }
        }

        return null;
    }
}

[Serializable]
public class CosmeticPoolRandomiserWeight
{
    [AssetsOnly]
    public Cosmetic Cosmetic;
    public float Weight;
}
