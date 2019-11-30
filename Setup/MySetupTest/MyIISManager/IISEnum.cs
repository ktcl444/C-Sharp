using System;

namespace MyIISManager
{

	/// <summary>
	/// ������IIS�汾
	/// </summary>
	public enum IISVersionEnum
	{
		/// <summary>
		/// δ֪�汾
		/// </summary>
		Unknown,
		/// <summary>
		/// �汾IIS 4.0
		/// </summary>
		IIS4,
		/// <summary>
		/// �汾 IIS 5.0,5.1
		/// </summary>
		IIS5,
		/// <summary>
		/// �汾IIS 6.0
		/// </summary>
		IIS6,
		/// <summary>
		/// �汾IIS 7.0
		/// </summary>
		IIS7
	}


	/// <summary>
	/// IISWebServer��״̬
	/// </summary>
	public enum IISServerState
	{
		/// <summary>
		/// ��������
		/// </summary>
		Starting = 1,

		/// <summary>
		/// ������
		/// </summary>
		Started = 2,

		/// <summary>
		/// ����ֹͣ
		/// </summary>
		Stopping = 3,

		/// <summary>
		/// ��ֹͣ
		/// </summary>
		Stopped = 4,

		/// <summary>
		/// ������ͣ
		/// </summary>
		Pausing = 5,

		/// <summary>
		/// ����ͣ
		/// </summary>
		Paused = 6,

		/// <summary>
		/// ����ȡ����ͣ
		/// </summary>
		Continuing = 7,

		/// <summary>
		/// δ֪
		/// </summary>
		Unkonwn = 8
	}


}
