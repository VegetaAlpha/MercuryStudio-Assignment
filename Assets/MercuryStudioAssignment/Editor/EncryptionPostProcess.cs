#if UNITY_IOS
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;

public class EncryptionPostProcess
{
    [PostProcessBuild(100)]  // Số càng cao càng chạy sau
    public static void OnPostProcessBuild(BuildTarget buildTarget, string pathToBuiltProject)
    {
        if (buildTarget == BuildTarget.iOS)
        {
            string plistPath = pathToBuiltProject + "/Info.plist";
            
            PlistDocument plist = new PlistDocument();
            plist.ReadFromString(File.ReadAllText(plistPath));
            
            PlistElementDict rootDict = plist.root;
            
            // Set thành NO (false) - hầu hết app Unity dùng cái này
            rootDict.SetBoolean("ITSAppUsesNonExemptEncryption", false);
            
            // Nếu sau này Apple yêu cầu code (rất hiếm), thì thêm dòng dưới và thay code thật:
            // rootDict.SetString("ITSEncryptionExportComplianceCode", "YOUR_CODE_HERE");
            
            File.WriteAllText(plistPath, plist.WriteToString());
            
            UnityEngine.Debug.Log("Đã thêm ITSAppUsesNonExemptEncryption = false vào Info.plist");
        }
    }
}
#endif
