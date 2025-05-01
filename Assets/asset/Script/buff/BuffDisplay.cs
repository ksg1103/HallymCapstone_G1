using UnityEngine;
    using UnityEngine.UI;

    public class BuffDisplay : MonoBehaviour
    {
        public Image iconImage;   // 디버프 아이콘
        public Text countText;    // 디버프 카운트 텍스트

        public void SetBuff(int amount)
        {
            countText.text = amount.ToString();
        }
    }