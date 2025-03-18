using UnityEditor;
using UnityEngine;

/*
 * ***********免责声明
    Xdreamer的宗旨是：致力于在虚拟现实领域推广开源精神。为了更好地履行本宗旨，请您仔细阅读以下条款。
如果您对本声明的任何条款表示异议，您可以选择不使用本资源库的任何资源；获取或使用本资源库的任何资源
则意味着您同意并将遵守本声明的全部规定。（未成年人应在法定监护人陪同下审阅本声明）
 
第1条 本声明最终解释权归属于本资源库主体——北京讯驰视界科技有限公司所有。
 
第2条 本声明适用于本资源库中所有已发布的历史资源及将来要发布的所有资源。
 
第3条 凡是浏览或使用本资源库资源的用户均为本资源库用户（以下统称“用户”）。
 
第4条 本声明所指的资源包括但不限于贴图、模型、图片、视频、音频、材质、场景、第三方插件等。
 
第5条 本资源库是为用户提供其资源展示与分享的开放平台。本资源库无偿向所有用户提供资源展示与分享服务,
不以盈利为目的。本资源库主体与用户之间不存在买卖关系，也不是服务提供商与客户的关系。
 
第6条 本资源库分享的所有资源不得用于任何商业或非法用途。否则，一切法律后果由该用户或第三方自负，与
本资源库主体无关。
 
第7条 本资源库中所累积的所有资源，均来自热心用户的积极分享以及开源网站搜集。本资源库不会修改用户分享
的资源。对于用户分享的资源内容之真实性引发的全部责任，由用户自行承担。一旦由于用户分享的资源发生权利
纠纷或侵犯了任何第三方的合法权益，其责任由用户本人承担，因此给本公众号账号主体或任何第三方造成损失的，
用户应负责赔偿。

第8条 用户须承诺：其所分享的所有资源符合中国法律法规和规范性文件的相关规定，不侵犯任何第三方的合法权
益。用户应承担一切因其个人行为而直接或间接导致的民事或刑事法律责任。因用户行为给本资源库主体造成损失
的，用户应负责赔偿。
 
第9条 如因用户分享的资源侵犯了第三方的合法权利，该第三方可向本资源库提出异议。经核实后，我方将在第一
时间删除侵权内容，并向该第三方提供该资源相关的所有信息，包括但不限于相关用户的资料、相关资源的来源或
出处等信息。我方也将积极与该第三方协商可能涉及的赔偿事宜。
 
第10条 本资源库有权根据中华人民共和国法律、法规及规范性文件的变化以及互联网及自身业务的发展情况，不断
修改和完善本声明。

第11条 本声明未涉及的问题参见国家有关法律法规。当本声明与国家法律法规冲突时，以国家法律法规为准。
 */

namespace XCSJ.EditorExtension.XAssets.Libs
{
    /// <summary>
    /// 声明面板
    /// </summary>
    public class DeclarationPanel : BasePanel
    {
        /// <summary>
        /// 滚动条
        /// </summary>
        private Vector2 scrollPosition = Vector2.zero;

        private GUIStyle titleLabelStyle = null;

        private GUIStyle contentLabelStyle = null;

        void InitStyle()
        {
            titleLabelStyle = new GUIStyle();
            titleLabelStyle.fontSize = 20;
            titleLabelStyle.fontStyle = FontStyle.Bold;
            titleLabelStyle.alignment = TextAnchor.MiddleCenter;
            titleLabelStyle.wordWrap = true;
            titleLabelStyle.clipping = TextClipping.Clip;
            titleLabelStyle.normal.textColor = EditorStyles.label.normal.textColor;

            contentLabelStyle = new GUIStyle();
            contentLabelStyle.fontSize = 16;
            contentLabelStyle.richText = true;
            contentLabelStyle.alignment = TextAnchor.MiddleLeft;
            contentLabelStyle.wordWrap = true;
            contentLabelStyle.clipping = TextClipping.Overflow;
            contentLabelStyle.normal.textColor = EditorStyles.label.normal.textColor;
        }

