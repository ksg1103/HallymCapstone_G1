using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// ������ ����
public enum BuffType
{
    Burn,
    Bleeding,
    Curse,
    Blind,
    Holy
}

public class BuffDisplay : MonoBehaviour, IPointerEnterHandler
{
    public Image iconImage;     // ����� ������
    public Text countText;      // ����� ��ġ

    [Header("�� ���� �������� ����")]
    public BuffType buffType;   // ���������� ����

    private int currentAmount = 0; // <����> ǥ�ÿ� ���� ��

    public void SetBuff(int amount)
    {
        currentAmount = amount;
        countText.text = amount.ToString();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Text nameText = GameObject.Find("NameTxt")?.GetComponent<Text>();
        Text infoText = GameObject.Find("infoText")?.GetComponent<Text>();

        if (nameText == null || infoText == null) return;

        switch (buffType)
        {
            case BuffType.Burn:
                nameText.text = "ȭ��";
                infoText.text = $"���� {currentAmount} ��ŭ�� �������� �ְ�, ������ �ϳ��� �پ��ϴ�.";
                break;
            case BuffType.Bleeding:
                nameText.text = "����";
                infoText.text = $"���� {currentAmount} ��ŭ�� �������� �ְ�, ������ �ϳ��� �پ��ϴ�.";
                break;
            case BuffType.Curse:
                nameText.text = "����";
                infoText.text = $"���� {currentAmount} ��ŭ�� �������� �ְ�, ������ �ϳ��� �پ��ϴ�.";
                break;
            case BuffType.Blind:
                nameText.text = "�Ǹ�";
                infoText.text = $"{currentAmount}% ��ŭ ���� ���߷��� �������ϴ�.";
                break;
            case BuffType.Holy:
                nameText.text = "�ż�";
                infoText.text = $"{currentAmount} ��ŭ �÷��̾ ȸ����Ű��, �ٸ� ������� �ִ�{currentAmount} ��ŭ ��ȭ�ϰ� ������ϴ�." +
                    $"����������, �ϴ� ������ �ϳ��� �پ��ϴ�.";
                break;
        }
    }
}
