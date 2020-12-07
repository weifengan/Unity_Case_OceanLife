using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Cameo;

namespace Cameo
{
	public class EventChannel : Singleton<EventChannel> 
	{
		public Dictionary<string, Action> _channelDic = new Dictionary<string, Action>();
	
		public void AttachListener(string channel, Action act)
		{
			if(channel == null)
			{
				Debug.LogError("[EventChannel.AttachListener] Channel cannot be null");
				return;
			}

			if(!_channelDic.ContainsKey(channel))
			{
				_channelDic.Add(channel, delegate {});
			}

			_channelDic[channel] += act;
		}

		public void DetachListner(string channel, Action act)
		{
			if(channel == null)
			{
				Debug.LogError("[EventChannel.DetachListner] Channel cannot be null");
				return;
			}
	
			if(_channelDic.ContainsKey(channel))
			{
				_channelDic[channel] -= act;
			}
		}

		public void Invoke(string channel)
		{
			if(_channelDic.ContainsKey(channel))
			{
				_channelDic[channel].Invoke();
			}
		}
	}
}