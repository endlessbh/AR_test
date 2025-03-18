using UnityEditor;
using UnityEngine.SceneManagement;
using XCSJ.Algorithms;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    /// <summary>
    /// 类<see cref="SceneView"/>关联类型
    /// </summary>
    [LinkType(typeof(SceneView))]
    public class SceneView_LinkType : SearchableEditorWindow_LinkType<SceneView_LinkType>
    {
        public SceneView_LinkType() { }
        public SceneView_LinkType(SceneView obj) : base(obj) { }
        public SceneView_LinkType(object obj) : base(obj) { }

        /// <summary>
        /// 设置对象
        /// </summary>
        /// <param name="obj"></param>
        public void SetObject(SceneView obj) => this.obj = obj;

        #region

        public static XPropertyInfo customScene_PropertyInfo { get; } = GetXPropertyInfo(nameof(customScene));

        /// <summary>
        /// 关联<see cref="SceneView.customScene"/>
        /// </summary>
        public Scene customScene
        {
            get => (Scene)customScene_PropertyInfo.GetValue(obj);
        }

        #endregion
    }
}
