using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class DoorControl : MonoBehaviour
{
    [SerializeField] bool hasKey;
    [SerializeField] bool isDoubleDoor;
    private GameObject doorTextObject;
    private TextMeshProUGUI doorTextMeshPro;
    private bool isOpen = false;
    private Animator animator;
    private LF.LongForgotten.Character player;

    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<LF.LongForgotten.Character>();
        doorTextObject = transform.GetChild(0).gameObject;
        doorTextMeshPro = doorTextObject.GetComponentInChildren<TextMeshProUGUI>();
    }

    public void openDoor()
    {
        isOpen = true;
        doorTextObject.SetActive(false);
        if (isDoubleDoor)
        {
            animator.Play("DoubleDoorOpen");
        }
        else
        {
            animator.Play("DoorOpen");
        }
    }

    public void closeDoor()
    {
        isOpen = false;
        if (isDoubleDoor)
        {
            animator.Play("DoubleDoorClose");
        }
        else
        {
            animator.Play("DoorClose");
        }
    }

    public void lockedDoor()
    {
        doorTextMeshPro.text = "Key Required";
        resetDoorText();
    }

    public void destroyDoor(bool[] keys)
    {
        isOpen = true;
        doorTextObject.SetActive(false);
        int doorNumber = int.Parse(tag.Replace("Door", "")) - 1;
        if (keys[doorNumber]) {
            if (isDoubleDoor)
            {
                animator.Play("DoubleDoorFall");
            }
            else
            {
                animator.Play("DoorFall");
            }
            StartCoroutine(destroyAfterAnimation(2f));
        }
    }

    public void destroyKeylessDoor()
    {
        isOpen = true;
        doorTextObject.SetActive(false);
        if (isDoubleDoor)
        {
            animator.Play("DoubleDoorFall");
        }
        else
        {
            animator.Play("DoorFall");
        }
        destroyAfterAnimation(2f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && player != null)
        {
            if (!isOpen)
            {
                doorTextObject.SetActive(true);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && player != null)
        {
            player.DoorInteraction(gameObject.GetComponent<DoorControl>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && player != null)
        {
            if (!isOpen)
            {
                doorTextObject.SetActive(false);
            }
        }
    }

    private IEnumerator destroyAfterAnimation(float delaySeconds)
    {
        GetComponent<Collider>().enabled = false;
        yield return new WaitForSeconds(delaySeconds);
        Destroy(gameObject);
    }

    private async void resetDoorText()
    {
        await Task.Delay((int)(2f * 1000));
        doorTextObject.SetActive(false);
        doorTextMeshPro.text = "Press \"E\" to Open Door";
    }

    // Getter / Setter
    public bool IsOpen
    {
        get { return isOpen; }
        set { isOpen = value; }
    }

    public bool HasKey
    {
        get { return hasKey; }
        set { hasKey = value; }
    }
}
