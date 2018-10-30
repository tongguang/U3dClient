namespace U3dClient.FsmMgr
{
    public interface IFsmState
    {
        void OnEnter();

        void OnUpdate();

        void OnExit();
    }
}