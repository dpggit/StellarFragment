#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class AddHeaderToExistingScripts : EditorWindow
{
    // Define the exact header we want to apply
    private const string HEADER_TEXT =
    @"/* ==========================================
 * Project: Stellar Fragment, 2D game similar to Asteroids
 * Architecture: SOLID + MVC
 * Unity Version: 6.3.8
 * Copyright (c) 2026 [Diego Pena Gayo]. 
 * All rights reserved.
 * 
 * This code is proprietary and confidential. Unauthorized copying, 
 * modification, or distribution of this file via any medium is strictly 
 * prohibited without the express written permission of the author.
 * ========================================== */
";

    // This will create a new option in the top menu of Unity: Tools > Add Headers to All Scripts
    [MenuItem("Tools/Add Headers to All Scripts")]
    public static void ApplyHeaders()
    {
        // Search for all files with the .cs extension inside the Assets folder
        string[] scriptFiles = Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories);
        int modifiedCount = 0;

        foreach (string filePath in scriptFiles)
        {
            // Ignore scripts located inside any "Editor" folder (such as this tool itself)
            if (filePath.Contains(Path.DirectorySeparatorChar + "Editor" + Path.DirectorySeparatorChar))
                continue;

            string content = File.ReadAllText(filePath);

            // Avoid duplicating the header if the script already starts with it
            if (!content.StartsWith("/* =========================================="))
            {
                // Combine the header with the existing code
                string newContent = HEADER_TEXT + content;
                File.WriteAllText(filePath, newContent);
                modifiedCount++;
            }
        }

        // Refresh the Unity Asset Database to apply the changes in the editor
        AssetDatabase.Refresh();

        Debug.Log($"<b>[Header Tool]</b> Process completed. The header has been added to {modifiedCount} scripts.");
        EditorUtility.DisplayDialog("Headers Added", $"Successfully updated {modifiedCount} scripts.", "OK");
    }
}
#endif

