namespace ET
{
    [FriendClassAttribute(typeof(ET.TaskInfo))]
    public static class TaskInfoSystem
    {


        public static void SetTaskState(this TaskInfo self, TaskState taskState)
        {
            self.TaskState = (int)taskState;
        }

        public static bool IsTaskState(this TaskInfo self, TaskState taskState)
        {
            return self.TaskState == (int)taskState;
        }

        //public static void UpdateProgress(this TaskInfo self, int count)
        //{

        //}
    }
}
