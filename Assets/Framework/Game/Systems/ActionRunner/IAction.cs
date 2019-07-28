namespace U3dClient
{
    public interface IAction
    {
        void OnStart();
        bool Execute();
        void OnEnd();
    }
}