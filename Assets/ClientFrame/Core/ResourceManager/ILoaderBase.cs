using System.Collections;

namespace U3dClient.ResourceMgr
{
    public interface ILoaderBase
    {
        void OnReuse();
        void OnRecycle();
        IEnumerator LoadFunc();
    }
}