using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;

public class Room2 : MonoBehaviour
{
    public GameObject roomView;
    public GameObject doorView;
    public GameObject shelfView;
    public GameObject safefView;
    public GameObject paintingView;
    public GameObject bedView;
    public GameObject tableView;
    GameObject currentView;

    public GameObject inventorySlots;

    public GameObject splashHint;

    public GameObject roomBulb;
    public GameObject roomHammer;

    public GameObject clueNote;
    public GameObject clueUI;
    public GameObject notebookUI;

    public GameObject padlockBtns;
    public TMP_Text padlockPw;

    public SpriteRenderer safe;
    public Sprite safeOpen;
    public GameObject keyItem;
    public GameObject shelfViewClose;
    public GameObject shelfViewOpen;

    public SpriteRenderer shelf;
    public Sprite shelfOpen;

    public SpriteRenderer paintView;
    public Sprite paintViewWhite;
    public Sprite paintViewPurple;
    public SpriteRenderer bulb;
    public Sprite bulbWhite;
    public Sprite bulbPurple;
    public SpriteRenderer painting;
    public Sprite paintingWhite;
    public Sprite paintingPurple;

    public SpriteRenderer invBulb;
    public Sprite itemBulbWhite;
    public Sprite itemBulbPurple;

    public SpriteRenderer door;
    public Sprite doorOpen;
    public GameObject successUI;

