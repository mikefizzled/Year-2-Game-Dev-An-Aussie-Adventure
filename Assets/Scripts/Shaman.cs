using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Shaman : MonoBehaviour, IInteractable
{
    public Canvas ShopCanvas;
    public Button ShapeShifterBtn;
    public TextMeshProUGUI ResponseText;
    public void Interaction()
    {
        ShopCanvas.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            Player.Instance.InteractionInstance(this);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            Player.Instance.ResetInteractionInstance();
    }

    public void BuyShaperShifter()
    {
        if (!Progress.Instance.Shapeshifter)
        {
            Progress.Instance.Shapeshifter = true;
        }
        else
        {
            ResponseText.text = "You already own that power!";
        }
    }

    public void BuyCaveLight()
    {
        if (!Progress.Instance.Cave)
        {
            Progress.Instance.Cave = true;
        }
        else
        {

        }
    }

    public void MysteryUnlock()
    {
        if (!Progress.Instance.Thylacine)
        {

        }
        else
        {

        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
