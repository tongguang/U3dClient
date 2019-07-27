namespace U3dClient
{
    public class GameConfig
    {
        #region Enum

        public enum AssetLoadModeEnum
        {
            EditMode,
            AssetBundleMode
        }

        public enum LuaScriptLoadModeEnum
        {
            RawFileMode,
            AssetBundleMode
        }

        #endregion


        #region PublicVal

        public AssetLoadModeEnum AssetLoadMode;
        public LuaScriptLoadModeEnum LuaScriptLoadMode;

        #endregion
    }
}