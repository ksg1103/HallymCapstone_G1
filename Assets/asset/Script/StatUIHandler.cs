using UnityEngine;
using UnityEngine.UI;


public class StatUIHandler : MonoBehaviour
{
    public Text statText;  // �ν����Ϳ��� ������ UI �ؽ�Ʈ

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateStatUI()
    {
        if (InventoryManager.InventoryInstance == null)
        {
            Debug.LogWarning("InventoryManager �ν��Ͻ��� ã�� �� �����ϴ�.");
            return;
        }

        var s = InventoryManager.InventoryInstance.GetTotalStats();

        statText.text =
            $"�ż�: {s.holy}     ȭ��: {s.burn}     ����: {s.bleeding}\n" +
            $"�Ǹ�: {s.blind}     ����: {s.curse}\n" +
            $"�ϴ� �߻� ���� �߼�: {s.bang}\n" +
            $"�ϴ� ���� �Ѿ� ��: {s.bullet}";
    }

}
