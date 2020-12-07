using UnityEngine;
using System.Collections;

namespace Cameo
{
	public class BaseMessageBox : MonoBehaviour 
	{
		public delegate void MessagBoxEventCallback(BaseMessageBox msgBox);
		protected MessagBoxEventCallback _onOpenedCallback = delegate { };
		protected MessagBoxEventCallback _onClosedCallback = delegate { };

		public virtual void Open(MessagBoxEventCallback onOpenedCallback, MessagBoxEventCallback onClosedCallback, object[] values)
		{
			_onOpenedCallback += onOpenedCallback;
			_onClosedCallback += onClosedCallback;
		}

		public virtual void Close()
		{
			onClosed ();
		}

		public virtual void onOpend()
		{
			_onOpenedCallback (this);
		}

		public virtual void onClosed()
		{
			_onClosedCallback (this);
		}
	}

	[System.Serializable]
	public class MessageBoxInfo
	{
		public string TypeName;
		public BaseMessageBox MessageBox;
	}
}
