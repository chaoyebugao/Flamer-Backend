using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flamer.Utility.Wroker
{
    public static class Retry
    {
        private static ILogger logger = LogManager.GetCurrentClassLogger();

        public static Task DoAscending(
            Func<Task> action,
            int maxAttemptCount = 3,
            TimeSpan? delayOnFirstTime = null,
            Action onSuccess = null,
            Action<Exception, int> onTryFailed = null)
        {
            int delayFactor = 0;
            int maxDelayFactor = 7;
            int delaySeconds = 0;

            return Task.Run(async () =>
            {
                for (int attempted = 0; attempted < maxAttemptCount; attempted++)
                {
                    try
                    {
                        if (attempted == 0)
                        {
                            if (delayOnFirstTime.HasValue)
                            {
                                Task.Delay(delayOnFirstTime.Value).Wait();
                            }
                        }
                        else
                        {
                            Task.Delay(delaySeconds * 1000).Wait();
                        }

                        await action();
                        onSuccess();
                        break;
                    }
                    catch (Exception ex)
                    {
                        delayFactor = delayFactor >= maxDelayFactor ? maxDelayFactor : delayFactor + 1;
                        delaySeconds = (int)Math.Pow(2, delayFactor);

                        try
                        {
                            onTryFailed(ex, delaySeconds);
                        }
                        catch (Exception exCallback)
                        {
                            logger.Error(exCallback);
                        }
                    }
                }
            });
        }
    }

}
