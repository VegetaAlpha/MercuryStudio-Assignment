using UnityEngine;

namespace VegetaSystem
{
    public abstract class ObjPoolable : MonoBehaviour
    {
        private string keyPool;
        private bool isRelease;

        public virtual void Init()
        {
            this.gameObject.SetActive(false);
        }

        public abstract void Get();
        public abstract void Release();



        #region  Internal
        internal void In_Init(string key)
        {
            this.keyPool = key;
            this.isRelease = true;
            Init();
        }

        internal string In_GetKeyPool()
        {
            return keyPool;
        }

        internal void In_Get()
        {
            isRelease = false;
            Get();
        }

        internal void In_Release()
        {
            isRelease = true;
            Release();
        }

        internal bool In_GetRelease()
        {
            return isRelease;
        }

        #endregion
    }
}
