using System.Linq;

namespace ET
{
    [FriendClass(typeof(TasksComponent))]
    [FriendClass(typeof(TaskInfo))]
    public static class TasksComponentSystem
    {


        public static TaskInfo GetTaskInfoByConfigId(this TasksComponent self, int configId) // 没讲到
        {
            self.TaskInfoDict.TryGetValue(configId, out TaskInfo taskInfo);
            return taskInfo;
        }

        public static int GetTaskInfoCount(this TasksComponent self)
        {
            self.TaskInfoList.Clear();
            self.TaskInfoList = self.TaskInfoDict.Values.Where(a => a.IsTaskState(TaskState.Received)).ToList();
            self.TaskInfoList.Sort((a, b) => { return b.TaskState - a.TaskState; });
            return self.TaskInfoList.Count();
        }

        public static TaskInfo GetTaskInfoByIndex(this TasksComponent self, int index)
        {
            if (index < 0 || index >= self.TaskInfoList.Count)
            {
                return null;
            }
            return self.TaskInfoList[index];
        }

        public static bool IsExitTaskComplete(this TasksComponent self)
        {

        }
    }
}
