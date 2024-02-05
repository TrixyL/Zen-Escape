using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameInteractions : MonoBehaviour
{
    public GameObject roomView;
    public GameObject doorView;
    public GameObject inventorySlots;
    public GameObject splashHint;
    public GameObject padlockBtns;
    public Text padlockPw;
    public SpriteRenderer door;
    public Sprite doorOpen;
    public GameObject successUI;

    GameObject interacted;
    GameObject activeItem;
    int pwCount = 0;
    int correctPw = 427;
    int splashHintCount = 0;
    bool canEscape = false;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        // mouse click + collision + no UI element
        if (Input.GetMouseButtonDown(0) && hit.collider != null && !EventSystem.current.IsPointerOverGameObject())
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
        if (activeItem != null && Time.timeScale != 0)
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
                Time.timeScale = 0;
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
        if (interacted.name == "Splash1" || interacted.name == "Splash2" || interacted.name == "Splash3")
        {
            if (activeItem != null && activeItem.name.Contains("Towels"))
            {
                Destroy(interacted);
            }
            else
            {
                splashHintCount++;
                Debug.Log("Hint Count: " + splashHintCount);
                if (splashHintCount > 2)
                {
                    StartCoroutine(HintDisplay(splashHint));
                }
            }
        }
    }

    void InputCode()
    {
        if (interacted.transform.IsChildOf(padlockBtns.transform) && pwCount < 3)
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

    IEnumerator HintDisplay(GameObject hint)
    {
        hint.SetActive(true);
        yield return new WaitForSeconds(1);
        hint.SetActive(false);

    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
