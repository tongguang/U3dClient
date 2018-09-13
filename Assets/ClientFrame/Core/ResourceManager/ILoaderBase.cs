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
    public interface ILoaderBase
    {
        void OnReuse();
        void OnRecycle();
        IEnumerator LoadFuncEnumerator();
    }
}