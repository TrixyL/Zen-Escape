using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameInteractions : MonoBehaviour
{
    public GameObject roomView;
    public GameObject doorView;
    public GameObject inventorySlots;
    public GameObject padlockBtns;
    public Text padlockPw;
    public SpriteRenderer door;
    public Sprite doorOpen;
    public GameObject successUI;

    GameObject interacted;
    GameObject activeItem;
    int pwCount = 0;
    int correctPw = 427;
    bool canEscape = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (Input.GetMouseButtonDown(0) && hit.collider != null)
        {
            Debug.Log(hit.collider);
            interacted = hit.collider.gameObject;

            // put game items into inventory
            if (interacted.tag == "Item")
            {
                foreach (Transform child in inventorySlots.transform)
                {
                    if (child.childCount < 1)
                    {
                        interacted.transform.position = child.position;
                        interacted.transform.parent = child.transform;
                        interacted.GetComponent<BoxCollider2D>().enabled = false;
                        break;
                    }
                }
            }

            // (un)select item in inventory
            if (interacted.tag == "Inventory" && interacted.transform.childCount > 0)
            {
                // select
                if (!activeItem)
                {
                    activeItem = Instantiate(interacted.transform.GetChild(0).gameObject, mousePos, Quaternion.identity); //need change to ui? (in front of everything)
                    activeItem.transform.localScale = new Vector3(activeItem.transform.localScale.x / 2, activeItem.transform.localScale.y / 2, activeItem.transform.localScale.z / 2);
                }
                else
                {
                    // unselect
                    if (activeItem.name.Contains(interacted.transform.GetChild(0).name))
                    {
                        Destroy(activeItem);
                    }

                    // select another
                    else
                    {
                        Destroy(activeItem);
                        activeItem = Instantiate(interacted.transform.GetChild(0).gameObject, mousePos, Quaternion.identity); //need change to ui? (in front of everything)
                        activeItem.transform.localScale = new Vector3(activeItem.transform.localScale.x / 2, activeItem.transform.localScale.y / 2, activeItem.transform.localScale.z / 2);
                    }
                }
            }

            ViewChange();

            ItemUse();

            InputCode();
        }

        // active item follow mouse
        if (activeItem != null)
        {
            activeItem.transform.position = mousePos;
        }
    }

    void ViewChange()
    {
        if (interacted.name == "Door")
        {
            if (canEscape == false)
            {
                doorView.SetActive(true);
                roomView.SetActive(false);
            }
            else
            {
                successUI.SetActive(true);
            }
        }

        if (interacted.name == "BackBtn")
        {
            roomView.SetActive(true);
            doorView.SetActive(false);
        }
    }

    void ItemUse()
    {
        if (activeItem != null && activeItem.name.Contains("Towels") && (interacted.name == "Splash1" || interacted.name == "Splash2" || interacted.name == "Splash3"))
        {
            Destroy(interacted);
        }
    }

    void InputCode()
    {
        if (interacted.transform.IsChildOf(padlockBtns.transform) && pwCount < 4)
        {
            int numPressed = interacted.transform.GetSiblingIndex() + 1;
            padlockPw.text += numPressed;

            pwCount++;

            if (pwCount == 3)
            {
                if (int.Parse(padlockPw.text) == correctPw)
                {
                    Debug.Log("success!");
                    canEscape = true;
                    StartCoroutine(EscapeSuccess());
                }
                else
                {
                    Debug.Log("failed...");
                    StartCoroutine(ShortDisplay());

                }
            }
        }
    }

    IEnumerator EscapeSuccess()
    {
        padlockPw.color = Color.green;
        yield return new WaitForSeconds(1);
        door.sprite = doorOpen;
        roomView.SetActive(true);
        doorView.SetActive(false);

    }

    IEnumerator ShortDisplay()
    {
        padlockPw.color = Color.red;
        yield return new WaitForSeconds(1);
        padlockPw.text = "";
        pwCount = 0;
        padlockPw.color = Color.black;

    }
}
