using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    public interface IPoolObjectGenerator<T> where T : Component
    {
        public T Generate();
    }
    
    public class ObjectPool<T> where T : Component
    {
        private class ObjectPoolData
        {
            public bool isUsing;
            public T obj;

            public ObjectPoolData(T obj)
            {
                this.obj = obj;
            }
        }


        private readonly List<ObjectPoolData> _poolObjectsData;
        private readonly IPoolObjectGenerator<T> _generator;
        
        public ObjectPool(IPoolObjectGenerator<T> generator)
        {
            _poolObjectsData = new List<ObjectPoolData>();
            _generator = generator;
        }

        public T GetObject(Vector3 pos,Vector3 scale)
        {
            T obj=GetObject();
            obj.transform.position = pos;
            obj.transform.localScale = scale;
            return obj;
        }
        
        public T GetObject(Vector3 pos)
        {
            T obj=GetObject();
            obj.transform.position = pos;
            return obj;
        }

        public T GetObject()
        {
            ObjectPoolData poolData=GetNotUsingObjectData();
            poolData.isUsing = true;
            return poolData.obj;
        }

        private ObjectPoolData GetNotUsingObjectData()
        {
            foreach (var poolData in _poolObjectsData)
            {
                if (!poolData.isUsing)
                {
                    poolData.obj.gameObject.SetActive(true);
                    return poolData;
                }
            }

            T instance = _generator.Generate();
            ObjectPoolData data = new ObjectPoolData(instance);
            _poolObjectsData.Add(data);
            return data;
        }

        public void ReleaseObject(T obj)
        {
            obj.gameObject.SetActive(false);
            ObjectPoolData releaseObjInData=_poolObjectsData.Find(x=>x.obj==obj);
            if (releaseObjInData==null)
            {
                Debug.LogError($"Not Found Match PoolObject");
                return;
            }
            releaseObjInData.isUsing = false;
        }
    }
}