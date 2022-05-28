using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
    [FriendClass(typeof(DlgTask))]
    [FriendClassAttribute(typeof(ET.TaskInfo))]
    public static class DlgTaskSystem
    {
        public static void RegisterUIEvent(this DlgTask self)
        {
            self.View.E_TasksLoopVerticalScrollRect.AddItemRefreshListener(self.OnTaskItemLoopHandler);
        }

        public static void ShowWindow(this DlgTask self, Entity contextData = null)
        {
            self.Refresh();
        }

        public static void Refresh(this DlgTask self)
        {
            int count = self.ZoneScene().GetComponent<TasksComponent>().GetTaskInfoCount();
            self.AddUIScrollItems(ref self.ScrollItemTasks, count);
            self.View.E_TasksLoopVerticalScrollRect.SetVisible(true, count);
        }

        public static void OnTaskItemLoopHandler(this DlgTask self, Transform transform, int index)
        {
            Scroll_Item_task scrollItemTask = self.ScrollItemTasks[index].BindTrans(transform);
            TaskInfo taskInfo = self.ZoneScene().GetComponent<TasksComponent>().GetTaskInfoByIndex(index);

            TaskConfig taskConfig = TaskConfigCategory.Instance.Get(taskInfo.ConfigId);

            scrollItemTask.E_TaskNameText.SetText(taskConfig.TaskName);
            scrollItemTask.E_TaskDescText.SetText(taskConfig.TaskDesc);
            scrollItemTask.E_TaskProgressText.SetText($"{taskInfo.TaskProgress} / {taskConfig.TaskTargetCount}");
            scrollItemTask.E_TaskRewardCountText.SetText(taskConfig.RewardGoldCount.ToString());
            scrollItemTask.E_ReceiveTipText.SetText(taskInfo.IsTaskState(TaskState.Complete) ? "领取奖励" : "未完成");
            scrollItemTask.E_ReceiveButton.interactable = taskInfo.IsTaskState(TaskState.Complete);
            scrollItemTask.E_ReceiveButton.AddListenerAsyncWithId(self.OnReceiveRewardHandler, taskInfo.ConfigId);
        }

        public static async ETTask OnReceiveRewardHandler(this DlgTask self, int taskConfigId)
        {
            try
            {
                int errorCode = await TaskHelper.GetTaskReward(self.ZoneScene(), taskConfigId);
                if (errorCode != ErrorCode.ERR_Success)
                {
                    Log.Error(errorCode.ToString());
                    return;
                }
                self.Refresh();
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }
        }
    }
}