    GameObject interacted;
    GameObject activeItem;
    int pwCount = 0;
    int correctPw = 835;
    //int splashHintCount = 0;
    bool canEscape = false;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
        currentView = roomView;
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
                        interacted.transform.localScale = new Vector2(0.4f, 0.4f);
                        interacted.GetComponent<BoxCollider2D>().enabled = false;
                        break;
                    }
                }

                if (interacted.name == "Bulb")
                {
                    Destroy(roomBulb);
                }
                else if (interacted.name == "Hammer")
                {
                    Destroy(roomHammer);
                }
            }

            // (un)select item in inventory
            if (interacted.tag == "Inventory" && interacted.transform.childCount > 0)
            {
                // select
                if (!activeItem)
                {
                    activeItem = Instantiate(interacted.transform.GetChild(0).gameObject, mousePos, Quaternion.identity); //need change to ui? (in front of everything)
                    activeItem.transform.localScale = new Vector3(activeItem.transform.localScale.x * 2/3, activeItem.transform.localScale.y * 2 / 3, activeItem.transform.localScale.z * 2 / 3);

                    if (activeItem.name.Contains("Bulb"))
                    {
                        invBulb = interacted.transform.GetChild(0).GetComponent<SpriteRenderer>();
                    }
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
                        activeItem.transform.localScale = new Vector3(activeItem.transform.localScale.x * 2 / 3, activeItem.transform.localScale.y * 2 / 3, activeItem.transform.localScale.z * 2 / 3);
                    }
                }

                activeItem.GetComponent<SpriteRenderer>().sortingOrder = 15;
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

        if (Input.GetMouseButtonDown(0) && hit.collider != null)
        {
            if (interacted.name == "Notes")
            {
                notebookUI.SetActive(true);
            }

            if (interacted.name == "ClueNote")
            {
                clueUI.SetActive(true);
            }
        }
    }

    void ViewChange()
    {
        currentView.SetActive(false);

        if (interacted.name == "Door")
        {
            if (canEscape == true)
            {
                successUI.SetActive(true);
                Time.timeScale = 0;
            }
            else
            {
                currentView = doorView;
            }
        }
        else if (interacted.name == "Shelf")
        {
            currentView = shelfView;
        }
        else if (interacted.name.Contains("Safe"))
        {
            currentView = safefView;
        }
        else if (interacted.name == "Painting")
        {
            currentView = paintingView;
        }
        else if (interacted.name == "Table" || interacted.name == "Chair")
        {
            currentView = tableView;
        }
        else if (interacted.name == "Bed" || interacted.name == "Piggy")
        {
            currentView = bedView;
        }

        if (interacted.name == "BackBtn")
        {
            currentView.SetActive(false);

            if (currentView == safefView)
            {
                currentView = shelfView;
            }
            else
            {
                currentView = roomView;
            }

            currentView.SetActive(true);
        }
        else
        {
            currentView.SetActive(true);
        }
    }

    void ItemUse()
    {
        if (interacted.name == "Piggy")
        {
            if (activeItem != null && activeItem.name.Contains("Hammer"))
            {
                clueNote.SetActive(true);
                Destroy(interacted);
            }
            //else
            //{
            //    splashHintCount++;
            //    Debug.Log("Hint Count: " + splashHintCount);
            //    if (splashHintCount > 2)
            //    {
            //        StartCoroutine(HintDisplay(splashHint));
            //    }
            //}
        }
        if (interacted.name == "Painting Light")
        {
            if (activeItem != null && activeItem.name.Contains("Bulb"))
            {
                //Destroy(interacted);
                if (bulb.sprite == bulbWhite)
                {
                    bulb.sprite = bulbPurple;
                    painting.sprite = paintingPurple;

                    paintView.sprite = paintViewPurple;
                    paintView.gameObject.transform.localScale = new Vector2(2.22f, 2.215f);

                    invBulb.sprite = itemBulbWhite;
                    activeItem.GetComponent<SpriteRenderer>().sprite = itemBulbWhite;
                }
                else if (bulb.sprite == bulbPurple)
                {
                    bulb.sprite = bulbWhite;
                    painting.sprite = paintingWhite;

                    paintView.sprite = paintViewWhite;
                    paintView.gameObject.transform.localScale = new Vector2(2.35f, 2.34f);

                    invBulb.sprite = itemBulbPurple;
                    activeItem.GetComponent<SpriteRenderer>().sprite = itemBulbPurple;
                }
            }
            //else
            //{
            //    splashHintCount++;
            //    Debug.Log("Hint Count: " + splashHintCount);
            //    if (splashHintCount > 2)
            //    {
            //        StartCoroutine(HintDisplay(splashHint));
            //    }
            //}
        }
        if (interacted.name == "Lock")
        {
            if (activeItem != null && activeItem.name.Contains("Key"))
            {
                canEscape = true;
                StartCoroutine(KeySuccess());
            }
            //else
            //{
            //    splashHintCount++;
            //    Debug.Log("Hint Count: " + splashHintCount);
            //    if (splashHintCount > 2)
            //    {
            //        StartCoroutine(HintDisplay(splashHint));
            //    }
            //}
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
                    //canEscape = true;
                    StartCoroutine(SafeSuccess());
                }
                else
                {
                    Debug.Log("failed...");
                    StartCoroutine(ShortDisplay());

                }
            }
        }
    }

    IEnumerator SafeSuccess()
    {
        padlockPw.color = Color.green;
        yield return new WaitForSeconds(1);
        padlockBtns.SetActive(false);
        padlockPw.gameObject.SetActive(false);
        safe.sprite = safeOpen;
        safe.gameObject.transform.localScale = new Vector2(2.3f, 2.175f);
        keyItem.SetActive(true);
        shelf.sprite = shelfOpen;
        shelfViewClose.SetActive(false);
        shelfViewOpen.SetActive(true);
        //door.sprite = doorOpen;
        //roomView.SetActive(true);
        //doorView.SetActive(false);


    }
    IEnumerator KeySuccess()
    {
        //padlockPw.color = Color.green;
        yield return new WaitForSeconds(1);
        door.sprite = doorOpen;
        roomView.SetActive(true);
        doorView.SetActive(false);
        currentView = roomView;

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
    public void GoBack()
    {
        EventSystem.current.currentSelectedGameObject.transform.parent.gameObject.SetActive(false);
    }
}