        public override void Render()
        {
            InitStyle();
            Rect position = AssetLibWindow.instance.position;
            Rect rect = new Rect(0, 0, position.width , position.height);
            GUILayout.BeginArea(rect);

            scrollPosition = GUILayout.BeginScrollView(this.scrollPosition, false, false, new GUILayoutOption[0]);

            GUILayout.Space(80);
            GUILayout.Label("免责声明", titleLabelStyle);
            GUILayout.Space(40);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label( "      XDreamer的宗旨是：致力于在虚拟现实领域推广开源精神。为了更好地履行本宗旨，请您仔细阅读以下条款。\n" +
                            "如果您对本声明的任何条款表示异议，您可以选择不使用本资源库的任何资源；获取或使用本资源库的任何资源则\n" +
                            "意味着您同意并将遵守本声明的全部规定。（未成年人应在法定监护人陪同下审阅本声明）\n" +
                            "第1条   本声明最终解释权归属于本资源库主体——北京讯驰视界科技有限公司所有。\n" +
                            "第2条   本声明适用于本资源库中所有已发布的历史资源及将来要发布的所有资源。\n" +
                            "第3条   凡是浏览或使用本资源库资源的用户均为本资源库用户（以下统称“用户”）。\n" +
                            "第4条   本声明所指的资源包括但不限于贴图、模型、图片、视频、音频、材质、场景、第三方插件等。\n" +
                            "第5条   本资源库是为用户提供其资源展示与分享的开放平台。本资源库<color=red>无偿</color>向所有用户提供资源展示与分享服务, \n" +
                            "          <color=red>不以盈利为目的</color>。本资源库主体与用户之间<color=red>不存在</color>买卖关系，也<color=red>不是</color>服务提供商与客户的关系。\n" +
                            "第6条   本资源库分享的所有资源<color=red>不得</color>用于任何商业或非法用途。否则，一切法律后果由该用户或第三方自负，与本\n" + 
                            "          资源库主体无关。\n" +
                            "第7条   本资源库中所累积的所有资源，均来自热心用户的积极分享以及开源网站搜集。本资源库不会修改用户分享\n" +
                            "          的资源。对于用户分享的资源内容之真实性引发的全部责任，由用户自行承担。一旦由于用户分享的资源发\n" +
                            "          生权利纠纷或侵犯了任何第三方的合法权益，其责任由用户本人承担，因此给本公众号账号主体或任何第三\n" +
                            "          方造成损失的，用户应负责赔偿。\n" +
                            "第8条   用户须承诺：其所分享的所有资源符合中国法律法规和规范性文件的相关规定，不侵犯任何第三方的合法权\n" +
                            "          益。用户应承担一切因其个人行为而直接或间接导致的民事或刑事法律责任。因用户行为给本资源库主体造\n" +
                            "          成损失的，用户应负责赔偿。\n" +
                            "第9条   如因用户分享的资源侵犯了第三方的合法权利，该第三方可向本资源库提出异议。经核实后，我方将在第一\n" +
                            "          时间删除侵权内容，并向该第三方提供该资源相关的所有信息，包括但不限于相关用户的资料、相关资源的\n" +
                            "          来源或出处等信息。我方也将积极与该第三方协商可能涉及的赔偿事宜。\n" +
                            "第10条 本资源库有权根据中华人民共和国法律、法规及规范性文件的变化以及互联网及自身业务的发展情况，不断\n" +
                            "          修改和完善本声明。\n" +
                            "第11条 本声明未涉及的问题参见国家有关法律法规。当本声明与国家法律法规冲突时，以国家法律法规为准。", contentLabelStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(36);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("我已阅读并同意声明",  GUILayout.Width(140), GUILayout.Height(50)))
            {
                AssetLibWindow.instance.RecieveDeclaration();
            }
            GUILayout.Space(200);
            if (GUILayout.Button("取     消",  GUILayout.Width(140), GUILayout.Height(50)))
            {
                AssetLibWindow.instance.CloseWindow();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndScrollView();

            GUILayout.EndArea();
        }
    }
}
