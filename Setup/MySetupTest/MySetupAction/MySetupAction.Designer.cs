namespace MySetupAction
{
    partial class MySetupAction
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private string strWebName = string.Empty;
        private string strWebPort = string.Empty;
        private string strMachineName = string.Empty;
        private string strVDirectory = string.Empty;
        private string strWebDir = string.Empty;
        private string strAppFriendlyName = string.Empty;
        private string strSourceDir = string.Empty;
        private string strAppPool10 = "ASP.NET 1.0";
        private string strAppPool20 = "ASP.NET 2.0";

        private int appIsolated = 2;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }

        #endregion
    }
}