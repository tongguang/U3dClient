namespace U3dClient.Frame
{
    public interface IFsmState
    {
        void OnEnter();

        void OnUpdate();

        void OnExit();
    }
}