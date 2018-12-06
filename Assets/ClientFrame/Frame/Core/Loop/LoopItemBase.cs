namespace U3dClient.Frame
{
    public class LoopItemBase
    {
        public int UpdateIndex = -1;
        public bool IsValid = false;

        public virtual void Update()
        {

        }
    }
}