using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

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
    private bool enabled = true;         

    [SerializeField] private float howLongToDisappear = 1.5f; 

    private List<int> connectedCircleIDs;

    public RightPerson rightPerson;

    public float lineThickness = 2.0f;


    void Start()
    {
        circles = new Dictionary<int, CircleIdentifier>();
        lines = new List<CircleIdentifier>();
        connectedCircleIDs = new List<int>();

        // Assign IDs to circles and initialize them
        for (int i = 0; i < transform.childCount; i++)
        {
            var circle = transform.GetChild(i);

            var identifier = circle.GetComponent<CircleIdentifier>();
            identifier.id = i;

            circles.Add(i, identifier);

            // Disable the Animator component
            Animator circleAnimator = circle.GetComponent<Animator>();
            if (circleAnimator != null)
            {
                circleAnimator.enabled = false;
            }
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

            lineOnEditRcTs.sizeDelta = new Vector2(lineThickness, distance);
            lineOnEditRcTs.anchoredPosition = startPos;
            lineOnEditRcTs.rotation = Quaternion.FromToRotation(Vector3.up, direction.normalized);
        }
    }

    GameObject CreateLine(Vector2 pos, int id)
    {
        var line = Instantiate(linePrefab, canvas.transform);

        var lineRcTs = line.GetComponent<RectTransform>();
        lineRcTs.pivot = new Vector2(0.5f, 0f); // Set pivot to top-center
        lineRcTs.anchoredPosition = pos;

        lineRcTs.sizeDelta = new Vector2(lineThickness, 0f);

        var lineIdentifier = line.AddComponent<CircleIdentifier>();
        lineIdentifier.id = id;

        // Disable the Animator component
        Animator lineAnimator = line.GetComponent<Animator>();
        if (lineAnimator != null)
        {
            lineAnimator.enabled = false;
        }

        lines.Add(lineIdentifier);

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

        // Add the circle ID to the connectedCircleIDs list
        connectedCircleIDs.Add(circle.id);

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
            circle.Value.GetComponent<Image>().color = Color.white;
            circle.Value.GetComponent<Animator>().enabled = false;
        }

        foreach (var line in lines)
        {
            Destroy(line.gameObject);
        }

        lines.Clear();
        connectedCircleIDs.Clear(); // Clear the list after use

        lineOnEdit = null;
        lineOnEditRcTs = null;
        circleOnEdit = null;

        enabled = true;
    }

    void EnableColorFade(Animator anim)
    {
        if (anim != null)
        {
            anim.enabled = true;
            anim.Rebind();
        }
    }

    public void OnMouseExitCircle(CircleIdentifier idf)
    {
        if (!enabled)
        {
            return;
        }
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

            lineOnEditRcTs.sizeDelta = new Vector2(lineThickness, distance);
            lineOnEditRcTs.anchoredPosition = startPos;
            lineOnEditRcTs.rotation = Quaternion.FromToRotation(Vector3.up, direction.normalized);

            TrySetLineEdit(idf);

            // Update the sprite in RightPerson with a delay
            rightPerson.UpdateSpriteWithDelay(idf.id);
        }
    }


    public void OnMouseDownCircle(CircleIdentifier idf)
    {
        if (!enabled)
        {
            return;
        }

        unlocking = true;

        // Clear the connectedCircleIDs list
        connectedCircleIDs.Clear();

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
            // Enable animations on circles
            foreach (var line in lines)
            {
                Animator circleAnimator = circles[line.id].GetComponent<Animator>();
                EnableColorFade(circleAnimator);
            }

            // Enable animation on the last circle (circleOnEdit)
            if (circleOnEdit != null)
            {
                Animator lastCircleAnimator = circleOnEdit.GetComponent<Animator>();
                EnableColorFade(lastCircleAnimator);
            }

            // Enable animations on lines
            foreach (var line in lines)
            {
                Animator lineAnimator = line.GetComponent<Animator>();
                EnableColorFade(lineAnimator);
            }

            // Remove the last line (the one still being drawn)
            if (lines.Count > 0)
            {
                Destroy(lines[lines.Count - 1].gameObject);
                lines.RemoveAt(lines.Count - 1);
            }

            // Get the connected pattern
            int[] playerPattern = GetConnectedPattern();

            // Check the pattern with the SpellManager
            SpellManager spellManager = FindObjectOfType<SpellManager>();
            if (spellManager != null)
            {
                // Check if the pattern matches any spell
                bool spellMatched = spellManager.CheckPattern(playerPattern);
                if (!spellMatched)
                {
                    spellManager.OnSpellFailed();
                }
            }

            StartCoroutine(Release());
        }

        unlocking = false;
    }

    public int[] GetConnectedPattern()
    {
        // Return the connected circle IDs as an array
        return connectedCircleIDs.ToArray();
    }

    // Helper method to get the local point in the Canvas's coordinate space
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
