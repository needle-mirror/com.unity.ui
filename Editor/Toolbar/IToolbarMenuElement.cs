using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.UIElements
{
    public interface IToolbarMenuElement
    {
        DropdownMenu menu { get; }
    }

    public static class ToolbarMenuElementExtensions
    {
        public static void ShowMenu(this IToolbarMenuElement tbe)
        {
            if (tbe == null || !tbe.menu.MenuItems().Any())
                return;

            var ve = tbe as VisualElement;
            if (ve == null)
                return;

            tbe.menu.DoDisplayEditorMenu(ve.worldBound);
        }
    }
}
