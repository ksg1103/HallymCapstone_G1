using UnityEngine;
    using UnityEngine.UI;

    public class BuffDisplay : MonoBehaviour
    {
        public Image iconImage;   // ����� ������
        public Text countText;    // ����� ī��Ʈ �ؽ�Ʈ

        public void SetBuff(int amount)
        {
            countText.text = amount.ToString();
        }
    }