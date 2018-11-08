namespace U3dClient.Fsm
{
    public interface IFsmState
    {
        void OnEnter();

        void OnUpdate();

        void OnExit();
    }
}