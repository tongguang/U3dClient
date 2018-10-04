using System.Collections;

namespace U3dClient.ResourceMgr
{
    enum LoadState
    {
        Init,
        WaitLoad,
        Loading,
        Complete
    }
    public abstract class LoaderBase
    {
        protected abstract void OnReuse();
        protected abstract void OnRecycle();
        protected abstract void ResetData();
        protected abstract IEnumerator LoadFuncEnumerator();
    }
}