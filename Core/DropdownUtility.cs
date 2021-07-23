using System;

namespace UnityEngine.UIElements
{
    internal static class DropdownUtility
    {
        internal static Func<IGenericMenu> MakeDropdownFunc;

        internal static IGenericMenu CreateDropdown()
        {
            return MakeDropdownFunc != null ? MakeDropdownFunc.Invoke() : new GenericDropdownMenu();
        }
    }
}
