using System.Collections.Generic;
using UnityEngine;
using XCSJ.Interfaces;

namespace XCSJ.EditorTools.Windows.Layouts
{
    public interface ILayoutWindow : IExpanded
    {

    }

    public interface ILayoutWindow<T> : ILayoutWindow
    {
        bool OnGUI(List<T> list, params T[] standards);
    }

    public interface ITransformLayoutWindow : ILayoutWindow<Transform>
    {
    }

    public interface IRectTransformLayoutWindow : ILayoutWindow<RectTransform>
    {

    }
}
