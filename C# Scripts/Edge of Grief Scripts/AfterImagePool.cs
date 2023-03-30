using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImagePool : MonoBehaviour
{
    [SerializeField]
    private GameObject afterImagePrefab;


    private Queue<GameObject> availableObjects = new Queue<GameObject>();

    public static AfterImagePool Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Fill the object pool with more of its assigned object
    /// </summary>
    private void GrowPool()
    {
        for (int i = 0; i < 10; i++)
        {
            var instanceToAdd = Instantiate(afterImagePrefab);
            instanceToAdd.transform.SetParent(transform);
            AddToPool(instanceToAdd);
        }
    }

    /// <summary>
    /// Diable the instance passed through and put it back into its object pool
    /// </summary>
    /// <param name="instance">The instance to add to the object pool</param>
    public void AddToPool(GameObject instance)
    {
        instance.SetActive(false);
        availableObjects.Enqueue(instance);
    }


    /// <summary>
    /// Returns a GameObject reference to an object in the pool that is available
    /// </summary>
    /// <returns></returns>
    public GameObject GetFromPool()
    {
        if (availableObjects.Count == 0)
        {
            GrowPool();
        }

        var instance = availableObjects.Dequeue();
        instance.SetActive(true);
        return instance;
    }
}
