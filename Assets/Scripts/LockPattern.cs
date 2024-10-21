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
    private RectTransform lineOnEditRcTs;
    private CircleIdentifier circleOnEdit;

    private bool unlocking;
    new bool enabled = true;

    [SerializeField] private float howLongToDisappear = 1.5f;

    void Start()
    {
        circles = new Dictionary<int, CircleIdentifier>();
        lines = new List<CircleIdentifier>();

        // Assign IDs
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
        if (!enabled)
        {
            return;
        }

        if (unlocking)
        {
            Vector2 mousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.GetComponent<RectTransform>(),
                Input.mousePosition,
                canvas.worldCamera,
                out mousePos
            );

            Vector2 startPos = GetLocalPointInCanvas(circleOnEdit.transform);
            Vector2 direction = mousePos - startPos;
            float distance = direction.magnitude;

            lineOnEditRcTs.sizeDelta = new Vector2(lineOnEditRcTs.sizeDelta.x, distance);
            lineOnEditRcTs.anchoredPosition = startPos;
            lineOnEditRcTs.rotation = Quaternion.FromToRotation(Vector3.up, direction.normalized);
        }
    }

    GameObject CreateLine(Vector2 pos, int id)
    {
        var line = GameObject.Instantiate(linePrefab, canvas.transform);

        var lineRcTs = line.GetComponent<RectTransform>();
        lineRcTs.pivot = new Vector2(0.5f, 0f); // Set pivot to top-center
        lineRcTs.anchoredPosition = pos;

        var lineidf = line.AddComponent<CircleIdentifier>();
        lineidf.id = id;

        lines.Add(lineidf);

        return line;
    }

    void TrySetLineEdit(CircleIdentifier circle)
    {
        foreach (var line in lines)
        {
            if (line.id == circle.id)
            {
                return;
            }
        }

        Vector2 startPos = GetLocalPointInCanvas(circle.transform);
        lineOnEdit = CreateLine(startPos, circle.id);
        lineOnEditRcTs = lineOnEdit.GetComponent<RectTransform>();
        circleOnEdit = circle;
    }

    IEnumerator Release()
    {
        enabled = false;

        yield return new WaitForSeconds(howLongToDisappear);

        foreach (var circle in circles)
        {
            circle.Value.GetComponent<UnityEngine.UI.Image>().color = Color.white;
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
        if (!enabled)
        {
            return;
        }

        if (unlocking)
        {
            Vector2 startPos = GetLocalPointInCanvas(circleOnEdit.transform);
            Vector2 endPos = GetLocalPointInCanvas(idf.transform);
            Vector2 direction = endPos - startPos;
            float distance = direction.magnitude;

            lineOnEditRcTs.sizeDelta = new Vector2(lineOnEditRcTs.sizeDelta.x, distance);
            lineOnEditRcTs.anchoredPosition = startPos;
            lineOnEditRcTs.rotation = Quaternion.FromToRotation(Vector3.up, direction.normalized);

            TrySetLineEdit(idf);
        }
    }

    public void OnMouseExitCircle(CircleIdentifier idf)
    {
        if (!enabled)
        {
            return;
        }
    }

    public void OnMouseDownCircle(CircleIdentifier idf)
    {
        if (!enabled)
        {
            return;
        }

        unlocking = true;

        TrySetLineEdit(idf);
    }

    public void OnMouseUpCircle(CircleIdentifier idf)
    {
        if (!enabled)
        {
            return;
        }

        if (unlocking)
        {
            foreach (var line in lines)
            {
                EnableColorFade(circles[line.id].gameObject.GetComponent<Animator>());
            }

            Destroy(lines[lines.Count - 1].gameObject);
            lines.RemoveAt(lines.Count - 1);

            foreach (var line in lines)
            {
                EnableColorFade(line.GetComponent<Animator>());
            }

            StartCoroutine(Release());
        }

        unlocking = false;
    }

    // Helper function to get the local point in the Canvas's coordinate space
    Vector2 GetLocalPointInCanvas(Transform objTransform)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, objTransform.position),
            canvas.worldCamera,
            out localPoint
        );
        return localPoint;
    }
}
