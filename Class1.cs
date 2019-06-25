using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TaskBird.Tasks
{
    public class TaskBirdClient
    {
        public readonly Form Parent;
        public TaskBirdClient(Form parent)
        {
            Parent = parent;
        }

        public void CreateTask(Action Tasker, AsyncCallback callback = null)
        {
            var uiContext = TaskScheduler.FromCurrentSynchronizationContext();

            // Start a task - this runs on the background thread...
            Task task = Task.Factory.StartNew(() =>
            {
                Task.Factory.StartNew(() =>
                {
                    Thread start = new Thread(() =>
                    {
                        Tasker();
                    });
                    start.IsBackground = true;
                    start.Start();
                }, CancellationToken.None, TaskCreationOptions.None, uiContext);
            });
        }

        public async void CreateTask(Task[] tasks, AsyncCallback callback = null)
        {
            tasks.RunAllTasks();
            await Task.WhenAll(tasks);
            callback?.Invoke(null);
        }
    }

    public static class TaskExtension
    {
        public static async void RunAllTasks(this Task[] tasks)
        {
            await Task.Run(() =>
            {
                for (int i = 0; i < tasks.Length; i++)
                {
                   tasks[i].Start();
                }

            });
        }
    }
}
