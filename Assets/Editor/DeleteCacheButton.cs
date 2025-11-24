using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BillboardImagePicker))]
public class DeleteCacheButton : Editor
{
    [MenuItem("Tools/Billboard/Delete Cache")]
    private static void DeleteCache()
    {
        BillboardImagePicker[] pickers = FindObjectsByType<BillboardImagePicker>(FindObjectsSortMode.None);
        foreach (var picker in pickers)
        {
            picker.ClearCachedImage();
        }
    }
}
