using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyIISManager
{
    public interface IIISWebServer
    {
        //新建网站
        bool Create();

        //删除网站
        void Delete();

        //更新配置
        bool Update();

        //运行
        void Start();

        //暂停
        void Pause();

        //停止
        void Stop();

        //取消暂停
        void Continue();

        //销毁
        void Dispose();

        //获得当前状态
        IISServerState GetStatus();

        //检测端口是否重复
        bool CheckPortRepeated();

        //初始化
        void Init(string strMachine, string strWebName, string strWebPort, string strWebDir, string strAppFriendlyName, int appIsolated, string strAppPool);
    }
}
