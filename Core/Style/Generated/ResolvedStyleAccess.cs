/******************************************************************************/
//
//                             DO NOT MODIFY
//          This file has been generated by the UIElementsGenerator tool
//              See ResolvedStyleAccessGenerator class for details
//
/******************************************************************************/
namespace UnityEngine.UIElements
{
    public partial class VisualElement : IResolvedStyle
    {
        /// <summary>
        /// Returns the <see cref="VisualElement"/> resolved style values.
        /// </summary>
        public IResolvedStyle resolvedStyle => this;

        Align IResolvedStyle.alignContent => computedStyle.alignContent.value;
        Align IResolvedStyle.alignItems => computedStyle.alignItems.value;
        Align IResolvedStyle.alignSelf => computedStyle.alignSelf.value;
        Color IResolvedStyle.backgroundColor => computedStyle.backgroundColor.value;
        Color IResolvedStyle.borderBottomColor => computedStyle.borderBottomColor.value;
        float IResolvedStyle.borderBottomLeftRadius => computedStyle.borderBottomLeftRadius.value.value;
        float IResolvedStyle.borderBottomRightRadius => computedStyle.borderBottomRightRadius.value.value;
        float IResolvedStyle.borderBottomWidth => computedStyle.borderBottomWidth.value;
        Color IResolvedStyle.borderLeftColor => computedStyle.borderLeftColor.value;
        float IResolvedStyle.borderLeftWidth => computedStyle.borderLeftWidth.value;
        Color IResolvedStyle.borderRightColor => computedStyle.borderRightColor.value;
        float IResolvedStyle.borderRightWidth => computedStyle.borderRightWidth.value;
        Color IResolvedStyle.borderTopColor => computedStyle.borderTopColor.value;
        float IResolvedStyle.borderTopLeftRadius => computedStyle.borderTopLeftRadius.value.value;
        float IResolvedStyle.borderTopRightRadius => computedStyle.borderTopRightRadius.value.value;
        float IResolvedStyle.borderTopWidth => computedStyle.borderTopWidth.value;
        float IResolvedStyle.bottom => yogaNode.LayoutBottom;
        Color IResolvedStyle.color => computedStyle.color.value;
        DisplayStyle IResolvedStyle.display => computedStyle.display.value;
        StyleFloat IResolvedStyle.flexBasis => new StyleFloat(yogaNode.ComputedFlexBasis);
        FlexDirection IResolvedStyle.flexDirection => computedStyle.flexDirection.value;
        float IResolvedStyle.flexGrow => computedStyle.flexGrow.value;
        float IResolvedStyle.flexShrink => computedStyle.flexShrink.value;
        Wrap IResolvedStyle.flexWrap => computedStyle.flexWrap.value;
        float IResolvedStyle.fontSize => computedStyle.fontSize.value.value;
        float IResolvedStyle.height => yogaNode.LayoutHeight;
        Justify IResolvedStyle.justifyContent => computedStyle.justifyContent.value;
        float IResolvedStyle.left => yogaNode.LayoutX;
        float IResolvedStyle.marginBottom => yogaNode.LayoutMarginBottom;
        float IResolvedStyle.marginLeft => yogaNode.LayoutMarginLeft;
        float IResolvedStyle.marginRight => yogaNode.LayoutMarginRight;
        float IResolvedStyle.marginTop => yogaNode.LayoutMarginTop;
        StyleFloat IResolvedStyle.maxHeight => ResolveLengthValue(computedStyle.maxHeight, false);
        StyleFloat IResolvedStyle.maxWidth => ResolveLengthValue(computedStyle.maxWidth, true);
        StyleFloat IResolvedStyle.minHeight => ResolveLengthValue(computedStyle.minHeight, false);
        StyleFloat IResolvedStyle.minWidth => ResolveLengthValue(computedStyle.minWidth, true);
        float IResolvedStyle.opacity => computedStyle.opacity.value;
        float IResolvedStyle.paddingBottom => yogaNode.LayoutPaddingBottom;
        float IResolvedStyle.paddingLeft => yogaNode.LayoutPaddingLeft;
        float IResolvedStyle.paddingRight => yogaNode.LayoutPaddingRight;
        float IResolvedStyle.paddingTop => yogaNode.LayoutPaddingTop;
        Position IResolvedStyle.position => computedStyle.position.value;
        float IResolvedStyle.right => yogaNode.LayoutRight;
        TextOverflow IResolvedStyle.textOverflow => computedStyle.textOverflow.value;
        float IResolvedStyle.top => yogaNode.LayoutY;
        Color IResolvedStyle.unityBackgroundImageTintColor => computedStyle.unityBackgroundImageTintColor.value;
        ScaleMode IResolvedStyle.unityBackgroundScaleMode => computedStyle.unityBackgroundScaleMode.value;
        Font IResolvedStyle.unityFont => computedStyle.unityFont.value;
        FontStyle IResolvedStyle.unityFontStyleAndWeight => computedStyle.unityFontStyleAndWeight.value;
        int IResolvedStyle.unitySliceBottom => computedStyle.unitySliceBottom.value;
        int IResolvedStyle.unitySliceLeft => computedStyle.unitySliceLeft.value;
        int IResolvedStyle.unitySliceRight => computedStyle.unitySliceRight.value;
        int IResolvedStyle.unitySliceTop => computedStyle.unitySliceTop.value;
        TextAnchor IResolvedStyle.unityTextAlign => computedStyle.unityTextAlign.value;
        TextOverflowPosition IResolvedStyle.unityTextOverflowPosition => computedStyle.unityTextOverflowPosition.value;
        Visibility IResolvedStyle.visibility => computedStyle.visibility.value;
        WhiteSpace IResolvedStyle.whiteSpace => computedStyle.whiteSpace.value;
        float IResolvedStyle.width => yogaNode.LayoutWidth;
    }
}
