using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;

namespace U3dClient.GameTools
{
    public class BuildAssetBundleWin: OdinEditorWindow
    {
        public static void OpenWindow()
        {
            var window = GetWindow<BuildAssetBundleWin>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(700, 700);
        }

        public string RawResRootPath = "";
    }
}
