using UnityEngine;
using UnityEditor;
//https://wiki.unity3d.com/index.php/TransformUtilities

public class TransformUtilitiesWindow : EditorWindow
{
    //Window control values
    public int toolbarOption = 0;
    public string[] toolbarTexts = { "Align", "Copy", "Randomize", "Add noise" };

    private bool xCheckbox = true;
    private bool yCheckbox = true;
    private bool zCheckbox = true;

    private Transform source;
    private float randomRangeMin = 0f;
    private float randomRangeMax = 1f;

    /// <summary>
    /// Window drawing operations
    /// </summary>
    void OnGUI()
    {
        CreateAxisCheckboxes("Randomize");
        CreateRandomizeTransformWindow();
    }

    /// <summary>
    /// Draws the 3 axis checkboxes (x y z)
    /// </summary>
    /// <param name="operationName"></param>
    private void CreateAxisCheckboxes(string operationName)
    {
        GUILayout.Label(operationName + " on axis", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();
        xCheckbox = GUILayout.Toggle(xCheckbox, "X");
        yCheckbox = GUILayout.Toggle(yCheckbox, "Y");
        zCheckbox = GUILayout.Toggle(zCheckbox, "Z");
        GUILayout.EndHorizontal();

        EditorGUILayout.Space();
    }

    /// <summary>
    /// Draws the range min and max fields
    /// </summary>
    private void CreateRangeFields()
    {
        GUILayout.Label("Range", EditorStyles.boldLabel);
        randomRangeMin = EditorGUILayout.FloatField("Min:", randomRangeMin);
        randomRangeMax = EditorGUILayout.FloatField("Max:", randomRangeMax);
        EditorGUILayout.Space();
    }


    /// <summary>
    /// Creates the Randomize transform window
    /// </summary>
    private void CreateRandomizeTransformWindow()
    {
        CreateRangeFields();

        //Position
        if (GUILayout.Button("Randomize Position"))
        {
            foreach (Transform t in Selection.transforms)
            {
                Vector3 tmp = new Vector3();
                tmp.x = xCheckbox ? Random.Range(randomRangeMin, randomRangeMax) : t.position.x;
                tmp.y = yCheckbox ? Random.Range(randomRangeMin, randomRangeMax) : t.position.y;
                tmp.z = zCheckbox ? Random.Range(randomRangeMin, randomRangeMax) : t.position.z;

                Undo.RegisterCompleteObjectUndo(t, "Randomize position");
                t.position = tmp;
            }
        }

        //Rotation
        if (GUILayout.Button("Randomize Rotation"))
        {
            foreach (Transform t in Selection.transforms)
            {
                Vector3 tmp = new Vector3();
                tmp.x = xCheckbox ? Random.Range(randomRangeMin, randomRangeMax) : t.rotation.eulerAngles.x;
                tmp.y = yCheckbox ? Random.Range(randomRangeMin, randomRangeMax) : t.rotation.eulerAngles.y;
                tmp.z = zCheckbox ? Random.Range(randomRangeMin, randomRangeMax) : t.rotation.eulerAngles.z;
                Quaternion tmp2 = t.rotation;
                tmp2.eulerAngles = tmp;

                Undo.RegisterCompleteObjectUndo(t, "Randomize rotation");
                t.rotation = tmp2;
            }
        }

        //Local Scale
        if (GUILayout.Button("Randomize Local Scale"))
        {
            foreach (Transform t in Selection.transforms)
            {
                Vector3 tmp = new Vector3();
                tmp.x = xCheckbox ? Random.Range(randomRangeMin, randomRangeMax) : t.localScale.x;
                tmp.y = yCheckbox ? Random.Range(randomRangeMin, randomRangeMax) : t.localScale.y;
                tmp.z = zCheckbox ? Random.Range(randomRangeMin, randomRangeMax) : t.localScale.z;

                Undo.RegisterCompleteObjectUndo(t, "Randomize local scale");
                t.localScale = tmp;
            }
        }
    }

    /// <summary>
    /// Retrives the TransformUtilities window or creates a new one
    /// </summary>
    [MenuItem("Window/TransformUtilities %t")]
    static void Init()
    {
        TransformUtilitiesWindow window = (TransformUtilitiesWindow)EditorWindow.GetWindow(typeof(TransformUtilitiesWindow));
        window.Show();
    }
}
