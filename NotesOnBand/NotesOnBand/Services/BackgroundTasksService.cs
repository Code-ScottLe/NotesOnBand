using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace NotesOnBand.Services
{
    public static class BackgroundTasksService
    {
        /// <summary>
        /// Check if a background task has been registered with the application or not.
        /// </summary>
        /// <param name="backgroundTaskType">The type of the background task.</param>
        /// <returns></returns>
        public static bool IsBackgroundTaskRegistered(Type backgroundTaskType)
        {
            return BackgroundTaskRegistration.AllTasks.Where(t => t.Value.Name == backgroundTaskType.Name).Select(t => t).Count() >= 1 ;
        }
       

        /// <summary>
        /// Register a background task for the application with conditions
        /// </summary>
        /// <param name="backgroundTaskType">The type of the background task</param>
        /// <param name="trigger">Trigger that indicate when the background task should be invoked</param>
        /// <param name="enforceConditions">Indicate if the background task should quit if condition is no longer valid</param>
        /// <param name="conditions">Optional conditions for the background task to run with</param>
        /// <returns></returns>
        public static async Task<BackgroundTaskRegistration> RegisterAsync(Type backgroundTaskType, IBackgroundTrigger trigger, bool enforceConditions = true, params IBackgroundCondition[] conditions)
        {
            //Verify access
            BackgroundAccessStatus taskRequired = await BackgroundExecutionManager.RequestAccessAsync();

            if(taskRequired == BackgroundAccessStatus.Denied)
            {
                throw new InvalidOperationException("Background access is denied!");

            }

            //Get details about the background task
            string backgroundTaskName = backgroundTaskType.Name;
            string backgroundTaskEntryPoint = backgroundTaskType.FullName;

            //build the background task
            BackgroundTaskBuilder builder = new BackgroundTaskBuilder();
            builder.Name = backgroundTaskName;
            builder.TaskEntryPoint = backgroundTaskEntryPoint;
            builder.CancelOnConditionLoss = enforceConditions;

            if(conditions != null && conditions.Count() > 0)
            {
                foreach(IBackgroundCondition condition in conditions)
                {
                    builder.AddCondition(condition);
                }
            }

            builder.SetTrigger(trigger);

            //Register it
            BackgroundTaskRegistration registered = builder.Register();

            return registered;
          
        }


        /// <summary>
        /// Register a background task that will be invoked periodically
        /// </summary>
        /// <param name="backgroundTaskType">The type of the background task</param>
        /// <param name="delay">Time, in seconds, that indicates the delay between execution time of the background task</param>
        /// <param name="isOneTime">Indicate whether the background task will run only once</param>
        /// <returns></returns>
        public async static Task<BackgroundTaskRegistration> RegisterTimedBackgroundTaskAsync(Type backgroundTaskType, uint delay, bool isOneTime = false)
        {
            return await RegisterAsync(backgroundTaskType, new TimeTrigger(delay, isOneTime));
        }


        /// <summary>
        /// Register a background ták that ưill be invoked periodically, with conditions
        /// </summary>
        /// <param name="backgroundTaskType">The type of the background task</param>
        /// <param name="delay">Time, in seconds, that indicates the delay between execution time of the background task</param>
        /// <param name="isOneTime">Indicate whether the background task will run only once</param>
        /// <param name="enforceCondition">Indicate whether the background task will stop running if the condition is failed, regardless of running status</param>
        /// <param name="conditions">Optional conditions for the background task</param>
        /// <returns></returns>
        public async static Task<BackgroundTaskRegistration> RegisterTimedBackgroundTaskAsync(Type backgroundTaskType, uint delay, bool isOneTime = false, bool enforceCondition = true, params IBackgroundCondition[] conditions)
        {
            return await RegisterAsync(backgroundTaskType, new TimeTrigger(delay, isOneTime), enforceCondition, conditions);
        }


        /// <summary>
        /// Register a background task that will be invoked with a given system trigger/event
        /// </summary>
        /// <param name="backgroundTaskType">The type of the background task</param>
        /// <param name="type">Indicate which system trigger/event type that the background task should be responding to.</param>
        /// <param name="isOneTime">Indicate whether the background task will run only once</param>
        /// <param name="conditions">Optional conditions for the background task to run with</param>
        /// <returns></returns>
        public async static Task<BackgroundTaskRegistration> RegisterSystemBackgroundTaskAsync(Type backgroundTaskType, SystemTriggerType type, bool isOneTime = false)
        {
            return await RegisterAsync(backgroundTaskType, new SystemTrigger(type, isOneTime));
        }


        /// <summary>
        /// Register a background task that will be invoked with a given system trigger/event, with conditions
        /// </summary>
        /// <param name="backgroundTaskType">The type of the background task</param>
        /// <param name="type">Indicate which system trigger/event type that the background task should be responding to.</param>
        /// <param name="isOneTime">Indicate whether the background task will run only once</param>
        /// <param name="enforceCondition">Indicate whether the background task will stop running if the condition is failed, regardless of running status</param>
        /// <param name="conditions">Optional conditions for the background task to run with</param>
        /// <returns></returns>
        public async static Task<BackgroundTaskRegistration> RegisterSystemBackgroundTaskAsync(Type backgroundTaskType, SystemTriggerType type, bool isOnTime = false, bool enforceCondition = true, params IBackgroundCondition[] conditions)
        {
            return await RegisterAsync(backgroundTaskType, new SystemTrigger(type, isOnTime), enforceCondition, conditions);
        }

        /// <summary>
        /// Unregister a background task
        /// </summary>
        /// <param name="backgroundTaskType">The type of the background task</param>
        public static void Unregister(Type backgroundTaskType)
        {
            if(IsBackgroundTaskRegistered(backgroundTaskType) == false)
            {
                return;
            }

            IBackgroundTaskRegistration tobeUnregister = BackgroundTaskRegistration.AllTasks.Where(t => t.Value.Name == backgroundTaskType.Name).Select(t => t).FirstOrDefault().Value;

            tobeUnregister?.Unregister(true);
        }


        /// <summary>
        /// Get the given background task
        /// </summary>
        /// <param name="taskName">name of the background task.</param>
        /// <returns></returns>
        public static IBackgroundTaskRegistration GetBackgroundTask(string taskName)
        {
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == taskName)
                {
                    return task.Value;
                }
            }

            return null;
        }

        /// <summary>
        /// Get the given background task
        /// </summary>
        /// <param name="taskType">type of the background task class</param>
        /// <returns></returns>
        public static IBackgroundTaskRegistration GetBackgroundTask(Type taskType)
        {
            return GetBackgroundTask(taskType.Name);
        }
    }
}
