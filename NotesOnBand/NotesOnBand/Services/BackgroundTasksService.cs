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
        /// Register a background task for the application.
        /// </summary>
        /// <param name="backgroundTaskType">The type of the background task</param>
        /// <param name="trigger">Trigger that indicate when the background task should be invoked</param>
        /// <returns></returns>
        public static BackgroundTaskRegistration Register(Type backgroundTaskType, IBackgroundTrigger trigger)
        {
            return Register(backgroundTaskType, trigger, true);
        }


        /// <summary>
        /// Register a background task for the application with conditions
        /// </summary>
        /// <param name="backgroundTaskType">The type of the background task</param>
        /// <param name="trigger">Trigger that indicate when the background task should be invoked</param>
        /// <param name="enforceConditions">Indicate if the background task should quit if condition is no longer valid</param>
        /// <param name="conditions">Optional conditions for the background task to run with</param>
        /// <returns></returns>
        public static BackgroundTaskRegistration Register(Type backgroundTaskType, IBackgroundTrigger trigger, bool enforceConditions = true, params IBackgroundCondition[] conditions)
        {
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
        /// <param name="conditions">Optional conditions for the background task to run with</param>
        /// <returns></returns>
        public static BackgroundTaskRegistration RegisterTimedBackgroundTask(Type backgroundTaskType, uint delay, bool isOneTime = false, params IBackgroundCondition[] conditions)
        {
            return Register(backgroundTaskType, new TimeTrigger(delay, isOneTime), true ,conditions);
        }


        /// <summary>
        /// Register a background task that will be invoked with a given system trigger/event
        /// </summary>
        /// <param name="backgroundTaskType">The type of the background task</param>
        /// <param name="type">Indicate which system trigger/event type that the background task should be responding to.</param>
        /// <param name="isOneTime">Indicate whether the background task will run only once</param>
        /// <param name="conditions">Optional conditions for the background task to run with</param>
        /// <returns></returns>
        public static BackgroundTaskRegistration RegisterSystemBackgroundTask(Type backgroundTaskType, SystemTriggerType type, bool isOneTime = false, params IBackgroundCondition[] conditions)
        {
            return Register(backgroundTaskType, new SystemTrigger(type, isOneTime),true, conditions);
        }
        
    }
}
