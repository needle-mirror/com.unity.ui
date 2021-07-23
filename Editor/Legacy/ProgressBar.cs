using UnityEngine.UIElements;

namespace UnityEditor.UIElements
{
#if UIE_PACKAGE &&  (!UNITY_2021_1_OR_NEWER)
    public class ProgressBar : AbstractProgressBar
    {
        public new class UxmlFactory : UxmlFactory<ProgressBar, UxmlTraits> {}
    }
#endif
}
