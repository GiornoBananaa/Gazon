using System;
using TMPro;
using UnityEngine;

namespace Game.Runtime.PianoFeature
{
    public class ListOption : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Transform _model;
        
        public void SetText(string text)
        {
            _text.SetText(text);
        }
        
        public void SetSize(Vector3 size)
        {
            _model.localScale = size;
            _rectTransform.sizeDelta = size;
        }
    }
}