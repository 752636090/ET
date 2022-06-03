namespace ET
{
    public class TaskInfoAwakeSystem : AwakeSystem<TaskInfo, int>
    {
        public override void Awake(TaskInfo self, int configId)
        {
            self.ConfigId = configId;
            self.TaskProgress = 0;
            self.TaskState = (int)TaskState.Doing;
        }
    }

    public class TaskInfoDestroySystem : DestroySystem<TaskInfo>
    {
        public override void Destroy(TaskInfo self)
        {
            self.ConfigId = 0;
            self.TaskProgress = 0;
            self.TaskState = (int)TaskState.None;
        }
    }

    [FriendClassAttribute(typeof(ET.TaskInfo))]
    public static class TaskInfoSystem
    {
        public static void FromMessage(this TaskInfo self, TaskInfoProto taskInfoProto)
        {
            self.ConfigId = taskInfoProto.ConfigId;
            self.TaskProgress = taskInfoProto.TaskProgress;
            self.TaskState = taskInfoProto.TaskState;
        }

        public static TaskInfoProto ToMessage(this TaskInfo self)
        {
            TaskInfoProto TaskInfoProto = new TaskInfoProto();
            TaskInfoProto.ConfigId = self.ConfigId;
            TaskInfoProto.TaskProgress = self.TaskProgress;
            TaskInfoProto.TaskState = self.TaskState;
            return TaskInfoProto;
        }


        public static void SetTaskState(this TaskInfo self, TaskState taskState)
        {
            self.TaskState = (int)taskState;
        }

        public static bool IsTaskState(this TaskInfo self, TaskState taskState)
        {
            return self.TaskState == (int)taskState;
        }

        public static void UpdateProgress(this TaskInfo self, int count)
        {
            int taskActionType = TaskConfigCategory.Instance.Get(self.ConfigId).TaskActionType;
            TaskActionConfig config = TaskActionConfigCategory.Instance.Get(taskActionType);
            if (config.TaskProgressType == (int)TaskProgressType.Add)
            {
                self.TaskProgress += count;
            }
            else if (config.TaskProgressType == (int)TaskProgressType.Sub)
            {
                self.TaskProgress -= count;
            }
            else if (config.TaskProgressType == (int)TaskProgressType.Update)
            {
                self.TaskProgress = count;
            }
        }

        public static void TryCompleteTask(this TaskInfo self)
        {
            if (!self.IsCompleteProgress() || !self.IsTaskState(TaskState.Doing))
            {
                return;
            }

            self.TaskState = (int)TaskState.Complete;
        }

        public static bool IsCompleteProgress(this TaskInfo self)
        {
            return self.TaskProgress >= TaskConfigCategory.Instance.Get(self.ConfigId).TaskTargetCount;
        }
    }
}