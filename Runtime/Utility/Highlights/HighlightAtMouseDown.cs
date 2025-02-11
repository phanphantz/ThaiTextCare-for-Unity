using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PhEngine.ThaiTextCare.Utility
{
    public class HighlightAtMouseDown : MonoBehaviour
    {
        public HighlightTargetFilter filter;
        public GraphicRaycaster graphicRaycaster;
        public WordHighlighter highlighter;
        
        void Update()
        {
            if (!Input.GetMouseButtonDown(0))
                return;

            RaycastForHighlight();
        }

        void RaycastForHighlight()
        {
            if (graphicRaycaster == null)
                throw new NullReferenceException("Missing graphic raycaster");

            if (highlighter == null)
                throw new NullReferenceException("Missing highlighter");
            
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();

            graphicRaycaster.Raycast(pointerEventData, results);
            foreach (RaycastResult result in results)
            {
                var obj = result.gameObject;
                if (obj == null)
                    continue;

                if (filter == HighlightTargetFilter.Highlightable &&
                    obj.TryGetComponent<Highlightable>(out var highlightable) &&
                    highlightable.HighlightBy(highlighter))
                    return;

                if (filter == HighlightTargetFilter.TextMeshProUGUI &&
                    obj.TryGetComponent<TextMeshProUGUI>(out var textMeshPro)
                    && highlighter.TryHighlightWordFromMouse(textMeshPro, out _))
                    return;
            }
        }
    }

    public enum HighlightTargetFilter
    {
        Highlightable, TextMeshProUGUI
    }
}