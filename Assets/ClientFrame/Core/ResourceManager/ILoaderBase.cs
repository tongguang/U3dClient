using System.Collections;

namespace U3dClient.ResourceMgr
{
    enum LoadState
    {
        Init,
        Loading,
        Complete
    }
    public interface ILoaderBase
    {
        void OnReuse();
        void OnRecycle();
        IEnumerator LoadFuncEnumerator();
    }
}