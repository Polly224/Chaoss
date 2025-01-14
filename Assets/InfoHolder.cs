using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class InfoHolder : MonoBehaviour
{
    public static InfoHolder instance;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text descriptionText;
    [SerializeField] TMP_Text effectText;
    [SerializeField] TMP_Text rarityText;
    [SerializeField] TMP_Text healthText;
    [SerializeField] TMP_Text attackText;
    [SerializeField] TMP_Text energyText;
    public bool hoveredOver = false;
    [SerializeField] float alphaChangeSpeed;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(instance);
    }

    private void Update()
    {
        foreach(SpriteRenderer sR in transform.GetComponentsInChildren<SpriteRenderer>())
        {
            if(!hoveredOver)
            sR.color = new Color(sR.color.r, sR.color.g, sR.color.b, sR.color.a - Time.deltaTime * alphaChangeSpeed <= 0 ? 0 : sR.color.a - Time.deltaTime * alphaChangeSpeed);
            else sR.color = new Color(sR.color.r, sR.color.g, sR.color.b, sR.color.a + Time.deltaTime * alphaChangeSpeed >= 1 ? 1 : sR.color.a + Time.deltaTime * alphaChangeSpeed);
        }
        foreach(TMP_Text tmp in transform.GetComponentsInChildren<TMP_Text>())
        {
            if (!hoveredOver)
                tmp.color = new Color(tmp.color.r, tmp.color.g, tmp.color.b, tmp.color.a - Time.deltaTime * alphaChangeSpeed <= 0 ? 0 : tmp.color.a - Time.deltaTime * alphaChangeSpeed);
            else tmp.color = new Color(tmp.color.r, tmp.color.g, tmp.color.b, tmp.color.a + Time.deltaTime * alphaChangeSpeed >= 1 ? 1 : tmp.color.a + Time.deltaTime * alphaChangeSpeed);
        }
    }

    public void SetInfo(string name, string description, int health, int attack, int energy, PiecesDataStorage.Rarity rarity)
    {
        nameText.text = name;
        descriptionText.text = description;
        healthText.text = health.ToString();
        attackText.text = attack.ToString();
        energyText.text = energy.ToString();
        rarityText.text = rarity.ToString();
        rarityText.transform.parent.gameObject.GetComponent<SpriteRenderer>().color = PieceManager.GetColorForRarity(rarity);
        effectText.transform.parent.gameObject.SetActive(false);
        descriptionText.gameObject.transform.localPosition = Vector3.zero;
        descriptionText.rectTransform.sizeDelta = new Vector2(2.2f, 0.9f);
    }

    public void SetInfo(string name, string description, int health, int attack, int energy, PiecesDataStorage.Rarity rarity, string effect)
    {
        nameText.text = name;
        descriptionText.text = description;
        healthText.text = health.ToString();
        attackText.text = attack.ToString();
        energyText.text = energy.ToString();
        rarityText.text = rarity.ToString();
        rarityText.transform.parent.gameObject.GetComponent<SpriteRenderer>().color = PieceManager.GetColorForRarity(rarity);
        effectText.transform.parent.gameObject.SetActive(true);
        effectText.text = effect;
        descriptionText.gameObject.transform.localPosition = new Vector3(0, 0.48f, 0);
        descriptionText.rectTransform.sizeDelta = new Vector2(2.2f, 0.24f);
    }

    public void SetHover(bool hover)
    {
        if(PieceManager.pickedPiece == null)
        {
            hoveredOver = hover;
        }
        else
        {
            hoveredOver = true;
        }
    }
}
