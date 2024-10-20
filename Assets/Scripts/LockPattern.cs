using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockPattern : MonoBehaviour
{
    public GameObject linePrefab;
    public Canvas canvas;

    private Dictionary<int, CircleIdentifier> circles;

    private List<CircleIdentifier> lines;

    private GameObject lineOnEdit;
    private RectTransform lineOnEditRects;
    private CircleIdentifier circleOnEdit;

    private bool unlocking;

    // Start is called before the first frame update
    void Start()
    {
        circles = new Dictionary<int, CircleIdentifier>();
        lines = new List<CircleIdentifier>();

        for (int i = 0; i < transform.childCount; i++)
        {
            var circle = transform.GetChild(i);

            var identifier = circle.GetComponent<CircleIdentifier>();

            identifier.id = i;

            circles.Add(i, identifier);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    GameObject CreateLine(Vector3 pos, int id)
    {
        var line = GameObject.Instantiate(linePrefab, canvas.transform);

        line.transform.localPosition = pos;

        var lineidf = line.AddComponent<CircleIdentifier>();

        lineidf.id = id;

        lines.Add(lineidf);

        return line;
    }

    void TrySetLineEdit(CircleIdentifier circle)
    {
        foreach(var line in lines)
        {
            if(line.id == circle.id)
            {
                return;
            }
        }


    }

    public void OnMouseEnterCircle(CircleIdentifier idf)
    {
        Debug.Log(idf.id);
    }

    public void OnMouseExitCircle(CircleIdentifier idf)
    {
        Debug.Log(idf.id);

    }

    public void OnMouseDownCircle(CircleIdentifier idf)
    {
        Debug.Log(idf.id);

        unlocking = true;

        TrySetLineEdit(idf);

    }

    public void OnMouseUpCircle(CircleIdentifier idf)
    {
        Debug.Log(idf.id);

        unlocking = false;
    }
}
