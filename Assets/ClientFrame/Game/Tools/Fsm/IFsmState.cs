namespace U3dClient
{
    public interface IFsmState
    {
        void OnEnter();

        void OnUpdate();

        void OnExit();
    }
}