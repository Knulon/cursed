using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements.Experimental;

public class LetterRotateScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Start is called before the first frame update

    [SerializeField]
    GameObject zoomButton;

    float rotation = 0;
    bool rotate = false;

    float zoom = 0;
    bool zoomIn = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!zoomIn)
        {
            Vector2 scale = zoomButton.transform.localScale;
            if (scale.x == 0)
            {
                zoomButton.transform.localScale = new Vector2(0.2f, 0.2f);
            }
            else
            {
                zoomButton.transform.localScale = scale * 1.3f;
            }
        }
        zoomIn = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        rotate = true;
        zoomIn = false;
        transform.localScale = Vector3.one;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (rotate)
        {
            if(rotation > 1)
            {
                rotate = false;
                rotation = 0;
                return;
            }
            transform.rotation = Quaternion.Euler(new Vector3(0,0,Easing.InOutCubic(rotation)*360));
            rotation += Time.deltaTime;
        }
        
        if (zoomIn)
        {
            if(zoom > 0.2)
            {
                zoomIn = false;
                zoom = 0;
                return;
            }
            float val = 1 + Easing.InOutCubic(zoom*2);
            transform.localScale = new Vector3(val,val);
            zoom += Time.deltaTime;
        }
    }
}
