using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockPattern : MonoBehaviour
{
    public GameObject linePrefab;
    public Canvas canvas;

    private Dictionary<int, CircleIdentifier> circles;

    private List<CircleIdentifier> lines;

    private GameObject lineOnEdit; //current active line
    private RectTransform lineOnEditRcTs; //RectTransform of the active line, used for positioning and rotation
    private CircleIdentifier circleOnEdit; //circle currently being connected by the line

    private bool unlocking;

    new bool enabled = true;

    [SerializeField] private float howLongToDisappear = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        circles = new Dictionary<int, CircleIdentifier>();
        lines = new List<CircleIdentifier>();

        //assign ID
        for (int i = 0; i < transform.childCount; i++)
        {
            var circle = transform.GetChild(i);

            var identifier = circle.GetComponent<CircleIdentifier>();

            identifier.id = i;

            circles.Add(i, identifier);
        }
    }

    void Update()
    {
        if(enabled == false)
        {
            return;
        }

        if(unlocking)
        {
            Vector3 mousePos = canvas.transform.InverseTransformPoint(Input.mousePosition);

            lineOnEditRcTs.sizeDelta = new Vector2(lineOnEditRcTs.sizeDelta.x, Vector3.Distance(mousePos, circleOnEdit.transform.localPosition));

            lineOnEditRcTs.rotation = Quaternion.FromToRotation(Vector3.up,
                (mousePos - circleOnEdit.transform.localPosition).normalized);
        }
    }

    //instantiates a new line, and stores it in the lines list.
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

        lineOnEdit = CreateLine(circle.transform.localPosition, circle.id);
        lineOnEditRcTs = lineOnEdit.GetComponent<RectTransform>();
        circleOnEdit = circle;
    }

    //destroy and resets the line editing references
    IEnumerator Release()
    {
        print("Release being called");
        enabled = false;

        yield return new WaitForSeconds(howLongToDisappear);

        foreach(var circle in circles)
        {
            circle.Value.GetComponent<UnityEngine.UI.Image>().color = Color.white; //Back to [WHITE]
            circle.Value.GetComponent<Animator>().enabled = false;
        }


        foreach (var line in lines)
        {
            Destroy(line.gameObject);
        }

        lines.Clear();

        lineOnEdit = null;
        lineOnEditRcTs = null;
        circleOnEdit = null;

        enabled = true;
    }

    void EnableColorFade(Animator anim)
    {
        anim.enabled = true;
        anim.Rebind();
    }

    public void OnMouseEnterCircle(CircleIdentifier idf)
    {
        print("On mouse Enter CIrcle");

        if (enabled == false)
        {
            return;
        }

        if (unlocking)
        {
            lineOnEditRcTs.sizeDelta = new Vector2(lineOnEditRcTs.sizeDelta.x, Vector3.Distance(circleOnEdit.transform.localPosition, idf.transform.localPosition));
            lineOnEditRcTs.rotation = Quaternion.FromToRotation(
                Vector3.up,
                (idf.transform.localPosition - circleOnEdit.transform.localPosition).normalized
            );

            TrySetLineEdit(idf);
        }

    }

    public void OnMouseExitCircle(CircleIdentifier idf)
    {
        if (enabled == false)
        {
            return;
        }
    }

    public void OnMouseDownCircle(CircleIdentifier idf)
    {
        print("On mouse down CIrcle");


        if (enabled == false)
        {
            return;
        }

        unlocking = true;

        TrySetLineEdit(idf);

    }

    public void OnMouseUpCircle(CircleIdentifier idf)
    {
        if (enabled == false)
        {
            return;
        }

        if (unlocking)
        {
            foreach(var line in lines)
            {
                EnableColorFade(circles[line.id].gameObject.GetComponent<Animator>());
            }

            Destroy(lines[lines.Count - 1].gameObject);

            lines.RemoveAt(lines.Count - 1);

            foreach(var line in lines)
            {
                EnableColorFade(line.GetComponent<Animator>());
            }

            StartCoroutine(Release());
        }

        unlocking = false;


    }
}
