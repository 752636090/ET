﻿using System.Collections.Generic;

namespace ET
{
    public class TasksComponentAwakeSystem : AwakeSystem<TasksComponent>
    {
        public override void Awake(TasksComponent self)
        {
            self.Awake();
        }
    }

    public class TasksComponentDestroySystem : DestroySystem<TasksComponent>
    {
        public override void Destroy(TasksComponent self)
        {
            foreach (TaskInfo taskInfo in self.TaskInfoDict.Values)
            {
                taskInfo?.Dispose();
            }
            self.TaskInfoDict.Clear();
            self.CurrentTaskSet.Clear();
        }
    }

    [FriendClass(typeof(TaskInfo))]
    public class TasksComponentDeserializeSyetem : DeserializeSystem<TasksComponent>
    {
        public override void Deserialize(TasksComponent self)
        {
            foreach (Entity entity in self.Children.Values)
            {
                TaskInfo taskInfo = entity as TaskInfo;
                self.TaskInfoDict.Add(taskInfo.ConfigId, taskInfo);

                if (!taskInfo.IsTaskState(TaskState.Received))
                {
                    self.CurrentTaskSet.Add(taskInfo.ConfigId);
                }
            }
        }
    }

    [FriendClass(typeof(TasksComponent))]
    [FriendClass(typeof(TaskInfo))]
    public static class TasksComponentSystem
    {
        public static void Awake(this TasksComponent self)
        {
            if (self.TaskInfoDict.Count == 0)
            {
                self.UpdateAfterTaskInfo(beforeTaskConfigId: 0, isNoticeClient: false);
            }
        }

        public static void TriggerTaskAction(this TasksComponent self, TaskActionType taskActionType, int count, int targetId = 0)
        {
            foreach (int taskConfigId in self.CurrentTaskSet)
            {
                TaskConfig taskConfig = TaskConfigCategory.Instance.Get(taskConfigId);
                if (taskConfig.TaskActionType == (int)taskActionType && taskConfig.TaskTargetId == targetId)
                {
                    self.AddOrUpdateTaskInfo(taskConfigId, count);
                }
            }
        }

        public static void AddOrUpdateTaskInfo(this TasksComponent self, int taskConfigId, int count, bool isNoticeClient = true)
        {
            if (!self.TaskInfoDict.TryGetValue(taskConfigId, out TaskInfo taskInfo))
            {
                taskInfo = self.AddChild<TaskInfo, int>(taskConfigId);
                self.TaskInfoDict.Add(taskConfigId, taskInfo);
            }

            taskInfo.UpdateProgress(count);
            taskInfo.TryCompleteTask();
            if (isNoticeClient)
            {
                TaskNoticeHelper.SyncTaskInfo(self.GetParent<Unit>(), taskInfo, self.M2CUpdateTaskInfo);
            }
        }

        public static void UpdateAfterTaskInfo(this TasksComponent self, int beforeTaskConfigId, bool isNoticeClient = true)
        {
            self.CurrentTaskSet.Remove(beforeTaskConfigId);
            List<int> taskConfigIdList = TaskConfigCategory.Instance.GetAfterTaskIdListByBeforeId(beforeTaskConfigId);
            if (taskConfigIdList == null)
            {
                return;
            }

            foreach (int taskConfigId in taskConfigIdList)
            {
                self.CurrentTaskSet.Add(taskConfigId);
                int count = self.GetTaskInitProgressCount(taskConfigId);
                self.AddOrUpdateTaskInfo(taskConfigId, count, isNoticeClient);
            }
        }

        public static int GetTaskInitProgressCount(this TasksComponent self, int taskConfigId)
        {
            TaskConfig config = TaskConfigCategory.Instance.Get(taskConfigId);

            if (config.TaskActionType == (int)TaskActionType.UpLevel)
            {
                return self.GetParent<Unit>().GetComponent<NumericComponent>().GetAsInt(NumericType.Level);
            }
            return 0;
        }

        public static void ReceiveTaskRewardState(this TasksComponent self, Unit unit, int taskConfigId)
        {
            if (!self.TaskInfoDict.TryGetValue(taskConfigId, out TaskInfo taskInfo))
            {
                Log.Error($"TaskInfo Error :{taskConfigId}");
                return;
            }
            taskInfo.SetTaskState(TaskState.Received);
            TaskNoticeHelper.SyncTaskInfo(unit, taskInfo, self.M2CUpdateTaskInfo);
            self.UpdateAfterTaskInfo(taskConfigId);
        }

        public static int TryReceiveTaskReward(this TasksComponent self, int taskConfigId)
        {
            if (!TaskConfigCategory.Instance.Contain(taskConfigId))
            {
                return ErrorCode.ERR_NoTaskExist;
            }

            self.TaskInfoDict.TryGetValue(taskConfigId, out TaskInfo taskInfo);

            if (taskInfo == null || taskInfo.IsDisposed)
            {
                return ErrorCode.ERR_NoTaskInfoExist;
            }

            if (!self.IsBeforeTaskReceived(taskConfigId))
            {
                return ErrorCode.ERR_BeforeTaskNoOver;
            }

            if (taskInfo.IsTaskState(TaskState.Received))
            {
                return ErrorCode.ERR_TaskRewarded;
            }

            if (!taskInfo.IsTaskState(TaskState.Complete))
            {
                return ErrorCode.ERR_TaskNoCompleted;
            }

            return ErrorCode.ERR_Success;
        }

        public static bool IsBeforeTaskReceived(this TasksComponent self, int taskConfigId)
        {
            TaskConfig config = TaskConfigCategory.Instance.Get(taskConfigId);

            if (config.TaskBeforeId == 0)
            {
                return true;
            }

            if (!self.TaskInfoDict.TryGetValue(config.TaskBeforeId, out TaskInfo beforeTaskInfo))
            {
                return false;
            }

            if (!beforeTaskInfo.IsTaskState(TaskState.Received))
            {
                return false;
            }
            return true;
        }
    }
}
