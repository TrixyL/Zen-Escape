using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;

public class Room2 : MonoBehaviour
{
    GameObject currentView;

    public GameObject inventorySlots;

    public GameObject booksPos;
    public Sprite bookRed1;
    public Sprite bookRed2;
    public Sprite bookRed3;
    public Sprite bookGreen1;
    public Sprite bookGreen2;
    public Sprite bookGreen3;
    public Sprite bookBlue1;
    public Sprite bookBlue2;
    public Sprite bookBlue3;

    public GameObject plantsPos;
    public GameObject testPlant;


    public GameObject planetsPos;

    public GameObject keyPrefab;
    public GameObject clueNotePrefab;

    public GameObject splashHint;

    public Sprite clueNoteSprite;
    public GameObject clueUI;
    public GameObject notebookUI;

    public TMP_Text padlockPw;

    public Sprite safeOpenSprite;
    public Sprite keySprite;
    public Sprite paintingLightWhiteSprite;
    public Sprite paintingLightPurpleSprite;
    //public Sprite paintingWhiteSprite;
    //public Sprite paintingPurpleSprite;

    public SpriteRenderer invBulb;
    public Sprite itemBulbWhiteSprite;
    public Sprite itemBulbPurpleSprite;

    public Sprite doorOpenSprite;
    public GameObject successUI;

    GameObject interacted;
    GameObject activeItem;
    int pwCount = 0;
    string correctPw;
    //int splashHintCount = 0;
    bool canEscape = false;

    Dictionary<string, int> hintsCount = new Dictionary<string, int>();
    public GameObject hintsPos;
    public GameObject hintPrefab;
    List<string> hintsList = new List<string>();

    public Sprite piggySprite;
    public Sprite paintingSprite;
    public Sprite doorSprite;
    public Sprite safeSprite;


    public Camera cam2;

    Vector3 zoomPos;
    Vector3 zoomPosSpeed = Vector3.zero;
    float zoomScale;
    float zoomSpeed;
    float zoomTime = 0.5f;

    float roomScale;
    Vector3 roomPos;
    //bool backtoroom = false;

    bool isZooming = false;
    RaycastHit2D hit;
    Vector2 mousePos;
    //Vector2 testPos;

    AudioManager am;

    // Start is called before the first frame update
    void Start()
    {
        BookPass();
        PlantPass();
        PaintingPass();

        am = AudioManager.instance;

        Time.timeScale = 1;

        roomScale = cam2.orthographicSize;
        roomPos = cam2.transform.position;
        zoomScale = roomScale;
        zoomPos = roomPos;
    }

