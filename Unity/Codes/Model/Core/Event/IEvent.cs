using System;

namespace ET
{
	public interface IEvent
	{
		Type GetEventType();
	}

	public interface IEventClass: IEvent
	{
		void Handle(object a);
	}

	[Event]
	public abstract class AEventClass<A>: IEventClass where A: class
	{
		public Type GetEventType()
		{
			return typeof (A);
		}

		protected abstract void Run(object args); // 自己改的参数名

		public void Handle(object a)
		{
			try
			{
				Run(a);
			}
			catch (Exception e)
			{
				Log.Error(e);
			}
		}
	}
	
	[Event]
	public abstract class AEvent<A>: IEvent where A: struct
	{
		public Type GetEventType()
		{
			return typeof (A);
		}

		protected abstract void Run(A args); // 课程没改，自己改的参数名

		public void Handle(A a)
		{
			try
			{
				Run(a);
			}
			catch (Exception e)
			{
				Log.Error(e);
			}
		}
	}
	
	[Event]
	public abstract class AEventAsync<A>: IEvent where A: struct
	{
		public Type GetEventType()
		{
			return typeof (A);
		}

		protected abstract ETTask Run(A args); // 自己改的参数名

		public async ETTask Handle(A a)
		{
			try
			{
				await Run(a);
			}
			catch (Exception e)
			{
				Log.Error(e);
			}
		}
	}
}