using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIElementDragger : EventTrigger
{

    private bool dragging;

    private GameObject putBounds;
    private RectTransform rt;
    public Vector3 startPos;

    private UIElementDragView uiedv;
    private GameObject roadArea;
    //private RectTransform cloneRt;
    public void Start()
    {
        putBounds = GameObject.Find("Dotted Boundary");
        //startPos = Camera.main.ScreenToWorldPoint(transform.position);
        rt = GetComponent<RectTransform>();
        startPos = rt.position;
        //cloneRt = Instantiate(rt);
        //Debug.Log("Start Position is " + transform.position);
        uiedv = GetComponent<UIElementDragView>();
        roadArea = GameObject.Find("RoadArea");
    }

    public void Update()
    {

        if (dragging)
        {
            transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            //Debug.Log("Position is " + transform.position);
            //transform.position = Vector3.zero;
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        dragging = true;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        dragging = false;
        if (putBounds.transform.position.y < Camera.main.ScreenToWorldPoint(new Vector3(transform.position.x, transform.position.y - GetComponent<CircleCollider2D>().radius, transform.position.z)).y)
        {
            GameObject temp = Instantiate(uiedv.reference, Camera.main.ScreenToWorldPoint(transform.position), Quaternion.identity, roadArea.transform);
            temp.transform.position = new Vector3(temp.transform.position.x, temp.transform.position.y, 0f);
            //temp.transform.localScale = new Vector3(1f, 1f, 1f);

        }
        else
        {
            
            //Destroy(rt);
            //gameObject.AddComponent<RectTransform>();
            //GetComponent<RectTransform>().sizeDelta = new Vector2(2.0f, 2.0f);
            //transform.position = transform.parent.gameObject.GetComponent<RectTransform>().anchoredPosition;
            //rt.Reset();

            //rt.position = rt.anchoredPosition;
            //rt.position = startPos;
            //Debug.Log("Now Position is " + transform.position);
            //rt = cloneRt;
            //transform.position = startPos;//new Vector3(960, 194);//Camera.main.WorldToScreenPoint(startPos);

        }
        rt.localPosition = Vector3.zero;
    }
}