    // Update is called once per frame
    void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Camera.main.ScreenToViewportPoint(Input.mousePosition).x > 0.85f)
        {
            mousePos = new Vector2(mousePos.x, mousePos.y);
        }
        else if (cam2.orthographicSize == roomScale)
        {
            mousePos = new Vector2(mousePos.x, mousePos.y - 12);
        }
        else
        {
            float xOff = roomPos.x - zoomPos.x - 0.04f;
            float yOff = roomPos.y - zoomPos.y;
            float scaleOff = zoomScale / roomScale;
            mousePos = new Vector2(mousePos.x * scaleOff - xOff, mousePos.y * scaleOff - 12 - yOff);
        }

        hit = Physics2D.Raycast(mousePos, Vector2.zero);

        // mouse click + collision + no UI element  && !EventSystem.current.IsPointerOverGameObject()
        if (Input.GetMouseButtonDown(0) && hit.collider != null && Time.timeScale == 1)
        {
            //Debug.Log(mousePos);
            Debug.Log(hit.collider);

            //Vector2 testPos = cam2.ScreenToWorldPoint(Input.mousePosition);
            //GameObject idk = Instantiate(roomBulb, mousePos, Quaternion.identity);
            //idk.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

            interacted = hit.collider.gameObject;

            if (interacted.tag == "Untagged" || interacted.tag == "Zoom")
            {
                am.PlaySFX(am.sfxClick);
            }

            // put game items into inventory
            if (interacted.tag == "Item")
            {
                am.PlaySFX(am.sfxCollect);

                foreach (Transform child in inventorySlots.transform)
                {
                    if (child.childCount < 1)
                    {
                        interacted.transform.position = child.position;
                        interacted.transform.parent = child.transform;
                        interacted.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                        interacted.GetComponent<BoxCollider2D>().enabled = false;
                        child.tag = "Inventory";
                        break;
                    }
                }
            }

            // (un)select item in inventory
            if (interacted.tag == "Inventory" && interacted.transform.childCount > 0)
            {
                am.PlaySFX(am.sfxCollect);

                // select
                if (!activeItem)
                {
                    activeItem = Instantiate(interacted.transform.GetChild(0).gameObject, mousePos, Quaternion.identity); //need change to ui? (in front of everything)
                    activeItem.transform.localScale = new Vector3(activeItem.transform.localScale.x * 2 / 3, activeItem.transform.localScale.y * 2 / 3, activeItem.transform.localScale.z * 2 / 3);
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

                if (activeItem.name.Contains("Bulb"))
                {
                    invBulb = interacted.transform.GetChild(0).GetComponent<SpriteRenderer>();
                }

                activeItem.GetComponent<SpriteRenderer>().sortingOrder = 15;
            }

            ViewChange();

            ItemUse();

            if (padlockPw != null && padlockPw.gameObject.activeInHierarchy)
            {
                InputCode();
            }

            if (interacted.name == "Notes")
            {
                notebookUI.SetActive(true);
            }

            if (interacted.name == "ClueNote")
            {
                clueUI.SetActive(true);

                am.PlaySFX(am.sfxPaper);
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            am.PlaySFX(am.sfxClick);
        }

        if (notebookUI.activeInHierarchy || clueUI.activeInHierarchy || successUI.activeInHierarchy)
        {
            Time.timeScale = 0;
        }

        // active item follow mouse
        if (activeItem != null && Time.timeScale != 0) // && mousePos.x > -7
        {
            activeItem.transform.position = mousePos;

            float itemScale = 0.4f * 2 / 3;

            if (Camera.main.ScreenToViewportPoint(Input.mousePosition).x > 0.85f)
            {
                activeItem.transform.localScale = new Vector2(itemScale, itemScale);
            }
            else if (activeItem.transform.localScale.x != itemScale * (zoomScale / roomScale))
            {
                activeItem.transform.localScale = new Vector2(itemScale * (zoomScale / roomScale), itemScale * (zoomScale / roomScale));
            }
        }

    }

    void BookPass()
    {
        string[] bookColors = new string[] { "r", "g", "b" };
        int bookRedCount = 0;
        int bookGreenCount = 0;
        int bookBlueCount = 0;

        for (int i = 0; i < 8; i++)
        {
            int color = Random.Range(0, bookColors.Length);
            SpriteRenderer bookSpriteRenderer = booksPos.transform.GetChild(i).GetComponent<SpriteRenderer>();
            if (bookColors[color] == "r")
            {
                if (i == 7)
                {
                    bookSpriteRenderer.sprite = bookRed3;
                }
                else if (i == 6 || i == 5)
                {
                    bookSpriteRenderer.sprite = bookRed2;
                }
                else
                {
                    bookSpriteRenderer.sprite = bookRed1;
                }

                bookRedCount += 1;
            }
            else if (bookColors[color] == "g")
            {
                if (i == 7)
                {
                    bookSpriteRenderer.sprite = bookGreen3;
                }
                else if (i == 6 || i == 5)
                {
                    bookSpriteRenderer.sprite = bookGreen2;
                }
                else
                {
                    bookSpriteRenderer.sprite = bookGreen1;
                }

                bookGreenCount += 1;
            }
            else if (bookColors[color] == "b")
            {
                if (i == 7)
                {
                    bookSpriteRenderer.sprite = bookBlue3;
                }
                else if (i == 6 || i == 5)
                {
                    bookSpriteRenderer.sprite = bookBlue2;
                }
                else
                {
                    bookSpriteRenderer.sprite = bookBlue1;
                }

                bookBlueCount += 1;
            }
        }

        print(bookRedCount);
        print(bookGreenCount);
        print(bookBlueCount);

        correctPw = bookRedCount.ToString();
    }

    void PlantPass()
    {
        int plants = Random.Range(1, 6);

        if (plants == 1)
        {
            Instantiate(testPlant, plantsPos.transform.TransformPoint(1.58f, 1.7f, 0), Quaternion.identity, plantsPos.transform);
        }
        else if (plants == 2)
        {
            Instantiate(testPlant, plantsPos.transform.TransformPoint(0.5f, 1.7f, 0), Quaternion.identity, plantsPos.transform);
            Instantiate(testPlant, plantsPos.transform.TransformPoint(2.7f, 1.7f, 0), Quaternion.identity, plantsPos.transform);
        }
        else if (plants == 3)
        {
            Instantiate(testPlant, plantsPos.transform.TransformPoint(-0.3f, 1.7f, 0), Quaternion.identity, plantsPos.transform);
            Instantiate(testPlant, plantsPos.transform.TransformPoint(1.58f, 1.7f, 0), Quaternion.identity, plantsPos.transform);
            Instantiate(testPlant, plantsPos.transform.TransformPoint(3.46f, 1.7f, 0), Quaternion.identity, plantsPos.transform);
        }
        else if (plants == 4)
        {
            Instantiate(testPlant, plantsPos.transform.TransformPoint(-0.3f, 1.7f, 0), Quaternion.identity, plantsPos.transform);
            Instantiate(testPlant, plantsPos.transform.TransformPoint(1f, 2.3f, 0), Quaternion.identity, plantsPos.transform);
            Instantiate(testPlant, plantsPos.transform.TransformPoint(2.3f, 1.7f, 0), Quaternion.identity, plantsPos.transform);
            Instantiate(testPlant, plantsPos.transform.TransformPoint(3.46f, 2.3f, 0), Quaternion.identity, plantsPos.transform);
        }
        else if (plants == 5)
        {
            Instantiate(testPlant, plantsPos.transform.TransformPoint(-0.3f, 1.7f, 0), Quaternion.identity, plantsPos.transform);
            Instantiate(testPlant, plantsPos.transform.TransformPoint(1.58f, 1.7f, 0), Quaternion.identity, plantsPos.transform);
            Instantiate(testPlant, plantsPos.transform.TransformPoint(3.46f, 1.7f, 0), Quaternion.identity, plantsPos.transform);
            Instantiate(testPlant, plantsPos.transform.TransformPoint(0.65f, 2.3f, 0), Quaternion.identity, plantsPos.transform);
            Instantiate(testPlant, plantsPos.transform.TransformPoint(2.55f, 2.3f, 0), Quaternion.identity, plantsPos.transform);
        }

        correctPw += plants;
    }

    void PaintingPass()
    {
        int planetsCount = 0;

        for (int i = 0; i < 9; i++)
        {
            GameObject planetObj = planetsPos.transform.GetChild(i).gameObject;

            if (Random.value > 0.5f)
            {
                float xPos = planetObj.transform.localPosition.x;
                float yPos = planetObj.transform.localPosition.y;
                planetObj.transform.localPosition = new Vector3(Random.Range(xPos - 0.5f, xPos + 0.5f), Random.Range(yPos - 0.2f, yPos + 0.2f), Random.Range(0, 180));
                planetsCount += 1;
            }
            else
            {
                Destroy(planetObj);
            }
        }

        correctPw += planetsCount;
    }

    private void LateUpdate()
    {
        Zooming();

    }

    public void ZoomBack()
    {
        if (currentView.name == "Safe")
        {
            zoomScale = 1.9f;
            zoomPos = new Vector3(-1.25f, -9.75f, cam2.transform.position.z);
        }
        else
        {
            zoomPos = roomPos;
            zoomScale = roomScale;
        }

        isZooming = true;

        //currentView.GetComponents<BoxCollider2D>().enabled = true;
        foreach (BoxCollider2D col in currentView.GetComponents<BoxCollider2D>())
        {
            col.enabled = true;
        }
        foreach (Transform c in currentView.transform)
        {
            if (c.gameObject.GetComponent<BoxCollider2D>() != null)
            {
                c.gameObject.GetComponent<BoxCollider2D>().enabled = false;
            }
        }

        if (currentView.name == "Safe")
        {
            currentView = currentView.transform.parent.gameObject;
            if (padlockPw != null)
            {
                padlockPw.gameObject.SetActive(false);
            }
        }
    }

    void Zooming()
    {
        if (isZooming)
        {
            //Vector3 doorSpeed = Vector3.zero;
            Vector3 smoothPos = Vector3.SmoothDamp(cam2.transform.position, zoomPos, ref zoomPosSpeed, zoomTime);
            float smoothOrtho = Mathf.SmoothDamp(cam2.orthographicSize, zoomScale, ref zoomSpeed, zoomTime);

            cam2.transform.position = smoothPos;
            cam2.orthographicSize = smoothOrtho;

            if (Vector3.Distance(cam2.transform.position, zoomPos) < 0.01f && Mathf.Abs(cam2.orthographicSize - zoomScale) < 0.01f)
            {
                cam2.transform.position = zoomPos;
                cam2.orthographicSize = zoomScale;
                if (currentView.name == "Safe")
                {
                    if (padlockPw != null)
                    {
                        padlockPw.gameObject.SetActive(true);
                    }
                }
                isZooming = false;
                Debug.Log("done");
            }
        }
    }

    void ViewChange()
    {
        if (!isZooming && interacted.tag == "Zoom")
        {
            if (interacted.name == "Door")
            {
                if (canEscape == false)
                {
                    zoomScale = 0.8f;
                    zoomPos = new Vector3(-5.28f, -9.4f, cam2.transform.position.z);

                    currentView = interacted;
                }
                else
                {
                    am.PlaySFX(am.sfxEscapeSuccess);

                    successUI.SetActive(true);
                    Time.timeScale = 0;
                }
            }
            else if (interacted.name == "Shelf")
            {
                zoomScale = 1.9f;
                zoomPos = new Vector3(-1.25f, -9.75f, cam2.transform.position.z);
            }
            else if (interacted.name == "Painting")
            {
                zoomScale = 1.8f;
                zoomPos = new Vector3(4.21f, -9.38f, cam2.transform.position.z);
            }
            else if (interacted.name == "Table")
            {
                zoomScale = 2.2f;
                zoomPos = new Vector3(-2.65f, -14.3f, cam2.transform.position.z);
            }
            else if (interacted.name == "Bed")
            {
                zoomScale = 2.36f;
                zoomPos = new Vector3(4.35f, -13.6f, cam2.transform.position.z);
            }
            if (interacted.name == "Safe")
            {
                zoomScale = 0.52f;
                zoomPos = new Vector3(-0.76f, -11.05f, cam2.transform.position.z);
            }

            isZooming = true;

            currentView = interacted;
            foreach (BoxCollider2D col in currentView.GetComponents<BoxCollider2D>())
            {
                col.enabled = false;
            }
            foreach (Transform c in currentView.transform)
            {
                if (c.gameObject.GetComponent<BoxCollider2D>() != null)
                {
                    c.gameObject.GetComponent<BoxCollider2D>().enabled = true;
                }
            }
        }
    }

    void AddHint(string hintName, Sprite hintSprite, string hintMessage)
    {
        if (!hintsCount.ContainsKey(hintName))
        {
            hintsCount[hintName] = 1;
        }
        else if (hintsCount[hintName] < 2)
        {
            hintsCount[hintName] += 1;
        }
        else
        {
            hintsCount[hintName] += 1;

            // add hint to panel on third attempt
            if (hintsCount[hintName] == 3)
            {
                //AddHint("Piggy Hint", hintPiggy, "There seems to be something inside");
                GameObject newHint;

                newHint = Instantiate(hintPrefab, hintsPos.transform.position, Quaternion.identity, hintsPos.transform);

                newHint.name = hintName;
                Image hintImage = newHint.transform.GetChild(1).GetComponentInChildren<Image>();
                hintImage.sprite = hintSprite;
                hintImage.SetNativeSize();
                newHint.transform.GetComponentInChildren<TMP_Text>().text = hintMessage;

                hintsList.Add(hintName);
            }
        }


        
    }

    void DestroyHint(string hintName)
    {
        // if on hints panel
        if (hintsList.Contains(hintName))
        {
            int oldHint = hintsPos.transform.Find(hintName).GetSiblingIndex();

            // update positions of other hints
            if (hintsPos.transform.childCount > 1)
            {
                for (int i = hintsPos.transform.childCount - 1; i > oldHint; i--)
                {
                    //Vector3 newPos = hintsPos.transform.GetChild(i).position;
                    hintsPos.transform.GetChild(i).position = hintsPos.transform.GetChild(i-1).position;
                }
            }

            hintsList.Remove(hintName);
            Destroy(hintsPos.transform.GetChild(oldHint).gameObject);
        }
    }

    void ItemUse()
    {
        if (interacted.name == "Piggy")
        {
            if (activeItem != null && activeItem.name.Contains("Hammer"))
            {
                // instantiate clue note
                GameObject clueNoteItem = Instantiate(clueNotePrefab, currentView.transform.TransformPoint(interacted.transform.localPosition), Quaternion.identity, currentView.transform);
                clueNoteItem.name = "ClueNote";

                DestroyHint("Piggy Hint");

                Destroy(interacted);
                am.PlaySFX(am.sfxCeramicBreak);
            }
            else
            {
                am.PlaySFX(am.sfxClick);

                AddHint("Piggy Hint", piggySprite, "There seems to be something inside");
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
                if (!hintsCount.ContainsKey("Painting Hint"))
                {
                    hintsCount["Painting Hint"] = 4;
                }

                DestroyHint("Painting Hint");

                am.PlaySFX(am.sfxChangeBulb);

                //Destroy(interacted);
                SpriteRenderer paintingLight = interacted.GetComponent<SpriteRenderer>();
                SpriteRenderer painting = interacted.transform.parent.GetComponent<SpriteRenderer>();

                if (paintingLight.sprite == paintingLightWhiteSprite)
                {
                    paintingLight.sprite = paintingLightPurpleSprite;
                    foreach (Transform p in planetsPos.transform)
                    {
                        p.gameObject.SetActive(true);
                    }
                    //painting.sprite = paintingPurpleSprite;

                    invBulb.sprite = itemBulbWhiteSprite;
                    activeItem.GetComponent<SpriteRenderer>().sprite = itemBulbWhiteSprite;
                }
                else if (paintingLight.sprite == paintingLightPurpleSprite)
                {
                    paintingLight.sprite = paintingLightWhiteSprite;
                    foreach (Transform p in planetsPos.transform)
                    {
                        p.gameObject.SetActive(false);
                    }
                    //painting.sprite = paintingWhiteSprite;

                    invBulb.sprite = itemBulbPurpleSprite;
                    activeItem.GetComponent<SpriteRenderer>().sprite = itemBulbPurpleSprite;
                }
            }
            else
            {
                am.PlaySFX(am.sfxClick);

                AddHint("Painting Hint", paintingSprite, "It seems to be able to change");
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
                DestroyHint("Door Hint");
                StartCoroutine(KeySuccess());
            }
            else
            {
                am.PlaySFX(am.sfxDoorLocked);

                AddHint("Door Hint", doorSprite, "It seems to be locked");
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
        if (pwCount < 3)
        {
            am.PlaySFX(am.sfxBeep);

            //Debug.Log("press");
            //Debug.Log(interacted);
            int numPressed = interacted.transform.GetSiblingIndex() + 1;
            padlockPw.text += numPressed;

            pwCount++;

            if (pwCount == 3)
            {
                if (int.Parse(padlockPw.text) == int.Parse(correctPw))
                {
                    Debug.Log("success!");
                    //canEscape = true;
                    DestroyHint("Safe Hint");

                    StartCoroutine(SafeSuccess());
                }
                else
                {
                    Debug.Log("failed...");
                    StartCoroutine(ShortDisplay());

                    AddHint("Safe Hint", safeSprite, "There may be clues around the room about the code");

                }
            }
        }
    }

    IEnumerator SafeSuccess()
    {
        padlockPw.color = Color.green;
        yield return new WaitForSeconds(0.4f);
        am.PlaySFX(am.sfxSafeUnlock);
        yield return new WaitForSeconds(1);
        foreach (Transform b in currentView.transform)
        {
            Destroy(b.gameObject);
        }
        Destroy(padlockPw.gameObject);
        currentView.GetComponent<SpriteRenderer>().sprite = safeOpenSprite;

        // instantiate key
        GameObject keyItem = Instantiate(keyPrefab, currentView.transform.TransformPoint(1.015f, 0, 0), Quaternion.identity, currentView.transform);
        keyItem.name = "Key";

        currentView.transform.localPosition = new Vector3(currentView.transform.localPosition.x - 1.015f, currentView.transform.localPosition.y, currentView.transform.localPosition.z);
    }
    IEnumerator KeySuccess()
    {
        am.PlaySFX(am.sfxDoorUnlock);
        ZoomBack();
        yield return new WaitForSeconds(1);
        currentView.GetComponent<SpriteRenderer>().sprite = doorOpenSprite;

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

    public void GoBack()
    {
        EventSystem.current.currentSelectedGameObject.transform.parent.gameObject.SetActive(false);
        Time.timeScale = 1;
    }
}
