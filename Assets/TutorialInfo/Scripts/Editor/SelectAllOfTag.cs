using UnityEngine;
using UnityEditor;

public class SelectAllOfTag : ScriptableWizard
{
    public string searchTag = "Tag name";

    [MenuItem("Select/Select All of Tag...")]
    static void SelectAllOfTagWizzard()
    {
        ScriptableWizard.DisplayWizard<SelectAllOfTag>("Tag", "Find");
    }

    private void OnWizardCreate()
    {
        Selection.objects = GameObject.FindGameObjectsWithTag(searchTag);
    }
}
