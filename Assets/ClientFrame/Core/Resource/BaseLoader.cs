using System.Collections;

namespace U3dClient.Frame
{
    public enum LoadState
    {
        Init,
        WaitLoad,
        Loading,
        Complete
    }
    public abstract class BaseLoader
    {
        protected abstract void OnReuse();
        protected abstract void OnRecycle();
        protected abstract void ResetData();
        protected abstract IEnumerator LoadFuncEnumerator();
    }
}