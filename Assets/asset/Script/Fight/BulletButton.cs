using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BulletButton : MonoBehaviour, IPointerEnterHandler
{
    public BulletType bulletType;

    private bool isSelected = false;
    private PlayerState playerState;
    private GameObject targetEnemy;

    [Header("Optional Visuals")]
    public Image backgroundImage;
    public Color defaultColor = Color.white;
    public Color selectedColor = Color.cyan;

    void Start()
    {
        playerState = GameObject.FindWithTag("Player")?.GetComponent<PlayerState>();
        targetEnemy = GameObject.FindWithTag("Enemymob");

        if (backgroundImage == null)
            backgroundImage = GetComponent<Image>();

        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }

        UpdateVisual();
    }

    public void OnButtonClick()
    {
        if (TurnManager.Instance.CurrentPhase != TurnPhase.Preparation)
        {
            Debug.Log("������ �غ� �ܰ迡���� �����մϴ�.");
            return;
        }

        if (!isSelected && GetCurrentSelectedBulletCount() >= playerState.bang)
        {
            Debug.LogWarning("���� ���� �ʰ�! �� �̻� ������ �� �����ϴ�.");
            return;
        }

        isSelected = !isSelected;
        UpdateVisual();
    }

    private int GetCurrentSelectedBulletCount()
    {
        int count = 0;
        foreach (Transform bullet in transform.parent)
        {
            BulletButton bb = bullet.GetComponent<BulletButton>();
            if (bb != null && bb.isSelected)
                count++;
        }
        return count;
    }

    private void UpdateVisual()
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = isSelected ? selectedColor : defaultColor;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Text nameText = GameObject.Find("NameTxt")?.GetComponent<Text>();
        Text infoText = GameObject.Find("infoText")?.GetComponent<Text>();

        if (nameText == null || infoText == null)
        {
            Debug.LogWarning("NameTxt �Ǵ� infoText�� ã�� �� �����ϴ�.");
            return;
        }

        switch (bulletType)
        {
            case BulletType.Normal:
                nameText.text = "�Ϲ� źȯ";
                infoText.text = "�⺻ �������� ���ϴ� ǥ�� źȯ�Դϴ�.";
                break;
            case BulletType.Burn:
                nameText.text = "ȭ�� źȯ";
                infoText.text = "������ �� �������� ������ �� �� �������� �����ϴ�.";
                break;
            case BulletType.Bleeding:
                nameText.text = "���� źȯ";
                infoText.text = "������ ���� ȿ���� �ο��� �� �� �������� �����ϴ�.";
                break;
            case BulletType.Curse:
                nameText.text = "���� źȯ";
                infoText.text = "������ ���� ȿ���� �ο��� ���� �������� �����ϴ�.";
                break;
            case BulletType.Blind:
                nameText.text = "�Ǹ� źȯ";
                infoText.text = "���� ���߷��� ���ߴ� �Ǹ� ȿ���� �ο��մϴ�.";
                break;
            case BulletType.Holy:
                nameText.text = "�ż� źȯ";
                infoText.text = "�÷��̾ ȸ�������ִ� �ż��� źȯ�Դϴ�.";
                break;
        }
    }


    public bool IsSelected()
    {
        return isSelected;
    }

    public BulletType GetBulletType()
    {
        return bulletType;
    }

    public GameObject GetTarget()
    {
        return targetEnemy;
    }
}

