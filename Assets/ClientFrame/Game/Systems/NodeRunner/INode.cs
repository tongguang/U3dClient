namespace U3dClient
{
    public interface INode
    {
        void OnStart();
        bool Execute();
        void OnEnd();
    }
}