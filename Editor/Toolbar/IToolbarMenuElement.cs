using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.UIElements
{
    /// <summary>
    /// An interface for toolbar items that display drop-down menus.
    /// </summary>
    public interface IToolbarMenuElement
    {
        /// <summary>
        /// The drop-down menu for the element.
        /// </summary>
        DropdownMenu menu { get; }
    }

    /// <summary>
    /// An extension class that handles menu management for elements that are implemented with the IToolbarMenuElement interface, but are identical to DropdownMenu.
    /// </summary>
    public static class ToolbarMenuElementExtensions
    {
        /// <summary>
        /// Display the menu for the element.
        /// </summary>
        /// <param name="tbe">The element that is part of the menu to be displayed.</param>
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
