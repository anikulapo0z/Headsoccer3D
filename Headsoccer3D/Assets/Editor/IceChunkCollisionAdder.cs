using UnityEditor;
using UnityEngine;

public class IceChunkCollisionAdder : MonoBehaviour
{
    [MenuItem("Tools/Ice Crack Collision Adder")]
    static void ProcessSelected()
    {
        GameObject[] selected = Selection.gameObjects;

        if (selected.Length == 0)
        {
            return;
        }

        Undo.IncrementCurrentGroup();
        int group = Undo.GetCurrentGroup();

        for (int i = 0; i < selected.Length; i++)
        {
            GameObject original = selected[i];

            //if null or no mesh, skip this
            if (original == null) continue;
            MeshFilter mf = original.GetComponent<MeshFilter>();
            if (mf == null && mf.sharedMesh == null) continue;

            // Rename original object
            string newName = "IceCrack_" + (i + 1);
            Undo.RecordObject(original, "Rename Object");
            original.name = newName;

            // reate new object for collision
            GameObject child = new GameObject(newName + "_Collision");
            Undo.RegisterCreatedObjectUndo(child, "Create Collision Object");
            //and set as the child
            child.transform.parent = original.transform;
            //add the mesh collider
            MeshCollider collider = child.AddComponent<MeshCollider>();
            collider.sharedMesh = mf.sharedMesh;

        }

        Undo.CollapseUndoOperations(group);
    }
}
